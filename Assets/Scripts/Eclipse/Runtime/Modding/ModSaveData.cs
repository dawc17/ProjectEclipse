using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Eclipse.Modding
{
    // Operates on the existing save DOM. Missing content must never require decoding,
    // normalizing, moving, or rebuilding its ownership XML.
    public static class ModSaveData
    {
        public static bool IsExternalItem(string name)
        {
            DefinitionId id;
            return DefinitionId.TryParse(name, out id) && id.Namespace.Value != "core" && id.Category == "items";
        }

        public static bool IsMissingItem(XmlNode node, Func<string, bool> itemExists)
        {
            string name = node?.Attributes?["Name"]?.Value;
            return IsExternalItem(name) && !itemExists(name);
        }

        public static XmlNode CreateEquipmentView(XmlNode warrior, Func<string, bool> itemExists,
            Func<string, string> defaultItem)
        {
            if (warrior == null) return null;
            XmlNode view = warrior;
            foreach (string slot in new[] { "Weapon", "Armor", "Helm", "Ranged", "Magic" })
            {
                string name = warrior.Attributes?[slot]?.Value;
                if (!IsExternalItem(name) || itemExists(name)) continue;
                if (ReferenceEquals(view, warrior)) view = warrior.CloneNode(true);
                // Only the temporary model input changes. The original equipped reference
                // stays in the save until the player explicitly equips something else.
                view.Attributes[slot].Value = defaultItem(slot) ?? string.Empty;
            }
            return view;
        }

        public static bool RecordContext(XmlNode warrior, IReadOnlyList<ModDescriptor> activeMods,
            ModContentCatalog content = null, ModStateRuntime modState = null)
        {
            if (warrior == null || activeMods == null) return false;
            XmlElement state = warrior["EclipseMods"];
            if (state != null && state.GetAttribute("schema") != "1") return false;
            if (state == null)
            {
                state = warrior.OwnerDocument.CreateElement("EclipseMods");
                state.SetAttribute("schema", "1");
                warrior.AppendChild(state);
            }
            state.SetAttribute("api", ModPlatformVersions.Api.ToString());
            state.SetAttribute("core", ModPlatformVersions.Core.ToString());
            if (content != null)
                state.SetAttribute("contentHash", ComputeContentSetFingerprint(activeMods, content, modState));
            // Keep last-seen records for absent mods and all unrecognized attributes/children.
            foreach (XmlNode child in state.ChildNodes)
                if (child is XmlElement entry && entry.Name == "Mod") entry.SetAttribute("active", "false");
            foreach (ModDescriptor mod in activeMods)
            {
                XmlElement entry = null;
                foreach (XmlNode child in state.ChildNodes)
                    if (child is XmlElement candidate && candidate.Name == "Mod" && candidate.GetAttribute("id") == mod.Id.Value)
                    { entry = candidate; break; }
                if (entry == null)
                {
                    entry = warrior.OwnerDocument.CreateElement("Mod");
                    entry.SetAttribute("id", mod.Id.Value);
                    state.AppendChild(entry);
                }
                entry.SetAttribute("version", mod.Version.ToString());
                entry.SetAttribute("active", "true");
            }
            return true;
        }

        public static string ComputeContentSetFingerprint(IReadOnlyList<ModDescriptor> activeMods,
            ModContentCatalog content, ModStateRuntime modState = null)
        {
            if (activeMods == null) throw new ArgumentNullException(nameof(activeMods));
            if (content == null) throw new ArgumentNullException(nameof(content));

            var canonical = new StringBuilder();
            Append(canonical, "fingerprint-v6");
            Append(canonical, ModPlatformVersions.Api.ToString());
            Append(canonical, ModPlatformVersions.Core.ToString());

            var mods = new List<ModDescriptor>(activeMods.Count);
            for (int i = 0; i < activeMods.Count; i++)
                if (activeMods[i] != null) mods.Add(activeMods[i]);
            mods.Sort((left, right) => string.CompareOrdinal(left.Id.Value, right.Id.Value));
            Append(canonical, "mods");
            Append(canonical, mods.Count);
            foreach (ModDescriptor mod in mods)
            {
                Append(canonical, mod.Id.Value);
                Append(canonical, mod.Version.ToString());
            }

            var localizations = new List<LocalizationDefinition>(content.Localizations);
            localizations.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "localizations");
            Append(canonical, localizations.Count);
            foreach (LocalizationDefinition localization in localizations)
            {
                Append(canonical, localization.Id.ToString());
                Append(canonical, localization.LegacyKey ?? string.Empty);
                var languages = new List<string>(localization.Values.Keys);
                languages.Sort(StringComparer.Ordinal);
                Append(canonical, languages.Count);
                foreach (string language in languages)
                {
                    Append(canonical, language);
                    Append(canonical, localization.Values[language]);
                }
            }

            var patches = new List<ModContentPatchRecord>(content.Patches);
            patches.Sort((left, right) =>
            {
                int result = string.CompareOrdinal(left.Target.ToString(), right.Target.ToString());
                if (result != 0) return result;
                result = string.CompareOrdinal(left.Field, right.Field);
                if (result != 0) return result;
                result = string.CompareOrdinal(left.Owner.Value, right.Owner.Value);
                return result != 0 ? result : ((int)left.Operation).CompareTo((int)right.Operation);
            });
            Append(canonical, "patches");
            Append(canonical, patches.Count);
            for (int i = 0; i < patches.Count; i++)
            {
                ModContentPatchRecord patch = patches[i];
                Append(canonical, patch.Owner.Value);
                Append(canonical, patch.Target.ToString());
                Append(canonical, patch.Field);
                Append(canonical, ((int)patch.Operation).ToString(CultureInfo.InvariantCulture));
            }

            var items = new List<ItemDefinition>(content.Weapons.Count + content.Armors.Count +
                content.Helms.Count + content.Ranged.Count + content.Magic.Count);
            AddItems(items, content.Weapons);
            AddItems(items, content.Armors);
            AddItems(items, content.Helms);
            AddItems(items, content.Ranged);
            AddItems(items, content.Magic);
            AddItems(items, content.NonEquipmentItems);
            items.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "items");
            Append(canonical, items.Count);
            foreach (ItemDefinition item in items) AppendItem(canonical, item);

            var redirects = new List<ItemRedirectDefinition>(content.ItemRedirects);
            redirects.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "item-redirects");
            Append(canonical, redirects.Count);
            foreach (ItemRedirectDefinition redirect in redirects)
            {
                Append(canonical, redirect.Id.ToString());
                Append(canonical, redirect.IsTombstone ? "tombstone" : "alias");
                Append(canonical, redirect.IsTombstone ? string.Empty : redirect.Target.ToString());
            }

            var listings = new List<ShopListingDefinition>(content.ShopListings);
            listings.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "shop");
            Append(canonical, listings.Count);
            foreach (ShopListingDefinition listing in listings)
            {
                Append(canonical, listing.Id.ToString());
                Append(canonical, listing.Item.ToString());
                Append(canonical, ((int)listing.Section).ToString(CultureInfo.InvariantCulture));
                Append(canonical, listing.Level);
                Append(canonical, ((int)listing.Price.Currency).ToString(CultureInfo.InvariantCulture));
                Append(canonical, listing.Price.Amount.ToString(CultureInfo.InvariantCulture));
            }

            var perks = new List<PerkDefinition>(content.Perks);
            perks.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "perks");
            Append(canonical, perks.Count);
            foreach (PerkDefinition perk in perks)
            {
                Append(canonical, perk.Id.ToString());
                Append(canonical, perk.HasTemplate ? perk.Template.ToString() : string.Empty);
                Append(canonical, perk.HasBehavior ? perk.Behavior.ToString() : string.Empty);
                Append(canonical, perk.DisplayName.ToString());
                Append(canonical, perk.Description.ToString());
                Append(canonical, perk.Icon.ToString());
                Append(canonical, ((int)perk.Kind).ToString(CultureInfo.InvariantCulture));
                Append(canonical, perk.LegacyName ?? string.Empty);
                Append(canonical, perk.LegacyPerkXml ?? string.Empty);
                var parameterNames = new List<string>(perk.Parameters.Keys);
                parameterNames.Sort(StringComparer.Ordinal);
                Append(canonical, parameterNames.Count);
                foreach (string parameter in parameterNames)
                {
                    Append(canonical, parameter);
                    Append(canonical, perk.Parameters[parameter]);
                }
                AppendParameterValues(canonical, perk.InitialParameters);
            }

            var enchantments = new List<EnchantmentDefinition>(content.Enchantments);
            enchantments.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "enchantments");
            Append(canonical, enchantments.Count);
            foreach (EnchantmentDefinition enchantment in enchantments)
            {
                Append(canonical, enchantment.Id.ToString());
                Append(canonical, enchantment.HasPerk ? enchantment.Perk.ToString() : string.Empty);
                Append(canonical, enchantment.HasBehavior ? enchantment.Behavior.ToString() : string.Empty);
                Append(canonical, enchantment.DisplayName.ToString());
                Append(canonical, enchantment.Description.ToString());
                Append(canonical, enchantment.Icon.ToString());
                Append(canonical, ((int)enchantment.Recipe).ToString(CultureInfo.InvariantCulture));
                Append(canonical, enchantment.Equipment.Count);
                for (int i = 0; i < enchantment.Equipment.Count; i++)
                    Append(canonical, ((int)enchantment.Equipment[i]).ToString(CultureInfo.InvariantCulture));
                AppendParameterValues(canonical, enchantment.InitialParameters);
            }

            var behaviors = new List<ModBehaviorDefinition>(content.Behaviors);
            behaviors.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "behaviors");
            Append(canonical, behaviors.Count);
            foreach (ModBehaviorDefinition behavior in behaviors)
            {
                Append(canonical, behavior.Id.ToString());
                Append(canonical, behavior.StateLifetime);
                Append(canonical, behavior.StateVersion);
                if (behavior.StateSchema != null)
                    foreach (var field in behavior.StateSchema.Parameters)
                    {
                        Append(canonical, field.Name); Append(canonical, field.Type.ToString());
                        Append(canonical, field.HasDefault ? field.DefaultValue.ToWireString() : "");
                    }
                var parameters = new List<ModParameterDefinition>(behavior.Parameters.Parameters);
                parameters.Sort((left, right) => string.CompareOrdinal(left.Name, right.Name));
                Append(canonical, parameters.Count);
                for (int i = 0; i < parameters.Count; i++)
                {
                    ModParameterDefinition parameter = parameters[i];
                    Append(canonical, parameter.Name);
                    Append(canonical, ((int)parameter.Type).ToString(CultureInfo.InvariantCulture));
                    Append(canonical, parameter.Required ? "required" : "optional");
                    Append(canonical, parameter.HasDefault ? parameter.DefaultValue.ToWireString() : string.Empty);
                }
            }

            AppendPhaseOneContent(canonical, content);

            var stateDefinitions = modState == null
                ? new List<ModStateDefinition>()
                : new List<ModStateDefinition>(modState.Definitions);
            stateDefinitions.Sort((left, right) => string.CompareOrdinal(left.Owner.Value, right.Owner.Value));
            Append(canonical, "state-schemas");
            Append(canonical, stateDefinitions.Count);
            for (int i = 0; i < stateDefinitions.Count; i++)
            {
                ModStateDefinition definition = stateDefinitions[i];
                Append(canonical, definition.Owner.Value);
                Append(canonical, definition.Version);
                var fields = new List<ModParameterDefinition>(definition.Fields.Parameters);
                fields.Sort((left, right) => string.CompareOrdinal(left.Name, right.Name));
                Append(canonical, fields.Count);
                for (int j = 0; j < fields.Count; j++)
                {
                    ModParameterDefinition field = fields[j];
                    Append(canonical, field.Name);
                    Append(canonical, ((int)field.Type).ToString(CultureInfo.InvariantCulture));
                    Append(canonical, field.Required ? "required" : "optional");
                    Append(canonical, field.HasDefault ? field.DefaultValue.ToWireString() : string.Empty);
                }
                var aliases = new List<string>(definition.Aliases.Keys);
                aliases.Sort(StringComparer.Ordinal);
                Append(canonical, aliases.Count);
                for (int j = 0; j < aliases.Count; j++)
                {
                    Append(canonical, aliases[j]);
                    Append(canonical, definition.Aliases[aliases[j]]);
                }
                var tombstones = new List<string>(definition.Tombstones);
                tombstones.Sort(StringComparer.Ordinal);
                Append(canonical, tombstones.Count);
                for (int j = 0; j < tombstones.Count; j++) Append(canonical, tombstones[j]);
            }

            var modes = new List<ModModeDefinition>(content.Modes);
            modes.Sort((a,b) => string.CompareOrdinal(a.Id.ToString(), b.Id.ToString()));
            Append(canonical, "modes"); Append(canonical, modes.Count);
            foreach (var mode in modes)
            {
                Append(canonical, mode.Id.ToString()); Append(canonical, mode.Repeatable ? "repeat" : "once");
                Append(canonical, mode.ResetOnLoss ? "reset" : "retain"); Append(canonical, mode.Raid ? "raid" : "mode");
                Append(canonical, mode.HardMode ? "hard" : "normal"); Append(canonical, mode.MinimumLevel);
                Append(canonical, mode.StartsAt.ToString(CultureInfo.InvariantCulture)); Append(canonical, mode.EndsAt.ToString(CultureInfo.InvariantCulture));
                Append(canonical, mode.EntryItem.ToString()); Append(canonical, mode.EntryCount);
                foreach (var fight in mode.Fights) Append(canonical, fight.ToString());
            }
            var timers = new List<ModTimerPolicy>(content.TimerPolicies);
            timers.Sort((a,b) => string.CompareOrdinal(a.Subsystem, b.Subsystem));
            foreach (var timer in timers)
            {
                Append(canonical, timer.Owner.ToString()); Append(canonical, timer.Subsystem); Append(canonical, timer.Seconds);
                Append(canonical, timer.SkipEnabled ? "skip" : "wait");
            }
            var features = new List<string>(content.DisabledFeatures); features.Sort(StringComparer.Ordinal);
            foreach (var feature in features) Append(canonical, feature);
            byte[] data = Encoding.UTF8.GetBytes(canonical.ToString());
            byte[] hash;
            using (SHA256 sha = SHA256.Create()) hash = sha.ComputeHash(data);
            var hex = new StringBuilder(hash.Length * 2);
            for (int i = 0; i < hash.Length; i++) hex.Append(hash[i].ToString("x2", CultureInfo.InvariantCulture));
            return "sha256:" + hex;
        }

        private static void AddItems<T>(List<ItemDefinition> target, IReadOnlyList<T> source)
            where T : ItemDefinition
        {
            for (int i = 0; i < source.Count; i++) target.Add(source[i]);
        }

        private static void AppendItem(StringBuilder canonical, ItemDefinition item)
        {
            Append(canonical, item.GetType().Name);
            Append(canonical, item.Id.ToString());
            Append(canonical, item.DisplayName.ToString());
            Append(canonical, item.Icon.ToString());
            Append(canonical, item.Model.ToString());
            Append(canonical, item.LegacyName ?? string.Empty);
            Append(canonical, item.LegacyItemXml ?? string.Empty);
            Append(canonical, ((int)item.Progression).ToString(CultureInfo.InvariantCulture));

            if (item is WeaponDefinition weapon)
            {
                Append(canonical, weapon.SubType);
                Append(canonical, weapon.Damage);
            }
            else if (item is ArmorDefinition armor)
            {
                Append(canonical, armor.BodyDefense);
                Append(canonical, armor.HeadDefense);
                Append(canonical, armor.UnarmedDamage);
            }
            else if (item is HelmDefinition helm)
            {
                Append(canonical, helm.HeadDefense);
            }
            else if (item is RangedDefinition ranged)
            {
                Append(canonical, ranged.SubType);
                Append(canonical, ranged.RangedDamage);
                Append(canonical, ranged.WeaponDamage);
            }
            else if (item is MagicDefinition magic)
            {
                Append(canonical, magic.SubType);
                Append(canonical, magic.MagicDamage);
            }
            else if (item is NonEquipmentItemDefinition nonEquipment)
            {
                Append(canonical, (int)nonEquipment.Kind);
                Append(canonical, nonEquipment.SubType);
                Append(canonical, nonEquipment.PackLabel);
                Append(canonical, nonEquipment.SilentReceive);
                Append(canonical, nonEquipment.SpendAfterUse);
            }
        }

        private static void AppendPhaseOneContent(StringBuilder canonical, ModContentCatalog content)
        {
            AppendStageGraph(canonical, content);
            AppendQuests(canonical, content);
            AppendP1C(canonical, content);
            AppendP1D(canonical, content);
        }

        private static void AppendStageGraph(StringBuilder canonical, ModContentCatalog content)
        {
            var templates = new List<WarriorTemplateDefinition>(content.WarriorTemplates);
            templates.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "warrior-templates");
            Append(canonical, templates.Count);
            for (int i = 0; i < templates.Count; i++)
            {
                Append(canonical, templates[i].Id.ToString());
                Append(canonical, templates[i].LegacyName);
            }

            var zones = new List<ZoneDefinition>(content.Zones);
            zones.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "zones");
            Append(canonical, zones.Count);
            for (int i = 0; i < zones.Count; i++)
            {
                ZoneDefinition zone = zones[i];
                Append(canonical, zone.Id.ToString());
                Append(canonical, zone.LegacyName);
                Append(canonical, zone.FileName);
                Append(canonical, zone.IsStart);
                AppendIds(canonical, zone.Battles);
            }

            var battles = new List<BattleDefinition>(content.Battles);
            battles.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "battles");
            Append(canonical, battles.Count);
            for (int i = 0; i < battles.Count; i++)
            {
                BattleDefinition battle = battles[i];
                Append(canonical, battle.Id.ToString());
                Append(canonical, battle.Zone.ToString());
                Append(canonical, battle.LegacyName);
                Append(canonical, (int)battle.Kind);
                Append(canonical, battle.X); Append(canonical, battle.Y);
                Append(canonical, battle.Alias); Append(canonical, battle.Title);
                Append(canonical, battle.Icon); Append(canonical, battle.IconAtlas);
                Append(canonical, battle.EclipseToggleName); Append(canonical, battle.Preview);
                Append(canonical, battle.Description); Append(canonical, battle.Location);
                Append(canonical, battle.Music); Append(canonical, battle.RewardImage);
                Append(canonical, battle.ShowResistance);
                AppendIds(canonical, battle.Fights);
            }

            var fights = new List<FightDefinition>(content.Fights);
            fights.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "fights");
            Append(canonical, fights.Count);
            for (int i = 0; i < fights.Count; i++)
            {
                FightDefinition fight = fights[i];
                Append(canonical, fight.Id.ToString()); Append(canonical, fight.Battle.ToString());
                Append(canonical, fight.LegacyName); Append(canonical, fight.Replays);
                Append(canonical, fight.ReplayInterval); Append(canonical, fight.Power);
                Append(canonical, fight.Rounds); Append(canonical, fight.RoundTime);
                Append(canonical, fight.Location); Append(canonical, fight.Music);
                Append(canonical, fight.EvaluatedRating); Append(canonical, fight.HealthRecovery);
                Append(canonical, fight.Description); Append(canonical, fight.Locked);
                Append(canonical, fight.RewardImage);
                AppendIds(canonical, fight.Warriors); AppendIds(canonical, fight.Rules); AppendIds(canonical, fight.Rewards);
            }

            var warriors = new List<WarriorDefinition>(content.Warriors);
            warriors.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "warriors");
            Append(canonical, warriors.Count);
            for (int i = 0; i < warriors.Count; i++)
            {
                WarriorDefinition warrior = warriors[i];
                Append(canonical, warrior.Id.ToString());
                Append(canonical, warrior.HasTemplate ? warrior.Template.ToString() : string.Empty);
                Append(canonical, warrior.FirstName); Append(canonical, warrior.LastName);
                Append(canonical, warrior.Avatar); Append(canonical, warrior.Voice);
                Append(canonical, warrior.Level); Append(canonical, warrior.Tactic); Append(canonical, warrior.HealthBars);
                Append(canonical, warrior.Group); Append(canonical, warrior.Random);
                var attributeNames = new List<string>(warrior.Attributes.Keys);
                attributeNames.Sort(StringComparer.Ordinal);
                Append(canonical, attributeNames.Count);
                for (int j = 0; j < attributeNames.Count; j++)
                {
                    string name = attributeNames[j]; Append(canonical, name); Append(canonical, warrior.Attributes[name]);
                }
                Append(canonical, warrior.AttributeAlignments.Count);
                for (int j = 0; j < warrior.AttributeAlignments.Count; j++)
                {
                    WarriorAttributeAlignmentDefinition alignment = warrior.AttributeAlignments[j];
                    Append(canonical, alignment.Factor); Append(canonical, alignment.Shift);
                    Append(canonical, alignment.Priority); Append(canonical, (int)alignment.Mode);
                }
                AppendIds(canonical, warrior.Items); AppendIds(canonical, warrior.Perks);
            }

            var rules = new List<FightRuleDefinition>(content.FightRules);
            rules.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "fight-rules"); Append(canonical, rules.Count);
            for (int i = 0; i < rules.Count; i++)
            {
                FightRuleDefinition rule = rules[i];
                Append(canonical, rule.Id.ToString()); Append(canonical, (int)rule.Kind);
                Append(canonical, (int)rule.Target); Append(canonical, (int)rule.Mode);
                Append(canonical, rule.Rounds.Count);
                for (int j = 0; j < rule.Rounds.Count; j++) Append(canonical, rule.Rounds[j]);
                Append(canonical, rule.Name); Append(canonical, rule.HasItem ? rule.Item.ToString() : string.Empty);
                Append(canonical, rule.MinimumLevel); Append(canonical, rule.HasPerk ? rule.Perk.ToString() : string.Empty);
                var names = new List<string>(rule.Attributes.Keys); names.Sort(StringComparer.Ordinal);
                Append(canonical, names.Count);
                for (int j = 0; j < names.Count; j++) { Append(canonical, names[j]); Append(canonical, rule.Attributes[names[j]]); }
            }

            var rewards = new List<RewardDefinition>(content.Rewards);
            rewards.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "rewards"); Append(canonical, rewards.Count);
            for (int i = 0; i < rewards.Count; i++)
            {
                RewardDefinition reward = rewards[i]; Append(canonical, reward.Id.ToString()); Append(canonical, reward.Gems);
                Append(canonical, reward.Items.Count);
                for (int j = 0; j < reward.Items.Count; j++) AppendRewardGrant(canonical, reward.Items[j]);
                Append(canonical, reward.Choices.Count);
                for (int j = 0; j < reward.Choices.Count; j++)
                {
                    RewardChoiceDefinition choice = reward.Choices[j]; Append(canonical, choice.Items.Count);
                    for (int k = 0; k < choice.Items.Count; k++)
                    { AppendRewardGrant(canonical, choice.Items[k].Grant); Append(canonical, choice.Items[k].Weight); }
                }
            }
        }

        private static void AppendRewardGrant(StringBuilder canonical, RewardItemGrant grant)
        {
            Append(canonical, grant.Item.ToString()); Append(canonical, grant.UpgradeNumber);
        }

        private static void AppendQuests(StringBuilder canonical, ModContentCatalog content)
        {
            var quests = new List<QuestDefinition>(content.Quests);
            quests.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "quests"); Append(canonical, quests.Count);
            for (int i = 0; i < quests.Count; i++)
            {
                QuestDefinition quest = quests[i]; Append(canonical, quest.Id.ToString()); Append(canonical, quest.Priority);
                Append(canonical, quest.Unresumable); Append(canonical, quest.AllowDoubles); Append(canonical, (int)quest.Place);
                AppendStrings(canonical, quest.Groups); AppendStrings(canonical, quest.Marks);
                Append(canonical, quest.Events.Count);
                for (int j = 0; j < quest.Events.Count; j++) Append(canonical, (int)quest.Events[j]);
                Append(canonical, quest.Conditions.Count);
                for (int j = 0; j < quest.Conditions.Count; j++) AppendQuestCondition(canonical, quest.Conditions[j]);
                Append(canonical, quest.Actions.Count);
                for (int j = 0; j < quest.Actions.Count; j++) AppendQuestAction(canonical, quest.Actions[j]);
            }
        }

        private static void AppendQuestCondition(StringBuilder canonical, ModQuestCondition condition)
        {
            Append(canonical, (int)condition.Kind); Append(canonical, (int)condition.Operator); Append(canonical, condition.Not);
            if (condition.Kind == ModQuestConditionKind.Compare)
            { AppendQuestOperand(canonical, condition.Left); AppendQuestOperand(canonical, condition.Right); return; }
            Append(canonical, condition.Children.Count);
            for (int i = 0; i < condition.Children.Count; i++) AppendQuestCondition(canonical, condition.Children[i]);
        }

        private static void AppendQuestOperand(StringBuilder canonical, ModQuestOperand operand)
        {
            Append(canonical, (int)operand.Kind); Append(canonical, operand.Value);
            Append(canonical, operand.HasReference ? operand.Reference.ToString() : string.Empty);
        }

        private static void AppendQuestAction(StringBuilder canonical, ModQuestAction action)
        {
            Append(canonical, (int)action.Kind); Append(canonical, action.Name); Append(canonical, action.Value);
            Append(canonical, action.Title); Append(canonical, action.Image); Append(canonical, action.Flag);
            Append(canonical, action.HasReference ? action.Reference.ToString() : string.Empty);
            Append(canonical, action.Lines.Count);
            for (int i = 0; i < action.Lines.Count; i++)
            { Append(canonical, action.Lines[i].Text); Append(canonical, action.Lines[i].ButtonText); Append(canonical, action.Lines[i].Frames); }
            Append(canonical, action.Button != null);
            if (action.Button == null) return;
            Append(canonical, action.Button.Text); Append(canonical, action.Button.Color); Append(canonical, action.Button.Actions.Count);
            for (int i = 0; i < action.Button.Actions.Count; i++) AppendQuestAction(canonical, action.Button.Actions[i]);
        }

        private static void AppendP1C(StringBuilder canonical, ModContentCatalog content)
        {
            var sets = new List<ItemSetDefinition>(content.ItemSets);
            sets.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "item-sets"); Append(canonical, sets.Count);
            for (int i = 0; i < sets.Count; i++)
            {
                ItemSetDefinition set = sets[i]; Append(canonical, set.Id.ToString());
                Append(canonical, set.Title.ToString()); Append(canonical, set.Text.ToString()); Append(canonical, set.Brief.ToString());
                Append(canonical, set.Members.Count);
                for (int j = 0; j < set.Members.Count; j++)
                {
                    ModItemSetMember member = set.Members[j]; Append(canonical, member.Item.ToString());
                    Append(canonical, member.Scale); Append(canonical, member.Rotate); Append(canonical, member.X);
                    Append(canonical, member.Y); Append(canonical, member.IconsY);
                }
            }

            var profiles = new List<ForgeEconomicProfileDefinition>(content.ForgeEconomicProfiles);
            profiles.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "forge-profiles"); Append(canonical, profiles.Count);
            for (int i = 0; i < profiles.Count; i++)
            { Append(canonical, profiles[i].Id.ToString()); Append(canonical, profiles[i].RuntimeRecipeName); }

            var recipes = new List<ForgeRecipeFamilyDefinition>(content.ForgeRecipeFamilies);
            recipes.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "forge-recipes"); Append(canonical, recipes.Count);
            for (int i = 0; i < recipes.Count; i++)
            {
                ForgeRecipeFamilyDefinition recipe = recipes[i]; Append(canonical, recipe.Id.ToString());
                Append(canonical, recipe.Alias); Append(canonical, recipe.EconomicProfile.ToString());
                Append(canonical, recipe.Items.Count);
                for (int j = 0; j < recipe.Items.Count; j++)
                {
                    ModForgeRecipeItem item = recipe.Items[j]; Append(canonical, (int)item.Equipment);
                    Append(canonical, item.Enchantments); Append(canonical, item.BarScale);
                    Append(canonical, item.MinDeviation); Append(canonical, item.MaxDeviation); Append(canonical, item.RandomAspect);
                }
                Append(canonical, recipe.Candidates.Count);
                for (int j = 0; j < recipe.Candidates.Count; j++)
                {
                    ModForgeRecipeCandidate candidate = recipe.Candidates[j]; Append(canonical, candidate.Perk.ToString());
                    Append(canonical, (int)candidate.Equipment); Append(canonical, candidate.MinLevel); Append(canonical, candidate.MaxLevel);
                }
            }

            var availability = new List<ItemAvailabilityPolicyDefinition>(content.ItemAvailabilityPolicies);
            availability.Sort((left, right) => CompareIds(left.Item, right.Item));
            Append(canonical, "item-availability"); Append(canonical, availability.Count);
            for (int i = 0; i < availability.Count; i++)
            {
                ItemAvailabilityPolicyDefinition policy = availability[i]; Append(canonical, policy.Owner.Value);
                Append(canonical, policy.Item.ToString()); Append(canonical, (int)policy.Visibility); Append(canonical, policy.RequiredGroup);
            }

            var progression = new List<ProgressionBranchOverlayDefinition>(content.ProgressionBranches);
            progression.Sort((left, right) => left.Level.CompareTo(right.Level));
            Append(canonical, "progression-branches"); Append(canonical, progression.Count);
            for (int i = 0; i < progression.Count; i++)
            {
                ProgressionBranchOverlayDefinition branch = progression[i]; Append(canonical, branch.Owner.Value); Append(canonical, branch.Level);
                Append(canonical, branch.Entries.Count);
                for (int j = 0; j < branch.Entries.Count; j++)
                { Append(canonical, branch.Entries[j].Perk.ToString()); Append(canonical, (int)branch.Entries[j].Action); }
            }
        }

        private static void AppendP1D(StringBuilder canonical, ModContentCatalog content)
        {
            var locales = new List<LocaleMetadataDefinition>(content.LocaleMetadata);
            locales.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "locale-metadata"); Append(canonical, locales.Count);
            for (int i = 0; i < locales.Count; i++)
            {
                LocaleMetadataDefinition locale = locales[i]; Append(canonical, locale.Id.ToString());
                Append(canonical, locale.Name); Append(canonical, locale.Locale); Append(canonical, locale.Alias);
                Append(canonical, locale.FileIcon); Append(canonical, locale.FileIconSelected); Append(canonical, locale.LoaderImage);
                Append(canonical, locale.PreloaderImage); Append(canonical, locale.IsAsian); Append(canonical, locale.Fonts != null);
                if (locale.Fonts != null)
                {
                    Append(canonical, locale.Fonts.Content); Append(canonical, locale.Fonts.Title); Append(canonical, locale.Fonts.Button);
                    Append(canonical, locale.Fonts.FontSizeScale); Append(canonical, locale.Fonts.LineSpacing);
                    Append(canonical, locale.Fonts.CustomLineSpacingScale);
                }
            }

            var locations = new List<LocationDefinition>(content.Locations);
            locations.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "locations"); Append(canonical, locations.Count);
            for (int i = 0; i < locations.Count; i++)
            {
                LocationDefinition location = locations[i]; Append(canonical, location.Id.ToString()); Append(canonical, location.Color);
                Append(canonical, location.Wall); Append(canonical, location.Floor); Append(canonical, location.PositionY);
                Append(canonical, location.Width); Append(canonical, location.Height); Append(canonical, location.MinWidth);
                Append(canonical, location.FrictionForce); Append(canonical, location.GridSize);
                Append(canonical, location.HasMusic ? location.Music.ToString() : string.Empty); Append(canonical, location.Layers.Count);
                for (int j = 0; j < location.Layers.Count; j++)
                {
                    LocationLayerDefinition layer = location.Layers[j]; Append(canonical, layer.Type); Append(canonical, layer.Factor);
                    if (layer.Fighters != null)
                    {
                        Append(canonical, "fighters"); Append(canonical, layer.Fighters.PlayerX); Append(canonical, layer.Fighters.PlayerY);
                        Append(canonical, layer.Fighters.EnemyX); Append(canonical, layer.Fighters.EnemyY);
                    }
                    Append(canonical, layer.Scaling); Append(canonical, layer.Images.Count);
                    for (int k = 0; k < layer.Images.Count; k++)
                    {
                        LocationImageDefinition image = layer.Images[k]; Append(canonical, image.Sprite.ToString());
                        Append(canonical, image.X); Append(canonical, image.Y); Append(canonical, image.Width); Append(canonical, image.Height);
                        Append(canonical, image.IsOpaque); Append(canonical, image.FlipX); Append(canonical, image.FlipY); Append(canonical, image.IsMask);
                    }
                }
            }

            var templates = new List<MoveTemplateDefinition>(content.MoveTemplates);
            templates.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "move-templates"); Append(canonical, templates.Count);
            for (int i = 0; i < templates.Count; i++) AppendMoveNode(canonical, templates[i], string.Empty);

            var moves = new List<MoveDefinition>(content.Moves);
            moves.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "moves"); Append(canonical, moves.Count);
            for (int i = 0; i < moves.Count; i++) AppendMoveNode(canonical, moves[i], moves[i].Animation.ToString());

            var triggers = new List<MoveTriggerDefinition>(content.MoveTriggers);
            triggers.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "move-triggers"); Append(canonical, triggers.Count);
            for (int i = 0; i < triggers.Count; i++)
            {
                MoveTriggerDefinition trigger = triggers[i]; Append(canonical, trigger.Id.ToString());
                AppendMoveEvents(canonical, trigger.Events); AppendMoveConditions(canonical, trigger.Conditions);
                Append(canonical, trigger.Actions.Count);
                for (int j = 0; j < trigger.Actions.Count; j++)
                {
                    ModMoveAction action = trigger.Actions[j]; Append(canonical, (int)action.Kind); Append(canonical, action.Audio.ToString());
                    Append(canonical, action.Name); Append(canonical, action.Volume); Append(canonical, action.Looped);
                }
            }

            var tactics = new List<TacticDefinition>(content.Tactics);
            tactics.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "tactics"); Append(canonical, tactics.Count);
            for (int i = 0; i < tactics.Count; i++)
            {
                TacticDefinition tactic = tactics[i]; Append(canonical, tactic.Id.ToString()); Append(canonical, (int)tactic.Kind);
                Append(canonical, tactic.CoreTemplate); Append(canonical, tactic.MemoryStrikes); Append(canonical, tactic.MemoryRoundFactor);
                AppendTacticValue(canonical, tactic.CounterAttack); AppendTacticValue(canonical, tactic.Dodge);
                AppendTacticValue(canonical, tactic.Block); AppendTacticValue(canonical, tactic.SafeAttack);
                AppendTacticValue(canonical, tactic.TableAttack); AppendTacticValue(canonical, tactic.CautiousMovement);
                AppendTacticValue(canonical, tactic.DodgeMissiles); AppendTacticValue(canonical, tactic.DodgeMagic);
                AppendTacticAnimations(canonical, tactic.AnimationWeights); AppendTacticAnimations(canonical, tactic.QuickAttacks);
                AppendTacticAnimations(canonical, tactic.Evades); AppendTacticAnimations(canonical, tactic.ExpectedWait);
            }
        }

        private static void AppendMoveNode(StringBuilder canonical, MoveNodeDefinition node, string animation)
        {
            Append(canonical, node.Id.ToString()); Append(canonical, animation); AppendIds(canonical, node.Templates);
            AppendStrings(canonical, node.CoreTemplates); AppendMoveEvents(canonical, node.Events); AppendMoveConditions(canonical, node.Conditions);
            Append(canonical, node.Intervals.Count);
            for (int i = 0; i < node.Intervals.Count; i++) { Append(canonical, node.Intervals[i].Type); Append(canonical, node.Intervals[i].Name); }
            Append(canonical, node.Type); Append(canonical, node.Priority); Append(canonical, node.MidFrames);
            Append(canonical, node.FirstFrame); Append(canonical, node.EndFrame); Append(canonical, node.MirrorNode);
            Append(canonical, node.TacticEquivalent); Append(canonical, node.TacticWeapon); Append(canonical, node.Looped); Append(canonical, node.EndsStage);
        }

        private static void AppendMoveEvents(StringBuilder canonical, IReadOnlyList<ModMoveEvent> events)
        {
            Append(canonical, events.Count);
            for (int i = 0; i < events.Count; i++)
            { Append(canonical, (int)events[i].Kind); Append(canonical, events[i].Name); Append(canonical, events[i].Player); }
        }

        private static void AppendMoveConditions(StringBuilder canonical, IReadOnlyList<ModMoveCondition> conditions)
        {
            Append(canonical, conditions.Count);
            for (int i = 0; i < conditions.Count; i++) AppendMoveCondition(canonical, conditions[i]);
        }

        private static void AppendMoveCondition(StringBuilder canonical, ModMoveCondition condition)
        {
            Append(canonical, (int)condition.Kind); Append(canonical, condition.Name); Append(canonical, condition.Player);
            Append(canonical, condition.ItemType); Append(canonical, condition.ItemSubType); Append(canonical, condition.Not);
            Append(canonical, condition.Children.Count);
            for (int i = 0; i < condition.Children.Count; i++) AppendMoveCondition(canonical, condition.Children[i]);
        }

        private static void AppendTacticAnimations(StringBuilder canonical, IReadOnlyList<ModTacticAnimationValue> values)
        {
            Append(canonical, values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                ModTacticAnimationValue value = values[i]; Append(canonical, value.HasMove ? value.Move.ToString() : string.Empty);
                Append(canonical, value.Animation); AppendTacticValue(canonical, value.Value);
            }
        }

        private static void AppendTacticValue(StringBuilder canonical, ModTacticValue value)
        {
            Append(canonical, value != null);
            if (value == null) return;
            Append(canonical, value.Base); Append(canonical, value.CounterFactor); Append(canonical, value.DamageFactor);
            Append(canonical, value.HealthFactor); Append(canonical, value.EnemyHealthFactor); Append(canonical, value.AnimationFramesFactor);
            Append(canonical, value.ChildFramesFactor); Append(canonical, value.MagicBulletFactor); Append(canonical, value.MissileBulletFactor);
            Append(canonical, value.HitFactor); Append(canonical, value.DistanceFactor); Append(canonical, value.Shift);
            Append(canonical, value.Limit); Append(canonical, value.AntiLimit); Append(canonical, (int)value.FactorType);
        }

        private static void AppendIds(StringBuilder canonical, IReadOnlyList<DefinitionId> ids)
        {
            Append(canonical, ids.Count);
            for (int i = 0; i < ids.Count; i++) Append(canonical, ids[i].ToString());
        }

        private static void AppendStrings(StringBuilder canonical, IReadOnlyList<string> values)
        {
            Append(canonical, values.Count);
            for (int i = 0; i < values.Count; i++) Append(canonical, values[i]);
        }

        private static void AppendParameterValues(StringBuilder canonical,
            IReadOnlyDictionary<string, ModParameterValue> values)
        {
            var names = new List<string>(values.Keys);
            names.Sort(StringComparer.Ordinal);
            Append(canonical, names.Count);
            for (int i = 0; i < names.Count; i++)
            {
                string name = names[i];
                Append(canonical, name);
                Append(canonical, ((int)values[name].Type).ToString(CultureInfo.InvariantCulture));
                Append(canonical, values[name].ToWireString());
            }
        }

        private static int CompareIds(DefinitionId left, DefinitionId right)
        {
            return string.CompareOrdinal(left.ToString(), right.ToString());
        }

        private static void Append(StringBuilder builder, int value)
        {
            Append(builder, value.ToString(CultureInfo.InvariantCulture));
        }

        private static void Append(StringBuilder builder, uint value)
        {
            Append(builder, value.ToString(CultureInfo.InvariantCulture));
        }

        private static void Append(StringBuilder builder, float value)
        {
            Append(builder, value.ToString("R", CultureInfo.InvariantCulture));
        }

        private static void Append(StringBuilder builder, bool value)
        {
            Append(builder, value ? "1" : "0");
        }

        private static void Append(StringBuilder builder, string value)
        {
            value = value ?? string.Empty;
            builder.Append(value.Length.ToString(CultureInfo.InvariantCulture)).Append(':').Append(value).Append(';');
        }
    }

    public sealed class ModStateDefinition
    {
        public const int MaxVersion = 1000000;
        public const int MaxAliases = 64;
        public const int MaxTombstones = 64;

        private readonly IReadOnlyDictionary<string, string> _aliases;
        private readonly IReadOnlyCollection<string> _tombstones;

        public ModId Owner { get; }
        public int Version { get; }
        public ModParameterSchema Fields { get; }
        public IReadOnlyDictionary<string, string> Aliases => _aliases;
        public IReadOnlyCollection<string> Tombstones => _tombstones;

        internal ModStateDefinition(ModId owner, int version, ModParameterSchema fields,
            IReadOnlyDictionary<string, string> aliases, IReadOnlyCollection<string> tombstones)
        {
            if (version < 1 || version > MaxVersion)
                throw new ModContentException("State schema version must be 1.." + MaxVersion + ".");
            Owner = owner;
            Version = version;
            Fields = fields ?? throw new ArgumentNullException(nameof(fields));

            var aliasCopy = new Dictionary<string, string>(StringComparer.Ordinal);
            var aliasTargets = new HashSet<string>(StringComparer.Ordinal);
            if (aliases != null)
            {
                foreach (KeyValuePair<string, string> pair in aliases)
                {
                    if (aliasCopy.Count >= MaxAliases)
                        throw new ModContentException("State alias limit exceeded (" + MaxAliases + ").");
                    ModParameterDefinition.ValidateName(pair.Key);
                    ModParameterDefinition.ValidateName(pair.Value);
                    if (pair.Key == pair.Value)
                        throw new ModContentException("State alias cannot target itself: '" + pair.Key + "'.");
                    ModParameterDefinition current;
                    if (Fields.TryGet(pair.Key, out current))
                        throw new ModContentException("State alias source is still a current field: '" + pair.Key + "'.");
                    if (!Fields.TryGet(pair.Value, out current))
                        throw new ModContentException("State alias target is not a current field: '" + pair.Value + "'.");
                    if (!aliasTargets.Add(pair.Value))
                        throw new ModContentException("Multiple state aliases target '" + pair.Value + "'.");
                    aliasCopy.Add(pair.Key, pair.Value);
                }
            }

            var tombstoneCopy = new HashSet<string>(StringComparer.Ordinal);
            if (tombstones != null)
            {
                foreach (string name in tombstones)
                {
                    if (tombstoneCopy.Count >= MaxTombstones)
                        throw new ModContentException("State tombstone limit exceeded (" + MaxTombstones + ").");
                    ModParameterDefinition.ValidateName(name);
                    ModParameterDefinition current;
                    if (Fields.TryGet(name, out current))
                        throw new ModContentException("State tombstone is still a current field: '" + name + "'.");
                    if (aliasCopy.ContainsKey(name))
                        throw new ModContentException("State field cannot be both alias and tombstone: '" + name + "'.");
                    tombstoneCopy.Add(name);
                }
            }

            for (int i = 0; i < Fields.Parameters.Count; i++)
            {
                ModParameterDefinition field = Fields.Parameters[i];
                if (field.Required && !field.HasDefault)
                    throw new ModContentException("Required state field '" + field.Name +
                        "' must declare a default so a new save can initialize deterministically.");
            }

            _aliases = new System.Collections.ObjectModel.ReadOnlyDictionary<string, string>(aliasCopy);
            _tombstones = new System.Collections.ObjectModel.ReadOnlyCollection<string>(
                new List<string>(tombstoneCopy));
        }
    }

    public interface IModStateMigrationScriptContext
    {
        bool TryMigrateState(int fromVersion, IReadOnlyDictionary<string, ModParameterValue> values,
            out IReadOnlyDictionary<string, ModParameterValue> migrated, out string error);
    }

    public sealed class ModStateRuntime
    {
        public const string StateNodeName = "State";
        public const string ValueNodeName = "Value";
        public const string Format = "1";
        public const int MaxSavedValues = 256;

        private sealed class BoundState
        {
            public ModStateDefinition Definition;
            public XmlElement ModNode;
            public XmlElement StateNode;
            public Dictionary<string, ModParameterValue> Values;
        }

        private readonly Dictionary<ModId, ModStateDefinition> _definitions =
            new Dictionary<ModId, ModStateDefinition>();
        private readonly Dictionary<ModId, BoundState> _bound = new Dictionary<ModId, BoundState>();
        private bool _definitionsFrozen;

        public bool DefinitionsFrozen => _definitionsFrozen;

        public IReadOnlyCollection<ModStateDefinition> Definitions
        {
            get { return new List<ModStateDefinition>(_definitions.Values).AsReadOnly(); }
        }

        public ModStateDefinition RegisterDefinition(ModDescriptor mod, int version, ModParameterSchema fields,
            IReadOnlyDictionary<string, string> aliases = null, IReadOnlyCollection<string> tombstones = null)
        {
            if (mod == null) throw new ArgumentNullException(nameof(mod));
            if (_definitionsFrozen) throw new InvalidOperationException("State schemas are frozen.");
            if (_definitions.ContainsKey(mod.Id))
                throw new ModContentException("Mod '" + mod.Id + "' registered state more than once.");
            var definition = new ModStateDefinition(mod.Id, version, fields, aliases, tombstones);
            _definitions.Add(mod.Id, definition);
            return definition;
        }

        public void RemoveDefinition(ModId owner)
        {
            if (_definitionsFrozen) return;
            _definitions.Remove(owner);
            _bound.Remove(owner);
        }

        public void FreezeDefinitions()
        {
            _definitionsFrozen = true;
        }

        public bool TryGetDefinition(ModId owner, out ModStateDefinition definition)
        {
            return _definitions.TryGetValue(owner, out definition);
        }

        public IReadOnlyList<ModDiagnostic> Bind(XmlNode warrior, IReadOnlyList<IModScriptContext> contexts)
        {
            if (warrior == null) throw new ArgumentNullException(nameof(warrior));
            _bound.Clear();
            var diagnostics = new List<ModDiagnostic>();
            var contextByMod = new Dictionary<ModId, IModStateMigrationScriptContext>();
            if (contexts != null)
            {
                for (int i = 0; i < contexts.Count; i++)
                {
                    IModScriptContext context = contexts[i];
                    IModStateMigrationScriptContext stateContext = context as IModStateMigrationScriptContext;
                    if (context != null && stateContext != null) contextByMod[context.Mod.Id] = stateContext;
                }
            }

            XmlElement mods = warrior["EclipseMods"];
            if (mods == null || mods.GetAttribute("schema") != "1")
            {
                foreach (ModStateDefinition definition in _definitions.Values)
                    diagnostics.Add(new ModDiagnostic(ModDiagnosticSeverity.Error, "STATE001", definition.Owner.Value,
                        "Cannot bind mod state because EclipseMods schema 1 is unavailable."));
                return diagnostics.AsReadOnly();
            }

            var definitions = new List<ModStateDefinition>(_definitions.Values);
            definitions.Sort((left, right) => string.CompareOrdinal(left.Owner.Value, right.Owner.Value));
            for (int i = 0; i < definitions.Count; i++)
            {
                ModStateDefinition definition = definitions[i];
                XmlElement modNode = FindModNode(mods, definition.Owner);
                if (modNode == null)
                {
                    diagnostics.Add(new ModDiagnostic(ModDiagnosticSeverity.Error, "STATE002", definition.Owner.Value,
                        "Active mod has no EclipseMods ownership node."));
                    continue;
                }

                string error;
                BoundState bound;
                IModStateMigrationScriptContext migration;
                contextByMod.TryGetValue(definition.Owner, out migration);
                if (!TryBindDefinition(modNode, definition, migration, out bound, out error))
                {
                    diagnostics.Add(new ModDiagnostic(ModDiagnosticSeverity.Error, "STATE003", definition.Owner.Value,
                        error));
                    continue;
                }
                modNode.SetAttribute("stateSchema", definition.Version.ToString(CultureInfo.InvariantCulture));
                _bound[definition.Owner] = bound;
            }
            return diagnostics.AsReadOnly();
        }

        public bool TryGetValue(ModId owner, string name, out ModParameterValue value)
        {
            ModParameterDefinition.ValidateName(name);
            BoundState state = RequireBound(owner);
            return state.Values.TryGetValue(name, out value);
        }

        public IReadOnlyDictionary<string, ModParameterValue> Snapshot(ModId owner)
        {
            BoundState state = RequireBound(owner);
            return new System.Collections.ObjectModel.ReadOnlyDictionary<string, ModParameterValue>(
                new Dictionary<string, ModParameterValue>(state.Values, StringComparer.Ordinal));
        }

        public void SetValues(ModId owner, IReadOnlyDictionary<string, ModParameterValue> changes)
        {
            if (changes == null) throw new ArgumentNullException(nameof(changes));
            BoundState state = RequireBound(owner);
            var next = new Dictionary<string, ModParameterValue>(state.Values, StringComparer.Ordinal);
            foreach (KeyValuePair<string, ModParameterValue> pair in changes)
            {
                ModParameterDefinition field;
                if (!state.Definition.Fields.TryGet(pair.Key, out field))
                    throw new ModContentException("Unknown state field '" + pair.Key + "' for mod '" + owner + "'.");
                if (field.Type != pair.Value.Type)
                    throw new ModContentException("State field '" + pair.Key + "' has type " + pair.Value.Type +
                        ", expected " + field.Type + ".");
                next[pair.Key] = pair.Value;
            }
            next = state.Definition.Fields.ResolveValues(next);
            CommitKnownValues(state, next);
        }

        public void UnsetValue(ModId owner, string name)
        {
            ModParameterDefinition.ValidateName(name);
            BoundState state = RequireBound(owner);
            ModParameterDefinition field;
            if (!state.Definition.Fields.TryGet(name, out field))
                throw new ModContentException("Unknown state field '" + name + "' for mod '" + owner + "'.");
            var next = new Dictionary<string, ModParameterValue>(state.Values, StringComparer.Ordinal);
            next.Remove(name);
            next = state.Definition.Fields.ResolveValues(next);
            CommitKnownValues(state, next);
        }

        private static bool TryBindDefinition(XmlElement modNode, ModStateDefinition definition,
            IModStateMigrationScriptContext migration, out BoundState bound, out string error)
        {
            bound = null;
            error = string.Empty;
            XmlElement originalState = modNode[StateNodeName];
            int savedVersion = definition.Version;
            var raw = new Dictionary<string, ModParameterValue>(StringComparer.Ordinal);
            if (originalState != null)
            {
                if (!string.Equals(originalState.GetAttribute("format"), Format, StringComparison.Ordinal))
                {
                    error = "Unsupported mod state format '" + originalState.GetAttribute("format") + "'.";
                    return false;
                }
                if (!int.TryParse(originalState.GetAttribute("version"), NumberStyles.Integer,
                        CultureInfo.InvariantCulture, out savedVersion) || savedVersion < 1)
                {
                    error = "Mod state has an invalid schema version.";
                    return false;
                }
                if (!TryReadRaw(originalState, out raw, out error)) return false;
            }

            if (savedVersion > definition.Version)
            {
                error = "Saved state schema " + savedVersion + " is newer than supported schema " +
                    definition.Version + ". State was left untouched.";
                return false;
            }

            int version = savedVersion;
            while (version < definition.Version)
            {
                if (migration == null)
                {
                    error = "State migration " + version + " -> " + (version + 1) + " is unavailable.";
                    return false;
                }
                IReadOnlyDictionary<string, ModParameterValue> migrated;
                string migrationError;
                if (!migration.TryMigrateState(version, raw, out migrated, out migrationError))
                {
                    error = "State migration " + version + " -> " + (version + 1) + " failed: " +
                        (migrationError ?? string.Empty);
                    return false;
                }
                if (migrated == null)
                {
                    error = "State migration " + version + " -> " + (version + 1) + " returned no state.";
                    return false;
                }
                if (migrated.Count > MaxSavedValues)
                {
                    error = "Migrated state exceeds the saved value limit (" + MaxSavedValues + ").";
                    return false;
                }
                var migratedCopy = new Dictionary<string, ModParameterValue>(StringComparer.Ordinal);
                foreach (KeyValuePair<string, ModParameterValue> pair in migrated)
                    migratedCopy.Add(pair.Key, pair.Value);
                raw = migratedCopy;
                version++;
            }

            ApplyRedirects(definition, raw);
            Dictionary<string, ModParameterValue> known;
            if (!TryValidateCurrent(definition, raw, out known, out error)) return false;

            XmlElement candidate = originalState == null
                ? modNode.OwnerDocument.CreateElement(StateNodeName)
                : (XmlElement)originalState.CloneNode(true);
            RewriteKnownValues(candidate, definition, known);
            if (originalState == null) modNode.AppendChild(candidate);
            else modNode.ReplaceChild(candidate, originalState);

            bound = new BoundState
            {
                Definition = definition,
                ModNode = modNode,
                StateNode = candidate,
                Values = known,
            };
            return true;
        }

        private static bool TryReadRaw(XmlElement stateNode, out Dictionary<string, ModParameterValue> values,
            out string error)
        {
            values = new Dictionary<string, ModParameterValue>(StringComparer.Ordinal);
            error = string.Empty;
            foreach (XmlNode child in stateNode.ChildNodes)
            {
                XmlElement element = child as XmlElement;
                if (element == null || element.Name != ValueNodeName) continue;
                if (values.Count >= MaxSavedValues)
                {
                    error = "Saved mod state exceeds the value limit (" + MaxSavedValues + ").";
                    return false;
                }
                string name = element.GetAttribute("name");
                try { ModParameterDefinition.ValidateName(name); }
                catch (ModContentException exception) { error = exception.Message; return false; }
                if (values.ContainsKey(name))
                {
                    error = "Saved state field '" + name + "' appears more than once.";
                    return false;
                }
                ModParameterType type;
                if (!TryParseType(element.GetAttribute("type"), out type))
                {
                    error = "Saved state field '" + name + "' has unsupported type '" +
                        element.GetAttribute("type") + "'.";
                    return false;
                }
                ModParameterValue value;
                if (!ModParameterValue.TryParse(type, element.GetAttribute("value"), out value))
                {
                    error = "Saved state field '" + name + "' has an invalid " + type + " value.";
                    return false;
                }
                values.Add(name, value);
            }
            return true;
        }

        private static void ApplyRedirects(ModStateDefinition definition,
            Dictionary<string, ModParameterValue> values)
        {
            var aliasNames = new List<string>(definition.Aliases.Keys);
            aliasNames.Sort(StringComparer.Ordinal);
            for (int i = 0; i < aliasNames.Count; i++)
            {
                string oldName = aliasNames[i];
                ModParameterValue value;
                if (!values.TryGetValue(oldName, out value)) continue;
                string currentName = definition.Aliases[oldName];
                if (!values.ContainsKey(currentName)) values[currentName] = value;
                values.Remove(oldName);
            }
            foreach (string tombstone in definition.Tombstones) values.Remove(tombstone);
        }

        private static bool TryValidateCurrent(ModStateDefinition definition,
            Dictionary<string, ModParameterValue> raw, out Dictionary<string, ModParameterValue> known,
            out string error)
        {
            known = new Dictionary<string, ModParameterValue>(StringComparer.Ordinal);
            error = string.Empty;
            foreach (KeyValuePair<string, ModParameterValue> pair in raw)
            {
                ModParameterDefinition field;
                if (!definition.Fields.TryGet(pair.Key, out field)) continue;
                if (field.Type != pair.Value.Type)
                {
                    error = "Saved state field '" + pair.Key + "' has type " + pair.Value.Type +
                        ", expected " + field.Type + ".";
                    return false;
                }
                known.Add(pair.Key, pair.Value);
            }
            try { known = definition.Fields.ResolveValues(known); }
            catch (ModContentException exception) { error = exception.Message; return false; }
            return true;
        }

        private static void RewriteKnownValues(XmlElement stateNode, ModStateDefinition definition,
            IReadOnlyDictionary<string, ModParameterValue> known)
        {
            stateNode.SetAttribute("format", Format);
            stateNode.SetAttribute("version", definition.Version.ToString(CultureInfo.InvariantCulture));
            var ownedNames = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < definition.Fields.Parameters.Count; i++)
                ownedNames.Add(definition.Fields.Parameters[i].Name);
            foreach (string alias in definition.Aliases.Keys) ownedNames.Add(alias);
            foreach (string tombstone in definition.Tombstones) ownedNames.Add(tombstone);

            var remove = new List<XmlNode>();
            foreach (XmlNode child in stateNode.ChildNodes)
            {
                XmlElement value = child as XmlElement;
                if (value != null && value.Name == ValueNodeName && ownedNames.Contains(value.GetAttribute("name")))
                    remove.Add(value);
            }
            for (int i = 0; i < remove.Count; i++) stateNode.RemoveChild(remove[i]);

            var names = new List<string>(known.Keys);
            names.Sort(StringComparer.Ordinal);
            for (int i = 0; i < names.Count; i++)
            {
                string name = names[i];
                ModParameterValue value = known[name];
                XmlElement element = stateNode.OwnerDocument.CreateElement(ValueNodeName);
                element.SetAttribute("name", name);
                element.SetAttribute("type", TypeName(value.Type));
                element.SetAttribute("value", value.ToWireString());
                stateNode.AppendChild(element);
            }
        }

        private static void CommitKnownValues(BoundState state, Dictionary<string, ModParameterValue> values)
        {
            XmlElement candidate = (XmlElement)state.StateNode.CloneNode(true);
            RewriteKnownValues(candidate, state.Definition, values);
            state.ModNode.ReplaceChild(candidate, state.StateNode);
            state.StateNode = candidate;
            state.Values = values;
        }

        private BoundState RequireBound(ModId owner)
        {
            BoundState state;
            if (!_definitions.ContainsKey(owner))
                throw new ModContentException("Mod '" + owner + "' did not register a state schema.");
            if (!_bound.TryGetValue(owner, out state))
                throw new ModContentException("State for mod '" + owner + "' is not bound to a loaded player save.");
            return state;
        }

        private static XmlElement FindModNode(XmlElement mods, ModId owner)
        {
            foreach (XmlNode child in mods.ChildNodes)
            {
                XmlElement element = child as XmlElement;
                if (element != null && element.Name == "Mod" && element.GetAttribute("id") == owner.Value)
                    return element;
            }
            return null;
        }

        private static bool TryParseType(string text, out ModParameterType type)
        {
            switch (text)
            {
                case "number": type = ModParameterType.Number; return true;
                case "integer": type = ModParameterType.Integer; return true;
                case "boolean": type = ModParameterType.Boolean; return true;
                case "string": type = ModParameterType.String; return true;
                default: type = default; return false;
            }
        }

        private static string TypeName(ModParameterType type)
        {
            switch (type)
            {
                case ModParameterType.Number: return "number";
                case ModParameterType.Integer: return "integer";
                case ModParameterType.Boolean: return "boolean";
                case ModParameterType.String: return "string";
                default: throw new InvalidOperationException("Unsupported state type: " + type + ".");
            }
        }
    }

    public sealed class ModEffectInstance
    {
        private readonly IReadOnlyDictionary<string, ModParameterValue> _values;

        public DefinitionId Owner { get; }
        public IReadOnlyDictionary<string, ModParameterValue> Values => _values;

        public ModEffectInstance(DefinitionId owner, IDictionary<string, ModParameterValue> values)
        {
            Owner = owner;
            _values = new System.Collections.ObjectModel.ReadOnlyDictionary<string, ModParameterValue>(
                new Dictionary<string, ModParameterValue>(values, StringComparer.Ordinal));
        }
    }

    // Eclipse-owned typed state lives beside the recovered <Set> node, never inside it.
    // This keeps arbitrary Lua parameter names out of PerkInfoItem/PerkSetAttributes while
    // allowing missing mods and future fields to round-trip as opaque XML.
    public static class ModEffectSaveData
    {
        public const string NodeName = "EclipseParams";
        public const string ParameterNodeName = "Param";
        public const string Format = "1";

        public static bool TryRead(XmlNode effectNode, DefinitionId owner, ModParameterSchema schema,
            out ModEffectInstance instance, out string error)
        {
            instance = null;
            error = string.Empty;
            if (effectNode == null) { error = "Effect node is missing."; return false; }
            if (schema == null) { error = "Parameter schema is missing."; return false; }

            XmlNode paramsNode = effectNode[NodeName];
            if (paramsNode != null)
            {
                string format = paramsNode.Attributes?["Format"]?.Value;
                if (!string.Equals(format, Format, StringComparison.Ordinal))
                { error = "Unsupported Eclipse parameter format '" + (format ?? string.Empty) + "'."; return false; }
            }

            var raw = new Dictionary<string, string>(StringComparer.Ordinal);
            if (paramsNode != null)
            {
                foreach (XmlNode child in paramsNode.ChildNodes)
                {
                    if (child.NodeType != XmlNodeType.Element || child.Name != ParameterNodeName) continue;
                    string name = child.Attributes?["Name"]?.Value;
                    if (string.IsNullOrEmpty(name)) continue;
                    ModParameterDefinition known;
                    if (!schema.TryGet(name, out known)) continue;
                    if (raw.ContainsKey(name))
                    { error = "Parameter '" + name + "' is saved more than once."; return false; }
                    XmlAttribute value = child.Attributes?["Value"];
                    if (value == null)
                    { error = "Parameter '" + name + "' has no Value attribute."; return false; }
                    raw.Add(name, value.Value);
                }
            }

            var supplied = new Dictionary<string, ModParameterValue>(StringComparer.Ordinal);
            foreach (KeyValuePair<string, string> pair in raw)
            {
                ModParameterDefinition definition;
                schema.TryGet(pair.Key, out definition);
                ModParameterValue value;
                if (!ModParameterValue.TryParse(definition.Type, pair.Value, out value))
                { error = "Parameter '" + pair.Key + "' is not a valid " + definition.Type + "."; return false; }
                supplied.Add(pair.Key, value);
            }

            Dictionary<string, ModParameterValue> resolved;
            try { resolved = schema.ResolveValues(supplied); }
            catch (ModContentException exception) { error = exception.Message; return false; }
            instance = new ModEffectInstance(owner, resolved);
            return true;
        }

        public static void Write(XmlElement effectNode, DefinitionId owner, ModParameterSchema schema,
            IReadOnlyDictionary<string, ModParameterValue> values)
        {
            if (effectNode == null) throw new ArgumentNullException(nameof(effectNode));
            if (schema == null) throw new ArgumentNullException(nameof(schema));
            Dictionary<string, ModParameterValue> resolved = schema.ResolveValues(values);

            XmlElement paramsNode = effectNode[NodeName] as XmlElement;
            if (paramsNode != null)
            {
                string format = paramsNode.GetAttribute("Format");
                if (!string.Equals(format, Format, StringComparison.Ordinal))
                    throw new ModContentException("Cannot rewrite unsupported Eclipse parameter format '" + format + "'.");
            }
            else
            {
                paramsNode = effectNode.OwnerDocument.CreateElement(NodeName);
                paramsNode.SetAttribute("Format", Format);
                effectNode.AppendChild(paramsNode);
            }

            var knownNodes = new Dictionary<string, List<XmlElement>>(StringComparer.Ordinal);
            foreach (XmlNode child in paramsNode.ChildNodes)
            {
                XmlElement element = child as XmlElement;
                if (element == null || element.Name != ParameterNodeName) continue;
                string name = element.GetAttribute("Name");
                ModParameterDefinition ignored;
                if (!schema.TryGet(name, out ignored)) continue;
                List<XmlElement> nodes;
                if (!knownNodes.TryGetValue(name, out nodes))
                {
                    nodes = new List<XmlElement>();
                    knownNodes.Add(name, nodes);
                }
                nodes.Add(element);
            }

            for (int i = 0; i < schema.Parameters.Count; i++)
            {
                ModParameterDefinition definition = schema.Parameters[i];
                ModParameterValue value;
                bool hasValue = resolved.TryGetValue(definition.Name, out value);
                List<XmlElement> nodes;
                knownNodes.TryGetValue(definition.Name, out nodes);
                if (!hasValue)
                {
                    if (nodes != null) for (int j = 0; j < nodes.Count; j++) paramsNode.RemoveChild(nodes[j]);
                    continue;
                }

                XmlElement parameter;
                if (nodes == null || nodes.Count == 0)
                {
                    parameter = effectNode.OwnerDocument.CreateElement(ParameterNodeName);
                    paramsNode.AppendChild(parameter);
                }
                else
                {
                    parameter = nodes[0];
                    for (int j = 1; j < nodes.Count; j++) paramsNode.RemoveChild(nodes[j]);
                }
                parameter.SetAttribute("Name", definition.Name);
                parameter.SetAttribute("Value", value.ToWireString());
            }
        }
    }
}
