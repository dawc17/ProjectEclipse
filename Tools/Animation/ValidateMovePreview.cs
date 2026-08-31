// Compiled only by TestSf2Animation.ps1 -WithMovePreview in its isolated project.
using System;
using System.IO;
using System.Xml;
using Eclipse.Content;
using UnityEditor;
using UnityEngine;

public static class ValidateMovePreview
{
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }

    public static void Run()
    {
        try
        {
            string vanilla = File.ReadAllText(Path.Combine(Application.dataPath, "../vanilla-moves.xml"));
            var moves = new XmlDocument();
            moves.LoadXml(vanilla);
            Require(LocalAnimationPreview.Apply(moves), "Preview was not applied");
            var move = (XmlElement)moves.SelectSingleNode("/Movesxml/Moves/Move[@Name='FrontKick']");
            Require(move.GetAttribute("FileName") == "_eclipse_preview/kazuya_ff3.bytes", "Wrong animation binding");
            Require(move.GetAttribute("MidFrames") == "0" && move.GetAttribute("EndFrame") == "59", "Wrong timing");
            Require(move.SelectSingleNode("Conditions/Keys/Key[@Type='Kick'][@PressType='Tap']") != null &&
                move.SelectSingleNode("Conditions/Keys/Key[@Type='Forward'][@PressType='Hold']") != null, "Input changed");
            Require(move.SelectNodes("Intervals/Interval[@Type='Attack'][@Start='18'][@End='22']/AttackingParts/Edge").Count == 5,
                "Missing attack window");
            foreach (XmlElement edge in move.SelectNodes("Intervals/Interval[@Type='Attack']/AttackingParts/Edge"))
                Require(edge.GetAttribute("Name").EndsWith("_1"), "Wrong kicking side");
            byte[] payload;
            const string request = "gamedata/animations/binary/_eclipse_preview/kazuya_ff3.bytes";
            Require(LocalAnimationPreview.TryGetBinary(request, out payload), "Animation route failed");
            var reader = new RecoveredAnimationReader();
            reader.Read(payload);
            Require(reader._AnimationContainer.Length == 60 && reader._AnimationContainer[0].Length == 67, "Routed animation failed to decode");
            Require(!LocalAnimationPreview.TryGetBinary("gamedata/animations/binary/stance_idle.bytes", out payload), "Vanilla resource intercepted");
            string directory = LocalAnimationPreview.DirectoryPath;
            File.Delete(Path.Combine(directory, "enabled"));
            moves.LoadXml(vanilla);
            string pristine = moves.OuterXml;
            Require(!LocalAnimationPreview.Apply(moves) && moves.OuterXml == pristine, "Disabled preview altered vanilla");
            Require(!LocalAnimationPreview.TryGetBinary(request, out payload), "Disabled preview retained cached bytes");
            File.WriteAllText(Path.Combine(directory, "enabled"), "enabled");
            File.WriteAllBytes(Path.Combine(directory, "kazuya_ff3.bytes"), new byte[] { 60, 0, 0, 0 });
            Require(!LocalAnimationPreview.Apply(moves) && moves.OuterXml == pristine, "Broken preview did not preserve vanilla");
            Require(!LocalAnimationPreview.TryGetBinary(request, out payload), "Broken preview retained cached bytes");
            Debug.Log("[AnimationPreview] PASS input, attack edges/timing, binary routing/decoding, unrelated-resource fallback, disable/cache reset and corrupt-file fallback.");
            ValidateSf2Animation.Run();
        }
        catch (Exception ex)
        {
            Debug.LogError("[AnimationPreview] FAIL " + ex);
            EditorApplication.Exit(1);
        }
    }
}
