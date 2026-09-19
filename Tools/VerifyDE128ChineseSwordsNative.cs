// Run TestDE128ChineseSwords.ps1 first, then unity command eval_file --file Tools/VerifyDE128ChineseSwordsNative.cs --json.
// Uses private temporary catalogs and restores parser lookup objects in finally. No live move list or save is modified.
var flags = System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic;
var saved = new System.Collections.Generic.Dictionary<System.Reflection.FieldInfo, object>();
foreach (var owner in new[] { typeof(MovesParser), typeof(MovesMaps) })
    foreach (var field in owner.GetFields(flags))
        if (!field.IsLiteral && !field.IsInitOnly) saved.Add(field, field.GetValue(null));
try
{
    foreach (var field in typeof(MovesMaps).GetFields(flags))
        if (!field.IsLiteral && !field.IsInitOnly) field.SetValue(null, null);
    MovesMaps.Init();
    var source = new System.Xml.XmlDocument(); source.Load("Assets/vanillaXml/animations/moves.xml");
    var projected = new System.Xml.XmlDocument(); projected.Load("Temp/DE128ChineseSwords.projected.xml");
    var nodes = new System.Collections.Generic.Dictionary<string, System.Xml.XmlNode>();
    var templates = new System.Collections.Generic.Dictionary<string, TemplateAnimation>();
    foreach (System.Xml.XmlElement node in source.SelectNodes("/Movesxml/Templates/Template"))
    {
        nodes.Add(node.GetAttribute("Name"), node.CloneNode(true));
        templates.Add(node.GetAttribute("Name"), new TemplateAnimation(node));
    }
    typeof(MovesParser).GetField("_BaseTemplateNodes", flags).SetValue(null, nodes);
    typeof(MovesParser).GetField("_BaseLegacyTemplateNodes", flags).SetValue(null, new System.Collections.Generic.Dictionary<string, System.Xml.XmlNode>());
    var moves = new System.Collections.Generic.List<InfoAnimation>();
    var tricks = new System.Collections.Generic.List<Trick>();
    var triggers = new System.Collections.Generic.List<Trigger>();
    var count = (int)typeof(MovesParser).GetMethod("ParseAdditional", flags).Invoke(null, new object[] { projected, moves, templates, tricks, triggers });
    if (count != 2 || tricks.Count != 1) throw new Exception("Lost native moves/profile.");
    if (tricks[0].Name != "fixture.moves:moves/chinese_swords_super_slash" || tricks[0].DisplayName != "fixture.moves:localization/move.chinese_swords_super_slash" || tricks[0].Rank != 4)
        throw new Exception("Profile identity/title/rank mismatch.");
    foreach (var move in moves)
    {
        typeof(InfoAnimation).GetMethod("ReadAnimation", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            .Invoke(move, new object[] { System.IO.File.ReadAllBytes("Mods/de128/assets/animations/chinese_swords_super_slash_old.bytes") });
        var samples = (UnityEngine.Vector3[][])typeof(InfoAnimation).GetField("_AnimationContainer", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(move);
        if (samples.Length != 38 || samples[0].Length != 67) throw new Exception("Unexpected recovered binary dimensions.");
        // Archive recovery intervals extend beyond this binary; native GetIntervals
        // clamps their end to the last sample. Every authored attack/sound is in range.
        if (!move.Name.Contains("shop_"))
        {
            foreach (var interval in move.MoveData.Intervals)
                if (interval.Type == IntervalAnimation.NGAJJDIEDGF.INTERVAL_ATTACK && interval.GEJLNPIEDPF >= samples.Length)
                    throw new Exception("Attack extends beyond recovered binary.");
            var active = new System.Collections.Generic.List<IntervalAnimation>();
            var ended = new System.Collections.Generic.List<IntervalAnimation>();
            move.GetIntervals(37, active, ended);
            if (!active.Exists(interval => interval.Name == "Uninterrupt")) throw new Exception("Recovery interval lost before final sample.");
            move.GetIntervals(38, active, ended);
            if (active.Exists(interval => interval.Name == "Uninterrupt") || !ended.Exists(interval => interval.Name == "Uninterrupt"))
                throw new Exception("Native recovery clamp changed.");
        }
        foreach (var frame in samples)
        {
            if (frame.Length != samples[0].Length) throw new Exception("Inconsistent animation node count.");
            foreach (var point in frame)
                if (float.IsNaN(point.x) || float.IsInfinity(point.x) || float.IsNaN(point.y) || float.IsInfinity(point.y) || float.IsNaN(point.z) || float.IsInfinity(point.z)) throw new Exception("Nonfinite animation point.");
        }
        var state = new ModelConditions();
        state.IBBALIJOJMC = move.Name.Contains("shop_") ? SceneTypes.SceneShopWeapon : SceneTypes.SceneFight;
        state.OJIAKDDCGLB = new System.Collections.Generic.List<ItemInfo>();
        state.OJIAKDDCGLB.Add(new ItemInfo(null) { Type = "Weapon", SubType = "ChineseSwords" });
        state.OJIAKDDCGLB.Add(new ItemInfo(null) { Type = "Skeleton", SubType = "Skeleton" });
        foreach (var condition in move.MoveData.Locks) if (!condition.IsEqual(state)) throw new Exception("Native eligibility failed: " + move.Name);
        state.OJIAKDDCGLB[0].SubType = "Katana";
        bool rejected = false;
        foreach (var condition in move.MoveData.Locks) if (!condition.IsEqual(state)) rejected = true;
        if (!rejected) throw new Exception("Unrelated equipment accepted.");
    }
    return new { moves = count, profiles = tricks.Count, title = tricks[0].DisplayName, binaryFrames = 38, binaryNodes = 67, recoveryEndClamped = true, matchingEquipmentAccepted = true, unrelatedEquipmentRejected = true };
}
finally
{
    foreach (var pair in saved) pair.Key.SetValue(null, pair.Value);
}
