using System.Collections.Generic;
using Eclipse.Multiplayer.Online;

// The partial declarations give the codec direct access without reflection or
// derived-state setters. All authored snapshot machinery stays under Eclipse.
public partial class ModelNode
{
    internal void SaveRollbackNode(SnapshotTape tape)
    {
        tape.Object(_PairNode);
        tape.Object(_Name);
        SavePoint(tape, _Start);
        SavePoint(tape, _End);
        tape.Int((int)_Type);
        tape.Int(_Id);
        tape.Float(_Weight);
        tape.Float(_Attenuation);
        tape.Int((_IsNode ? 1 : 0) | (_IsFixed ? 2 : 0) | (_IsCloth ? 4 : 0) |
            (_IsPhysics ? 8 : 0) | (_defaultPhysics ? 16 : 0) | (_IsFixedAndNotNode ? 32 : 0) |
            (_Visible ? 64 : 0) | (_physicsActive ? 128 : 0) | (_IsShock ? 256 : 0) |
            (_Collisible ? 512 : 0) | (_Weak ? 1024 : 0) | (_skipMacroUpdate ? 2048 : 0));
    }

    internal void LoadRollbackNode(SnapshotTape tape)
    {
        _PairNode = (ModelNode)tape.ReadObject();
        _Name = (string)tape.ReadObject();
        _Start = LoadPoint(tape);
        _End = LoadPoint(tape);
        _Type = (NodeType)tape.ReadInt();
        _Id = tape.ReadInt();
        _Weight = tape.ReadFloat();
        _Attenuation = tape.ReadFloat();
        int flags = tape.ReadInt();
        _IsNode = (flags & 1) != 0; _IsFixed = (flags & 2) != 0;
        _IsCloth = (flags & 4) != 0; _IsPhysics = (flags & 8) != 0;
        _defaultPhysics = (flags & 16) != 0; _IsFixedAndNotNode = (flags & 32) != 0;
        _Visible = (flags & 64) != 0; _physicsActive = (flags & 128) != 0;
        _IsShock = (flags & 256) != 0; _Collisible = (flags & 512) != 0;
        _Weak = (flags & 1024) != 0; _skipMacroUpdate = (flags & 2048) != 0;
    }

    internal void WalkRollbackNode(List<object> into)
    {
        into.Add(_PairNode);
        // Exact vectors are already copied above. Preserve any future subclass's
        // extra fields through the generic walker rather than silently omitting them.
        if (_Start != null && _Start.GetType() != typeof(Vector3f)) into.Add(_Start);
        if (_End != null && _End.GetType() != typeof(Vector3f)) into.Add(_End);
    }

    private static void SavePoint(SnapshotTape tape, Vector3f point)
    {
        tape.Object(point);
        if (point != null) tape.Float3(point.GetX(), point.GetY(), point.GetZ());
    }

    private static Vector3f LoadPoint(SnapshotTape tape)
    {
        var point = (Vector3f)tape.ReadObject();
        if (point != null)
        {
            tape.ReadFloat3(out float x, out float y, out float z);
            point.Set(x, y, z);
        }
        return point;
    }
}

public partial class ModelMacroNode
{
    internal void SaveRollbackMacro(SnapshotTape tape)
    {
        tape.Object(NamedWeights);
        tape.Object(_nodeWeights);
    }

    internal void LoadRollbackMacro(SnapshotTape tape)
    {
        NamedWeights = (List<Pair<string, float>>)tape.ReadObject();
        _nodeWeights = (List<Pair<ModelNode, float>>)tape.ReadObject();
    }

    internal void WalkRollbackMacro(List<object> into)
    {
        into.Add(NamedWeights);
        into.Add(_nodeWeights);
    }
}
