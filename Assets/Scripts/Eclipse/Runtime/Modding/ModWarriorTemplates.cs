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

        internal WarriorTemplateDefinition(DefinitionId id, string legacyName)
        {
            Id = id;
            LegacyName = legacyName ?? string.Empty;
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
            if (_catalog.TryGetWarriorTemplate(id, out value)) return value;
            throw new ModContentException("Warrior template is not registered: '" + id + "'.");
        }
    }
}
