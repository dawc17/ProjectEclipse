using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
    public enum ModTrialAxis
    {
        X = 0,
        Y = 1,
    }

    public enum ModTrialIntervalType
    {
        Attack = 0,
        Block = 1,
        Invulnerable = 2,
        SelfUninterrupt = 3,
        Uninterrupt = 4,
        Unstable = 5,
    }

    public sealed class ModTrialNodeLimit
    {
        public string Name { get; }
        public ModTrialAxis Axis { get; }
        public float? Minimum { get; }
        public float? Maximum { get; }

        public ModTrialNodeLimit(string name, ModTrialAxis axis, float? minimum = null, float? maximum = null)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 128)
                throw new ModContentException("Trial rule node name must contain 1..128 characters.");
            if (!Enum.IsDefined(typeof(ModTrialAxis), axis)) throw new ModContentException("Unsupported trial rule axis.");
            if (!minimum.HasValue && !maximum.HasValue)
                throw new ModContentException("Trial rule node requires min or max.");
            if (minimum.HasValue && (float.IsNaN(minimum.Value) || float.IsInfinity(minimum.Value)) ||
                maximum.HasValue && (float.IsNaN(maximum.Value) || float.IsInfinity(maximum.Value)) ||
                minimum.HasValue && maximum.HasValue && minimum.Value >= maximum.Value)
                throw new ModContentException("Trial rule node bounds must be finite and ordered min < max.");
            Name = name;
            Axis = axis;
            Minimum = minimum;
            Maximum = maximum;
        }
    }

    public sealed class ModTrialRulePayload
    {
        private readonly ModTrialNodeLimit[] _nodes;
        private readonly string[] _animations;

        public ModFightRuleKind Kind { get; }
        public int Frames { get; }
        public IReadOnlyList<ModTrialNodeLimit> Nodes => Array.AsReadOnly(_nodes);
        public IReadOnlyList<string> Animations => Array.AsReadOnly(_animations);
        public string Node { get; }
        public ModTrialAxis Axis { get; }
        public float Minimum { get; }
        public float Maximum { get; }
        public float Rate { get; }
        public int FramesAfterHit { get; }
        public ModTrialIntervalType IntervalType { get; }

        private ModTrialRulePayload(ModFightRuleKind kind, int frames, ModTrialNodeLimit[] nodes,
            string[] animations, string node, ModTrialAxis axis, float minimum, float maximum,
            float rate, int framesAfterHit, ModTrialIntervalType intervalType)
        {
            Kind = kind;
            Frames = frames;
            _nodes = nodes == null ? Array.Empty<ModTrialNodeLimit>() : (ModTrialNodeLimit[])nodes.Clone();
            _animations = animations == null ? Array.Empty<string>() : (string[])animations.Clone();
            Node = node ?? string.Empty;
            Axis = axis;
            Minimum = minimum;
            Maximum = maximum;
            Rate = rate;
            FramesAfterHit = framesAfterHit;
            IntervalType = intervalType;
        }

        internal static ModTrialRulePayload HotGround(int frames, ModTrialNodeLimit[] nodes, string[] animations)
        {
            if (frames < 1 || frames > 216000) throw new ModContentException("Hot-ground frames must be 1..216000.");
            if (nodes == null || nodes.Length < 1 || nodes.Length > 32)
                throw new ModContentException("Hot-ground requires 1..32 node limits.");
            var seenNodes = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] == null) throw new ModContentException("Hot-ground node limits must not be null.");
                string key = nodes[i].Name + "\n" + nodes[i].Axis;
                if (!seenNodes.Add(key)) throw new ModContentException("Hot-ground contains a duplicate node/axis limit.");
            }
            if (animations != null && animations.Length > 64)
                throw new ModContentException("Hot-ground supports at most 64 animation exclusions.");
            var seenAnimations = new HashSet<string>(StringComparer.Ordinal);
            if (animations != null) foreach (string animation in animations)
            {
                if (string.IsNullOrWhiteSpace(animation) || animation.Length > 128 || !seenAnimations.Add(animation))
                    throw new ModContentException("Hot-ground animations must be distinct names of 1..128 characters.");
            }
            return new ModTrialRulePayload(ModFightRuleKind.HotGround, frames, nodes, animations, null,
                ModTrialAxis.X, 0f, 0f, 0f, 0, default);
        }

        internal static ModTrialRulePayload RingOut(string node, ModTrialAxis axis, float minimum, float maximum)
        {
            if (string.IsNullOrWhiteSpace(node) || node.Length > 128)
                throw new ModContentException("Ring-out node must contain 1..128 characters.");
            if (!Enum.IsDefined(typeof(ModTrialAxis), axis) || float.IsNaN(minimum) || float.IsInfinity(minimum) ||
                float.IsNaN(maximum) || float.IsInfinity(maximum) || minimum >= maximum)
                throw new ModContentException("Ring-out requires a valid axis and finite ordered min < max bounds.");
            return new ModTrialRulePayload(ModFightRuleKind.RingOut, 0, null, null, node, axis,
                minimum, maximum, 0f, 0, default);
        }

        internal static ModTrialRulePayload Regeneration(float rate, int framesAfterHit)
        {
            if (float.IsNaN(rate) || float.IsInfinity(rate) || rate < -1f || rate > 1f)
                throw new ModContentException("Regeneration rate must be finite and -1..1.");
            if (framesAfterHit < 0 || framesAfterHit > 216000)
                throw new ModContentException("Regeneration frames_after_hit must be 0..216000.");
            return new ModTrialRulePayload(ModFightRuleKind.Regeneration, 0, null, null, null,
                ModTrialAxis.X, 0f, 0f, rate, framesAfterHit, default);
        }

        internal static ModTrialRulePayload NoAnimation(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 128)
                throw new ModContentException("No-animation name must contain 1..128 characters.");
            return new ModTrialRulePayload(ModFightRuleKind.NoAnimation, 0, null, null, name,
                ModTrialAxis.X, 0f, 0f, 0f, 0, default);
        }

        internal static ModTrialRulePayload RemoveInterval(ModTrialIntervalType type)
        {
            if (!Enum.IsDefined(typeof(ModTrialIntervalType), type))
                throw new ModContentException("Unsupported remove-interval type.");
            return new ModTrialRulePayload(ModFightRuleKind.RemoveInterval, 0, null, null, null,
                ModTrialAxis.X, 0f, 0f, 0f, 0, type);
        }
    }

    public sealed partial class ModRegistrationTransaction
    {
        public FightRuleDefinition RegisterHotGroundRule(string localId, int frames, ModTrialNodeLimit[] nodes,
            string[] animations, ModRuleTarget target, ModRuleMode mode, int[] rounds) =>
            target == ModRuleTarget.All
                ? throw new ModContentException("Hot-ground target must be player or opponent; recovered ApplyTo=All copy loses the HotGroundRule runtime type.")
                : RegisterExtendedRule(localId, ModFightRuleKind.HotGround, target, mode, rounds, string.Empty,
                    default, false, 0, default, false, null, trial: ModTrialRulePayload.HotGround(frames, nodes, animations));

        public FightRuleDefinition RegisterRingOutRule(string localId, string node, ModTrialAxis axis,
            float minimum, float maximum, ModRuleTarget target, ModRuleMode mode, int[] rounds) =>
            RegisterExtendedRule(localId, ModFightRuleKind.RingOut, target, mode, rounds, string.Empty,
                default, false, 0, default, false, null, trial: ModTrialRulePayload.RingOut(node, axis, minimum, maximum));

        public FightRuleDefinition RegisterRegenerationRule(string localId, float rate, int framesAfterHit,
            ModRuleTarget target, ModRuleMode mode, int[] rounds) =>
            RegisterExtendedRule(localId, ModFightRuleKind.Regeneration, target, mode, rounds, string.Empty,
                default, false, 0, default, false, null, trial: ModTrialRulePayload.Regeneration(rate, framesAfterHit));

        public FightRuleDefinition RegisterNoAnimationRule(string localId, string name, ModRuleMode mode, int[] rounds) =>
            RegisterExtendedRule(localId, ModFightRuleKind.NoAnimation, ModRuleTarget.All, mode, rounds, string.Empty,
                default, false, 0, default, false, null, trial: ModTrialRulePayload.NoAnimation(name));

        public FightRuleDefinition RegisterRemoveIntervalRule(string localId, ModTrialIntervalType type,
            ModRuleTarget target, ModRuleMode mode, int[] rounds) =>
            RegisterExtendedRule(localId, ModFightRuleKind.RemoveInterval, target, mode, rounds, string.Empty,
                default, false, 0, default, false, null, trial: ModTrialRulePayload.RemoveInterval(type));
    }

    public sealed partial class ModApiFacade
    {
        public FightRuleDefinition RegisterPerkRule(string localId, DefinitionId perk, ModRuleTarget target,
            ModRuleMode mode, int[] rounds, double? aspect, IReadOnlyDictionary<string, double> parameters = null)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterPerkRule(localId, perk, target, mode, rounds, aspect, parameters);
        }

        public FightRuleDefinition RegisterHotGroundRule(string localId, int frames, ModTrialNodeLimit[] nodes,
            string[] animations, ModRuleTarget target, ModRuleMode mode, int[] rounds)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterHotGroundRule(localId, frames, nodes, animations, target, mode, rounds);
        }

        public FightRuleDefinition RegisterRingOutRule(string localId, string node, ModTrialAxis axis,
            float minimum, float maximum, ModRuleTarget target, ModRuleMode mode, int[] rounds)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterRingOutRule(localId, node, axis, minimum, maximum, target, mode, rounds);
        }

        public FightRuleDefinition RegisterRegenerationRule(string localId, float rate, int framesAfterHit,
            ModRuleTarget target, ModRuleMode mode, int[] rounds)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterRegenerationRule(localId, rate, framesAfterHit, target, mode, rounds);
        }

        public FightRuleDefinition RegisterNoAnimationRule(string localId, string name, ModRuleMode mode, int[] rounds)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterNoAnimationRule(localId, name, mode, rounds);
        }

        public FightRuleDefinition RegisterRemoveIntervalRule(string localId, ModTrialIntervalType type,
            ModRuleTarget target, ModRuleMode mode, int[] rounds)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterRemoveIntervalRule(localId, type, target, mode, rounds);
        }
    }
}
