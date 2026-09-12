using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Eclipse.Modding
{
    public sealed partial class LegacyContentAdapter : IDisposable
    {
        private readonly ModContentCatalog _content;
        private readonly List<string> _itemNames = new List<string>();
        private readonly List<string> _localizationKeys = new List<string>();
        private readonly List<string> _perkNames = new List<string>();
        private readonly List<string> _itemSetNames = new List<string>();
        private readonly List<string> _forgeRecipeNames = new List<string>();
        private readonly List<IDisposable> _forgeExclusionLifetimes = new List<IDisposable>();
        private readonly List<IDisposable> _forgeDeviationLifetimes = new List<IDisposable>();
        private readonly List<IDisposable> _defaultEnchantmentLifetimes = new List<IDisposable>();
        private readonly List<IDisposable> _innatePerkLifetimes = new List<IDisposable>();
        private readonly List<IDisposable> _tacticSubtypeLifetimes = new List<IDisposable>();
        private readonly List<ProgressionBranchBinding> _progressionBindings = new List<ProgressionBranchBinding>();
        private readonly List<string> _externalZoneNames = new List<string>();
        private readonly List<ExternalBattleBinding> _externalBattles = new List<ExternalBattleBinding>();
        private readonly List<QuestStage> _externalQuests = new List<QuestStage>();
        private readonly List<BattleSourceBinding> _battleSourceBindings = new List<BattleSourceBinding>();
        private readonly List<ExternalEnchantmentBinding> _enchantmentBindings =
            new List<ExternalEnchantmentBinding>();
        private Items _items;
        private PerkItems _perks;
        private ForgeManager _forge;
        private bool _itemsApplied;
        private bool _perksApplied;
        private bool _languageSubscribed;
        private bool _disposed;

        public LegacyContentAdapter(ModContentCatalog content)
        {
            _content = content ?? throw new ArgumentNullException(nameof(content));
            if (!content.IsFrozen)
                throw new InvalidOperationException("Legacy content may only adapt a frozen definition catalog.");
        }

        public void ApplyItems(Items items)
        {
            ThrowIfDisposed();
            if (_itemsApplied) throw new InvalidOperationException("Legacy items are already applied.");
            _items = items ?? throw new ArgumentNullException(nameof(items));

            foreach (ShopListingDefinition listing in _content.ShopListings)
            {
                ItemDefinition definition;
                if (!_content.TryGetItem(listing.Item, out definition))
                    throw new InvalidOperationException("Committed shop listing has no item: " + listing.Item);
                if (_items.KCCDBEEKBCG(definition.Id.ToString()) != null)
                    throw new InvalidOperationException("Legacy item already exists: " + definition.Id);
            }

            try
            {
                foreach (NonEquipmentItemDefinition definition in _content.NonEquipmentItems)
                {
                    if (definition.IsCore) continue;
                    if (_items.KCCDBEEKBCG(definition.Id.ToString()) != null)
                        throw new InvalidOperationException("Legacy item already exists: " + definition.Id);
                    ItemInfo item = _items.AddExternalItem(BuildNonEquipmentItemNode(definition));
                    _itemNames.Add(item.Name);
                }
                foreach (ShopListingDefinition listing in _content.ShopListings)
                {
                    ItemDefinition definition;
                    if (!_content.TryGetItem(listing.Item, out definition)) continue;
                    XmlElement node = BuildItemNode(definition, listing);
                    ItemInfo item = _items.AddExternalItem(node);
                    _itemNames.Add(item.Name);
                }
                foreach (ItemSetDefinition definition in _content.ItemSets)
                {
                    if (definition.IsCore) continue;
                    ItemSet itemSet = _items.DGKMILIPLLF().AddExternalSet(BuildItemSetNode(definition));
                    _itemSetNames.Add(itemSet.Name);
                }
                _itemsApplied = true;
            }
            catch
            {
                RemoveItems();
                throw;
            }
        }

        public void ApplyLocalization()
        {
            ThrowIfDisposed();
            RemoveLocalization();

            string language = LocalizationManager.ILAJKOBCHFH == null
                ? LocalizationManager.POIPGLLCCKC
                : LocalizationManager.ILAJKOBCHFH.name;
            foreach (LocalizationDefinition localization in _content.Localizations)
            {
                string value = localization.GetOrEnglish(language);
                if (string.IsNullOrEmpty(value)) continue;
                string key = localization.Id.ToString();
                LocalizationManager.SetExternalString(key, value);
                _localizationKeys.Add(key);
            }

            ApplyP3Localization(language);

            // Core localization patches bind to the exact recovered legacy key. The
            // LocalizationManager overlay makes this reversible: disposing the mod removes
            // only the overlay and immediately exposes the base language value again.
            string activeLanguage = (language ?? string.Empty).ToLowerInvariant();
            string activeField = string.IsNullOrEmpty(activeLanguage)
                ? string.Empty
                : "values/" + activeLanguage;
            foreach (ModContentPatchRecord patch in _content.Patches)
            {
                if (patch.Operation != ModContentPatchOperation.Replace ||
                    patch.Target.Category != "localization" || patch.Target.Namespace.Value != "core" ||
                    !string.Equals(patch.Field, activeField, StringComparison.Ordinal)) continue;
                LocalizationDefinition localization;
                if (!_content.TryGetLocalization(patch.Target, out localization) ||
                    string.IsNullOrEmpty(localization.LegacyKey)) continue;
                string value;
                if (!localization.TryGet(activeLanguage, out value) || string.IsNullOrEmpty(value)) continue;
                LocalizationManager.SetExternalString(localization.LegacyKey, value);
                _localizationKeys.Add(localization.LegacyKey);
            }

            // Recovered shop/item UI usually localizes an ItemInfo by ItemInfo.Name rather than
            // by its optional Text/TextButton fields. Keep the canonical namespaced localization
            // definition available, but also publish the display string under the legacy item id.
            foreach (ShopListingDefinition listing in _content.ShopListings)
            {
                ItemDefinition item;
                if (!_content.TryGetItem(listing.Item, out item)) continue;
                LocalizationDefinition displayName;
                if (!_content.TryGetLocalization(item.DisplayName, out displayName)) continue;
                string value = displayName.GetOrEnglish(language);
                if (string.IsNullOrEmpty(value)) continue;
                string key = item.Id.ToString();
                LocalizationManager.SetExternalString(key, value);
                _localizationKeys.Add(key);
            }
            foreach (NonEquipmentItemDefinition item in _content.NonEquipmentItems)
            {
                if (item.IsCore) continue;
                LocalizationDefinition displayName;
                if (!_content.TryGetLocalization(item.DisplayName, out displayName)) continue;
                string value = displayName.GetOrEnglish(language);
                if (string.IsNullOrEmpty(value)) continue;
                string key = item.Id.ToString();
                LocalizationManager.SetExternalString(key, value);
                _localizationKeys.Add(key);
            }

            // The map footer localizes the runtime zone identity, while mod strings
            // are registered in the localization category. Bridge the documented
            // zones/<local-id> key without allowing arbitrary base-string aliases.
            foreach (ZoneDefinition zone in _content.Zones)
            {
                if (zone.IsCore) continue;
                LocalizationDefinition title;
                DefinitionId key = DefinitionId.Parse(zone.Id.Namespace + ":localization/zones/" + zone.Id.LocalId);
                if (!_content.TryGetLocalization(key, out title)) continue;
                LocalizationManager.SetExternalString(zone.LegacyName, title.GetOrEnglish(language));
                _localizationKeys.Add(zone.LegacyName);
            }

            if (!_languageSubscribed)
            {
                LocalizationManager.OCLBJLPOKLB += OnLanguageChanged;
                _languageSubscribed = true;
            }
        }

        public void ApplyPerksAndEnchantments(PerkItems perks, ForgeManager forge)
        {
            ThrowIfDisposed();
            if (_perksApplied) throw new InvalidOperationException("Legacy perks are already applied.");
            _perks = perks ?? throw new ArgumentNullException(nameof(perks));
            _forge = forge ?? throw new ArgumentNullException(nameof(forge));

            try
            {
                var visiting = new HashSet<DefinitionId>();
                foreach (PerkDefinition definition in _content.Perks)
                    if (!definition.IsCore) EnsurePerkApplied(definition, visiting);

                ApplyForgeExclusions();
                ApplyForgeDeviations();
                ApplyDefaultEnchantments();
                ApplyInnatePerks();
                ApplyTacticSubtypes();

                foreach (ForgeRecipeFamilyDefinition definition in _content.ForgeRecipeFamilies)
                {
                    ForgeEconomicProfileDefinition profile;
                    if (!_content.TryGetForgeEconomicProfile(definition.EconomicProfile, out profile))
                        throw new InvalidOperationException("Committed forge recipe lost its economic profile: " + definition.Id);
                    var items = new List<ExternalRecipeItemSpec>();
                    for (int i = 0; i < definition.Items.Count; i++)
                    {
                        ModForgeRecipeItem item = definition.Items[i];
                        items.Add(new ExternalRecipeItemSpec(EquipmentType(item.Equipment), item.Enchantments,
                            item.BarScale, item.MinDeviation, item.MaxDeviation, item.RandomAspect));
                    }
                    string recipeName = definition.Id.ToString();
                    if (!_forge.AddExternalRecipeFamily(recipeName, definition.Alias, profile.RuntimeRecipeName, items))
                        throw new InvalidOperationException("Could not add external forge recipe family '" + definition.Id + "'.");
                    _forgeRecipeNames.Add(recipeName);
                    for (int i = 0; i < definition.Candidates.Count; i++)
                    {
                        ModForgeRecipeCandidate candidate = definition.Candidates[i];
                        PerkDefinition perk;
                        if (!_content.TryGetPerk(candidate.Perk, out perk))
                            throw new InvalidOperationException("Forge recipe candidate lost perk '" + candidate.Perk + "'.");
                        string perkName = RuntimePerkName(perk);
                        string itemType = EquipmentType(candidate.Equipment);
                        string perkKind = perk.Kind == ModPerkKind.Combo ? "Combo" : "Single";
                        if (!_forge.AddExternalPerkCandidate(recipeName, itemType, perkName, perkKind,
                                candidate.MinLevel, candidate.MaxLevel))
                            throw new InvalidOperationException("Could not add forge candidate '" + candidate.Perk +
                                "' to " + definition.Id + "/" + itemType + ".");
                        _enchantmentBindings.Add(new ExternalEnchantmentBinding(recipeName, itemType, perkName));
                    }
                }

                foreach (var perk in _content.Perks)
                {
                    if (perk.IsCore || perk.Upgrades.Count == 0) continue;
                    var document = new XmlDocument();
                    var root = document.CreateElement("Upgrades"); document.AppendChild(root);
                    foreach (var upgrade in perk.Upgrades)
                    {
                        var node = document.CreateElement("UpgradeLevel");
                        node.SetAttribute("Value", upgrade.Level.ToString(CultureInfo.InvariantCulture));
                        node.SetAttribute("Description", upgrade.Description.ToString());
                        var set = document.CreateElement("Set");
                        foreach (var pair in upgrade.Parameters) set.SetAttribute(pair.Key, pair.Value);
                        node.AppendChild(set); root.AppendChild(node);
                    }
                    _perks.AddExternalPerkUpgrades(RuntimePerkName(perk), root);
                }

                foreach (ProgressionBranchOverlayDefinition overlay in GetProgressionOverlays())
                {
                    var items = new List<PerkTree.PerkItem>();
                    for (int i = 0; i < overlay.Entries.Count; i++)
                    {
                        ModProgressionPerkEntry entry = overlay.Entries[i];
                        PerkDefinition perk;
                        if (!_content.TryGetPerk(entry.Perk, out perk))
                            throw new InvalidOperationException("Progression branch lost perk '" + entry.Perk + "'.");
                        items.Add(new PerkTree.PerkItem(entry.Action == ModProgressionPerkAction.Upgrade
                            ? PerkTree.AAAIBJGLPAI.TYPE_UPGRADE : PerkTree.AAAIBJGLPAI.TYPE_PERK,
                            RuntimePerkName(perk), overlay.Level));
                    }
                    PerkTree tree = PerkTree.GBPBIPFIOJH();
                    PerkTree.PerkBranch previous = tree.ReplaceExternalBranch(overlay.Level, items);
                    _progressionBindings.Add(new ProgressionBranchBinding(overlay.Level, previous));
                }

                foreach (EnchantmentDefinition enchantment in _content.Enchantments)
                {
                    string perkName;
                    string perkKind;
                    IReadOnlyDictionary<string, ModParameterValue> initialParameters = null;
                    if (enchantment.HasPerk)
                    {
                        PerkDefinition perk;
                        if (!_content.TryGetPerk(enchantment.Perk, out perk))
                            throw new InvalidOperationException("Committed enchantment has no perk: " + enchantment.Id);
                        perkName = RuntimePerkName(perk);
                        perkKind = perk.Kind == ModPerkKind.Combo ? "Combo" : "Single";
                    }
                    else if (enchantment.HasBehavior)
                    {
                        EnsureScriptedEnchantmentApplied(enchantment);
                        perkName = enchantment.Id.ToString();
                        perkKind = enchantment.Kind == ModPerkKind.Combo ? "Combo" : "Single";
                        initialParameters = enchantment.InitialParameters;
                    }
                    else
                    {
                        throw new InvalidOperationException("Committed enchantment has no behavior backend: " + enchantment.Id);
                    }
                    string recipeName = RecipeName(enchantment.Recipe);
                    for (int i = 0; i < enchantment.Equipment.Count; i++)
                    {
                        string itemType = EquipmentType(enchantment.Equipment[i]);
                        if (!_forge.AddExternalEnchantmentCandidate(recipeName, itemType, perkName,
                                enchantment.Id.ToString(), perkKind, ToWireParameters(initialParameters)))
                            throw new InvalidOperationException("Could not add external enchantment '" + enchantment.Id +
                                "' to " + recipeName + "/" + itemType + ".");
                        _enchantmentBindings.Add(new ExternalEnchantmentBinding(recipeName, itemType, perkName));
                    }
                }
                _perksApplied = true;
            }
            catch
            {
                RemovePerksAndEnchantments();
                throw;
            }
        }

        public void ApplyStages(ListSF list)
        {
            ThrowIfDisposed();
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (_externalZoneNames.Count != 0 || _externalBattles.Count != 0 || _battleSourceBindings.Count != 0)
                throw new InvalidOperationException("Mod stage content is already applied.");
            try
            {
                ApplyCoreFightPatches(list);
                foreach (ZoneDefinition zone in _content.Zones)
                {
                    if (zone.IsCore) continue;
                    list.AddExternalZone(BuildZoneNode(zone));
                    _externalZoneNames.Add(zone.LegacyName);
                }
                foreach (BattleDefinition battle in _content.Battles)
                {
                    if (battle.IsCore || battle.Zone.Namespace == battle.Id.Namespace) continue;
                    ZoneDefinition zone;
                    if (!_content.TryGetZone(battle.Zone, out zone))
                        throw new ModContentException("External battle target zone is unavailable: '" + battle.Zone + "'.");
                    var document = new XmlDocument { XmlResolver = null };
                    XmlElement battleNode = BuildBattleNode(document, battle);
                    document.AppendChild(battleNode);
                    list.AddExternalBattle(zone.LegacyName, battleNode);
                    _externalBattles.Add(new ExternalBattleBinding(zone.LegacyName, battle.LegacyName));
                }
            }
            catch
            {
                RemoveStages(list);
                throw;
            }
        }

        public void ApplyQuests(ListSF list)
        {
            ThrowIfDisposed();
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (_externalQuests.Count != 0) throw new InvalidOperationException("Mod quest content is already applied.");
            try
            {
                foreach (QuestDefinition quest in _content.Quests)
                    _externalQuests.Add(list.AddExternalQuest(BuildQuestNode(quest), quest.Id.Namespace.Value));
                list.SetEclipseSuppressedQuests(_content.SuppressedQuestKeys);
            }
            catch
            {
                RemoveQuests(list);
                throw;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            if (_languageSubscribed)
            {
                LocalizationManager.OCLBJLPOKLB -= OnLanguageChanged;
                _languageSubscribed = false;
            }
            RemoveLocalization();
            RemoveP1DContent();
            RemoveP3Content();
            RemoveQuests(ListSF.ELEBLBJKDBI());
            RemoveStages(ListSF.ELEBLBJKDBI());
            RemovePerksAndEnchantments();
            RemoveItems();
            _disposed = true;
        }

        private void OnLanguageChanged()
        {
            if (!_disposed) ApplyLocalization();
        }

        private void RemoveLocalization()
        {
            for (int i = 0; i < _localizationKeys.Count; i++)
                LocalizationManager.RemoveExternalString(_localizationKeys[i]);
            _localizationKeys.Clear();
        }

        private sealed class BattleSourceBinding
        {
            public DefinitionId Id;
            public Battle Battle;
            public XmlNode Original;
        }

        private sealed class ExternalBattleBinding
        {
            public readonly string ZoneName;
            public readonly string BattleName;

            public ExternalBattleBinding(string zoneName, string battleName)
            {
                ZoneName = zoneName;
                BattleName = battleName;
            }
        }

        private void ApplyCoreFightPatches(ListSF list)
        {
            var patched = new Dictionary<DefinitionId, XmlNode>();
            var bindings = new Dictionary<DefinitionId, BattleSourceBinding>();
            for (int i = 0; i < _content.Patches.Count; i++)
            {
                ModContentPatchRecord patch = _content.Patches[i];
                if (patch.Target.Category != "fights") continue;
                FightDefinition fight;
                if (!_content.TryGetFight(patch.Target, out fight) || !fight.IsCore) continue;
                BattleDefinition battleDefinition;
                ZoneDefinition zoneDefinition;
                if (!_content.TryGetBattle(fight.Battle, out battleDefinition) ||
                    !_content.TryGetZone(battleDefinition.Zone, out zoneDefinition))
                    throw new ModContentException("Core fight patch lost projected battle/zone identity: '" + patch.Target + "'.");

                XmlNode battleNode;
                BattleSourceBinding binding;
                if (!patched.TryGetValue(battleDefinition.Id, out battleNode))
                {
                    Battle runtimeBattle = list.FindBattleForModding(zoneDefinition.LegacyName, battleDefinition.LegacyName);
                    if (runtimeBattle == null)
                        throw new ModContentException("Recovered battle for core patch is unavailable: '" + battleDefinition.Id + "'.");
                    XmlNode original = runtimeBattle.CloneSourceDefinitionForModding();
                    if (original == null)
                        throw new ModContentException("Recovered battle source is unavailable: '" + battleDefinition.Id + "'.");
                    battleNode = original.CloneNode(true);
                    binding = new BattleSourceBinding { Id = battleDefinition.Id, Battle = runtimeBattle, Original = original };
                    patched.Add(battleDefinition.Id, battleNode);
                    bindings.Add(battleDefinition.Id, binding);
                    _battleSourceBindings.Add(binding);
                }

                XmlElement fightNode = FindFightNode(battleNode, fight.LegacyName);
                if (fightNode == null)
                    throw new ModContentException("Recovered fight for core patch is unavailable: '" + fight.Id + "'.");
                ModFightPatchProjection.Apply(fightNode, fight, patch.Field, _content,
                    rule => BuildRuleNode(fightNode.OwnerDocument, rule),
                    warrior => BuildWarriorNode(fightNode.OwnerDocument, warrior),
                    reward => BuildRewardNode(fightNode.OwnerDocument, reward));
            }

            foreach (KeyValuePair<DefinitionId, XmlNode> pair in patched)
            {
                BattleSourceBinding binding = bindings[pair.Key];
                string error;
                if (!binding.Battle.ReplaceSourceDefinitionForModding(pair.Value, out error))
                    throw new ModContentException("Failed to apply core battle patch '" + pair.Key + "': " + error);
            }
        }

        private void RemoveStages(ListSF list)
        {
            if (list != null)
            {
                for (int i = _externalBattles.Count - 1; i >= 0; i--)
                    list.RemoveExternalBattle(_externalBattles[i].ZoneName, _externalBattles[i].BattleName);
                for (int i = _externalZoneNames.Count - 1; i >= 0; i--)
                    list.RemoveExternalZone(_externalZoneNames[i]);
            }
            _externalBattles.Clear();
            _externalZoneNames.Clear();
            for (int i = _battleSourceBindings.Count - 1; i >= 0; i--)
            {
                BattleSourceBinding binding = _battleSourceBindings[i];
                if (binding.Battle == null || binding.Original == null) continue;
                string ignored;
                binding.Battle.RestoreSourceDefinitionForModding(binding.Original, out ignored);
            }
            _battleSourceBindings.Clear();
        }

        private void RemoveQuests(ListSF list)
        {
            if (list != null) list.ClearEclipseQuestSuppression();
            if (list != null)
                for (int i = _externalQuests.Count - 1; i >= 0; i--) list.RemoveExternalQuest(_externalQuests[i]);
            _externalQuests.Clear();
        }

        private XmlElement BuildQuestNode(QuestDefinition quest)
        {
            var document = new XmlDocument { XmlResolver = null };
            XmlElement node = document.CreateElement("Quest");
            document.AppendChild(node);
            node.SetAttribute("Name", quest.Id.ToString());
            if (quest.Priority != 0) node.SetAttribute("Priority", quest.Priority.ToString(CultureInfo.InvariantCulture));
            if (quest.Unresumable) node.SetAttribute("Unresumable", "1");
            if (quest.AllowDoubles) node.SetAttribute("AllowDoubles", "1");
            if (quest.Groups.Count != 0) node.SetAttribute("Group", string.Join("|", new List<string>(quest.Groups).ToArray()));

            XmlElement events = document.CreateElement("Events");
            node.AppendChild(events);
            for (int i = 0; i < quest.Events.Count; i++) events.AppendChild(document.CreateElement(QuestEventName(quest.Events[i])));

            XmlElement conditions = document.CreateElement("Conditions");
            node.AppendChild(conditions);
            for (int i = 0; i < quest.Conditions.Count; i++) conditions.AppendChild(BuildQuestConditionNode(document, quest.Conditions[i]));

            XmlElement actions = document.CreateElement("Actions");
            actions.SetAttribute("Place", QuestPlaceName(quest.Place));
            node.AppendChild(actions);
            for (int i = 0; i < quest.Actions.Count; i++) actions.AppendChild(BuildQuestActionNode(document, quest.Actions[i]));

            if (quest.Marks.Count != 0)
            {
                XmlElement marks = document.CreateElement("Marks");
                node.AppendChild(marks);
                for (int i = 0; i < quest.Marks.Count; i++)
                {
                    XmlElement mark = document.CreateElement("Mark");
                    mark.SetAttribute("Name", quest.Marks[i]);
                    marks.AppendChild(mark);
                }
            }
            return node;
        }

        private XmlElement BuildQuestConditionNode(XmlDocument document, ModQuestCondition condition)
        {
            if (condition.Kind != ModQuestConditionKind.Compare)
            {
                XmlElement group = document.CreateElement("Operator");
                group.SetAttribute("Type", condition.Kind == ModQuestConditionKind.All ? "And" : "Or");
                if (condition.Not) group.SetAttribute("Not", "1");
                for (int i = 0; i < condition.Children.Count; i++)
                    group.AppendChild(BuildQuestConditionNode(document, condition.Children[i]));
                return group;
            }
            XmlElement node = document.CreateElement(QuestComparisonName(condition.Operator));
            node.SetAttribute("Value1", QuestOperand(condition.Left));
            node.SetAttribute("Value2", QuestOperand(condition.Right));
            if (condition.Not) node.SetAttribute("Not", "1");
            return node;
        }

        private XmlElement BuildQuestActionNode(XmlDocument document, ModQuestAction action)
        {
            XmlElement node;
            switch (action.Kind)
            {
                case ModQuestActionKind.Dialog:
                    node = document.CreateElement("Dialog");
                    node.SetAttribute("Type", "Regular");
                    SetIfNotEmpty(node, "Title", action.Title);
                    SetIfNotEmpty(node, "Image", action.Image);
                    for (int i = 0; i < action.Lines.Count; i++)
                    {
                        XmlElement line = document.CreateElement("Line");
                        line.SetAttribute("Text", action.Lines[i].Text);
                        SetIfNotEmpty(line, "ButtonText", action.Lines[i].ButtonText);
                        node.AppendChild(line);
                    }
                    XmlElement button = document.CreateElement("Button");
                    button.SetAttribute("Type", "Right");
                    if (action.Button == null)
                    {
                        button.SetAttribute("Color", "Beige");
                        button.SetAttribute("Text", "OK");
                    }
                    else
                    {
                        button.SetAttribute("Color", action.Button.Color);
                        button.SetAttribute("Text", action.Button.Text);
                        for (int i = 0; i < action.Button.Actions.Count; i++)
                            button.AppendChild(BuildQuestActionNode(document, action.Button.Actions[i]));
                    }
                    node.AppendChild(button);
                    return node;
                case ModQuestActionKind.StoryScreen:
                    node = document.CreateElement("ActScreen");
                    for (int i = 0; i < action.Lines.Count; i++)
                    {
                        XmlElement line = document.CreateElement("Line");
                        line.SetAttribute("Text", action.Lines[i].Text);
                        if (action.Lines[i].Frames > 0) line.SetAttribute("Frames", action.Lines[i].Frames.ToString(CultureInfo.InvariantCulture));
                        node.AppendChild(line);
                    }
                    return node;
                case ModQuestActionKind.SetUserVariable:
                    node = document.CreateElement("SetVariable"); node.SetAttribute("Scope", "Users");
                    node.SetAttribute("Name", action.Name); node.SetAttribute("Value", action.Value); return node;
                case ModQuestActionKind.ShowBattle:
                    node = document.CreateElement("ShowBattle"); node.SetAttribute("Name", LegacyBattleId(action.Reference));
                    node.SetAttribute("Locked", action.Flag ? "1" : "0"); return node;
                case ModQuestActionKind.ToggleBattle:
                    node = document.CreateElement("ToggleBattle"); node.SetAttribute("Name", LegacyBattleId(action.Reference));
                    node.SetAttribute("Toggle", action.Flag ? "on" : "off"); return node;
                case ModQuestActionKind.SetMapFocus:
                    node = document.CreateElement("SetMapFocus"); node.SetAttribute("Battle", LegacyBattleId(action.Reference) + "|"); return node;
                case ModQuestActionKind.StartFight:
                    node = document.CreateElement("Fight"); node.SetAttribute("Name", LegacyFightId(action.Reference)); return node;
                case ModQuestActionKind.StartCurrentFight:
                    node = document.CreateElement("Fight"); node.SetAttribute("Name", "_$Fight"); return node;
                case ModQuestActionKind.ToggleEclipseMode:
                    node = document.CreateElement("ToggleEclipseMode"); node.SetAttribute("Toggle", action.Flag ? "On" : "Off"); return node;
                case ModQuestActionKind.UpdateEclipseBattles:
                    return document.CreateElement("UpdateEclipseBattles");
                case ModQuestActionKind.GiveItem:
                    node = document.CreateElement("GiveItem"); node.SetAttribute("Name", LegacyItemName(action.Reference));
                    node.SetAttribute("Quantity", "1"); return node;
                default: throw new ModContentException("Unsupported committed quest action '" + action.Kind + "'.");
            }
        }

        private string QuestOperand(ModQuestOperand operand)
        {
            switch (operand.Kind)
            {
                case ModQuestOperandKind.Literal: return operand.Value;
                case ModQuestOperandKind.UserVariable: return "_" + operand.Value;
                case ModQuestOperandKind.EventFight: return "_$Fight";
                case ModQuestOperandKind.EventFightResult: return "_$FightResult";
                case ModQuestOperandKind.CurrentFightBattle: return "?Fight[_$Fight].Battle";
                case ModQuestOperandKind.FightWinCount: return "?Fight[" + LegacyFightId(operand.Reference) + "].WinCount";
                case ModQuestOperandKind.FightId: return LegacyFightId(operand.Reference);
                default: throw new ModContentException("Unsupported quest operand '" + operand.Kind + "'.");
            }
        }

        private string LegacyBattleId(DefinitionId id)
        {
            BattleDefinition battle;
            ZoneDefinition zone;
            if (!_content.TryGetBattle(id, out battle) || !_content.TryGetZone(battle.Zone, out zone))
                throw new ModContentException("Missing battle reference '" + id + "'.");
            return zone.LegacyName + "|" + battle.LegacyName;
        }

        private string LegacyFightId(DefinitionId id)
        {
            FightDefinition fight;
            if (!_content.TryGetFight(id, out fight)) throw new ModContentException("Missing fight reference '" + id + "'.");
            return LegacyBattleId(fight.Battle) + "|" + fight.LegacyName;
        }

        private static string QuestComparisonName(ModQuestCompareOperator op)
        {
            switch (op) { case ModQuestCompareOperator.Equal: return "Equal"; case ModQuestCompareOperator.Greater: return "Greater";
                case ModQuestCompareOperator.GreaterEqual: return "GreaterEqual"; case ModQuestCompareOperator.Less: return "Less";
                case ModQuestCompareOperator.LessEqual: return "LessEqual"; default: throw new ModContentException("Unsupported quest comparison."); }
        }

        private static string QuestPlaceName(ModQuestActionPlace place) => place == ModQuestActionPlace.Fight ? "Fight" : place == ModQuestActionPlace.Dojo ? "Dojo" : "Map";

        private static string QuestEventName(ModQuestEventKind kind)
        {
            switch (kind)
            {
                case ModQuestEventKind.RaidFightEnter: return "RaidFightEnter";
                case ModQuestEventKind.RaidFightEnd: return "RaidFightEnd";
                case ModQuestEventKind.RaidEnter: return "RaidEnter";
                case ModQuestEventKind.RaidEnd: return "RaidEnd";
                case ModQuestEventKind.ResetMode: return "AscensionReset";
                case ModQuestEventKind.RaidMapEnter: return "RaidMapEnter";
                case ModQuestEventKind.RaidFloorChanged: return "RaidFloorChanged";
                case ModQuestEventKind.ShowRaidLoot: return "ShowRaidLoot";
                case ModQuestEventKind.FightEnter: return "FightEnter"; case ModQuestEventKind.FightEnd: return "FightEnd";
                case ModQuestEventKind.LevelUp: return "LevelUp"; case ModQuestEventKind.GotItem: return "GotItem";
                case ModQuestEventKind.Dialog: return "Dialog"; case ModQuestEventKind.Session: return "SessionStart";
                case ModQuestEventKind.Activate: return "Activate"; case ModQuestEventKind.Purchase: return "Purchase";
                case ModQuestEventKind.Delivery: return "Delivery"; case ModQuestEventKind.TimerEnd: return "TimerEnd";
                case ModQuestEventKind.MapButtonPress: return "MapButtonPress"; case ModQuestEventKind.Enchantment: return "Enchantment";
                case ModQuestEventKind.ActivatePerk: return "ActivatePerk"; case ModQuestEventKind.DeactivatePerk: return "DeactivatePerk";
                case ModQuestEventKind.SetItemAcquired: return "SetItemAcquired"; case ModQuestEventKind.SceneLoaded: return "SceneLoaded";
                case ModQuestEventKind.ShopEnter: return "ShopEnter"; default: throw new ModContentException("Unsupported quest event.");
            }
        }

        private XmlElement BuildZoneNode(ZoneDefinition zone)
        {
            var document = new XmlDocument { XmlResolver = null };
            XmlElement zoneNode = document.CreateElement("Zone");
            document.AppendChild(zoneNode);
            zoneNode.SetAttribute("Name", zone.LegacyName);
            if (!string.IsNullOrEmpty(zone.FileName)) zoneNode.SetAttribute("FileName", zone.FileName);
            if (zone.IsStart) zoneNode.SetAttribute("Start", "1");
            for (int i = 0; i < zone.Battles.Count; i++)
            {
                BattleDefinition battle;
                if (!_content.TryGetBattle(zone.Battles[i], out battle))
                    throw new ModContentException("Zone references missing battle '" + zone.Battles[i] + "'.");
                zoneNode.AppendChild(BuildBattleNode(document, battle));
            }
            return zoneNode;
        }

        private XmlElement BuildBattleNode(XmlDocument document, BattleDefinition battle)
        {
            XmlElement node = document.CreateElement("Battle");
            node.SetAttribute("Name", battle.LegacyName);
            node.SetAttribute("Type", BattleKindName(battle.Kind));
            node.SetAttribute("X", battle.X.ToString(CultureInfo.InvariantCulture));
            node.SetAttribute("Y", battle.Y.ToString(CultureInfo.InvariantCulture));
            SetIfNotEmpty(node, "Alias", battle.Alias);
            SetIfNotEmpty(node, "Title", battle.Title);
            SetIfNotEmpty(node, "Icon", battle.Icon);
            SetIfNotEmpty(node, "IconAtlas", battle.IconAtlas);
            SetIfNotEmpty(node, "EclipseToggleName", battle.EclipseToggleName);
            SetIfNotEmpty(node, "Preview", battle.Preview);
            SetIfNotEmpty(node, "Description", battle.Description);
            SetIfNotEmpty(node, "Location", battle.Location);
            SetIfNotEmpty(node, "Music", battle.Music);
            SetIfNotEmpty(node, "RewardImage", battle.RewardImage);
            if (battle.ShowResistance) node.SetAttribute("ShowResistance", "1");
            for (int i = 0; i < battle.Fights.Count; i++)
            {
                FightDefinition fight;
                if (!_content.TryGetFight(battle.Fights[i], out fight))
                    throw new ModContentException("Battle references missing fight '" + battle.Fights[i] + "'.");
                node.AppendChild(BuildFightNode(document, fight));
            }
            return node;
        }

        private XmlElement BuildFightNode(XmlDocument document, FightDefinition fight)
        {
            XmlElement node = document.CreateElement("Fight");
            node.SetAttribute("Name", fight.LegacyName);
            if (fight.Replays != 0) node.SetAttribute("Replays", fight.Replays.ToString(CultureInfo.InvariantCulture));
            if (fight.ReplayInterval != 0)
                node.SetAttribute("ReplayInterval", fight.ReplayInterval.ToString(CultureInfo.InvariantCulture));
            node.SetAttribute("Power", fight.Power.ToString(CultureInfo.InvariantCulture));
            node.SetAttribute("Rounds", fight.Rounds.ToString(CultureInfo.InvariantCulture));
            node.SetAttribute("RoundTime", fight.RoundTime.ToString(CultureInfo.InvariantCulture));
            SetIfNotEmpty(node, "Location", fight.Location);
            SetIfNotEmpty(node, "Music", fight.Music);
            if (fight.EvaluatedRating != -1f)
                node.SetAttribute("EvaluatedRating", fight.EvaluatedRating.ToString(CultureInfo.InvariantCulture));
            if (fight.HealthRecovery != 1f)
                node.SetAttribute("HealthRecovery", fight.HealthRecovery.ToString(CultureInfo.InvariantCulture));
            SetIfNotEmpty(node, "Description", fight.Description);
            if (fight.Locked) node.SetAttribute("Locked", "1");
            SetIfNotEmpty(node, "RewardImage", fight.RewardImage);

            XmlElement warriors = document.CreateElement("Warriors");
            node.AppendChild(warriors);
            for (int i = 0; i < fight.Warriors.Count; i++)
            {
                WarriorDefinition warrior;
                if (!_content.TryGetWarrior(fight.Warriors[i], out warrior))
                    throw new ModContentException("Fight references missing warrior '" + fight.Warriors[i] + "'.");
                warriors.AppendChild(BuildWarriorNode(document, warrior));
            }

            XmlElement rules = document.CreateElement("Rules");
            node.AppendChild(rules);
            AppendFightRules(rules, fight);

            XmlElement rewards = document.CreateElement("Rewards");
            node.AppendChild(rewards);
            for (int i = 0; i < fight.Rewards.Count; i++)
            {
                RewardDefinition reward;
                if (!_content.TryGetReward(fight.Rewards[i], out reward))
                    throw new ModContentException("Fight references missing reward '" + fight.Rewards[i] + "'.");
                rewards.AppendChild(BuildRewardNode(document, reward));
            }
            foreach (var edit in fight.RewardDrops) edit.Apply(node, reward => BuildRewardNode(document, reward));
            return node;
        }

        public XmlElement BuildEncounterNode(FightDefinition fight, ModEncounterPlan plan)
        {
            var document = new XmlDocument();
            var node = BuildFightNode(document,fight); document.AppendChild(node);
            var warriors = node["Warriors"];
            if (plan.Warriors.Count > 0)
            {
                warriors.RemoveAll();
                foreach (var id in plan.Warriors)
                {
                    if (id.Namespace != fight.Id.Namespace || !_content.TryGetWarrior(id,out var warrior))
                        throw new ModContentException("Generated encounter references an unavailable or foreign warrior: " + id);
                    warriors.AppendChild(BuildWarriorNode(document,warrior));
                }
            }
            if (plan.Level.HasValue)
                foreach (XmlElement warrior in warriors.ChildNodes) warrior.SetAttribute("Level",plan.Level.Value.ToString(CultureInfo.InvariantCulture));
            if (plan.Rounds.HasValue) node.SetAttribute("Rounds",plan.Rounds.Value.ToString(CultureInfo.InvariantCulture));
            if (plan.RoundTime.HasValue) node.SetAttribute("RoundTime",plan.RoundTime.Value.ToString(CultureInfo.InvariantCulture));
            return node;
        }

        private void AppendFightRules(XmlElement rules, FightDefinition fight)
        {
            for (int i = 0; i < fight.Rules.Count; i++)
            {
                FightRuleDefinition rule;
                if (!_content.TryGetFightRule(fight.Rules[i], out rule))
                    throw new ModContentException("Fight references missing rule '" + fight.Rules[i] + "'.");
                // Scripted rules run at the combat callback boundary, not through the XML action interpreter.
                if (rule.Kind != ModFightRuleKind.Behavior) rules.AppendChild(BuildRuleNode(rules.OwnerDocument, rule));
            }
        }

        private XmlElement BuildWarriorNode(XmlDocument document, WarriorDefinition warrior)
        {
            XmlElement node = document.CreateElement("Warrior");
            node.SetAttribute("EclipseCharacterId",warrior.Id.ToString());
            if (!string.IsNullOrEmpty(warrior.BodyModel.Path)) node.SetAttribute("EclipseBodyModel",warrior.BodyModel.ToString());
            if (warrior.SkinModels.Count > 0)
            {
                var skins = document.CreateElement("EclipseSkinModels"); node.AppendChild(skins);
                foreach (var model in warrior.SkinModels) { var skin=document.CreateElement("Model"); skin.SetAttribute("Asset",model.ToString()); skins.AppendChild(skin); }
            }
            if (warrior.HasTemplate)
            {
                WarriorTemplateDefinition template;
                if (!_content.TryGetWarriorTemplate(warrior.Template, out template))
                    throw new ModContentException("Warrior references missing template '" + warrior.Template + "'.");
                node.SetAttribute("Template", template.LegacyName);
            }
            SetIfNotEmpty(node, "FirstName", warrior.FirstName);
            SetIfNotEmpty(node, "LastName", warrior.LastName);
            SetIfNotEmpty(node, "Avatar", warrior.Avatar);
            SetIfNotEmpty(node, "Voice", warrior.Voice);
            if (warrior.Level != 0) node.SetAttribute("Level", warrior.Level.ToString(CultureInfo.InvariantCulture));
            SetIfNotEmpty(node, "Tactic", warrior.Tactic);
            SetIfNotEmpty(node, "Group", warrior.Group);
            if (warrior.Random != 0) node.SetAttribute("Random", warrior.Random.ToString(CultureInfo.InvariantCulture));
            foreach (KeyValuePair<string, float> pair in warrior.Attributes)
                node.SetAttribute(pair.Key, pair.Value.ToString(CultureInfo.InvariantCulture));
            if (warrior.HealthBars > 0) node.SetAttribute("ShieldTotal", warrior.HealthBars.ToString(CultureInfo.InvariantCulture));
            if (warrior.Items.Count != 0)
            {
                XmlElement items = document.CreateElement("Items");
                node.AppendChild(items);
                for (int i = 0; i < warrior.Items.Count; i++)
                {
                    XmlElement item = document.CreateElement("Item");
                    item.SetAttribute("Name", LegacyItemName(warrior.Items[i]));
                    items.AppendChild(item);
                }
            }
            if (warrior.Perks.Count != 0)
            {
                XmlElement perks = document.CreateElement("Perks");
                node.AppendChild(perks);
                for (int i = 0; i < warrior.Perks.Count; i++)
                {
                    XmlElement perk = document.CreateElement("Perk");
                    perk.SetAttribute("Name", LegacyPerkName(warrior.Perks[i]));
                    perk.SetAttribute("Level", "1");
                    perks.AppendChild(perk);
                }
            }
            if (warrior.AttributeAlignments.Count != 0)
            {
                XmlElement align = document.CreateElement("AttributesAlign");
                node.AppendChild(align);
                for (int i = 0; i < warrior.AttributeAlignments.Count; i++)
                {
                    WarriorAttributeAlignmentDefinition value = warrior.AttributeAlignments[i];
                    XmlElement delta = document.CreateElement("Delta");
                    delta.SetAttribute("Factor", value.Factor.ToString(CultureInfo.InvariantCulture));
                    delta.SetAttribute("Shift", value.Shift.ToString(CultureInfo.InvariantCulture));
                    if (value.Priority != 0) delta.SetAttribute("Priority", value.Priority.ToString(CultureInfo.InvariantCulture));
                    if (value.Mode == ModRuleMode.Normal) delta.SetAttribute("Eclipse", "0");
                    else if (value.Mode == ModRuleMode.Eclipse) delta.SetAttribute("Eclipse", "1");
                    align.AppendChild(delta);
                }
            }
            return node;
        }

        private XmlElement BuildRuleNode(XmlDocument document, FightRuleDefinition rule)
        {
            XmlElement node;
            if (rule.Kind == ModFightRuleKind.RequireItem || rule.Kind == ModFightRuleKind.EquipItem)
            {
                node = document.CreateElement(rule.Kind == ModFightRuleKind.RequireItem ? "RequireItem" : "EquipItem");
                node.SetAttribute("Name", LegacyItemName(rule.Item));
                if (rule.MinimumLevel != 0)
                    node.SetAttribute("MinLevel", rule.MinimumLevel.ToString(CultureInfo.InvariantCulture));
                if (rule.Kind == ModFightRuleKind.EquipItem) node.SetAttribute("ApplyTo", RuleTargetName(rule.Target));
            }
            else if (rule.Kind == ModFightRuleKind.NoPerks)
            {
                node = document.CreateElement("NoPerks");
                SetIfNotEmpty(node, "Name", rule.Name);
                node.SetAttribute("ApplyTo", RuleTargetName(rule.Target));
            }
            else if (rule.Kind == ModFightRuleKind.Avatar || rule.Kind == ModFightRuleKind.Name ||
                     rule.Kind == ModFightRuleKind.NoButton)
            {
                node = document.CreateElement(rule.Kind == ModFightRuleKind.Avatar ? "Avatar" :
                    rule.Kind == ModFightRuleKind.Name ? "Name" : "NoButton");
                node.SetAttribute("Name", rule.Name);
                node.SetAttribute("ApplyTo", RuleTargetName(rule.Target));
            }
            else if (rule.Kind == ModFightRuleKind.Perk)
            {
                node = document.CreateElement("Perk");
                node.SetAttribute("Name", LegacyPerkName(rule.Perk));
                node.SetAttribute("ApplyTo", RuleTargetName(rule.Target));
            }
            else if (rule.Kind == ModFightRuleKind.RechargeMagicEachRound)
            {
                node = document.CreateElement("RechargeMagicEachRound");
                node.SetAttribute("ApplyTo", RuleTargetName(rule.Target));
            }
            else if (rule.Kind == ModFightRuleKind.Attributes)
            {
                node = document.CreateElement("Attributes");
                node.SetAttribute("ApplyTo", RuleTargetName(rule.Target));
                foreach (KeyValuePair<string, float> pair in rule.Attributes)
                    node.SetAttribute(pair.Key, pair.Value.ToString(CultureInfo.InvariantCulture));
            }
            else throw new ModContentException("Unsupported typed fight rule '" + rule.Kind + "'.");
            if (rule.Mode == ModRuleMode.Normal) node.SetAttribute("Eclipse", "Normal");
            else if (rule.Mode == ModRuleMode.Eclipse) node.SetAttribute("Eclipse", "Eclipse");
            if (rule.Rounds.Count != 0)
            {
                var rounds = new string[rule.Rounds.Count];
                for (int i = 0; i < rounds.Length; i++) rounds[i] = rule.Rounds[i].ToString(CultureInfo.InvariantCulture);
                node.SetAttribute("Round", string.Join("|", rounds));
            }
            return node;
        }

        private XmlElement BuildRewardNode(XmlDocument document, RewardDefinition reward)
        {
            XmlElement node = document.CreateElement("Reward");
            if (reward.Gems > 0) node.SetAttribute("Bonus", reward.Gems.ToString(CultureInfo.InvariantCulture));
            for (int i = 0; i < reward.Items.Count; i++)
                node.AppendChild(BuildRewardItemNode(document, reward.Items[i], null));
            for (int i = 0; i < reward.Choices.Count; i++)
            {
                XmlElement choice = document.CreateElement("Choice");
                for (int j = 0; j < reward.Choices[i].Items.Count; j++)
                {
                    RewardChoiceItem item = reward.Choices[i].Items[j];
                    choice.AppendChild(BuildRewardItemNode(document, item.Grant, item.Weight));
                }
                node.AppendChild(choice);
            }
            return node;
        }

        private XmlElement BuildRewardItemNode(XmlDocument document, RewardItemGrant grant, float? weight)
        {
            XmlElement item = document.CreateElement("Item");
            item.SetAttribute("Name", LegacyItemName(grant.Item));
            // EndFightContent lists only item grants marked as drops.
            item.SetAttribute("Drop", "1");
            if (grant.UpgradeNumber != 0)
                item.SetAttribute("UpgradeNumber", grant.UpgradeNumber.ToString(CultureInfo.InvariantCulture));
            if (weight.HasValue) item.SetAttribute("Weight", weight.Value.ToString(CultureInfo.InvariantCulture));
            return item;
        }

        private string LegacyItemName(DefinitionId id)
        {
            ItemDefinition item;
            if (!_content.TryResolveItem(id, out item)) throw new ModContentException("Missing item '" + id + "'.");
            return item.IsCore && !string.IsNullOrEmpty(item.LegacyName) ? item.LegacyName : item.Id.ToString();
        }

        private string LegacyPerkName(DefinitionId id)
        {
            PerkDefinition perk;
            if (!_content.TryGetPerk(id, out perk)) throw new ModContentException("Missing perk '" + id + "'.");
            return perk.IsCore && !string.IsNullOrEmpty(perk.LegacyName) ? perk.LegacyName : perk.Id.ToString();
        }

        private static XmlElement FindFightNode(XmlNode battle, string legacyName)
        {
            foreach (XmlNode child in battle.ChildNodes)
            {
                XmlElement element = child as XmlElement;
                if (element != null && element.Name == "Fight" && element.GetAttribute("Name") == legacyName)
                    return element;
            }
            return null;
        }

        private static void SetIfNotEmpty(XmlElement node, string name, string value)
        {
            if (!string.IsNullOrEmpty(value)) node.SetAttribute(name, value);
        }

        private static string RuleTargetName(ModRuleTarget target)
        {
            switch (target)
            {
                case ModRuleTarget.Player: return "Player";
                case ModRuleTarget.Opponent: return "Bot";
                case ModRuleTarget.All: return "All";
                default: throw new ModContentException("Unsupported rule target '" + target + "'.");
            }
        }

        private static string BattleKindName(ModBattleKind kind)
        {
            switch (kind)
            {
                case ModBattleKind.Dummy: return "DUMMY";
                case ModBattleKind.Tutorial: return "TUTORIAL";
                case ModBattleKind.Challenge: return "CHALLENGE";
                case ModBattleKind.Bosses: return "BOSSES";
                case ModBattleKind.Tournament: return "TOURNAMENT";
                case ModBattleKind.Story: return "STORY";
                case ModBattleKind.Raid: return "RAID";
                case ModBattleKind.Survival: return "SURVIVAL";
                case ModBattleKind.Friendly: return "TACTICS";
                case ModBattleKind.Auto: return "AUTO";
                case ModBattleKind.Ai: return "AI";
                case ModBattleKind.Hidden: return "HIDDEN";
                case ModBattleKind.Fake: return "FAKE";
                case ModBattleKind.Pvp: return "PVP";
                case ModBattleKind.Final: return "FINAL_BATTLE";
                case ModBattleKind.FinalTitan: return "FINAL_BATTLE_TITAN";
                case ModBattleKind.BossesIntermission: return "BOSSES_INTERMISSION";
                default: throw new ModContentException("Battle kind '" + kind +
                    "' is not available through the ordinary stage adapter.");
            }
        }

        private void RemoveItems()
        {
            if (_items != null)
            {
                for (int i = _itemSetNames.Count - 1; i >= 0; i--)
                    _items.DGKMILIPLLF().RemoveExternalSet(_itemSetNames[i]);
                for (int i = _itemNames.Count - 1; i >= 0; i--)
                    _items.RemoveExternalItem(_itemNames[i]);
            }
            _itemSetNames.Clear();
            _itemNames.Clear();
            _itemsApplied = false;
        }

        private PerkInfoItem EnsurePerkApplied(PerkDefinition definition, HashSet<DefinitionId> visiting)
        {
            if (definition.IsCore)
            {
                PerkInfoItem core = _perks.ABAGJKMKCBA(definition.LegacyName);
                if (core == null)
                    throw new InvalidOperationException("Core perk template is unavailable at runtime: " + definition.Id);
                return core;
            }

            string runtimeName = RuntimePerkName(definition);
            PerkInfoItem existing = _perks.ABAGJKMKCBA(runtimeName);
            if (existing != null)
            {
                if (_perkNames.Contains(runtimeName)) return existing;
                throw new InvalidOperationException("Legacy perk already exists: " + runtimeName);
            }
            if (definition.HasBehavior)
            {
                XmlElement scriptedNode = BuildScriptedPerkNode(definition.Id, definition.DisplayName,
                    definition.Description, definition.Icon, definition.HasIcon, definition.Kind);
                PerkInfoItem scripted = _perks.AddExternalBasePerk(scriptedNode);
                _perkNames.Add(scripted.Name);
                return scripted;
            }
            if (!definition.HasTemplate)
                throw new InvalidOperationException("External perk has no template: " + definition.Id);
            if (!visiting.Add(definition.Id))
                throw new InvalidOperationException("Perk template cycle detected at " + definition.Id);

            try
            {
                PerkDefinition templateDefinition;
                if (!_content.TryGetPerk(definition.Template, out templateDefinition))
                    throw new InvalidOperationException("Perk template is unavailable: " + definition.Template);
                PerkInfoItem template = EnsurePerkApplied(templateDefinition, visiting);
                XmlElement node = BuildPerkNode(definition, template);
                PerkInfoItem applied = _perks.AddExternalBasePerk(node);
                _perkNames.Add(applied.Name);
                return applied;
            }
            finally
            {
                visiting.Remove(definition.Id);
            }
        }

        private XmlElement BuildPerkNode(PerkDefinition definition, PerkInfoItem template)
        {
            if (template == null || template.HAAKMBKCMCO == null)
                throw new InvalidOperationException("Perk template has no canonical runtime XML: " + definition.Template);
            var document = new XmlDocument();
            XmlElement node = document.ImportNode(template.HAAKMBKCMCO, true) as XmlElement;
            if (node == null) throw new InvalidOperationException("Perk template is not a Perk element: " + definition.Template);
            document.AppendChild(node);

            string templateName = template.Name;
            string inheritedTemplates = node.GetAttribute("Template");
            node.SetAttribute("Name", definition.Id.ToString());
			node.SetAttribute("ID", _perks.CJJEPHDFOCJ().Count.ToString(CultureInfo.InvariantCulture));
			node.SetAttribute("Alias", definition.DisplayName.ToString());
			node.SetAttribute("Description", definition.Description.ToString());
			// Keep the template's logical Image by default. Vanilla intentionally resolves
			// that one value through different shop and fight sprite paths.
			if (definition.HasIcon) node.SetAttribute("Image", definition.Icon.ToString());
			node.SetAttribute("Template", string.IsNullOrEmpty(inheritedTemplates)
                ? templateName : inheritedTemplates + "|" + templateName);

            XmlElement set = node["Set"];
            if (set == null && definition.Parameters.Count > 0)
            {
                set = document.CreateElement("Set");
                XmlNode firstTrigger = node.SelectSingleNode("Trigger");
                if (firstTrigger == null) node.AppendChild(set);
                else node.InsertBefore(set, firstTrigger);
            }
            if (set != null)
                foreach (KeyValuePair<string, string> parameter in definition.Parameters)
                    set.SetAttribute(parameter.Key, parameter.Value);
            return node;
        }

        private PerkInfoItem EnsureScriptedEnchantmentApplied(EnchantmentDefinition definition)
        {
            string runtimeName = definition.Id.ToString();
            PerkInfoItem existing = _perks.ABAGJKMKCBA(runtimeName);
            if (existing != null)
            {
                if (_perkNames.Contains(runtimeName)) return existing;
                throw new InvalidOperationException("Legacy perk already exists for scripted enchantment: " + runtimeName);
            }
            XmlElement node = BuildScriptedPerkNode(definition.Id, definition.DisplayName,
                definition.Description, definition.Icon, definition.HasIcon, definition.Kind);
            PerkInfoItem applied = _perks.AddExternalBasePerk(node);
            _perkNames.Add(applied.Name);
            return applied;
        }

        private XmlElement BuildScriptedPerkNode(DefinitionId id, DefinitionId displayName,
            DefinitionId description, AssetId icon, bool hasIcon, ModPerkKind kind)
        {
            var document = new XmlDocument();
            XmlElement node = document.CreateElement("Perk");
            document.AppendChild(node);
            node.SetAttribute("Name", id.ToString());
            node.SetAttribute("ID", _perks.CJJEPHDFOCJ().Count.ToString(CultureInfo.InvariantCulture));
            node.SetAttribute("Alias", displayName.ToString());
            node.SetAttribute("Description", description.ToString());
            if (hasIcon) node.SetAttribute("Image", icon.ToString());
            if (kind == ModPerkKind.Combo) node.SetAttribute("PerkType", "Combo");
            return node;
        }

        private static IReadOnlyDictionary<string, string> ToWireParameters(
            IReadOnlyDictionary<string, ModParameterValue> parameters)
        {
            if (parameters == null || parameters.Count == 0) return null;
            var result = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (KeyValuePair<string, ModParameterValue> pair in parameters)
                result.Add(pair.Key, pair.Value.ToWireString());
            return result;
        }

        private void ApplyTacticSubtypes()
        {
            foreach (var definition in _content.ItemTacticSubtypes)
            {
                if (_items == null || !_content.TryGetItem(definition.Item, out var target))
                    throw new InvalidOperationException("Tactic subtype requires applied items: " + definition.Item);
                var item = _items.KCCDBEEKBCG(target.IsCore ? target.LegacyName : target.Id.ToString());
                if (item == null || !item.TryOverrideTacticSubtype(definition.Group, out var lifetime))
                    throw new InvalidOperationException("Could not apply tactic subtype for '" + definition.Owner + "': " + definition.Item);
                _tacticSubtypeLifetimes.Add(lifetime);
            }
        }

        private void ApplyInnatePerks()
        {
            foreach (var definition in _content.ItemInnatePerks)
            {
                if (_items == null || !_content.TryGetItem(definition.Item, out var target))
                    throw new InvalidOperationException("Innate perks require applied items: " + definition.Item);
                var item = _items.KCCDBEEKBCG(target.IsCore ? target.LegacyName : target.Id.ToString());
                var xml = new XmlDocument();
                var root = xml.CreateElement("Perks"); xml.AppendChild(root);
                foreach (var entry in definition.Entries)
                {
                    if (!_content.TryGetPerk(entry.Perk, out var perk)) throw new InvalidOperationException("Missing innate perk: " + entry.Perk);
                    var node = xml.CreateElement("Perk"); node.SetAttribute("Name", RuntimePerkName(perk));
                    if (entry.Parameters.Count != 0)
                    {
                        var set = xml.CreateElement("Set");
                        foreach (var pair in entry.Parameters) set.SetAttribute(pair.Key, pair.Value.ToString("R", CultureInfo.InvariantCulture));
                        node.AppendChild(set);
                    }
                    root.AppendChild(node);
                }
                if (item == null || !item.TryOverrideInnatePerks(root, out var lifetime))
                    throw new InvalidOperationException("Could not apply innate perks for '" + definition.Owner + "': " + definition.Item);
                _innatePerkLifetimes.Add(lifetime);
            }
        }

        private void ApplyDefaultEnchantments()
        {
            foreach (var definition in _content.ItemDefaultEnchantments)
            {
                if (_items == null || !_content.TryGetItem(definition.Item, out var target))
                    throw new InvalidOperationException("Default enchantments require applied items: " + definition.Item);
                var item = _items.KCCDBEEKBCG(target.IsCore ? target.LegacyName : target.Id.ToString());
                var xml = new XmlDocument();
                var root = xml.CreateElement("Enchantments"); xml.AppendChild(root);
                foreach (var entry in definition.Entries)
                {
                    if (!_content.TryGetPerk(entry.Perk, out var perk)) throw new InvalidOperationException("Missing default perk: " + entry.Perk);
                    var node = xml.CreateElement("Perk"); node.SetAttribute("Name", RuntimePerkName(perk));
                    if (entry.Aspect.HasValue)
                    {
                        var set = xml.CreateElement("Set");
                        set.SetAttribute("Aspect", entry.Aspect.Value.ToString(CultureInfo.InvariantCulture));
                        node.AppendChild(set);
                    }
                    root.AppendChild(node);
                }
                if (item == null || !item.TryOverrideDefaultEnchantments(root, out var lifetime))
                    throw new InvalidOperationException("Could not apply default enchantments for '" + definition.Owner + "': " + definition.Item);
                _defaultEnchantmentLifetimes.Add(lifetime);
            }
        }

        private void ApplyForgeDeviations()
        {
            foreach (var deviation in _content.ForgeDeviations)
            {
                if (!_content.TryGetForgeEconomicProfile(deviation.Profile, out var profile) ||
                    !_forge.TryOverrideDeviation(profile.RuntimeRecipeName, EquipmentType(deviation.Equipment),
                        deviation.Minimum, deviation.Maximum, out var lifetime))
                    throw new InvalidOperationException("Could not override forge deviation for '" + deviation.Owner + "': " +
                        deviation.Profile + "/" + deviation.Equipment + ". Target must be an existing random-aspect category without another override.");
                _forgeDeviationLifetimes.Add(lifetime);
            }
        }

        private void ApplyForgeExclusions()
        {
            foreach (var exclusion in _content.ForgeCandidateExclusions)
            {
                if (!_content.TryGetForgeEconomicProfile(exclusion.Profile, out var profile) ||
                    !_content.TryGetPerk(exclusion.Perk, out var perk))
                    throw new InvalidOperationException("Committed forge exclusion lost a referenced definition: " + exclusion.Profile + "/" + exclusion.Perk);
                if (!_forge.TryExcludeNativeCandidate(profile.RuntimeRecipeName, EquipmentType(exclusion.Equipment),
                        RuntimePerkName(perk), out var lifetime))
                    throw new InvalidOperationException("Could not exclude native forge candidate for '" + exclusion.Owner + "': " +
                        exclusion.Profile + "/" + exclusion.Equipment + "/" + exclusion.Perk + ". Candidate or equipment is missing, or another exclusion is active.");
                _forgeExclusionLifetimes.Add(lifetime);
            }
        }

        private void RemoveForgeExclusions()
        {
            for (int i = _forgeExclusionLifetimes.Count - 1; i >= 0; i--) _forgeExclusionLifetimes[i].Dispose();
            _forgeExclusionLifetimes.Clear();
        }

        private void RemovePerksAndEnchantments()
        {
            for (int i = _innatePerkLifetimes.Count - 1; i >= 0; i--) _innatePerkLifetimes[i].Dispose();
            for (int i = _tacticSubtypeLifetimes.Count - 1; i >= 0; i--) _tacticSubtypeLifetimes[i].Dispose();
            _tacticSubtypeLifetimes.Clear();
            _innatePerkLifetimes.Clear();
            for (int i = _defaultEnchantmentLifetimes.Count - 1; i >= 0; i--) _defaultEnchantmentLifetimes[i].Dispose();
            _defaultEnchantmentLifetimes.Clear();
            for (int i = _forgeDeviationLifetimes.Count - 1; i >= 0; i--) _forgeDeviationLifetimes[i].Dispose();
            _forgeDeviationLifetimes.Clear();
            RemoveForgeExclusions();
            for (int i = _progressionBindings.Count - 1; i >= 0; i--)
            {
                ProgressionBranchBinding binding = _progressionBindings[i];
                PerkTree.GBPBIPFIOJH().RestoreExternalBranch(binding.Level, binding.Previous);
            }
            _progressionBindings.Clear();
            if (_forge != null)
            {
                for (int i = _enchantmentBindings.Count - 1; i >= 0; i--)
                {
                    ExternalEnchantmentBinding binding = _enchantmentBindings[i];
                    _forge.RemoveExternalEnchantmentCandidate(binding.Recipe, binding.ItemType, binding.PerkName);
                }
                for (int i = _forgeRecipeNames.Count - 1; i >= 0; i--)
                    _forge.RemoveExternalRecipeFamily(_forgeRecipeNames[i]);
            }
            _enchantmentBindings.Clear();
            _forgeRecipeNames.Clear();
            if (_perks != null)
                for (int i = _perkNames.Count - 1; i >= 0; i--) _perks.RemoveExternalBasePerk(_perkNames[i]);
            _perkNames.Clear();
            _perksApplied = false;
            _perks = null;
            _forge = null;
        }

        private static string RuntimePerkName(PerkDefinition definition)
        {
            return definition.IsCore ? definition.LegacyName : definition.Id.ToString();
        }

        private static string RecipeName(ModEnchantmentRecipe recipe)
        {
            switch (recipe)
            {
                case ModEnchantmentRecipe.Simple: return "Simple";
                case ModEnchantmentRecipe.Medium: return "Medium";
                case ModEnchantmentRecipe.Complex: return "Complex";
                default: throw new InvalidOperationException("Unsupported enchantment recipe: " + recipe);
            }
        }

        private static string EquipmentType(ModEquipmentKind kind)
        {
            switch (kind)
            {
                case ModEquipmentKind.Weapon: return "Weapon";
                case ModEquipmentKind.Armor: return "Armor";
                case ModEquipmentKind.Helm: return "Helm";
                case ModEquipmentKind.Ranged: return "Ranged";
                case ModEquipmentKind.Magic: return "Magic";
                default: throw new InvalidOperationException("Unsupported equipment kind: " + kind);
            }
        }

        private sealed class ExternalEnchantmentBinding
        {
            public readonly string Recipe;
            public readonly string ItemType;
            public readonly string PerkName;

            public ExternalEnchantmentBinding(string recipe, string itemType, string perkName)
            {
                Recipe = recipe;
                ItemType = itemType;
                PerkName = perkName;
            }
        }

        private sealed class ProgressionBranchBinding
        {
            public readonly int Level;
            public readonly PerkTree.PerkBranch Previous;
            public ProgressionBranchBinding(int level, PerkTree.PerkBranch previous)
            {
                Level = level;
                Previous = previous;
            }
        }

        private IEnumerable<ProgressionBranchOverlayDefinition> GetProgressionOverlays()
        {
            for (int level = 1; level <= ModRegistrationTransaction.MaxEquipmentLevel; level++)
                if (_content.TryGetProgressionBranch(level, out ProgressionBranchOverlayDefinition overlay)) yield return overlay;
        }

        private XmlElement BuildNonEquipmentItemNode(NonEquipmentItemDefinition definition)
        {
            var document = new XmlDocument();
            XmlElement item = document.CreateElement("Item");
            document.AppendChild(item);
            Set(item, "Name", definition.Id.ToString());
            Set(item, "Type", definition.Kind == ModNonEquipmentItemKind.Consumable ? "Consumable" :
                definition.Kind == ModNonEquipmentItemKind.Free ? "Free" : "Seal");
            if (!string.IsNullOrEmpty(definition.SubType)) Set(item, "SubType", definition.SubType);
            if (!string.IsNullOrEmpty(definition.PackLabel)) Set(item, "PackLabel", definition.PackLabel);
            if (definition.HasIcon) Set(item, "Image", definition.Icon.ToString());
            if (definition.HasModel) Set(item, "Model", definition.Model.ToString());
            Set(item, "Text", definition.DisplayName.ToString());
            Set(item, "TextButton", definition.DisplayName.ToString());
            if (definition.SilentReceive) Set(item, "SilentRecieve", "1");
            if (definition.SpendAfterUse) Set(item, "SpendAfterUse", "1");
            return item;
        }

        private XmlElement BuildItemSetNode(ItemSetDefinition definition)
        {
            var document = new XmlDocument();
            XmlElement set = document.CreateElement("ItemSet");
            document.AppendChild(set);
            Set(set, "Name", definition.Id.ToString());
            Set(set, "Title", definition.Title.ToString());
            Set(set, "Text", definition.Text.ToString());
            Set(set, "Brief", definition.Brief.ToString());
            for (int i = 0; i < definition.Members.Count; i++)
            {
                ModItemSetMember member = definition.Members[i];
                XmlElement item = document.CreateElement("Item");
                Set(item, "Name", LegacyItemName(member.Item));
                Set(item, "Scale", member.Scale.ToString(CultureInfo.InvariantCulture));
                Set(item, "Rotate", member.Rotate.ToString(CultureInfo.InvariantCulture));
                Set(item, "X", member.X.ToString(CultureInfo.InvariantCulture));
                Set(item, "Y", member.Y.ToString(CultureInfo.InvariantCulture));
                Set(item, "IconsY", member.IconsY.ToString(CultureInfo.InvariantCulture));
                set.AppendChild(item);
            }
            return set;
        }

        private XmlElement BuildItemNode(ItemDefinition definition, ShopListingDefinition listing)
        {
            if (definition.Progression != ItemProgressionKind.Vanilla)
                throw new InvalidOperationException("External item does not use the vanilla progression profile: " +
                    definition.Id);

            var document = new XmlDocument();
            XmlElement item = document.CreateElement("Item");
            document.AppendChild(item);
            Set(item, "Name", definition.Id.ToString());
            Set(item, "Image", definition.Icon.ToString());
            Set(item, "Model", definition.Model.ToString());
            Set(item, "Text", definition.DisplayName.ToString());
            Set(item, "TextButton", definition.DisplayName.ToString());
            Set(item, "Level", listing.Level.ToString(CultureInfo.InvariantCulture));
            Set(item, "UpgradeLevel", (listing.Level * 100).ToString(CultureInfo.InvariantCulture));

            string upgradeTemplate;
            if (definition is WeaponDefinition weapon)
            {
                Set(item, "Type", "Weapon");
                Set(item, "SubType", weapon.SubType);
                if (weapon.TacticSubtype != null) Set(item, "TacticSubtype", weapon.TacticSubtype);
                upgradeTemplate = "Weapon_Bonus";
                Set(item, "WeaponDamage", ResolveVanillaStat(upgradeTemplate, listing.Level, "WeaponDamage")
                    .ToString(CultureInfo.InvariantCulture));
            }
            else if (definition is ArmorDefinition)
            {
                Set(item, "Type", "Armor");
                upgradeTemplate = "Armor_Bonus";
                Set(item, "BodyDefense", ResolveVanillaStat(upgradeTemplate, listing.Level, "BodyDefense")
                    .ToString(CultureInfo.InvariantCulture));
                Set(item, "UnarmedDamage", ResolveVanillaStat(upgradeTemplate, listing.Level, "UnarmedDamage")
                    .ToString(CultureInfo.InvariantCulture));
            }
            else if (definition is HelmDefinition)
            {
                Set(item, "Type", "Helm");
                upgradeTemplate = "Helm_Bonus";
                Set(item, "HeadDefense", ResolveVanillaStat(upgradeTemplate, listing.Level, "HeadDefense")
                    .ToString(CultureInfo.InvariantCulture));
            }
            else if (definition is RangedDefinition ranged)
            {
                Set(item, "Type", "Ranged");
                Set(item, "SubType", ranged.SubType);
                upgradeTemplate = "Ranged_Bonus";
                Set(item, "RangedDamage", ResolveVanillaStat(upgradeTemplate, listing.Level, "RangedDamage")
                    .ToString(CultureInfo.InvariantCulture));
            }
            else if (definition is MagicDefinition magic)
            {
                Set(item, "Type", "Magic");
                Set(item, "SubType", magic.SubType);
                upgradeTemplate = "Magic_Bonus";
                Set(item, "MagicDamage", ResolveVanillaStat(upgradeTemplate, listing.Level, "MagicDamage")
                    .ToString(CultureInfo.InvariantCulture));
            }
            else throw new InvalidOperationException("Unsupported external item definition: " + definition.Id);

            if (listing.Price.Currency == ModPriceCurrency.Coins)
                Set(item, "Price", listing.Price.Amount.ToString(CultureInfo.InvariantCulture));
            else
                Set(item, "BonusPrice", listing.Price.Amount.ToString(CultureInfo.InvariantCulture));

            XmlElement upgrades = document.CreateElement("Upgrades");
            upgrades.SetAttribute("Template", upgradeTemplate);
            item.AppendChild(upgrades);
            return item;
        }

        private int ResolveVanillaStat(string template, int level, string attribute)
        {
            // The shared vanilla tables begin at level 3 for melee/defense equipment and
            // level 6 for ranged/magic. Preserve the canonical early normal-item baselines
            // instead of inventing a formula for values that are stored directly on items.
            if (template == "Weapon_Bonus")
            {
                if (level == 1 && attribute == "WeaponDamage") return 5;
                if (level == 2 && attribute == "WeaponDamage") return 20;
            }
            else if (template == "Armor_Bonus" && level == 2)
            {
                if (attribute == "BodyDefense") return 22;
                if (attribute == "UnarmedDamage") return 8;
            }
            else if (template == "Helm_Bonus" && level == 2 && attribute == "HeadDefense")
            {
                return 18;
            }

            UpgradeDataContainer upgrades = _items.BKPOCLGODDM(template);
            if (upgrades == null)
                throw new InvalidOperationException("Vanilla upgrade template is unavailable: " + template);

            int upgradeLevel = checked(level * 100);
            for (int i = 0; i < upgrades.KPAPEBOAKIE.Count; i++)
            {
                UpgradeData upgrade = upgrades.KPAPEBOAKIE[i];
                if (upgrade.OGLHOJNMEBD.Level != level || upgrade.OGLHOJNMEBD.AKKLOMFOLNO != upgradeLevel)
                    continue;
                int value = 0;
                if (!upgrade.OGLHOJNMEBD.IBLHIAHECLK.Get(attribute, ref value, false))
                    throw new InvalidOperationException("Vanilla progression milestone " + template + " level " +
                        level + " does not define " + attribute + ".");
                return value;
            }

            throw new InvalidOperationException("Vanilla progression milestone is unavailable: " + template +
                " level " + level + ".");
        }

        private static void Set(XmlElement element, string name, string value)
        {
            element.SetAttribute(name, value ?? string.Empty);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(LegacyContentAdapter));
        }
    }
}
