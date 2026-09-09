using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
    public sealed class LocaleFontDefinition
    {
        public string Content { get; }
        public string Title { get; }
        public string Button { get; }
        public float FontSizeScale { get; }
        public float LineSpacing { get; }
        public float CustomLineSpacingScale { get; }

        public LocaleFontDefinition(string content, string title, string button, float fontSizeScale = 1f,
            float lineSpacing = 1f, float customLineSpacingScale = 1f)
        {
            Content = Required(content, "Locale content font");
            Title = Required(title, "Locale title font");
            Button = Required(button, "Locale button font");
            FontSizeScale = Positive(fontSizeScale, "Locale font-size scale");
            LineSpacing = Positive(lineSpacing, "Locale line spacing");
            CustomLineSpacingScale = Positive(customLineSpacingScale, "Locale custom line-spacing scale");
        }

        private static string Required(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ModContentException(name + " must not be empty.");
            return value.Trim();
        }

        private static float Positive(float value, string name)
        {
            if (float.IsNaN(value) || float.IsInfinity(value) || value <= 0f)
                throw new ModContentException(name + " must be finite and greater than zero.");
            return value;
        }
    }

    public sealed class LocaleMetadataDefinition
    {
        public DefinitionId Id { get; }
        public string Name { get; }
        public string Locale { get; }
        public string Alias { get; }
        public string FileIcon { get; }
        public string FileIconSelected { get; }
        public string LoaderImage { get; }
        public string PreloaderImage { get; }
        public bool IsAsian { get; }
        public LocaleFontDefinition Fonts { get; }

        internal LocaleMetadataDefinition(DefinitionId id, string name, string locale, string alias,
            string fileIcon, string fileIconSelected, string loaderImage, string preloaderImage, bool isAsian,
            LocaleFontDefinition fonts)
        {
            Id = id;
            Name = Required(name, "Locale name");
            Locale = Required(locale, "Locale platform code");
            Alias = alias ?? string.Empty;
            FileIcon = fileIcon ?? string.Empty;
            FileIconSelected = fileIconSelected ?? string.Empty;
            LoaderImage = loaderImage ?? string.Empty;
            PreloaderImage = preloaderImage ?? string.Empty;
            IsAsian = isAsian;
            Fonts = fonts;
        }

        private static string Required(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ModContentException(name + " must not be empty.");
            return value.Trim();
        }
    }

    public sealed class LocationImageDefinition
    {
        public AssetId Sprite { get; }
        public float X { get; }
        public float Y { get; }
        public float Width { get; }
        public float Height { get; }
        public bool IsOpaque { get; }
        public bool FlipX { get; }
        public bool FlipY { get; }
        public bool IsMask { get; }

        public LocationImageDefinition(AssetId sprite, float x, float y, float width, float height,
            bool isOpaque = false, bool flipX = false, bool flipY = false, bool isMask = false)
        {
            if (string.IsNullOrEmpty(sprite.Path)) throw new ModContentException("Location image requires a sprite asset.");
            ValidateFinite(x, "Location image X");
            ValidateFinite(y, "Location image Y");
            if (!IsPositive(width) || !IsPositive(height))
                throw new ModContentException("Location image width and height must be finite and greater than zero.");
            Sprite = sprite;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            IsOpaque = isOpaque;
            FlipX = flipX;
            FlipY = flipY;
            IsMask = isMask;
        }

        private static bool IsPositive(float value) => !float.IsNaN(value) && !float.IsInfinity(value) && value > 0f;
        private static void ValidateFinite(float value, string name)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) throw new ModContentException(name + " must be finite.");
        }
    }

    public sealed class LocationFighterPositions
    {
        public float PlayerX { get; }
        public float PlayerY { get; }
        public float EnemyX { get; }
        public float EnemyY { get; }

        public LocationFighterPositions(float playerX, float playerY, float enemyX, float enemyY)
        {
            foreach (float value in new[] { playerX, playerY, enemyX, enemyY })
                if (float.IsNaN(value) || float.IsInfinity(value))
                    throw new ModContentException("Location fighter positions must be finite.");
            PlayerX = playerX; PlayerY = playerY; EnemyX = enemyX; EnemyY = enemyY;
        }
    }

    public sealed class LocationLayerDefinition
    {
        private readonly LocationImageDefinition[] _images;
        public int Type { get; }
        public float Factor { get; }
        public bool Scaling { get; }
        public LocationFighterPositions Fighters { get; }
        public IReadOnlyList<LocationImageDefinition> Images => _images;

        public LocationLayerDefinition(int type, float factor, bool scaling, LocationImageDefinition[] images,
            LocationFighterPositions fighters = null)
        {
            if (float.IsNaN(factor) || float.IsInfinity(factor))
                throw new ModContentException("Location layer factor must be finite.");
            if ((images == null || images.Length == 0) && fighters == null)
                throw new ModContentException("Location layer requires a typed sprite image or fighter positions.");
            if (fighters != null && type != 2)
                throw new ModContentException("Location fighter positions require the recovered gameplay layer (type 2).");
            Type = type;
            Factor = factor;
            Scaling = scaling;
            _images = images == null ? Array.Empty<LocationImageDefinition>() : (LocationImageDefinition[])images.Clone();
            Fighters = fighters;
        }
    }

    public sealed class LocationDefinition
    {
        private readonly LocationLayerDefinition[] _layers;
        public DefinitionId Id { get; }
        public string RuntimeName => Id.ToString();
        public string Color { get; }
        public float Wall { get; }
        public float Floor { get; }
        public float PositionY { get; }
        public float Width { get; }
        public float Height { get; }
        public float MinWidth { get; }
        public float FrictionForce { get; }
        public int GridSize { get; }
        public AssetId Music { get; }
        public bool HasMusic => !string.IsNullOrEmpty(Music.Path);
        public IReadOnlyList<LocationLayerDefinition> Layers => _layers;

        internal LocationDefinition(DefinitionId id, string color, float wall, float floor, float positionY,
            float width, float height, float minWidth, float frictionForce, int gridSize, AssetId music,
            LocationLayerDefinition[] layers)
        {
            if (!Positive(width) || !Positive(height) || !Positive(minWidth))
                throw new ModContentException("Location width, height, and min-width must be finite and greater than zero.");
            Finite(wall, "Location wall");
            Finite(floor, "Location floor");
            Finite(positionY, "Location position Y");
            Finite(frictionForce, "Location friction force");
            if (gridSize < 0) throw new ModContentException("Location grid size must not be negative.");
            if (layers == null || layers.Length == 0) throw new ModContentException("Location requires at least one layer.");
            Id = id;
            Color = string.IsNullOrWhiteSpace(color) ? "0x000000" : color.Trim();
            Wall = wall;
            Floor = floor;
            PositionY = positionY;
            Width = width;
            Height = height;
            MinWidth = minWidth;
            FrictionForce = frictionForce;
            GridSize = gridSize;
            Music = music;
            _layers = (LocationLayerDefinition[])layers.Clone();
        }

        private static bool Positive(float value) => !float.IsNaN(value) && !float.IsInfinity(value) && value > 0f;
        private static void Finite(float value, string name)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) throw new ModContentException(name + " must be finite.");
        }
    }

    public enum ModMoveEventKind
    {
        AnimationEnd, AnimationStart, IntervalEnd, IntervalStart, Hit, Strike, EveryFrame, Birth,
        RoundStageStart, ModExpires
    }

    public sealed class ModMoveEvent
    {
        public ModMoveEventKind Kind { get; }
        public string Name { get; }
        public string Player { get; }
        public ModMoveEvent(ModMoveEventKind kind, string name = null, string player = null)
        {
            Kind = kind;
            Name = name ?? string.Empty;
            Player = player ?? string.Empty;
        }
    }

    public enum ModMoveConditionKind { CurrentAnimation, CurrentInterval, Item, All, Any, Perk }

    public sealed class ModMoveCondition
    {
        private readonly ModMoveCondition[] _children;
        public ModMoveConditionKind Kind { get; }
        public string Name { get; }
        public string Player { get; }
        public string ItemType { get; }
        public string ItemSubType { get; }
        public bool Not { get; }
        public IReadOnlyList<ModMoveCondition> Children => _children;

        public ModMoveCondition(ModMoveConditionKind kind, string name = null, string player = null,
            string itemType = null, string itemSubType = null, bool not = false, ModMoveCondition[] children = null)
        {
            Kind = kind;
            Name = name ?? string.Empty;
            Player = player ?? string.Empty;
            ItemType = itemType ?? string.Empty;
            ItemSubType = itemSubType ?? string.Empty;
            Not = not;
            _children = children == null ? Array.Empty<ModMoveCondition>() : (ModMoveCondition[])children.Clone();
            bool group = kind == ModMoveConditionKind.All || kind == ModMoveConditionKind.Any;
            if (group && _children.Length == 0) throw new ModContentException("Grouped move condition requires children.");
            if (!group && _children.Length != 0) throw new ModContentException("Only grouped move conditions may have children.");
        }
    }

    public sealed class ModMoveInterval
    {
        public string Type { get; }
        public string Name { get; }
        public ModMoveInterval(string type = null, string name = null)
        {
            Type = type ?? string.Empty;
            Name = name ?? string.Empty;
            if (Type.Length == 0 && Name.Length == 0)
                throw new ModContentException("Move interval requires a type or name.");
        }
    }

    public enum ModMoveActionKind { Sound, HitEffect }

    public sealed class ModMoveAction
    {
        public ModMoveActionKind Kind { get; }
        public AssetId Audio { get; }
        public string Name { get; }
        public float Volume { get; }
        public bool Looped { get; }

        private ModMoveAction(ModMoveActionKind kind, AssetId audio, string name, float volume, bool looped)
        {
            Kind = kind;
            Audio = audio;
            Name = name ?? string.Empty;
            Volume = volume;
            Looped = looped;
        }

        public static ModMoveAction Sound(AssetId audio, float volume = 1f, bool looped = false)
        {
            if (string.IsNullOrEmpty(audio.Path)) throw new ModContentException("Move sound action requires an audio asset.");
            if (float.IsNaN(volume) || float.IsInfinity(volume) || volume < 0f)
                throw new ModContentException("Move sound volume must be finite and non-negative.");
            return new ModMoveAction(ModMoveActionKind.Sound, audio, null, volume, looped);
        }

        public static ModMoveAction HitEffect(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ModContentException("Hit-effect name must not be empty.");
            return new ModMoveAction(ModMoveActionKind.HitEffect, default, name.Trim(), 1f, false);
        }
    }

    public abstract class MoveNodeDefinition
    {
        private readonly DefinitionId[] _templates;
        private readonly string[] _coreTemplates;
        private readonly ModMoveEvent[] _events;
        private readonly ModMoveCondition[] _conditions;
        private readonly ModMoveInterval[] _intervals;
        public DefinitionId Id { get; }
        public string RuntimeName => Id.ToString();
        public IReadOnlyList<DefinitionId> Templates => _templates;
        public IReadOnlyList<string> CoreTemplates => _coreTemplates;
        public IReadOnlyList<ModMoveEvent> Events => _events;
        public IReadOnlyList<ModMoveCondition> Conditions => _conditions;
        public IReadOnlyList<ModMoveInterval> Intervals => _intervals;
        public string Type { get; }
        public int Priority { get; }
        public int MidFrames { get; }
        public int FirstFrame { get; }
        public int EndFrame { get; }
        public string MirrorNode { get; }
        public string TacticEquivalent { get; }
        public string TacticWeapon { get; }
        public bool Looped { get; }
        public bool EndsStage { get; }

        protected MoveNodeDefinition(DefinitionId id, DefinitionId[] templates, string[] coreTemplates,
            ModMoveEvent[] events, ModMoveCondition[] conditions, ModMoveInterval[] intervals, string type,
            int priority, int midFrames, int firstFrame, int endFrame, string mirrorNode, string tacticEquivalent,
            string tacticWeapon, bool looped, bool endsStage)
        {
            if (priority < 0 || midFrames < 0 || firstFrame < 0 || endFrame < 0)
                throw new ModContentException("Move/template frame and priority values must not be negative.");
            Id = id;
            _templates = templates == null ? Array.Empty<DefinitionId>() : (DefinitionId[])templates.Clone();
            _coreTemplates = coreTemplates == null ? Array.Empty<string>() : (string[])coreTemplates.Clone();
            _events = events == null ? Array.Empty<ModMoveEvent>() : (ModMoveEvent[])events.Clone();
            _conditions = conditions == null ? Array.Empty<ModMoveCondition>() : (ModMoveCondition[])conditions.Clone();
            _intervals = intervals == null ? Array.Empty<ModMoveInterval>() : (ModMoveInterval[])intervals.Clone();
            Type = type ?? string.Empty;
            Priority = priority;
            MidFrames = midFrames;
            FirstFrame = firstFrame;
            EndFrame = endFrame;
            MirrorNode = mirrorNode ?? string.Empty;
            TacticEquivalent = tacticEquivalent ?? string.Empty;
            TacticWeapon = tacticWeapon ?? string.Empty;
            Looped = looped;
            EndsStage = endsStage;
        }
    }

    public sealed class MoveTemplateDefinition : MoveNodeDefinition
    {
        internal MoveTemplateDefinition(DefinitionId id, DefinitionId[] templates, string[] coreTemplates,
            ModMoveEvent[] events, ModMoveCondition[] conditions, ModMoveInterval[] intervals, string type,
            int priority, int midFrames, int firstFrame, int endFrame, string mirrorNode, string tacticEquivalent,
            string tacticWeapon, bool looped, bool endsStage)
            : base(id, templates, coreTemplates, events, conditions, intervals, type, priority, midFrames, firstFrame,
                endFrame, mirrorNode, tacticEquivalent, tacticWeapon, looped, endsStage) { }
    }

    public sealed class MoveDefinition : MoveNodeDefinition
    {
        public AssetId Animation { get; }
        internal MoveDefinition(DefinitionId id, AssetId animation, DefinitionId[] templates, string[] coreTemplates,
            ModMoveEvent[] events, ModMoveCondition[] conditions, ModMoveInterval[] intervals, string type,
            int priority, int midFrames, int firstFrame, int endFrame, string mirrorNode, string tacticEquivalent,
            string tacticWeapon, bool looped, bool endsStage)
            : base(id, templates, coreTemplates, events, conditions, intervals, type, priority, midFrames, firstFrame,
                endFrame, mirrorNode, tacticEquivalent, tacticWeapon, looped, endsStage)
        {
            if (string.IsNullOrEmpty(animation.Path)) throw new ModContentException("Move requires a binary animation asset.");
            Animation = animation;
        }
    }

    public sealed class MoveTriggerDefinition
    {
        private readonly ModMoveEvent[] _events;
        private readonly ModMoveCondition[] _conditions;
        private readonly ModMoveAction[] _actions;
        public DefinitionId Id { get; }
        public string RuntimeName => Id.ToString();
        public IReadOnlyList<ModMoveEvent> Events => _events;
        public IReadOnlyList<ModMoveCondition> Conditions => _conditions;
        public IReadOnlyList<ModMoveAction> Actions => _actions;
        internal MoveTriggerDefinition(DefinitionId id, ModMoveEvent[] events, ModMoveCondition[] conditions,
            ModMoveAction[] actions)
        {
            if (events == null || events.Length == 0) throw new ModContentException("Move trigger requires at least one event.");
            Id = id;
            _events = (ModMoveEvent[])events.Clone();
            _conditions = conditions == null ? Array.Empty<ModMoveCondition>() : (ModMoveCondition[])conditions.Clone();
            _actions = actions == null ? Array.Empty<ModMoveAction>() : (ModMoveAction[])actions.Clone();
        }
    }

    public enum ModTacticKind { Random, Tabular }
    public enum ModTacticFactorType { Linear, Exponential }

    public sealed class ModTacticValue
    {
        public float Base { get; }
        public float CounterFactor { get; }
        public float DamageFactor { get; }
        public float HealthFactor { get; }
        public float EnemyHealthFactor { get; }
        public float AnimationFramesFactor { get; }
        public float ChildFramesFactor { get; }
        public float MagicBulletFactor { get; }
        public float MissileBulletFactor { get; }
        public float HitFactor { get; }
        public float DistanceFactor { get; }
        public float Shift { get; }
        public float Limit { get; }
        public float AntiLimit { get; }
        public ModTacticFactorType FactorType { get; }

        public ModTacticValue(float @base = 0f, float counterFactor = 0f, float damageFactor = 0f,
            float healthFactor = 0f, float enemyHealthFactor = 0f, float animationFramesFactor = 0f,
            float childFramesFactor = 0f, float magicBulletFactor = 0f, float missileBulletFactor = 0f,
            float hitFactor = 0f, float distanceFactor = 0f, float shift = 0f, float limit = 0f,
            float antiLimit = 0f, ModTacticFactorType factorType = ModTacticFactorType.Linear)
        {
            float[] values = { @base, counterFactor, damageFactor, healthFactor, enemyHealthFactor,
                animationFramesFactor, childFramesFactor, magicBulletFactor, missileBulletFactor, hitFactor,
                distanceFactor, shift, limit, antiLimit };
            for (int i = 0; i < values.Length; i++)
                if (float.IsNaN(values[i]) || float.IsInfinity(values[i]))
                    throw new ModContentException("Tactic values must be finite.");
            Base = @base; CounterFactor = counterFactor; DamageFactor = damageFactor; HealthFactor = healthFactor;
            EnemyHealthFactor = enemyHealthFactor; AnimationFramesFactor = animationFramesFactor;
            ChildFramesFactor = childFramesFactor; MagicBulletFactor = magicBulletFactor;
            MissileBulletFactor = missileBulletFactor; HitFactor = hitFactor; DistanceFactor = distanceFactor;
            Shift = shift; Limit = limit; AntiLimit = antiLimit; FactorType = factorType;
        }
    }

    public sealed class ModTacticAnimationValue
    {
        public DefinitionId Move { get; }
        public string Animation { get; }
        public bool HasMove => !string.IsNullOrEmpty(Move.LocalId);
        public ModTacticValue Value { get; }
        public ModTacticAnimationValue(DefinitionId move, string animation, ModTacticValue value)
        {
            bool hasMove = !string.IsNullOrEmpty(move.LocalId);
            bool hasName = !string.IsNullOrWhiteSpace(animation);
            if (hasMove == hasName) throw new ModContentException("Tactic animation entry requires exactly one move handle or core animation name.");
            Move = move;
            Animation = hasName ? animation.Trim() : string.Empty;
            Value = value ?? new ModTacticValue();
        }
    }

    public sealed class TacticDefinition
    {
        private readonly ModTacticAnimationValue[] _weights;
        private readonly ModTacticAnimationValue[] _quickAttacks;
        private readonly ModTacticAnimationValue[] _evades;
        private readonly ModTacticAnimationValue[] _expectedWait;
        public DefinitionId Id { get; }
        public string RuntimeName => Id.ToString();
        public ModTacticKind Kind { get; }
        public string CoreTemplate { get; }
        public int MemoryStrikes { get; }
        public float MemoryRoundFactor { get; }
        public ModTacticValue CounterAttack { get; }
        public ModTacticValue Dodge { get; }
        public ModTacticValue Block { get; }
        public ModTacticValue SafeAttack { get; }
        public ModTacticValue TableAttack { get; }
        public ModTacticValue CautiousMovement { get; }
        public ModTacticValue DodgeMissiles { get; }
        public ModTacticValue DodgeMagic { get; }
        public IReadOnlyList<ModTacticAnimationValue> AnimationWeights => _weights;
        public IReadOnlyList<ModTacticAnimationValue> QuickAttacks => _quickAttacks;
        public IReadOnlyList<ModTacticAnimationValue> Evades => _evades;
        public IReadOnlyList<ModTacticAnimationValue> ExpectedWait => _expectedWait;

        internal TacticDefinition(DefinitionId id, ModTacticKind kind, string coreTemplate, int memoryStrikes,
            float memoryRoundFactor, ModTacticValue counterAttack, ModTacticValue dodge, ModTacticValue block,
            ModTacticValue safeAttack, ModTacticValue tableAttack, ModTacticValue cautiousMovement,
            ModTacticValue dodgeMissiles, ModTacticValue dodgeMagic, ModTacticAnimationValue[] weights,
            ModTacticAnimationValue[] quickAttacks, ModTacticAnimationValue[] evades,
            ModTacticAnimationValue[] expectedWait)
        {
            if (memoryStrikes < 0 || float.IsNaN(memoryRoundFactor) || float.IsInfinity(memoryRoundFactor))
                throw new ModContentException("Tactic memory values are invalid.");
            Id = id; Kind = kind; CoreTemplate = coreTemplate ?? string.Empty; MemoryStrikes = memoryStrikes;
            MemoryRoundFactor = memoryRoundFactor; CounterAttack = counterAttack; Dodge = dodge; Block = block;
            SafeAttack = safeAttack; TableAttack = tableAttack; CautiousMovement = cautiousMovement;
            DodgeMissiles = dodgeMissiles; DodgeMagic = dodgeMagic;
            _weights = weights ?? Array.Empty<ModTacticAnimationValue>();
            _quickAttacks = quickAttacks ?? Array.Empty<ModTacticAnimationValue>();
            _evades = evades ?? Array.Empty<ModTacticAnimationValue>();
            _expectedWait = expectedWait ?? Array.Empty<ModTacticAnimationValue>();
        }
    }

    public sealed partial class ModContentCatalog
    {
        private readonly DefinitionRegistry<LocaleMetadataDefinition> _localeMetadata = new DefinitionRegistry<LocaleMetadataDefinition>(v => v.Id);
        private readonly DefinitionRegistry<LocationDefinition> _locations = new DefinitionRegistry<LocationDefinition>(v => v.Id);
        private readonly DefinitionRegistry<MoveTemplateDefinition> _moveTemplates = new DefinitionRegistry<MoveTemplateDefinition>(v => v.Id);
        private readonly DefinitionRegistry<MoveDefinition> _moves = new DefinitionRegistry<MoveDefinition>(v => v.Id);
        private readonly DefinitionRegistry<MoveTriggerDefinition> _moveTriggers = new DefinitionRegistry<MoveTriggerDefinition>(v => v.Id);
        private readonly DefinitionRegistry<TacticDefinition> _tactics = new DefinitionRegistry<TacticDefinition>(v => v.Id);

        public IReadOnlyList<LocaleMetadataDefinition> LocaleMetadata => _localeMetadata.Values;
        public IReadOnlyList<LocationDefinition> Locations => _locations.Values;
        public IReadOnlyList<MoveTemplateDefinition> MoveTemplates => _moveTemplates.Values;
        public IReadOnlyList<MoveDefinition> Moves => _moves.Values;
        public IReadOnlyList<MoveTriggerDefinition> MoveTriggers => _moveTriggers.Values;
        public IReadOnlyList<TacticDefinition> Tactics => _tactics.Values;
        public bool TryGetMove(DefinitionId id, out MoveDefinition value) => _moves.TryGet(id, out value);
        public bool TryGetMoveTemplate(DefinitionId id, out MoveTemplateDefinition value) => _moveTemplates.TryGet(id, out value);

        internal void ValidateP1DCanAdd(LocaleMetadataDefinition[] locales, LocationDefinition[] locations,
            MoveTemplateDefinition[] templates, MoveDefinition[] moves, MoveTriggerDefinition[] triggers,
            TacticDefinition[] tactics)
        {
            _localeMetadata.ValidateCanAdd(locales);
            _locations.ValidateCanAdd(locations);
            _moveTemplates.ValidateCanAdd(templates);
            _moves.ValidateCanAdd(moves);
            _moveTriggers.ValidateCanAdd(triggers);
            _tactics.ValidateCanAdd(tactics);
        }

        internal void AddP1D(LocaleMetadataDefinition[] locales, LocationDefinition[] locations,
            MoveTemplateDefinition[] templates, MoveDefinition[] moves, MoveTriggerDefinition[] triggers,
            TacticDefinition[] tactics)
        {
            _localeMetadata.AddRange(locales);
            _locations.AddRange(locations);
            _moveTemplates.AddRange(templates);
            _moves.AddRange(moves);
            _moveTriggers.AddRange(triggers);
            _tactics.AddRange(tactics);
        }
    }

    public sealed partial class ModRegistrationTransaction
    {
        private readonly Dictionary<DefinitionId, LocaleMetadataDefinition> _p1dLocales = new Dictionary<DefinitionId, LocaleMetadataDefinition>();
        private readonly Dictionary<DefinitionId, LocationDefinition> _p1dLocations = new Dictionary<DefinitionId, LocationDefinition>();
        private readonly Dictionary<DefinitionId, MoveTemplateDefinition> _p1dMoveTemplates = new Dictionary<DefinitionId, MoveTemplateDefinition>();
        private readonly Dictionary<DefinitionId, MoveDefinition> _p1dMoves = new Dictionary<DefinitionId, MoveDefinition>();
        private readonly Dictionary<DefinitionId, MoveTriggerDefinition> _p1dMoveTriggers = new Dictionary<DefinitionId, MoveTriggerDefinition>();
        private readonly Dictionary<DefinitionId, TacticDefinition> _p1dTactics = new Dictionary<DefinitionId, TacticDefinition>();

        private int P1DRegistrationCount => _p1dLocales.Count + _p1dLocations.Count + _p1dMoveTemplates.Count +
            _p1dMoves.Count + _p1dMoveTriggers.Count + _p1dTactics.Count;

        public LocaleMetadataDefinition RegisterLocaleMetadata(string localId, string name, string locale, string alias,
            string fileIcon, string fileIconSelected, string loaderImage, string preloaderImage, bool isAsian,
            LocaleFontDefinition fonts)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("locales", localId);
            var value = new LocaleMetadataDefinition(id, name, locale, alias, fileIcon, fileIconSelected, loaderImage,
                preloaderImage, isAsian, fonts);
            AddP1D(_p1dLocales, id, value);
            return value;
        }

        public LocationDefinition RegisterLocation(string localId, string color, float wall, float floor,
            float positionY, float width, float height, float minWidth, float frictionForce, int gridSize,
            AssetId music, LocationLayerDefinition[] layers)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("locations", localId);
            ValidateAssetReference(music, "location music");
            if (layers != null)
                for (int i = 0; i < layers.Length; i++)
                    for (int j = 0; j < layers[i].Images.Count; j++)
                        ValidateAssetReference(layers[i].Images[j].Sprite, "location sprite");
            var value = new LocationDefinition(id, color, wall, floor, positionY, width, height, minWidth,
                frictionForce, gridSize, music, layers);
            AddP1D(_p1dLocations, id, value);
            return value;
        }

        public MoveTemplateDefinition RegisterMoveTemplate(string localId, DefinitionId[] templates,
            string[] coreTemplates, ModMoveEvent[] events, ModMoveCondition[] conditions, ModMoveInterval[] intervals,
            string type, int priority, int midFrames, int firstFrame, int endFrame, string mirrorNode,
            string tacticEquivalent, string tacticWeapon, bool looped, bool endsStage)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("move-templates", localId);
            var value = new MoveTemplateDefinition(id, templates, coreTemplates, events, conditions, intervals, type,
                priority, midFrames, firstFrame, endFrame, mirrorNode, tacticEquivalent, tacticWeapon, looped, endsStage);
            AddP1D(_p1dMoveTemplates, id, value);
            return value;
        }

        public MoveDefinition RegisterMove(string localId, AssetId animation, DefinitionId[] templates,
            string[] coreTemplates, ModMoveEvent[] events, ModMoveCondition[] conditions, ModMoveInterval[] intervals,
            string type, int priority, int midFrames, int firstFrame, int endFrame, string mirrorNode,
            string tacticEquivalent, string tacticWeapon, bool looped, bool endsStage)
        {
            ThrowIfCompleted();
            ValidateAssetReference(animation, "move animation");
            DefinitionId id = Qualify("moves", localId);
            var value = new MoveDefinition(id, animation, templates, coreTemplates, events, conditions, intervals, type,
                priority, midFrames, firstFrame, endFrame, mirrorNode, tacticEquivalent, tacticWeapon, looped, endsStage);
            AddP1D(_p1dMoves, id, value);
            return value;
        }

        public MoveTriggerDefinition RegisterMoveTrigger(string localId, ModMoveEvent[] events,
            ModMoveCondition[] conditions, ModMoveAction[] actions)
        {
            ThrowIfCompleted();
            if (actions != null)
                for (int i = 0; i < actions.Length; i++)
                    if (actions[i].Kind == ModMoveActionKind.Sound) ValidateAssetReference(actions[i].Audio, "trigger audio");
            DefinitionId id = Qualify("move-triggers", localId);
            var value = new MoveTriggerDefinition(id, events, conditions, actions);
            AddP1D(_p1dMoveTriggers, id, value);
            return value;
        }

        public TacticDefinition RegisterTactic(string localId, ModTacticKind kind, string coreTemplate,
            int memoryStrikes, float memoryRoundFactor, ModTacticValue counterAttack, ModTacticValue dodge,
            ModTacticValue block, ModTacticValue safeAttack, ModTacticValue tableAttack,
            ModTacticValue cautiousMovement, ModTacticValue dodgeMissiles, ModTacticValue dodgeMagic,
            ModTacticAnimationValue[] weights, ModTacticAnimationValue[] quickAttacks,
            ModTacticAnimationValue[] evades, ModTacticAnimationValue[] expectedWait)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("tactics", localId);
            var value = new TacticDefinition(id, kind, coreTemplate, memoryStrikes, memoryRoundFactor, counterAttack,
                dodge, block, safeAttack, tableAttack, cautiousMovement, dodgeMissiles, dodgeMagic, weights,
                quickAttacks, evades, expectedWait);
            AddP1D(_p1dTactics, id, value);
            return value;
        }

        private void ValidateP1DCommit()
        {
            LocaleMetadataDefinition[] locales = Values(_p1dLocales);
            LocationDefinition[] locations = Values(_p1dLocations);
            MoveTemplateDefinition[] templates = Values(_p1dMoveTemplates);
            MoveDefinition[] moves = Values(_p1dMoves);
            MoveTriggerDefinition[] triggers = Values(_p1dMoveTriggers);
            TacticDefinition[] tactics = Values(_p1dTactics);
            _catalog.ValidateP1DCanAdd(locales, locations, templates, moves, triggers, tactics);
            for (int i = 0; i < templates.Length; i++) ValidateTemplateRefs(templates[i]);
            for (int i = 0; i < moves.Length; i++) ValidateTemplateRefs(moves[i]);
            for (int i = 0; i < triggers.Length; i++) ValidateMovePerkRefs(triggers[i].Conditions);
            for (int i = 0; i < tactics.Length; i++)
            {
                ValidateTacticMoveRefs(tactics[i].AnimationWeights);
                ValidateTacticMoveRefs(tactics[i].QuickAttacks);
                ValidateTacticMoveRefs(tactics[i].Evades);
                ValidateTacticMoveRefs(tactics[i].ExpectedWait);
            }
        }

        private void ApplyP1DCommit()
        {
            _catalog.AddP1D(Values(_p1dLocales), Values(_p1dLocations), Values(_p1dMoveTemplates),
                Values(_p1dMoves), Values(_p1dMoveTriggers), Values(_p1dTactics));
        }

        private void ClearP1DPending()
        {
            _p1dLocales.Clear(); _p1dLocations.Clear(); _p1dMoveTemplates.Clear(); _p1dMoves.Clear();
            _p1dMoveTriggers.Clear(); _p1dTactics.Clear();
        }

        private void ValidateTemplateRefs(MoveNodeDefinition node)
        {
            ValidateMovePerkRefs(node.Conditions);
            for (int i = 0; i < node.Templates.Count; i++)
            {
                DefinitionId id = node.Templates[i];
                if (id.Category != "move-templates" || !CanReferenceNamespace(id.Namespace))
                    throw new ModContentException("Move/template references invalid template '" + id + "'.");
                if (!_p1dMoveTemplates.ContainsKey(id) && !_catalog.TryGetMoveTemplate(id, out MoveTemplateDefinition ignored))
                    throw new ModContentException("Move/template references missing template '" + id + "'.");
            }
        }

        private void ValidateMovePerkRefs(IReadOnlyList<ModMoveCondition> conditions)
        {
            foreach (ModMoveCondition condition in conditions)
            {
                if (condition.Kind == ModMoveConditionKind.Perk) GetPerk(condition.Name);
                ValidateMovePerkRefs(condition.Children);
            }
        }

        private void ValidateTacticMoveRefs(IReadOnlyList<ModTacticAnimationValue> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (!values[i].HasMove) continue;
                DefinitionId id = values[i].Move;
                if (id.Category != "moves" || !CanReferenceNamespace(id.Namespace) ||
                    (!_p1dMoves.ContainsKey(id) && !_catalog.TryGetMove(id, out MoveDefinition ignored)))
                    throw new ModContentException("Tactic references missing or inaccessible move '" + id + "'.");
            }
        }

        private void ValidateAssetReference(AssetId id, string kind)
        {
            if (string.IsNullOrEmpty(id.Path)) return;
            if (!CanReferenceNamespace(id.Namespace))
                throw new ModContentException("Mod '" + Mod.Id + "' cannot reference undeclared namespace '" +
                    id.Namespace + "' for " + kind + ".");
        }

        private void AddP1D<T>(Dictionary<DefinitionId, T> target, DefinitionId id, T value)
        {
            EnsureCapacityForNewRegistration();
            if (target.ContainsKey(id)) throw new ModContentException("Duplicate definition: '" + id + "'.");
            target.Add(id, value);
        }

        private static T[] Values<T>(Dictionary<DefinitionId, T> values)
        {
            var result = new T[values.Count];
            values.Values.CopyTo(result, 0);
            Array.Sort(result, (a, b) => string.CompareOrdinal(IdOf(a).ToString(), IdOf(b).ToString()));
            return result;
        }

        private static DefinitionId IdOf<T>(T value)
        {
            if (value is LocaleMetadataDefinition locale) return locale.Id;
            if (value is LocationDefinition location) return location.Id;
            if (value is MoveTemplateDefinition template) return template.Id;
            if (value is MoveDefinition move) return move.Id;
            if (value is MoveTriggerDefinition trigger) return trigger.Id;
            if (value is TacticDefinition tactic) return tactic.Id;
            throw new InvalidOperationException("Unknown P1D definition type.");
        }
    }
}
