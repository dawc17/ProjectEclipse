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

    public sealed class LocationCurvePoint
    {
        public float Period { get; }
        public float Value { get; }
        public float Ease { get; }
        public LocationCurvePoint(float period, float value, float ease = 0)
        {
            if (float.IsNaN(period) || period < 1f / 60f || period > 120f ||
                float.IsNaN(value) || Math.Abs(value) > 10000f || float.IsNaN(ease) || Math.Abs(ease) > 1000f ||
                (ease != 0 && Math.Abs(ease) < 0.0001f))
                throw new ModContentException("Location curve points require period 1/60..120 seconds, value -10000..10000 and ease zero or magnitude 0.0001..1000.");
            Period = period; Value = value; Ease = ease;
        }
    }

    public sealed class LocationCurveDefinition
    {
        private readonly IReadOnlyList<LocationCurvePoint> _points;
        public IReadOnlyList<LocationCurvePoint> Points => _points;
        public float Offset { get; }
        public LocationCurveDefinition(float offset, LocationCurvePoint[] points)
        {
            if (points == null || points.Length < 2 || points.Length > 64)
                throw new ModContentException("Location curves require 2..64 points.");
            float duration = 0;
            foreach (var point in points)
            {
                if (point == null) throw new ModContentException("Location curve points cannot be null.");
                duration += point.Period;
            }
            if (float.IsNaN(offset) || offset < 0 || offset > duration)
                throw new ModContentException("Location curve offset must be between zero and its total duration.");
            Offset = offset; _points = Array.AsReadOnly((LocationCurvePoint[])points.Clone());
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
        public LocationCurveDefinition MotionX { get; }
        public LocationCurveDefinition MotionY { get; }
        public LocationCurveDefinition Rotation { get; }
        public LocationCurveDefinition Opacity { get; }
        public bool IsAnimated => MotionX != null || MotionY != null || Rotation != null || Opacity != null;

        public LocationImageDefinition(AssetId sprite, float x, float y, float width, float height,
            bool isOpaque = false, bool flipX = false, bool flipY = false, bool isMask = false,
            LocationCurveDefinition motionX = null, LocationCurveDefinition motionY = null,
            LocationCurveDefinition rotation = null, LocationCurveDefinition opacity = null)
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
            MotionX = motionX; MotionY = motionY; Rotation = rotation; Opacity = opacity;
            if (IsAnimated && (isMask || isOpaque))
                throw new ModContentException("Animated location images cannot use mask or opaque flags.");
            if (opacity != null)
                foreach (var point in opacity.Points)
                    if (point.Value < 0 || point.Value > 100)
                        throw new ModContentException("Location opacity values must be 0..100 percent.");
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
        public bool IsDojo { get; }
        public AssetId Music { get; }
        public IReadOnlyList<AssetId> MusicChoices { get; }
        public bool HasMusic => !string.IsNullOrEmpty(Music.Path);
        public IReadOnlyList<LocationLayerDefinition> Layers => _layers;

        internal LocationDefinition(DefinitionId id, string color, float wall, float floor, float positionY,
            float width, float height, float minWidth, float frictionForce, int gridSize, AssetId music,
            LocationLayerDefinition[] layers, AssetId[] musicChoices = null, bool dojo = false)
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
            IsDojo = dojo;
            Music = music;
            var choices = musicChoices ?? new AssetId[0];
            if (choices.Length > 16 || (HasMusic && choices.Length != 0))
                throw new ModContentException("Location accepts music or up to 16 music_choices, not both.");
            var unique = new HashSet<AssetId>();
            foreach (var choice in choices)
                if (string.IsNullOrEmpty(choice.Path) || !unique.Add(choice))
                    throw new ModContentException("Location music_choices require distinct audio assets.");
            MusicChoices = Array.AsReadOnly((AssetId[])choices.Clone());
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
        RoundStageStart, ModExpires, KeyPressed
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

    public enum ModMoveConditionKind { CurrentAnimation, CurrentInterval, Item, All, Any, Perk, Keys, Character, RoundStage, ModExists, Screen, ActorName, Bullets, Distance }

    public sealed class ModMoveKey
    {
        public string Key { get; }
        public string Press { get; }
        public ModMoveKey(string key,string press = "Tap")
        {
            if (Array.IndexOf(new[]{"Up","Up-Forward","Forward","Down-Forward","Down","Down-Back","Back","Up-Back","Punch","Kick","Ranged","Magic","RaidCharge","Super"},key)<0 ||
                (press!="Tap" && press!="Hold" && press!="Release")) throw new ModContentException("Invalid move key or press type.");
            Key=key; Press=press;
        }
    }

    public sealed class ModMoveBulletRange
    {
        public string Type { get; }
        public int Minimum { get; }
        public int Maximum { get; }
        public ModMoveBulletRange(string type, int minimum = 0, int maximum = int.MaxValue)
        {
            if (type != "MagicBullet" && type != "RaidChargeBullet") throw new ModContentException("Unsupported condition bullet type.");
            if (minimum < 0 || maximum < minimum) throw new ModContentException("Bullet bounds must be ordered nonnegative integers.");
            Type = type; Minimum = minimum; Maximum = maximum;
        }
    }

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
        public IReadOnlyList<ModMoveKey> Keys { get; }
        public ModMoveBulletRange Bullets { get; }
        public ModMoveDistance Distance { get; }

        public ModMoveCondition(ModMoveConditionKind kind, string name = null, string player = null,
            string itemType = null, string itemSubType = null, bool not = false, ModMoveCondition[] children = null, ModMoveKey[] keys = null, ModMoveBulletRange bullets = null, ModMoveDistance distance = null)
        {
            Kind = kind;
            Name = name ?? string.Empty;
            Player = player ?? string.Empty;
            ItemType = itemType ?? string.Empty;
            ItemSubType = itemSubType ?? string.Empty;
            Not = not;
            if ((kind == ModMoveConditionKind.Bullets) != (bullets != null)) throw new ModContentException("Only bullets conditions require a bullet range.");
            Bullets = bullets;
            if ((kind == ModMoveConditionKind.Distance) != (distance != null)) throw new ModContentException("Only distance conditions require a distance payload.");
            Distance = distance;
            if (kind == ModMoveConditionKind.Distance && (Name.Length != 0 || Player.Length != 0 || ItemType.Length != 0 || ItemSubType.Length != 0))
                throw new ModContentException("Distance uses from/to players, not named/item condition fields.");
            if (kind == ModMoveConditionKind.ActorName || kind == ModMoveConditionKind.Bullets)
            {
                if (kind == ModMoveConditionKind.ActorName) ModMoveScheduledAction.ValidateSymbol(Name, "actor");
                else if (Name.Length != 0) throw new ModContentException("Bullets condition does not accept name.");
                if (Player != "" && Array.IndexOf(new[] { "Me", "Enemy", "Parent", "Child", "EnemyChild", "Both" }, Player) < 0)
                    throw new ModContentException("Unsupported condition player.");
                if (ItemType.Length != 0 || ItemSubType.Length != 0) throw new ModContentException("Actor/bullets conditions do not accept item fields.");
            }
            Keys=Array.AsReadOnly(keys == null ? Array.Empty<ModMoveKey>() : (ModMoveKey[])keys.Clone());
            if ((kind==ModMoveConditionKind.Keys && (Keys.Count<1 || Keys.Count>14)) || (kind!=ModMoveConditionKind.Keys && Keys.Count!=0))
                throw new ModContentException("A keys condition requires 1..14 keys.");
            // Native KeyData preserves repeated taps as a sequence. Do not deduplicate it.
            foreach(var key in Keys) if(key==null) throw new ModContentException("Null move key.");
            if (kind == ModMoveConditionKind.RoundStage || kind == ModMoveConditionKind.ModExists || kind == ModMoveConditionKind.Screen)
            {
                if (string.IsNullOrWhiteSpace(Name) || Name.Length > 128 || Name != Name.Trim())
                    throw new ModContentException("Named move condition requires a name of 1..128 characters without surrounding whitespace.");
                if (Player != "" && Player != "Me" && Player != "Enemy" && Player != "Both")
                    throw new ModContentException("Move condition player must be Me, Enemy or Both.");
                if (ItemType.Length != 0 || ItemSubType.Length != 0)
                    throw new ModContentException("Named move condition does not accept item fields.");
                if (kind == ModMoveConditionKind.RoundStage && Array.IndexOf(new[]{"StartStance","Fight","EndStance","TryOn"}, Name) < 0)
                    throw new ModContentException("Unsupported round_stage name.");
                if (kind == ModMoveConditionKind.Screen && Array.IndexOf(new[]{"ShopArmor","ShopWeapon","ShopHelm","ShopMissile","ShopMagic","ShopRuby","ShopFree","ShopRaidItemPack","Profile","Fight"}, Name) < 0)
                    throw new ModContentException("Unsupported screen name.");
            }
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
        public int? Start { get; }
        public int? End { get; }
        public ModMoveAttack Attack { get; }
        public ModMoveInterval(string type = null, string name = null, int? start = null, int? end = null, ModMoveAttack attack = null)
        {
            Type = type ?? string.Empty;
            Name = name ?? string.Empty;
            if (start < 0 || start > 100000 || end < 0 || end > 100000 || (start.HasValue && end.HasValue && end < start))
                throw new ModContentException("Interval frame bounds must be ordered in 0..100000.");
            if (attack != null && Type != "Attack") throw new ModContentException("Attack data requires an Attack interval.");
            Start=start; End=end; Attack=attack;
            if (Type.Length == 0 && Name.Length == 0)
                throw new ModContentException("Move interval requires a type or name.");
        }
    }

    public sealed class ModMoveDamageTerm
    {
        public string Type { get; }
        public double Shift { get; }
        public ModMoveDamageTerm(string type, double shift = 0)
        {
            if (Array.IndexOf(new[]{"UnarmedDamage","WeaponDamage","RangedDamage","MagicDamage"}, type) < 0)
                throw new ModContentException("Unsupported damage term type.");
            if (double.IsNaN(shift) || double.IsInfinity(shift) || Math.Abs(shift) > 1000)
                throw new ModContentException("Damage term shift must be finite in -1000..1000.");
            Type = type; Shift = shift;
        }
    }

    public sealed class ModMoveAttackOptions
    {
        public bool NoEffect { get; }
        public bool NoCritical { get; }
        public bool IgnoresBlock { get; }
        public string BodyPart { get; }
        public IReadOnlyList<string> DefenseTypes { get; }
        public IReadOnlyList<string> IgnoresInvulnerable { get; }
        public bool HasContent => NoEffect || NoCritical || IgnoresBlock || BodyPart.Length != 0 || DefenseTypes.Count != 0 || IgnoresInvulnerable.Count != 0;
        public ModMoveAttackOptions(bool noEffect = false, bool noCritical = false, bool ignoresBlock = false,
            string bodyPart = null, string[] defenseTypes = null, string[] ignoresInvulnerable = null)
        {
            if (bodyPart != null && bodyPart != "Body" && bodyPart != "Head") throw new ModContentException("Attack body_part must be Body or Head.");
            defenseTypes = defenseTypes ?? Array.Empty<string>(); ignoresInvulnerable = ignoresInvulnerable ?? Array.Empty<string>();
            if (defenseTypes.Length > 2 || ignoresInvulnerable.Length > 32) throw new ModContentException("Attack supports at most 2 defense types and 32 invulnerability names.");
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var name in defenseTypes)
                if ((name != "BodyDefense" && name != "HeadDefense") || !seen.Add(name)) throw new ModContentException("Invalid or duplicate defense type.");
            seen.Clear();
            foreach (var name in ignoresInvulnerable)
            {
                ModMoveScheduledAction.ValidateSymbol(name, "invulnerability interval");
                if (!seen.Add(name)) throw new ModContentException("Duplicate invulnerability interval.");
            }
            NoEffect = noEffect; NoCritical = noCritical; IgnoresBlock = ignoresBlock; BodyPart = bodyPart ?? string.Empty;
            DefenseTypes = Array.AsReadOnly((string[])defenseTypes.Clone()); IgnoresInvulnerable = Array.AsReadOnly((string[])ignoresInvulnerable.Clone());
        }
    }

    public sealed class ModMoveAttack
    {
        public IReadOnlyList<string> Edges { get; }
        public int Id { get; }
        public double Damage { get; }
        public ModMoveAttackOptions Options { get; }
        public string DamageType { get; }
        public IReadOnlyList<ModMoveDamageTerm> DamageTerms { get; }
        public string Hit { get; }
        public double X { get; }
        public double Y { get; }
        public double Z { get; }
        public ModMoveAttack(string[] edges, double damage, string damageType=null, string hit="High", int id=0, double x=0,double y=0,double z=0,
            ModMoveDamageTerm[] damageTerms = null, ModMoveAttackOptions options = null)
        {
            if (edges==null || edges.Length<1 || edges.Length>64 || id<0 || id>999 || double.IsNaN(damage) || double.IsInfinity(damage) || damage<0 || damage>16)
                throw new ModContentException("Attack needs 1..64 edges, damage 0..16 and id 0..999.");
            foreach(var edge in edges) if(string.IsNullOrWhiteSpace(edge) || edge.Length>128) throw new ModContentException("Invalid attack edge name.");
            if (damageTerms != null && damageType != null)
                throw new ModContentException("Use damage_type or damage_terms, not both.");
            var terms = damageTerms == null ? new[]{new ModMoveDamageTerm(damageType ?? "UnarmedDamage")} : (ModMoveDamageTerm[])damageTerms.Clone();
            if (terms.Length < 1 || terms.Length > 4) throw new ModContentException("damage_terms requires 1..4 terms.");
            var types = new HashSet<string>(StringComparer.Ordinal);
            foreach (var term in terms)
                if (term == null || !types.Add(term.Type)) throw new ModContentException("Duplicate/null damage term.");
            DamageTerms = Array.AsReadOnly(terms);
            if (Array.IndexOf(new[]{"High","Middle","Low","Spinning","HighHeavy","MiddleShortPlus","Physycal","HighLong"},hit)<0)
                throw new ModContentException("Unsupported hit reaction.");
            foreach(var value in new[]{x,y,z}) if(double.IsNaN(value) || double.IsInfinity(value) || Math.Abs(value)>100000) throw new ModContentException("Invalid attack impulse.");
            Options = options ?? new ModMoveAttackOptions();
            Edges=Array.AsReadOnly((string[])edges.Clone()); Damage=damage; DamageType=terms[0].Type; Hit=hit; Id=id; X=x; Y=y; Z=z;
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

    public sealed class ModMovePoint
    {
        public string Object { get; }
        public string Player { get; }
        public string Part { get; }
        public double ShiftX { get; }
        public double ShiftY { get; }
        public ModMovePoint(string obj,string player=null,string part=null,double shiftX=0,double shiftY=0)
        {
            if (Array.IndexOf(new[]{"Nodes","Pivot","Wall","Animation","Floor","COM"},obj)<0)
                throw new ModContentException("Unsupported move point object.");
            if (player!=null && Array.IndexOf(new[]{"Me","Enemy","Parent","Child","EnemyChild"},player)<0)
                throw new ModContentException("Unsupported move point player.");
            if ((obj=="Nodes" && string.IsNullOrWhiteSpace(part)) || (part!=null && (part.Length>128 || part!=part.Trim())))
                throw new ModContentException("Nodes point requires an exact part name of 1..128 characters.");
            foreach(var value in new[]{shiftX,shiftY})
                if(double.IsNaN(value)||double.IsInfinity(value)||Math.Abs(value)>100000)
                    throw new ModContentException("Move point shifts must be finite in -100000..100000.");
            Object=obj;Player=player??string.Empty;Part=part??string.Empty;ShiftX=shiftX;ShiftY=shiftY;
        }
    }

    public sealed class ModMoveDistance
    {
        public string Axis { get; }
        public double Minimum { get; }
        public double Maximum { get; }
        public ModMovePoint From { get; }
        public ModMovePoint To { get; }
        public ModMoveDistance(string axis, ModMovePoint from, ModMovePoint to, double minimum = -1000000, double maximum = 1000000)
        {
            if (axis != "X" && axis != "Y" && axis != "Full") throw new ModContentException("Distance axis must be X, Y or Full.");
            if (from == null || to == null || from.Object == "Animation" || to.Object == "Animation") throw new ModContentException("Distance requires from/to distance points, not Animation.");
            foreach (double value in new[] { minimum, maximum })
                if (double.IsNaN(value) || double.IsInfinity(value) || Math.Abs(value) > 1000000) throw new ModContentException("Distance bounds must be finite in -1000000..1000000.");
            if (minimum > maximum) throw new ModContentException("Distance minimum exceeds maximum.");
            Axis = axis; From = from; To = to; Minimum = minimum; Maximum = maximum;
        }
    }

    public sealed class ModMoveAlignment
    {
        public IReadOnlyList<string> Axes { get; }
        public ModMovePoint Pivot { get; }
        public ModMovePoint Position { get; }
        public ModMoveAlignment(string[] axes,ModMovePoint pivot,ModMovePoint position)
        {
            if(axes==null||axes.Length<1||axes.Length>3) throw new ModContentException("Align axes requires 1..3 unique axes.");
            var seen=new HashSet<string>(StringComparer.Ordinal);
            foreach(var axis in axes)
                if(Array.IndexOf(new[]{"X","Y","Z"},axis)<0||!seen.Add(axis)) throw new ModContentException("Invalid or duplicate align axis.");
            if(pivot==null||position==null) throw new ModContentException("Align requires pivot and position points.");
            foreach(var point in new[]{pivot,position})
                if(point.Object=="Floor"||point.Object=="COM") throw new ModContentException("Align supports Nodes, Pivot, Animation or Wall points.");
            if(pivot.ShiftX!=0||pivot.ShiftY!=0) throw new ModContentException("Align pivot shifts are unsupported; shift the position instead.");
            Axes=Array.AsReadOnly((string[])axes.Clone());Pivot=pivot;Position=position;
        }
    }

    public sealed class ModMoveDirection
    {
        public ModMovePoint From { get; }
        public ModMovePoint To { get; }
        public ModMoveDirection(ModMovePoint from,ModMovePoint to)
        {
            if(from==null||to==null) throw new ModContentException("Direction requires from and to points.");
            foreach(var point in new[]{from,to})
                if(point.Object=="Animation"||point.Player.Length==0) throw new ModContentException("Direction points require a player and a distance-point object.");
            From=from;To=to;
        }
    }

    public sealed class ModMoveTransition
    {
        public int? FrameShift { get; }
        public int? FirstFrame { get; }
        public IReadOnlyList<ModMoveCondition> Conditions { get; }
        public ModMoveTransition(ModMoveCondition[] conditions,int? frameShift=null,int? firstFrame=null)
        {
            if(frameShift.HasValue==firstFrame.HasValue) throw new ModContentException("Transition requires exactly one of frame_shift or first_frame.");
            if(frameShift < -100000||frameShift > 100000||firstFrame < 0||firstFrame > 100000)
                throw new ModContentException("Transition frame value is out of bounds.");
            if(conditions==null||conditions.Length<1||conditions.Length>64) throw new ModContentException("Transition requires 1..64 conditions.");
            foreach(var condition in conditions) if(condition==null) throw new ModContentException("Null transition condition.");
            FrameShift=frameShift;FirstFrame=firstFrame;Conditions=Array.AsReadOnly((ModMoveCondition[])conditions.Clone());
        }
    }

    public sealed class ModMoveEffect
    {
        public string Name { get; }
        public string CoreSequence { get; }
        public double Scale { get; }
        public double TimeScale { get; }
        public bool Looped { get; }
        public ModMovePoint Position { get; }
        public bool Follow { get; }
        public ModMoveEffect(string name, string coreSequence, double scale = 1, double timeScale = 1,
            bool looped = false, ModMovePoint position = null, bool follow = false)
        {
            ModMoveScheduledAction.ValidateSymbol(name, "effect");
            ModMoveScheduledAction.ValidateSymbol(coreSequence, "core effect sequence");
            foreach (var value in new[] { scale, timeScale })
                if (double.IsNaN(value) || double.IsInfinity(value) || value <= 0 || value > 100)
                    throw new ModContentException("Effect scale/time_scale must be finite in (0,100].");
            if (position != null && position.Object == "Animation")
                throw new ModContentException("Effect position uses a distance point, not Animation.");
            if (follow && position == null) throw new ModContentException("Following an effect requires a position.");
            Name = name; CoreSequence = coreSequence; Scale = scale; TimeScale = timeScale;
            Looped = looped; Position = position; Follow = follow;
        }
    }

    public sealed class ModMoveProjectile
    {
        public string Name { get; }
        public string CoreSkeleton { get; }
        public string CopyParentType { get; }
        public string CoreStartAnimation { get; }
        public DefinitionId? StartMove { get; }
        public ModMoveProjectile(string name, string coreSkeleton, string copyParentType,
            string coreStartAnimation = null, DefinitionId? startMove = null)
        {
            ModMoveScheduledAction.ValidateSymbol(name, "projectile actor");
            ModMoveScheduledAction.ValidateSymbol(coreSkeleton, "core skeleton");
            if (Array.IndexOf(new[] { "Weapon", "Ranged", "Magic" }, copyParentType) < 0)
                throw new ModContentException("Projectile copy_parent_type requires Weapon, Ranged or Magic.");
            if (coreStartAnimation != null) ModMoveScheduledAction.ValidateSymbol(coreStartAnimation, "core start animation");
            if (startMove.HasValue && (startMove.Value.Category != "moves" || coreStartAnimation != null))
                throw new ModContentException("Projectile accepts either start_move or core_start_animation.");
            Name = name; CoreSkeleton = coreSkeleton; CopyParentType = copyParentType;
            CoreStartAnimation = coreStartAnimation ?? string.Empty; StartMove = startMove;
        }
    }

    public sealed class ModMoveBulletChange
    {
        public string Type { get; }
        public int Value { get; }
        public ModMoveBulletChange(string type, int value)
        {
            if (type != "MagicBullet" && type != "RaidChargeBullet") throw new ModContentException("Unsupported bullet type.");
            if (value == 0 || value < -100000 || value > 100000) throw new ModContentException("Bullet value must be a nonzero integer in -100000..100000.");
            Type = type; Value = value;
        }
    }

    public sealed class ModMoveScheduledAction
    {
        public string Kind { get; }
        public int? Frame { get; }
        public string Event { get; }
        public IReadOnlyList<string> CoreSounds { get; }
        public ModMoveEffect Effect { get; }
        public string EffectName { get; }
        public ModMoveProjectile Projectile { get; }
        public ModMoveBulletChange Bullets { get; }
        public string DeletePlayer { get; }
        public ModMoveScheduledAction(string kind, int? frame, string eventName, string[] coreSounds = null,
            ModMoveEffect effect = null, string effectName = null, ModMoveProjectile projectile = null,
            ModMoveBulletChange bullets = null, string deletePlayer = null)
        {
            if (Array.IndexOf(new[] { "random_sound", "try_on_end", "effect", "stop_effect", "stop_follow_effect", "create_projectile", "add_bullets", "delete_actor" }, kind) < 0) throw new ModContentException("Unknown scheduled move action.");
            if (frame.HasValue == (eventName != null)) throw new ModContentException("Move action requires exactly one of frame or event.");
            if (frame < 0 || frame > 100000) throw new ModContentException("Action frame must be in 0..100000.");
            if (eventName != null && Array.IndexOf(new[] { "RoundStage", "KeyPressed", "KeyReleased", "RoundStart", "RoundEnd", "Hit", "Strike", "WallHit", "AnimationStart", "AnimationEnd", "IntervalStart", "IntervalEnd", "EveryFrame", "Birth", "ModExpires" }, eventName) < 0)
                throw new ModContentException("Unknown action event.");
            coreSounds = coreSounds ?? Array.Empty<string>();
            if (kind == "random_sound" && (coreSounds.Length < 1 || coreSounds.Length > 32)) throw new ModContentException("Random sound requires 1..32 core sound names.");
            if (kind != "random_sound" && coreSounds.Length != 0) throw new ModContentException("Only random_sound accepts sounds.");
            if ((kind == "effect") != (effect != null)) throw new ModContentException("Only effect actions require an effect table.");
            bool stopsEffect = kind == "stop_effect" || kind == "stop_follow_effect";
            if (stopsEffect != (effectName != null)) throw new ModContentException("Effect stop actions require effect_name exclusively.");
            if (effectName != null) ValidateSymbol(effectName, "effect");
            if ((kind == "create_projectile") != (projectile != null)) throw new ModContentException("Only create_projectile requires a projectile table.");
            if ((kind == "add_bullets") != (bullets != null)) throw new ModContentException("Only add_bullets requires a bullets table.");
            if ((kind == "delete_actor") != (deletePlayer != null)) throw new ModContentException("Only delete_actor requires player.");
            if (deletePlayer != null && Array.IndexOf(new[] { "Me", "Enemy", "Parent", "Child", "EnemyChild" }, deletePlayer) < 0)
                throw new ModContentException("Unsupported delete actor player.");
            Projectile = projectile; Bullets = bullets; DeletePlayer = deletePlayer ?? string.Empty;
            Effect = effect; EffectName = effectName ?? string.Empty;
            foreach (var name in coreSounds) ValidateSymbol(name, "core sound");
            Kind = kind; Frame = frame; Event = eventName ?? string.Empty;
            CoreSounds = Array.AsReadOnly((string[])coreSounds.Clone());
        }
        internal static void ValidateSymbol(string value, string label)
        {
            if (string.IsNullOrEmpty(value) || value.Length > 128 || value != value.Trim()) throw new ModContentException("Invalid " + label + " name.");
            foreach (char c in value)
                if (!(char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.')) throw new ModContentException("Invalid " + label + " symbol.");
        }
    }

    public sealed class ModMoveProfile
    {
        public int Rank { get; }
        public string CoreIcon { get; }
        public DefinitionId? DisplayName { get; }
        public ModMoveProfile(int rank, string coreIcon, DefinitionId? displayName = null)
        {
            if (rank < 0 || rank > 100000) throw new ModContentException("Profile rank must be in 0..100000.");
            ModMoveScheduledAction.ValidateSymbol(coreIcon, "profile icon");
            if (displayName.HasValue && displayName.Value.Category != "localization") throw new ModContentException("Move profile display_name requires a localization handle.");
            Rank = rank; CoreIcon = coreIcon; DisplayName = displayName;
        }
    }

    public sealed class ModMoveTacticDistance
    {
        public string Axis { get; }
        public double Minimum { get; }
        public double Maximum { get; }
        public ModMoveDirection Points { get; }
        public ModMoveTacticDistance(string axis, double minimum, double maximum, ModMovePoint from, ModMovePoint to)
        {
            if (axis != "X" && axis != "Y" && axis != "Full") throw new ModContentException("Tactic distance axis must be X, Y or Full.");
            foreach (double value in new[] { minimum, maximum })
                if (double.IsNaN(value) || double.IsInfinity(value) || Math.Abs(value) > 1000000) throw new ModContentException("Tactic distance must be finite in -1000000..1000000.");
            if (minimum > maximum) throw new ModContentException("Tactic minimum exceeds maximum.");
            Points = new ModMoveDirection(from, to); Axis = axis; Minimum = minimum; Maximum = maximum;
        }
    }

    public sealed class ModMoveVelocity
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }
        public double Ax { get; }
        public double Ay { get; }
        public double Az { get; }
        public bool SaveVelocity { get; }
        public ModMoveVelocity(double x = 0, double y = 0, double z = 0, double ax = 0, double ay = 0, double az = 0, bool saveVelocity = false)
        {
            foreach (var value in new[] { x, y, z, ax, ay, az })
                if (double.IsNaN(value) || double.IsInfinity(value) || Math.Abs(value) > 100000)
                    throw new ModContentException("Move velocity/acceleration must be finite in -100000..100000.");
            X = x; Y = y; Z = z; Ax = ax; Ay = ay; Az = az; SaveVelocity = saveVelocity;
        }
    }

    public sealed class ModMovePresentation
    {
        public IReadOnlyList<ModMoveScheduledAction> Actions { get; }
        public ModMoveProfile Profile { get; }
        public ModMoveTacticDistance TacticDistance { get; }
        public bool NoWallRepulsion { get; }
        public bool NoInterpolationFrames { get; }
        public bool NoMagicRecharge { get; }
        public ModMoveVelocity Velocity { get; }
        public bool HasContent => Actions.Count != 0 || Profile != null || TacticDistance != null || NoWallRepulsion || NoInterpolationFrames || NoMagicRecharge || Velocity != null;
        public ModMovePresentation(ModMoveScheduledAction[] actions = null, ModMoveProfile profile = null,
            ModMoveTacticDistance tacticDistance = null, bool noWallRepulsion = false, bool noInterpolationFrames = false, bool noMagicRecharge = false, ModMoveVelocity velocity = null)
        {
            actions = actions ?? Array.Empty<ModMoveScheduledAction>();
            if (actions.Length > 64) throw new ModContentException("At most 64 scheduled move actions are supported.");
            foreach (var action in actions) if (action == null) throw new ModContentException("Null scheduled action.");
            Actions = Array.AsReadOnly((ModMoveScheduledAction[])actions.Clone()); Profile = profile; TacticDistance = tacticDistance;
            NoWallRepulsion = noWallRepulsion; NoInterpolationFrames = noInterpolationFrames;
            NoMagicRecharge = noMagicRecharge; Velocity = velocity;
        }
    }

    public sealed class ModMoveGraph
    {
        public IReadOnlyList<ModMoveCondition> Locks { get; }
        public IReadOnlyList<ModMoveTransition> Transitions { get; }
        public ModMoveAlignment Align { get; }
        public ModMoveDirection Direction { get; }
        public ModMovePresentation Presentation { get; }
        public bool HasContent => Locks.Count!=0||Transitions.Count!=0||Align!=null||Direction!=null;
        public ModMoveGraph(ModMoveCondition[] locks=null,ModMoveTransition[] transitions=null,ModMoveAlignment align=null,ModMoveDirection direction=null,ModMovePresentation presentation=null)
        {
            locks=locks??Array.Empty<ModMoveCondition>();transitions=transitions??Array.Empty<ModMoveTransition>();
            if(locks.Length>64||transitions.Length>32) throw new ModContentException("Move graph supports at most 64 locks and 32 transitions.");
            foreach(var value in locks) if(value==null) throw new ModContentException("Null move lock.");
            foreach(var value in transitions) if(value==null) throw new ModContentException("Null move transition.");
            Locks=Array.AsReadOnly((ModMoveCondition[])locks.Clone());Transitions=Array.AsReadOnly((ModMoveTransition[])transitions.Clone());Align=align;Direction=direction;
            Presentation=presentation??new ModMovePresentation();
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
        public ModMoveGraph Graph { get; }

        protected MoveNodeDefinition(DefinitionId id, DefinitionId[] templates, string[] coreTemplates,
            ModMoveEvent[] events, ModMoveCondition[] conditions, ModMoveInterval[] intervals, string type,
            int priority, int midFrames, int firstFrame, int endFrame, string mirrorNode, string tacticEquivalent,
            string tacticWeapon, bool looped, bool endsStage, ModMoveGraph graph = null)
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
            Graph = graph ?? new ModMoveGraph();
        }
    }

    public sealed class MoveTemplateDefinition : MoveNodeDefinition
    {
        internal MoveTemplateDefinition(DefinitionId id, DefinitionId[] templates, string[] coreTemplates,
            ModMoveEvent[] events, ModMoveCondition[] conditions, ModMoveInterval[] intervals, string type,
            int priority, int midFrames, int firstFrame, int endFrame, string mirrorNode, string tacticEquivalent,
            string tacticWeapon, bool looped, bool endsStage, ModMoveGraph graph = null)
            : base(id, templates, coreTemplates, events, conditions, intervals, type, priority, midFrames, firstFrame,
                endFrame, mirrorNode, tacticEquivalent, tacticWeapon, looped, endsStage, graph) { }
    }

    public sealed class MoveDefinition : MoveNodeDefinition
    {
        public AssetId Animation { get; }
        internal MoveDefinition(DefinitionId id, AssetId animation, DefinitionId[] templates, string[] coreTemplates,
            ModMoveEvent[] events, ModMoveCondition[] conditions, ModMoveInterval[] intervals, string type,
            int priority, int midFrames, int firstFrame, int endFrame, string mirrorNode, string tacticEquivalent,
            string tacticWeapon, bool looped, bool endsStage, ModMoveGraph graph = null)
            : base(id, templates, coreTemplates, events, conditions, intervals, type, priority, midFrames, firstFrame,
                endFrame, mirrorNode, tacticEquivalent, tacticWeapon, looped, endsStage, graph)
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
            _p1dMoves.Count + _p1dMoveTriggers.Count + _p1dTactics.Count + MovePerkLockRegistrationCount + _moveItemLockExtensions.Count + _moveCombatPatches.Count;

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
            AssetId music, LocationLayerDefinition[] layers, AssetId[] musicChoices = null, bool dojo = false)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("locations", localId);
            ValidateAssetReference(music, "location music");
            if (musicChoices != null)
                foreach (var choice in musicChoices) ValidateAssetReference(choice, "location music choice");
            if (layers != null)
                for (int i = 0; i < layers.Length; i++)
                    for (int j = 0; j < layers[i].Images.Count; j++)
                        ValidateAssetReference(layers[i].Images[j].Sprite, "location sprite");
            var value = new LocationDefinition(id, color, wall, floor, positionY, width, height, minWidth,
                frictionForce, gridSize, music, layers, musicChoices, dojo);
            AddP1D(_p1dLocations, id, value);
            return value;
        }

        public MoveTemplateDefinition RegisterMoveTemplate(string localId, DefinitionId[] templates,
            string[] coreTemplates, ModMoveEvent[] events, ModMoveCondition[] conditions, ModMoveInterval[] intervals,
            string type, int priority, int midFrames, int firstFrame, int endFrame, string mirrorNode,
            string tacticEquivalent, string tacticWeapon, bool looped, bool endsStage, ModMoveGraph graph = null)
        {
            ThrowIfCompleted();
            if (graph != null && graph.Transitions.Count != 0) throw new ModContentException("Transitions must be declared directly on a move, not a template.");
            if (graph != null && graph.Presentation.HasContent) throw new ModContentException("Presentation must be declared directly on a move, not a template.");
            DefinitionId id = Qualify("move-templates", localId);
            var value = new MoveTemplateDefinition(id, templates, coreTemplates, events, conditions, intervals, type,
                priority, midFrames, firstFrame, endFrame, mirrorNode, tacticEquivalent, tacticWeapon, looped, endsStage, graph);
            AddP1D(_p1dMoveTemplates, id, value);
            return value;
        }

        public MoveDefinition RegisterMove(string localId, AssetId animation, DefinitionId[] templates,
            string[] coreTemplates, ModMoveEvent[] events, ModMoveCondition[] conditions, ModMoveInterval[] intervals,
            string type, int priority, int midFrames, int firstFrame, int endFrame, string mirrorNode,
            string tacticEquivalent, string tacticWeapon, bool looped, bool endsStage, ModMoveGraph graph = null)
        {
            ThrowIfCompleted();
            ValidateAssetReference(animation, "move animation");
            DefinitionId id = Qualify("moves", localId);
            var value = new MoveDefinition(id, animation, templates, coreTemplates, events, conditions, intervals, type,
                priority, midFrames, firstFrame, endFrame, mirrorNode, tacticEquivalent, tacticWeapon, looped, endsStage, graph);
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
            int dojoCount = 0;
            foreach (var location in _catalog.Locations) if (location.IsDojo) dojoCount++;
            foreach (var location in locations) if (location.IsDojo) dojoCount++;
            if (dojoCount > 256) throw new ModContentException("At most 256 dojo choices may be active.");
            MoveTemplateDefinition[] templates = Values(_p1dMoveTemplates);
            MoveDefinition[] moves = Values(_p1dMoves);
            MoveTriggerDefinition[] triggers = Values(_p1dMoveTriggers);
            TacticDefinition[] tactics = Values(_p1dTactics);
            _catalog.ValidateP1DCanAdd(locales, locations, templates, moves, triggers, tactics);
            ValidateMovePerkLockCommit();
            _catalog.ValidateItemLockExtensions(_moveItemLockExtensions);
            _catalog.ValidateCombatPatches(_moveCombatPatches);
            foreach (var patch in _moveCombatPatches) ValidateMovePerkRefs(patch.Conditions);
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
            ApplyMovePerkLockCommit();
            _catalog.AddItemLockExtensions(_moveItemLockExtensions);
            _catalog.AddCombatPatches(_moveCombatPatches);
        }

        private void ClearP1DPending()
        {
            _p1dLocales.Clear(); _p1dLocations.Clear(); _p1dMoveTemplates.Clear(); _p1dMoves.Clear();
            _p1dMoveTriggers.Clear(); _p1dTactics.Clear();
            ClearMovePerkLockPending();
            _moveItemLockExtensions.Clear();
            _moveCombatPatches.Clear();
        }

        private void ValidateTemplateRefs(MoveNodeDefinition node)
        {
            if (node.Graph.Presentation.Profile?.DisplayName != null)
                GetLocalization(node.Graph.Presentation.Profile.DisplayName.Value.ToString());
            foreach (var action in node.Graph.Presentation.Actions)
            {
                if (action.Projectile == null || !action.Projectile.StartMove.HasValue) continue;
                var id = action.Projectile.StartMove.Value;
                if (!CanReferenceNamespace(id.Namespace) ||
                    (!_p1dMoves.ContainsKey(id) && !_catalog.TryGetMove(id, out MoveDefinition ignored)))
                    throw new ModContentException("Projectile references missing or inaccessible start_move: " + id);
            }
            ValidateMovePerkRefs(node.Conditions);
            ValidateMovePerkRefs(node.Graph.Locks);
            foreach(var transition in node.Graph.Transitions) ValidateMovePerkRefs(transition.Conditions);
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
