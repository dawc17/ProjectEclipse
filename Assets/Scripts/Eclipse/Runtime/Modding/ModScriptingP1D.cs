namespace Eclipse.Modding
{
    public sealed partial class ModApiFacade
    {
        public LocaleMetadataDefinition RegisterLocaleMetadata(string localId, string name, string locale,
            string alias, string fileIcon, string fileIconSelected, string loaderImage, string preloaderImage,
            bool isAsian, LocaleFontDefinition fonts)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterLocaleMetadata(localId, name, locale, alias, fileIcon,
                fileIconSelected, loaderImage, preloaderImage, isAsian, fonts);
        }

        public LocationDefinition RegisterLocation(string localId, string color, float wall, float floor,
            float positionY, float width, float height, float minWidth, float frictionForce, int gridSize,
            AssetId music, LocationLayerDefinition[] layers, AssetId[] musicChoices = null, bool dojo = false)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterLocation(localId, color, wall, floor, positionY, width, height,
                minWidth, frictionForce, gridSize, music, layers, musicChoices, dojo);
        }

        public MoveTemplateDefinition RegisterMoveTemplate(string localId, DefinitionId[] templates,
            string[] coreTemplates, ModMoveEvent[] events, ModMoveCondition[] conditions, ModMoveInterval[] intervals,
            string type, int priority, int midFrames, int firstFrame, int endFrame, string mirrorNode,
            string tacticEquivalent, string tacticWeapon, bool looped, bool endsStage)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterMoveTemplate(localId, templates, coreTemplates, events, conditions,
                intervals, type, priority, midFrames, firstFrame, endFrame, mirrorNode, tacticEquivalent, tacticWeapon,
                looped, endsStage);
        }

        public MoveDefinition RegisterMove(string localId, AssetId animation, DefinitionId[] templates,
            string[] coreTemplates, ModMoveEvent[] events, ModMoveCondition[] conditions, ModMoveInterval[] intervals,
            string type, int priority, int midFrames, int firstFrame, int endFrame, string mirrorNode,
            string tacticEquivalent, string tacticWeapon, bool looped, bool endsStage)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterMove(localId, animation, templates, coreTemplates, events,
                conditions, intervals, type, priority, midFrames, firstFrame, endFrame, mirrorNode, tacticEquivalent,
                tacticWeapon, looped, endsStage);
        }

        public MoveTriggerDefinition RegisterMoveTrigger(string localId, ModMoveEvent[] events,
            ModMoveCondition[] conditions, ModMoveAction[] actions)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterMoveTrigger(localId, events, conditions, actions);
        }

        public TacticDefinition RegisterTactic(string localId, ModTacticKind kind, string coreTemplate,
            int memoryStrikes, float memoryRoundFactor, ModTacticValue counterAttack, ModTacticValue dodge,
            ModTacticValue block, ModTacticValue safeAttack, ModTacticValue tableAttack,
            ModTacticValue cautiousMovement, ModTacticValue dodgeMissiles, ModTacticValue dodgeMagic,
            ModTacticAnimationValue[] weights, ModTacticAnimationValue[] quickAttacks,
            ModTacticAnimationValue[] evades, ModTacticAnimationValue[] expectedWait)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterTactic(localId, kind, coreTemplate, memoryStrikes, memoryRoundFactor,
                counterAttack, dodge, block, safeAttack, tableAttack, cautiousMovement, dodgeMissiles, dodgeMagic,
                weights, quickAttacks, evades, expectedWait);
        }
    }
}
