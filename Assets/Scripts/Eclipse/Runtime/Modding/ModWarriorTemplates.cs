using System;
using System.Collections.Generic;
using System.Xml;

namespace Eclipse.Modding
{
    public sealed class WarriorAttributeAlignmentDefinition
    {
        public float Factor { get; }
        public float Shift { get; }
        public int Priority { get; }
        public ModRuleMode Mode { get; }

        public WarriorAttributeAlignmentDefinition(float factor, float shift, int priority = 0,
            ModRuleMode mode = ModRuleMode.All)
        {
            if (float.IsNaN(factor) || float.IsInfinity(factor) || float.IsNaN(shift) || float.IsInfinity(shift))
                throw new ModContentException("Warrior attribute alignment values must be finite.");
            if (!Enum.IsDefined(typeof(ModRuleMode), mode)) throw new ModContentException("Invalid warrior alignment mode.");
            Factor = factor;
            Shift = shift;
            Priority = priority;
            Mode = mode;
        }
    }

    public sealed class WarriorTemplateDefinition
    {
        public DefinitionId Id { get; }
        public string LegacyName { get; }
        // Mod-owned templates carry a warrior-shaped body projected as a native
        // Stages/Warriors/Templates entry; core templates are identities only.
        public WarriorDefinition Body { get; }
        public bool IsCore => Id.Namespace.Value == "core";

        internal WarriorTemplateDefinition(DefinitionId id, string legacyName, WarriorDefinition body = null)
        {
            Id = id;
            LegacyName = legacyName ?? string.Empty;
            Body = body;
        }
    }

    public sealed partial class ModContentCatalog
    {
        private readonly DefinitionRegistry<WarriorTemplateDefinition> _warriorTemplates =
            new DefinitionRegistry<WarriorTemplateDefinition>(value => value.Id);

        public IReadOnlyList<WarriorTemplateDefinition> WarriorTemplates => _warriorTemplates.Values;

        public bool TryGetWarriorTemplate(DefinitionId id, out WarriorTemplateDefinition value)
        {
            value = null;
            return id.Category == "warrior-templates" && _warriorTemplates.TryGet(id, out value);
        }

        internal void ValidateCanAddModWarriorTemplates(WarriorTemplateDefinition[] templates)
        {
            if (IsFrozen) throw new InvalidOperationException("Definition registries are frozen.");
            _warriorTemplates.ValidateCanAdd(templates);
        }

        internal void AddModWarriorTemplates(WarriorTemplateDefinition[] templates)
        {
            _warriorTemplates.AddRange(templates);
        }

        internal void ImportCoreWarriorTemplates(WarriorTemplateDefinition[] templates)
        {
            if (IsFrozen) throw new InvalidOperationException("Definition registries are frozen.");
            if (templates == null) throw new ArgumentNullException(nameof(templates));
            _warriorTemplates.ValidateCanAdd(templates);
            for (int i = 0; i < templates.Length; i++)
                if (templates[i].Id.Namespace.Value != "core" || string.IsNullOrEmpty(templates[i].LegacyName))
                    throw new ModContentException("Invalid core warrior template import: " + templates[i].Id);
            _warriorTemplates.AddRange(templates);
        }
    }

    public static partial class CoreContentImporter
    {
        public static DefinitionId WarriorTemplateId(string legacyName)
        {
            return DefinitionId.Parse("core:warrior-templates/" + StageSegment(legacyName));
        }

        public static int ImportWarriorTemplates(ModContentCatalog catalog, XmlNode templatesRoot)
        {
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            if (templatesRoot == null) return 0;
            var result = new List<WarriorTemplateDefinition>();
            var seen = new HashSet<DefinitionId>();
            foreach (XmlNode node in templatesRoot.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element) continue;
                XmlAttribute nameAttribute = node.Attributes?["Name"];
                if (nameAttribute == null || string.IsNullOrWhiteSpace(nameAttribute.Value)) continue;
                DefinitionId id = WarriorTemplateId(nameAttribute.Value);
                if (!seen.Add(id)) throw new ModContentException("Duplicate projected warrior template ID: '" + id + "'.");
                result.Add(new WarriorTemplateDefinition(id, nameAttribute.Value));
            }
            catalog.ImportCoreWarriorTemplates(result.ToArray());
            return result.Count;
        }
    }

    public sealed partial class ModRegistrationTransaction
    {
        private readonly Dictionary<DefinitionId, WarriorTemplateDefinition> _warriorTemplates =
            new Dictionary<DefinitionId, WarriorTemplateDefinition>();
        private readonly List<DefinitionId> _warriorTemplateOrder = new List<DefinitionId>();

        internal int WarriorTemplateRegistrationCount => _warriorTemplates.Count;

        internal bool TryGetAnyWarriorTemplate(DefinitionId id, out WarriorTemplateDefinition value)
        {
            if (_warriorTemplates.TryGetValue(id, out value)) return true;
            return _catalog.TryGetWarriorTemplate(id, out value);
        }

        // Same fields and validation as a warrior. The parent (core or earlier owned
        // template) must already exist, so registration order is inheritance order.
        public WarriorTemplateDefinition RegisterWarriorTemplate(string localId, string firstName, string lastName,
            string avatar, string voice, int level, string tactic, DefinitionId[] items, DefinitionId template,
            bool hasTemplate, IReadOnlyDictionary<string, float> attributes,
            WarriorAttributeAlignmentDefinition[] attributeAlignments, int healthBars, WarriorPerkDefinition[] perkLoadout,
            string skeleton = null)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("warrior-templates", localId);
            if (_warriorTemplates.ContainsKey(id) || _catalog.TryGetWarriorTemplate(id, out _))
                throw new ModContentException("Duplicate warrior template definition: '" + id + "'.");
            var body = BuildWarriorDefinition(id, firstName, lastName, avatar, voice, level, tactic, items, null,
                template, hasTemplate, null, 0, attributes, attributeAlignments, healthBars, default, null, perkLoadout, skeleton);
            var definition = new WarriorTemplateDefinition(id, id.ToString(), body);
            _warriorTemplates.Add(id, definition);
            _warriorTemplateOrder.Add(id);
            return definition;
        }

        internal WarriorTemplateDefinition[] PendingWarriorTemplates()
        {
            var result = new WarriorTemplateDefinition[_warriorTemplateOrder.Count];
            for (int i = 0; i < result.Length; i++) result[i] = _warriorTemplates[_warriorTemplateOrder[i]];
            return result;
        }

        internal void ClearPendingWarriorTemplates()
        {
            _warriorTemplates.Clear();
            _warriorTemplateOrder.Clear();
        }

        public WarriorTemplateDefinition GetWarriorTemplate(string reference)
        {
            ThrowIfCompleted();
            if (string.IsNullOrWhiteSpace(reference)) throw new ModContentException("Warrior template reference must not be empty.");
            DefinitionId id;
            try { id = DefinitionId.Parse(reference); }
            catch (FormatException exception) { throw new ModContentException(exception.Message, exception); }
            if (id.Category != "warrior-templates" || !CanReferenceNamespace(id.Namespace))
                throw new ModContentException("Warrior template belongs to an invalid or undeclared namespace: '" + id + "'.");
            WarriorTemplateDefinition value;
            if (TryGetAnyWarriorTemplate(id, out value)) return value;
            throw new ModContentException("Warrior template is not registered: '" + id + "'.");
        }
    }
}
