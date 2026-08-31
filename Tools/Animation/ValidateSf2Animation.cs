// Compiled only by TestSf2Animation.ps1 in an isolated Unity 2022.3 project.
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ValidateSf2Animation
{
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }

    public static void Run()
    {
        try
        {
            string[] expected = File.ReadAllLines(Path.Combine(Application.dataPath, "../expected.txt"));
            int frameCount = int.Parse(expected[0]);
            int nodeCount = int.Parse(expected[1]);
            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/sample.bytes");
            Require(asset != null, "Unity did not import the bytes as a TextAsset");
            byte[] data = asset.bytes;
            var reader = new RecoveredAnimationReader();
            reader.Read(data);
            Vector3[][] frames = reader._AnimationContainer;
            Require(frames.Length == frameCount, "Frame count mismatch");
            Require(reader.LHHAGECFIOL == frameCount - 1, "EndFrame mismatch");
            Require(data.Length == 4 + frameCount * (5 + nodeCount * 12), "Binary length mismatch");
            int checks = 4;
            for (int i = 0; i < frames.Length; i++)
            {
                Require(frames[i].Length == nodeCount, "Node count mismatch");
                checks++;
                for (int j = 0; j < nodeCount; j++)
                {
                    Vector3 p = frames[i][j];
                    int offset = 4 + i * (5 + nodeCount * 12) + 5 + j * 12;
                    Require(!float.IsNaN(p.sqrMagnitude) && !float.IsInfinity(p.sqrMagnitude), "Nonfinite point");
                    Require(p.x == BitConverter.ToSingle(data, offset), "X changed");
                    Require(p.y == -BitConverter.ToSingle(data, offset + 4), "Y sign mismatch");
                    Require(p.z == BitConverter.ToSingle(data, offset + 8), "Z changed");
                    checks += 4;
                }
            }
            // Force a real malformed-payload failure through the recovered method.
            bool rejected = false;
            try { new RecoveredAnimationReader().Read(new byte[] { 60, 0, 0, 0 }); }
            catch (Exception) { rejected = true; }
            Require(rejected, "Truncated payload was accepted");
            Debug.Log("[AnimationReader] PASS " + frameCount + " frames, " + nodeCount + " nodes; " + (checks+1) + " checks. Unity TextAsset + unchanged recovered ReadAnimation method.");
            EditorApplication.Exit(0);
        }
        catch (Exception ex)
        {
            Debug.LogError("[AnimationReader] FAIL " + ex);
            EditorApplication.Exit(1);
        }
    }
}
