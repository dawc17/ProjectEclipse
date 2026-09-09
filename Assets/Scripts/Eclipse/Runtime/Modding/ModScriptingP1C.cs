using System.Collections.Generic;

namespace Eclipse.Modding
{
    public sealed partial class ModApiFacade
    {
        public NonEquipmentItemDefinition RegisterNonEquipmentItem(string localId, ModNonEquipmentItemKind kind,
            DefinitionId displayName, AssetId icon, AssetId model, string subType, string packLabel,
            bool silentReceive, bool spendAfterUse)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterNonEquipmentItem(localId, kind, displayName, icon, model,
                subType, packLabel, silentReceive, spendAfterUse);
        }

        public ItemAvailabilityPolicyDefinition SetItemAvailability(DefinitionId item, ModItemVisibility visibility,
            string requiredGroup)
        {
            RequireCapability("content.patch");
            return RequireRegistration().SetItemAvailability(item, visibility, requiredGroup);
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
    }
}
