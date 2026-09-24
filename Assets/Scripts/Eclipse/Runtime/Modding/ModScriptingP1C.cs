using System.Collections.Generic;

namespace Eclipse.Modding
{
    public sealed partial class ModApiFacade
    {
        public void SetItemPresentation(DefinitionId item, AssetId icon, AssetId model)
        {
            RequireCapability("content.patch");
            RequireRegistration().SetItemPresentation(item, icon, model);
        }

        public void SetItemShopPrice(DefinitionId item, ModPrice price, ModPrice? secondaryPrice = null)
        {
            RequireCapability("content.patch");
            RequireRegistration().SetItemShopPrice(item, price, secondaryPrice);
        }

        public void SetCombatSubtype(DefinitionId item, string subtype)
        {
            RequireCapability("content.patch");
            RequireRegistration().SetCombatSubtype(item, subtype);
        }

        public void SetItemInitialProfile(DefinitionId item, int level, int upgradeLevel,
            ModEquipmentInitialStats initialStats, string upgradeTemplate = null, string legacyPaidItem = null,
            bool clearLocalUpgrades = false)
        {
            RequireCapability("content.patch");
            RequireRegistration().SetItemInitialProfile(item, level, upgradeLevel, initialStats, upgradeTemplate,
                legacyPaidItem, clearLocalUpgrades);
        }

        public void SetTacticSubtype(DefinitionId item, string group)
        {
            RequireCapability("content.patch");
            RequireRegistration().SetTacticSubtype(item, group);
        }

        public void SetInnatePerks(DefinitionId item, ModInnatePerk[] entries)
        {
            RequireCapability("content.patch");
            RequireRegistration().SetInnatePerks(item, entries);
        }

        public void SetDefaultEnchantments(DefinitionId item, ModDefaultEnchantment[] entries)
        {
            RequireCapability("content.patch");
            RequireRegistration().SetDefaultEnchantments(item, entries);
        }

        public NonEquipmentItemDefinition RegisterNonEquipmentItem(string localId, ModNonEquipmentItemKind kind,
            DefinitionId displayName, AssetId icon, AssetId model, string subType, string packLabel,
            bool silentReceive, bool spendAfterUse)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterNonEquipmentItem(localId, kind, displayName, icon, model,
                subType, packLabel, silentReceive, spendAfterUse);
        }

        public ItemAvailabilityPolicyDefinition SetItemAvailability(DefinitionId item, ModItemVisibility visibility,
            string requiredGroup, int minimumLevel = 0)
        {
            RequireCapability("content.patch");
            return RequireRegistration().SetItemAvailability(item, visibility, requiredGroup, minimumLevel);
        }

        public ItemSetDefinition RegisterItemSet(string localId, DefinitionId title, DefinitionId text,
            DefinitionId brief, ModItemSetMember[] members)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterItemSet(localId, title, text, brief, members);
        }

        public ProgressionBranchOverlayDefinition ReplaceProgressionBranch(int level,
            ModProgressionPerkEntry[] entries)
        {
            RequireCapability("content.patch");
            return RequireRegistration().ReplaceProgressionBranch(level, entries);
        }

        public ForgeEconomicProfileDefinition GetForgeEconomicProfile(string reference)
        {
            RequireCapability("content.register");
            return RequireRegistration().GetForgeEconomicProfile(reference);
        }

        public ForgeRecipeFamilyDefinition RegisterForgeRecipeFamily(string localId, string alias,
            DefinitionId economicProfile, ModForgeRecipeItem[] items, ModForgeRecipeCandidate[] candidates)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterForgeRecipeFamily(localId, alias, economicProfile, items, candidates);
        }

        public void OverrideForgeDeviation(DefinitionId profile, ModEquipmentKind equipment, int minimum, int maximum)
        {
            RequireCapability("content.patch");
            RequireRegistration().OverrideForgeDeviation(profile, equipment, minimum, maximum);
        }

        public void ExcludeForgeCandidate(DefinitionId profile, DefinitionId perk, ModEquipmentKind equipment)
        {
            RequireCapability("content.patch");
            RequireRegistration().ExcludeForgeCandidate(profile, perk, equipment);
        }
    }
}
