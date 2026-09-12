using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Eclipse.Modding
{
    // Stores evaluated results, never a recipe that would draw randomness on resume.
    internal static class ModLotteryPrizeCodec
    {
        internal static XmlElement Write(XmlDocument document, FightResult.ResultPrizeStruct prize)
        {
            if (prize == null) throw new ArgumentNullException(nameof(prize));
            if (prize.FAPDEKOMOGH != null)
                throw new InvalidOperationException("Resolve nested lottery draws before persisting a prize.");
            var root = document.CreateElement("Prize");
            root.SetAttribute("Format", "1");
            Set(root, "Money", prize.GBGNFPNCGED);
            Set(root, "Bonus", prize.PNDAIFALIKF);
            Set(root, "Experience", prize.exp);
            foreach (var grant in prize.HELFDCAIJNE)
            {
                var item = document.CreateElement("Item");
                item.SetAttribute("Name", grant.DLKPBAJDHBO.Name);
                Set(item, "Level", grant.DLKPBAJDHBO.MHGODOLNDLE);
                Set(item, "Upgrade", grant.DLKPBAJDHBO.OBJDGBBFJOO);
                item.SetAttribute("Drop", grant.IDGKPLBKDIB ? "1" : "0");
                foreach (var perk in grant.NAIEGGHELIH.LDLPCOFHFKE)
                {
                    var effect = document.CreateElement("Perk");
                    effect.SetAttribute("Name", perk.get_Name());
                    effect.SetAttribute("ItemType", string.Join("|", perk.NMOKPAPJLCN));
                    effect.SetAttribute(PerkStruct.EclipseEnchantmentAttribute, perk.EclipseEnchantment);
                    effect.SetAttribute(PerkStruct.EclipseKindAttribute, perk.EclipseKind);
                    var values = document.CreateElement("Set");
                    foreach (var pair in perk.Pairs) values.SetAttribute(pair.Key, pair.Value);
                    effect.AppendChild(values);
                    var parameters = document.CreateElement(ModEffectSaveData.NodeName);
                    parameters.SetAttribute("Format", ModEffectSaveData.Format);
                    foreach (var pair in perk.EclipseParameters)
                    {
                        var parameter = document.CreateElement(ModEffectSaveData.ParameterNodeName);
                        parameter.SetAttribute("Name", pair.Key);
                        parameter.SetAttribute("Value", pair.Value);
                        parameters.AppendChild(parameter);
                    }
                    effect.AppendChild(parameters);
                    item.AppendChild(effect);
                }
                root.AppendChild(item);
            }
            foreach (var grant in prize.KIMJGOHCCPO)
            {
                var currency = document.CreateElement("Currency");
                currency.SetAttribute("Name", grant.NAKKNKPJNHB.BKDEAGGPNAO.Name);
                Set(currency, "Count", (int)grant.NAKKNKPJNHB.Count);
                currency.SetAttribute("Drop", grant.IDGKPLBKDIB ? "1" : "0");
                root.AppendChild(currency);
            }
            foreach (var grant in prize.KBMDJACLAOH)
            {
                var resistance = document.CreateElement("Resistance");
                resistance.SetAttribute("Name", grant.JIDLBLPFAAE.PIFOHOOFJDE.Name);
                Set(resistance, "Count", (int)grant.JIDLBLPFAAE.Count);
                resistance.SetAttribute("Drop", grant.IDGKPLBKDIB ? "1" : "0");
                root.AppendChild(resistance);
            }
            return root;
        }

        internal static FightResult.ResultPrizeStruct Read(XmlElement root,
            Func<string, int, int, ItemInfo> resolveItem,
            Func<string, GameCurrency> resolveCurrency,
            Func<string, GameResistance> resolveResistance)
        {
            if (root == null || root.Name != "Prize" || root.GetAttribute("Format") != "1")
                throw new InvalidDataException("Unsupported saved lottery prize.");
            var prize = new FightResult.ResultPrizeStruct {
                GBGNFPNCGED = Number(root, "Money"), PNDAIFALIKF = Number(root, "Bonus"),
                exp = checked((uint)Number(root, "Experience"))
            };
            foreach (XmlNode child in root.ChildNodes)
            {
                if (!(child is XmlElement node)) continue;
                string name = node.GetAttribute("Name");
                if (string.IsNullOrEmpty(name)) throw new InvalidDataException("Saved prize entry has no name.");
                string dropText = node.GetAttribute("Drop");
                if (dropText != "0" && dropText != "1") throw new InvalidDataException("Invalid saved prize drop flag.");
                bool drop = dropText == "1";
                switch (node.Name)
                {
                    case "Item":
                        int level = checked((int)Number(node, "Level"));
                        int upgrade = checked((int)Number(node, "Upgrade"));
                        var item = resolveItem(name, level, upgrade);
                        if (item == null || item.Name != name || item.MHGODOLNDLE != level || item.OBJDGBBFJOO != upgrade)
                            throw new InvalidDataException("Saved lottery item is unavailable or its upgrade changed: " + name);
                        var rewardNode = root.OwnerDocument.CreateElement("Item");
                        rewardNode.SetAttribute("Name", name);
                        var reward = new RewardItem(rewardNode);
                        foreach (XmlNode effect in node.ChildNodes)
                        {
                            if (!(effect is XmlElement)) continue;
                            if (effect.Name != "Perk") throw new InvalidDataException("Unknown saved item payload.");
                            reward.LDLPCOFHFKE.Add(new PerkStruct(effect));
                        }
                        prize.HELFDCAIJNE.Add(new FightResult.LJFFIBFBGID {
                            DLKPBAJDHBO = item, NAIEGGHELIH = reward, IDGKPLBKDIB = drop
                        });
                        break;
                    case "Currency":
                        var currency = resolveCurrency(name);
                        if (currency == null || currency.Name != name) throw new InvalidDataException("Saved lottery currency unavailable: " + name);
                        prize.KIMJGOHCCPO.Add(new FightResult.NFBOLAJJIAD {
                            NAKKNKPJNHB = new CurrencyStruct(currency, checked((int)Number(node, "Count"))), IDGKPLBKDIB = drop
                        });
                        break;
                    case "Resistance":
                        var resistance = resolveResistance(name);
                        if (resistance == null || resistance.Name != name) throw new InvalidDataException("Saved lottery resistance unavailable: " + name);
                        prize.KBMDJACLAOH.Add(new FightResult.OLJIFHLGHNM {
                            JIDLBLPFAAE = new ResistanceStruct(resistance, checked((int)Number(node, "Count"))), IDGKPLBKDIB = drop
                        });
                        break;
                    default: throw new InvalidDataException("Unknown saved lottery prize entry: " + node.Name);
                }
            }
            return prize;
        }

        private static void Set(XmlElement node, string key, long value)
        {
            if (value < 0) throw new InvalidDataException("Negative lottery prize value: " + key);
            node.SetAttribute(key, value.ToString(CultureInfo.InvariantCulture));
        }

        private static long Number(XmlElement node, string key)
        {
            if (!long.TryParse(node.GetAttribute(key), NumberStyles.None, CultureInfo.InvariantCulture, out long value))
                throw new InvalidDataException("Invalid saved lottery prize value: " + key);
            return value;
        }
    }
}
