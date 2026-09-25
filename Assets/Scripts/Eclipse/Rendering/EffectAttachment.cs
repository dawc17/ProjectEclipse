using System;
using System.Globalization;
using System.Xml;
using UnityEngine;

namespace Eclipse.Rendering
{
    // The recovered effect action retained its Attach XML but never interpreted it.
    // Resolve the two live model nodes each frame so effects follow animation and mirroring.
    public sealed class EffectAttachment
    {
        private readonly string _player;
        private readonly string _rootPoint;
        private readonly string _attachPoint;
        private readonly Vector2 _offset;
        private readonly float _startAngle;

        public EffectAttachment(XmlNode node)
        {
            _player = node.Attributes["Player"]?.Value ?? "Me";
            _rootPoint = node.Attributes["RootPoint"]?.Value ?? string.Empty;
            _attachPoint = node.Attributes["AttachPoint"]?.Value ?? string.Empty;
            string[] offset = (node.Attributes["OffsetVector"]?.Value ?? "0;0").Split(';');
            if (offset.Length != 2 || _rootPoint.Length == 0 || _attachPoint.Length == 0)
                throw new FormatException("Effect Attach requires RootPoint, AttachPoint and a two-component OffsetVector.");
            _offset = new Vector2(float.Parse(offset[0], CultureInfo.InvariantCulture),
                float.Parse(offset[1], CultureInfo.InvariantCulture));
            _startAngle = float.Parse(node.Attributes["StartRotAngle"]?.Value ?? "0", CultureInfo.InvariantCulture);
        }

        public bool TryGetTransform(Model actor, out Vector3 position, out Quaternion rotation)
        {
            position = default;
            rotation = Quaternion.identity;
            Model target = actor.GetModelByRole(_player);
            if (target == null) return false;
            ModelObject body = target.GetModelObject();
            if (body == null) return false;
            ModelNode rootNode = body.FindNodeOrParent(_rootPoint);
            ModelNode attachNode = body.FindNodeOrParent(_attachPoint);
            if (rootNode == null || attachNode == null) return false;

            Vector3f root = rootNode.GetStart();
            Vector3f attach = attachNode.GetStart();
            float dx = attach.GetX() - root.GetX();
            float dy = attach.GetY() - root.GetY();
            if (dx * dx + dy * dy < 0.0001f) return false;
            float angle = Mathf.Atan2(dy, dx);
            float cosine = Mathf.Cos(angle);
            float sine = Mathf.Sin(angle);
            // Archived OffsetVector uses the two-node local frame; positive Y is
            // down in the move data, as it is for ordinary effect position shifts.
            position = new Vector3(root.GetX() + _offset.x * cosine + _offset.y * sine,
                root.GetY() + _offset.x * sine - _offset.y * cosine, root.GetZ());
            rotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg + _startAngle);
            return true;
        }
    }
}
