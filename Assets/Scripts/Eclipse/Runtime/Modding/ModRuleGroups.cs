using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
    public enum ModRuleRefresh
    {
        EachFight = 0,
        EachRound = 1,
    }

    // Payload for recovered rule classes that carry no trial data: NoHealthBar,
    // InvertJoystick, RandomArea, and the composite ComplexRule / RandomRule wrappers.
    public sealed class ModRuleGroupPayload
    {
        private readonly DefinitionId[] _children;

        public ModFightRuleKind Kind { get; }
        public IReadOnlyList<DefinitionId> Children => Array.AsReadOnly(_children);
        // Group: optional native localization key shown as the rule description.
        public string Description { get; }
        // Random: refresh cadence and the native NoDoubles flag.
        public ModRuleRefresh Refresh { get; }
        public bool NoDoubles { get; }
        // RandomArea: image names under Textures/fight/rules/randomarea/ and timing in frames.
        public string Image { get; }
        public string Icon { get; }
        public float Width { get; }
        public int FadeIn { get; }
        public int FramesOn { get; }
        public int FadeOut { get; }
        public int FramesOff { get; }
        // LightInTheDarkness: normalized radius and circle/square shape blend.
        public float LightRadius { get; }
        public float LightShape { get; }

        private ModRuleGroupPayload(ModFightRuleKind kind, DefinitionId[] children, string description,
            ModRuleRefresh refresh, bool noDoubles, string image, string icon, float width,
            int fadeIn, int framesOn, int fadeOut, int framesOff, float lightRadius = 0f, float lightShape = 0f)
        {
            Kind = kind;
            _children = children == null ? Array.Empty<DefinitionId>() : (DefinitionId[])children.Clone();
            Description = description ?? string.Empty;
            Refresh = refresh;
            NoDoubles = noDoubles;
            Image = image ?? string.Empty;
            Icon = icon ?? string.Empty;
            Width = width;
            FadeIn = fadeIn; FramesOn = framesOn; FadeOut = fadeOut; FramesOff = framesOff;
            LightRadius = lightRadius; LightShape = lightShape;
        }

        internal static bool IsGroupKind(ModFightRuleKind kind) =>
            kind == ModFightRuleKind.NoHealthBar || kind == ModFightRuleKind.InvertJoystick ||
            kind == ModFightRuleKind.RandomArea || kind == ModFightRuleKind.Group || kind == ModFightRuleKind.Random ||
            kind == ModFightRuleKind.LightInTheDarkness;

        internal static ModRuleGroupPayload LightInTheDarkness(float radius, float shape)
        {
            if (float.IsNaN(radius) || float.IsInfinity(radius) || radius <= 0f || radius > 1f ||
                float.IsNaN(shape) || float.IsInfinity(shape) || shape < 0f || shape > 1f)
                throw new ModContentException("Light-in-the-darkness radius must be in (0, 1] and shape in [0, 1].");
            return new ModRuleGroupPayload(ModFightRuleKind.LightInTheDarkness, null, null, default, false,
                null, null, 0f, 0, 0, 0, 0, radius, shape);
        }

        internal static ModRuleGroupPayload Flag(ModFightRuleKind kind)
        {
            if (kind != ModFightRuleKind.NoHealthBar && kind != ModFightRuleKind.InvertJoystick)
                throw new ModContentException("Unsupported flag rule kind.");
            return new ModRuleGroupPayload(kind, null, null, default, false, null, null, 0f, 0, 0, 0, 0);
        }

        internal static ModRuleGroupPayload RandomArea(string image, string icon, float width,
            int fadeIn, int framesOn, int fadeOut, int framesOff)
        {
            ValidateImageName(image, "image", true);
            ValidateImageName(icon, "icon", false);
            if (float.IsNaN(width) || float.IsInfinity(width) || width <= 0 || width > 100000)
                throw new ModContentException("Random-area width must be finite and in (0, 100000].");
            // The native alpha ramps divide by the fade-in and fade-out spans.
            if (fadeIn < 1 || fadeOut < 1 || framesOn < 0 || framesOff < 0 ||
                fadeIn > 36000 || fadeOut > 36000 || framesOn > 36000 || framesOff > 36000)
                throw new ModContentException("Random-area fades must be 1..36000 frames and on/off spans 0..36000.");
            return new ModRuleGroupPayload(ModFightRuleKind.RandomArea, null, null, default, false,
                image, icon, width, fadeIn, framesOn, fadeOut, framesOff);
        }

        internal static ModRuleGroupPayload Group(string description, DefinitionId[] children)
        {
            ValidateChildren(children);
            if (description != null && description.Length > 256) throw new ModContentException("Rule description key is too long.");
            return new ModRuleGroupPayload(ModFightRuleKind.Group, children, description, default, false, null, null, 0f, 0, 0, 0, 0);
        }

        internal static ModRuleGroupPayload Random(ModRuleRefresh refresh, bool noDoubles, DefinitionId[] children)
        {
            ValidateChildren(children);
            if (!Enum.IsDefined(typeof(ModRuleRefresh), refresh)) throw new ModContentException("Unsupported random-rule refresh.");
            return new ModRuleGroupPayload(ModFightRuleKind.Random, children, null, refresh, noDoubles, null, null, 0f, 0, 0, 0, 0);
        }

        private static void ValidateChildren(DefinitionId[] children)
        {
            if (children == null || children.Length < 1 || children.Length > 64)
                throw new ModContentException("Rule groups require 1..64 child rules.");
            var seen = new HashSet<DefinitionId>();
            foreach (var child in children)
                if (child.Category != "rules" || !seen.Add(child)) throw new ModContentException("Rule group children must be unique rule handles.");
        }

        private static void ValidateImageName(string value, string field, bool required)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (required) throw new ModContentException("Random-area " + field + " is required.");
                return;
            }
            if (value.Length > 128) throw new ModContentException("Random-area " + field + " name is too long.");
            foreach (char c in value)
                if (!(char.IsLetterOrDigit(c) || c == '_' || c == '-'))
                    throw new ModContentException("Random-area " + field + " must be a plain texture name.");
        }
    }

    public sealed partial class ModRegistrationTransaction
    {
        public FightRuleDefinition RegisterFlagRule(string localId, ModFightRuleKind kind, ModRuleTarget target,
            ModRuleMode mode, int[] rounds) =>
            RegisterExtendedRule(localId, kind, target, mode, rounds, string.Empty, default, false, 0, default, false,
                null, group: ModRuleGroupPayload.Flag(kind));

        public FightRuleDefinition RegisterRandomAreaRule(string localId, string image, string icon, float width,
            int fadeIn, int framesOn, int fadeOut, int framesOff, ModRuleTarget target, ModRuleMode mode, int[] rounds) =>
            RegisterExtendedRule(localId, ModFightRuleKind.RandomArea, target, mode, rounds, string.Empty, default, false, 0,
                default, false, null, group: ModRuleGroupPayload.RandomArea(image, icon, width, fadeIn, framesOn, fadeOut, framesOff));

        public FightRuleDefinition RegisterLightInTheDarknessRule(string localId, float radius, float shape,
            ModRuleTarget target, ModRuleMode mode, int[] rounds) =>
            RegisterExtendedRule(localId, ModFightRuleKind.LightInTheDarkness, target, mode, rounds, string.Empty,
                default, false, 0, default, false, null, group: ModRuleGroupPayload.LightInTheDarkness(radius, shape));

        public FightRuleDefinition RegisterGroupRule(string localId, DefinitionId? description, DefinitionId[] children,
            ModRuleMode mode, int[] rounds)
        {
            ValidateGroupChildren(children);
            string key = description.HasValue ? NativeLocalizationKey(description.Value) : string.Empty;
            return RegisterExtendedRule(localId, ModFightRuleKind.Group, ModRuleTarget.All, mode, rounds, string.Empty,
                default, false, 0, default, false, null, group: ModRuleGroupPayload.Group(key, children));
        }

        public FightRuleDefinition RegisterRandomRule(string localId, ModRuleRefresh refresh, bool noDoubles,
            DefinitionId[] children, ModRuleMode mode, int[] rounds)
        {
            ValidateGroupChildren(children);
            return RegisterExtendedRule(localId, ModFightRuleKind.Random, ModRuleTarget.All, mode, rounds, string.Empty,
                default, false, 0, default, false, null, group: ModRuleGroupPayload.Random(refresh, noDoubles, children));
        }

        // Children must already exist and project natively. Behavior rules run from
        // Lua at the fight boundary and would be lost inside a native wrapper.
        private void ValidateGroupChildren(DefinitionId[] children)
        {
            if (children == null) throw new ModContentException("Rule groups require child rules.");
            foreach (var child in children)
            {
                if (!CanReferenceNamespace(child.Namespace))
                    throw new ModContentException("Rule group child belongs to an undeclared dependency: '" + child + "'.");
                if (!_fightRules.TryGetValue(child, out var rule) && !_catalog.TryGetFightRule(child, out rule))
                    throw new ModContentException("Rule group references missing rule '" + child + "'.");
                if (rule.Kind == ModFightRuleKind.Behavior)
                    throw new ModContentException("Behavior rules cannot be placed inside a native rule group.");
            }
        }
    }

    public sealed partial class ModApiFacade
    {
        public FightRuleDefinition RegisterFlagRule(string localId, ModFightRuleKind kind, ModRuleTarget target,
            ModRuleMode mode, int[] rounds)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterFlagRule(localId, kind, target, mode, rounds);
        }

        public FightRuleDefinition RegisterRandomAreaRule(string localId, string image, string icon, float width,
            int fadeIn, int framesOn, int fadeOut, int framesOff, ModRuleTarget target, ModRuleMode mode, int[] rounds)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterRandomAreaRule(localId, image, icon, width, fadeIn, framesOn, fadeOut, framesOff,
                target, mode, rounds);
        }

        public FightRuleDefinition RegisterLightInTheDarknessRule(string localId, float radius, float shape,
            ModRuleTarget target, ModRuleMode mode, int[] rounds)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterLightInTheDarknessRule(localId, radius, shape, target, mode, rounds);
        }

        public FightRuleDefinition RegisterGroupRule(string localId, DefinitionId? description, DefinitionId[] children,
            ModRuleMode mode, int[] rounds)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterGroupRule(localId, description, children, mode, rounds);
        }

        public FightRuleDefinition RegisterRandomRule(string localId, ModRuleRefresh refresh, bool noDoubles,
            DefinitionId[] children, ModRuleMode mode, int[] rounds)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterRandomRule(localId, refresh, noDoubles, children, mode, rounds);
        }
    }
}
