using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Eclipse.Modding
{
    public sealed partial class LegacyContentAdapter
    {
        private readonly List<string> _p1dLocations = new List<string>();
        private readonly List<string> _p1dTactics = new List<string>();
        private bool _p1dApplied;

        public void ApplyP1DContent()
        {
            ThrowIfDisposed();
            if (_p1dApplied) throw new InvalidOperationException("P1D content is already applied.");
            try
            {
                ApplyLocaleMetadata();
                ApplyLocations();
                ApplyMoves();
                ApplyTactics();
                _p1dApplied = true;
            }
            catch
            {
                RemoveP1DContent();
                throw;
            }
        }

        private void ApplyLocaleMetadata()
        {
            foreach (LocaleMetadataDefinition definition in _content.LocaleMetadata)
            {
                var metadata = new ExternalLocaleMetadata
                {
                    Name = definition.Name,
                    Locale = definition.Locale,
                    Alias = definition.Alias,
                    FileIcon = definition.FileIcon,
                    FileIconSelected = definition.FileIconSelected,
                    LoaderImage = definition.LoaderImage,
                    PreloaderImage = definition.PreloaderImage,
                    IsAsian = definition.IsAsian
                };
                if (definition.Fonts != null)
                {
                    metadata.ContentFont = definition.Fonts.Content;
                    metadata.TitleFont = definition.Fonts.Title;
                    metadata.ButtonFont = definition.Fonts.Button;
                    metadata.FontSizeScale = definition.Fonts.FontSizeScale;
                    metadata.LineSpacing = definition.Fonts.LineSpacing;
                    metadata.CustomLineSpacingScale = definition.Fonts.CustomLineSpacingScale;
                }
                ExternalLocaleRuntime.Add(metadata);
            }
        }

        private void ApplyLocations()
        {
            foreach (LocationDefinition definition in _content.Locations)
            {
                ExternalLocationRuntime.Set(definition.RuntimeName, BuildLocationDocument(definition),
                    definition.HasMusic ? definition.Music.ToString() : string.Empty);
                _p1dLocations.Add(definition.RuntimeName);
            }
        }

        private XmlDocument BuildLocationDocument(LocationDefinition definition)
        {
            var document = new XmlDocument { XmlResolver = null };
            XmlElement root = document.CreateElement("Root");
            document.AppendChild(root);
            Set(root, "Color", definition.Color);
            Set(root, "Wall", F(definition.Wall));
            Set(root, "Floor", F(definition.Floor));
            Set(root, "PositionY", F(definition.PositionY));
            Set(root, "Width", F(definition.Width));
            Set(root, "Height", F(definition.Height));
            Set(root, "MinWidth", F(definition.MinWidth));
            Set(root, "FrictionForce", F(definition.FrictionForce));
            Set(root, "GridSize", definition.GridSize.ToString(CultureInfo.InvariantCulture));
            // ExternalLocationRuntime supplies music directly, but Location.init expects the
            // recovered Root attribute to exist on every valid params document.
            Set(root, "Music", string.Empty);

            for (int i = 0; i < definition.Layers.Count; i++)
            {
                LocationLayerDefinition layer = definition.Layers[i];
                XmlElement node = document.CreateElement("Layer");
                Set(node, "Type", layer.Type.ToString(CultureInfo.InvariantCulture));
                Set(node, "Factor", F(layer.Factor));
                if (layer.Scaling) Set(node, "Scaling", "1");
                string path = LocationAssetDirectory(layer.Images[0].Sprite);
                Set(node, "Path", path);
                for (int j = 0; j < layer.Images.Count; j++)
                {
                    LocationImageDefinition image = layer.Images[j];
                    string imagePath = LocationAssetDirectory(image.Sprite);
                    if (!string.Equals(path, imagePath, StringComparison.Ordinal))
                        throw new ModContentException("All images in one location layer must share an asset directory; split them into separate layers.");
                    XmlElement imageNode = document.CreateElement(image.IsMask ? "SpriteMask" : "Image");
                    Set(imageNode, "ClassName", LocationAssetLeaf(image.Sprite));
                    Set(imageNode, "X", F(image.X));
                    Set(imageNode, "Y", F(image.Y));
                    Set(imageNode, "Width", F(image.Width));
                    Set(imageNode, "Height", F(image.Height));
                    if (image.IsOpaque) Set(imageNode, "IsOpaque", "1");
                    if (image.FlipX) Set(imageNode, "FlipX", "1");
                    if (image.FlipY) Set(imageNode, "FlipY", "1");
                    node.AppendChild(imageNode);
                }
                root.AppendChild(node);
            }
            return document;
        }

        private void ApplyMoves()
        {
            if (_content.MoveTemplates.Count == 0 && _content.Moves.Count == 0 && _content.MoveTriggers.Count == 0)
                return;
            ExternalCombatContentRuntime.ApplyMoves(BuildMovesDocument());
        }

        private XmlDocument BuildMovesDocument()
        {
            var document = new XmlDocument { XmlResolver = null };
            XmlElement root = document.CreateElement("Movesxml");
            document.AppendChild(root);
            XmlElement templates = document.CreateElement("Templates");
            XmlElement moves = document.CreateElement("Moves");
            XmlElement triggers = document.CreateElement("Triggers");
            root.AppendChild(templates);
            root.AppendChild(moves);
            root.AppendChild(triggers);
            foreach (MoveTemplateDefinition definition in _content.MoveTemplates)
                templates.AppendChild(BuildMoveNode(document, "Template", definition, default(AssetId)));
            foreach (MoveDefinition definition in _content.Moves)
                moves.AppendChild(BuildMoveNode(document, "Move", definition, definition.Animation));
            foreach (MoveTriggerDefinition definition in _content.MoveTriggers)
                triggers.AppendChild(BuildTriggerNode(document, definition));
            return document;
        }

        private XmlElement BuildMoveNode(XmlDocument document, string elementName, MoveNodeDefinition definition,
            AssetId animation)
        {
            XmlElement node = document.CreateElement(elementName);
            Set(node, "Name", definition.RuntimeName);
            string templateNames = MoveTemplateNames(definition);
            if (templateNames.Length != 0) Set(node, "Template", templateNames);
            if (!string.IsNullOrEmpty(animation.Path)) Set(node, "FileName", animation.ToString());
            if (definition.Type.Length != 0) Set(node, "Type", definition.Type);
            if (definition.Priority != 0) Set(node, "Priority", definition.Priority.ToString(CultureInfo.InvariantCulture));
            if (definition.MidFrames != 0) Set(node, "MidFrames", definition.MidFrames.ToString(CultureInfo.InvariantCulture));
            if (definition.FirstFrame != 0) Set(node, "FirstFrame", definition.FirstFrame.ToString(CultureInfo.InvariantCulture));
            if (definition.EndFrame != 0) Set(node, "EndFrame", definition.EndFrame.ToString(CultureInfo.InvariantCulture));
            if (definition.MirrorNode.Length != 0) Set(node, "MirrorNode", definition.MirrorNode);
            if (definition.TacticEquivalent.Length != 0) Set(node, "TacticEquivalent", definition.TacticEquivalent);
            if (definition.TacticWeapon.Length != 0) Set(node, "TacticWeapon", definition.TacticWeapon);
            if (definition.Looped) Set(node, "Looped", "1");
            if (definition.EndsStage) Set(node, "EndsStage", "1");
            AppendEvents(document, node, definition.Events);
            AppendConditions(document, node, definition.Conditions, "Conditions");
            if (definition.Intervals.Count != 0)
            {
                XmlElement intervals = document.CreateElement("Intervals");
                foreach (ModMoveInterval interval in definition.Intervals)
                {
                    XmlElement item = document.CreateElement("Interval");
                    if (interval.Type.Length != 0) Set(item, "Type", interval.Type);
                    if (interval.Name.Length != 0) Set(item, "Name", interval.Name);
                    intervals.AppendChild(item);
                }
                node.AppendChild(intervals);
            }
            return node;
        }

        private string MoveTemplateNames(MoveNodeDefinition definition)
        {
            var names = new List<string>();
            for (int i = 0; i < definition.CoreTemplates.Count; i++)
                if (!string.IsNullOrWhiteSpace(definition.CoreTemplates[i])) names.Add(definition.CoreTemplates[i].Trim());
            for (int i = 0; i < definition.Templates.Count; i++) names.Add(definition.Templates[i].ToString());
            return string.Join("|", names.ToArray());
        }

        private XmlElement BuildTriggerNode(XmlDocument document, MoveTriggerDefinition definition)
        {
            XmlElement node = document.CreateElement("Trigger");
            Set(node, "Name", definition.RuntimeName);
            AppendEvents(document, node, definition.Events);
            AppendConditions(document, node, definition.Conditions, "Conditions");
            if (definition.Actions.Count != 0)
            {
                XmlElement actions = document.CreateElement("Actions");
                foreach (ModMoveAction action in definition.Actions)
                {
                    XmlElement item;
                    if (action.Kind == ModMoveActionKind.Sound)
                    {
                        item = document.CreateElement("Sound");
                        Set(item, "Name", action.Audio.ToString());
                        Set(item, "Volume", F(action.Volume));
                        if (action.Looped) Set(item, "Looped", "1");
                    }
                    else
                    {
                        item = document.CreateElement("HitEffect");
                        Set(item, "FileName", action.Name);
                    }
                    actions.AppendChild(item);
                }
                node.AppendChild(actions);
            }
            return node;
        }

        private void AppendEvents(XmlDocument document, XmlElement parent, IReadOnlyList<ModMoveEvent> values)
        {
            if (values.Count == 0) return;
            XmlElement events = document.CreateElement("Events");
            for (int i = 0; i < values.Count; i++)
            {
                ModMoveEvent value = values[i];
                XmlElement item = document.CreateElement(MoveEventElement(value.Kind));
                if (value.Name.Length != 0) Set(item, "Name", value.Name);
                if (value.Player.Length != 0) Set(item, "Player", value.Player);
                events.AppendChild(item);
            }
            parent.AppendChild(events);
        }

        private void AppendConditions(XmlDocument document, XmlElement parent, IReadOnlyList<ModMoveCondition> values,
            string containerName)
        {
            if (values.Count == 0) return;
            XmlElement conditions = document.CreateElement(containerName);
            for (int i = 0; i < values.Count; i++) conditions.AppendChild(BuildMoveCondition(document, values[i]));
            parent.AppendChild(conditions);
        }

        private XmlElement BuildMoveCondition(XmlDocument document, ModMoveCondition value)
        {
            if (value.Kind == ModMoveConditionKind.All || value.Kind == ModMoveConditionKind.Any)
            {
                XmlElement op = document.CreateElement("Operator");
                Set(op, "Type", value.Kind == ModMoveConditionKind.All ? "And" : "Or");
                if (value.Not) Set(op, "Not", "1");
                for (int i = 0; i < value.Children.Count; i++) op.AppendChild(BuildMoveCondition(document, value.Children[i]));
                return op;
            }
            string element = value.Kind == ModMoveConditionKind.CurrentAnimation ? "CurrentAnimation" :
                value.Kind == ModMoveConditionKind.CurrentInterval ? "CurrentInterval" : "Item";
            XmlElement node = document.CreateElement(element);
            if (value.Name.Length != 0) Set(node, "Name", value.Name);
            if (value.Player.Length != 0) Set(node, "Player", value.Player);
            if (value.ItemType.Length != 0) Set(node, "Type", value.ItemType);
            if (value.ItemSubType.Length != 0) Set(node, "SubType", value.ItemSubType);
            if (value.Not) Set(node, "Not", "1");
            return node;
        }

        private static string MoveEventElement(ModMoveEventKind kind)
        {
            switch (kind)
            {
                case ModMoveEventKind.AnimationEnd: return "AnimationEnd";
                case ModMoveEventKind.AnimationStart: return "AnimationStart";
                case ModMoveEventKind.IntervalEnd: return "IntervalEnd";
                case ModMoveEventKind.IntervalStart: return "IntervalStart";
                case ModMoveEventKind.Hit: return "Hit";
                case ModMoveEventKind.Strike: return "Strike";
                case ModMoveEventKind.EveryFrame: return "EveryFrame";
                case ModMoveEventKind.Birth: return "Birth";
                case ModMoveEventKind.RoundStageStart: return "RoundStageStart";
                case ModMoveEventKind.ModExpires: return "ModExpires";
                default: throw new ModContentException("Unsupported move event kind: " + kind);
            }
        }

        private void ApplyTactics()
        {
            if (_content.Tactics.Count == 0) return;
            var document = new XmlDocument { XmlResolver = null };
            XmlElement root = document.CreateElement("TacticsSettings");
            XmlElement tactics = document.CreateElement("Tactics");
            document.AppendChild(root);
            root.AppendChild(tactics);
            foreach (TacticDefinition definition in _content.Tactics)
            {
                tactics.AppendChild(BuildTacticNode(document, definition));
                _p1dTactics.Add(definition.RuntimeName);
            }
            ExternalCombatContentRuntime.ApplyTactics(document);
        }

        private XmlElement BuildTacticNode(XmlDocument document, TacticDefinition definition)
        {
            XmlElement node = document.CreateElement("Tactic");
            Set(node, "Name", definition.RuntimeName);
            Set(node, "Type", definition.Kind == ModTacticKind.Random ? "Random" : "Tabular");
            if (definition.CoreTemplate.Length != 0) Set(node, "Template", definition.CoreTemplate);
            if (definition.AnimationWeights.Count != 0)
            {
                XmlElement weights = document.CreateElement("AnimationWeights");
                for (int i = 0; i < definition.AnimationWeights.Count; i++)
                    weights.AppendChild(BuildTacticAnimationValue(document, "Animation", definition.AnimationWeights[i]));
                node.AppendChild(weights);
            }
            if (definition.CounterAttack != null || definition.Dodge != null || definition.Block != null)
            {
                XmlElement defense = document.CreateElement("UseDefense");
                if (definition.CounterAttack != null) defense.AppendChild(BuildTacticValue(document, "CounterAttackChance", definition.CounterAttack));
                if (definition.Dodge != null) defense.AppendChild(BuildTacticValue(document, "DodgeChance", definition.Dodge));
                if (definition.Block != null) defense.AppendChild(BuildTacticValue(document, "BlockChance", definition.Block));
                node.AppendChild(defense);
            }
            AppendTacticValue(document, node, "UseSafeAttackChance", definition.SafeAttack);
            AppendTacticValue(document, node, "TableAttackChance", definition.TableAttack);
            AppendTacticValue(document, node, "CautiousMovementsChance", definition.CautiousMovement);
            AppendTacticValue(document, node, "DodgeMissilesChance", definition.DodgeMissiles);
            AppendTacticValue(document, node, "DodgeMagicChance", definition.DodgeMagic);
            AppendTacticAnimationList(document, node, "QuickAttacks", "QuickAttackChance", definition.QuickAttacks);
            AppendTacticAnimationList(document, node, "Evades", "EvadeChance", definition.Evades);
            AppendTacticAnimationList(document, node, "ExpectedWait", "Animation", definition.ExpectedWait);
            if (definition.MemoryStrikes != 0 || definition.MemoryRoundFactor != 0f)
            {
                XmlElement memory = document.CreateElement("Memory");
                Set(memory, "Strikes", definition.MemoryStrikes.ToString(CultureInfo.InvariantCulture));
                Set(memory, "RoundFactor", F(definition.MemoryRoundFactor));
                node.AppendChild(memory);
            }
            return node;
        }

        private void AppendTacticValue(XmlDocument document, XmlElement parent, string name, ModTacticValue value)
        {
            if (value != null) parent.AppendChild(BuildTacticValue(document, name, value));
        }

        private XmlElement BuildTacticValue(XmlDocument document, string name, ModTacticValue value)
        {
            XmlElement node = document.CreateElement(name);
            Set(node, "Base", F(value.Base)); Set(node, "CounterFactor", F(value.CounterFactor));
            Set(node, "DamageFactor", F(value.DamageFactor)); Set(node, "HealthFactor", F(value.HealthFactor));
            Set(node, "EnemyHealthFactor", F(value.EnemyHealthFactor));
            Set(node, "AnimationFramesFactor", F(value.AnimationFramesFactor));
            Set(node, "ChildFramesFactor", F(value.ChildFramesFactor));
            Set(node, "MagicBulletFactor", F(value.MagicBulletFactor));
            Set(node, "MissileBulletFactor", F(value.MissileBulletFactor)); Set(node, "HitFactor", F(value.HitFactor));
            Set(node, "DistanceFactor", F(value.DistanceFactor)); Set(node, "Shift", F(value.Shift));
            Set(node, "Limit", F(value.Limit)); Set(node, "AntiLimit", F(value.AntiLimit));
            Set(node, "FactorType", value.FactorType == ModTacticFactorType.Exponential ? "Exponential" : "Linear");
            return node;
        }

        private XmlElement BuildTacticAnimationValue(XmlDocument document, string element, ModTacticAnimationValue value)
        {
            XmlElement node = BuildTacticValue(document, element, value.Value);
            Set(node, "Name", TacticAnimationName(value));
            if (element == "QuickAttackChance" || element == "EvadeChance")
            {
                node.RemoveAttribute("Name");
                Set(node, "Animation", TacticAnimationName(value));
            }
            return node;
        }

        private void AppendTacticAnimationList(XmlDocument document, XmlElement parent, string container,
            string element, IReadOnlyList<ModTacticAnimationValue> values)
        {
            if (values.Count == 0) return;
            XmlElement node = document.CreateElement(container);
            for (int i = 0; i < values.Count; i++) node.AppendChild(BuildTacticAnimationValue(document, element, values[i]));
            parent.AppendChild(node);
        }

        private static string TacticAnimationName(ModTacticAnimationValue value)
        {
            return value.HasMove ? value.Move.ToString() : value.Animation;
        }

        private void RemoveP1DContent()
        {
            for (int i = _p1dTactics.Count - 1; i >= 0; i--) ExternalCombatContentRuntime.RemoveTactic(_p1dTactics[i]);
            _p1dTactics.Clear();
            for (int i = _p1dLocations.Count - 1; i >= 0; i--) ExternalLocationRuntime.Remove(_p1dLocations[i]);
            _p1dLocations.Clear();
            ExternalLocaleRuntime.Clear();
            _p1dApplied = false;
        }

        private static string LocationAssetDirectory(AssetId id)
        {
            int slash = id.Path.LastIndexOf('/');
            if (slash <= 0) return id.Namespace + ":";
            return id.Namespace + ":" + id.Path.Substring(0, slash);
        }

        private static string LocationAssetLeaf(AssetId id)
        {
            int slash = id.Path.LastIndexOf('/');
            return slash < 0 ? id.Path : id.Path.Substring(slash + 1);
        }

        private static string F(float value) => value.ToString(CultureInfo.InvariantCulture);
    }
}
