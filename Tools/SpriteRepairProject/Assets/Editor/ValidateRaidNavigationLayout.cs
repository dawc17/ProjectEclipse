using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Nekki.SF2.GUI.Map;

// Uses copies of the actual runtime layout helper and WideScreenController.
// No game/profile initialization, no access to a save, no main-project editor.
public static class ValidateRaidNavigationLayout
{
    public static void Run()
    {
        try
        {
            MethodInfo inset = typeof(WideScreenController).GetMethod("Run",
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new[] { typeof(RectTransform), typeof(float) }, null);
            int checks = 0;
            foreach (float width in new[] { 2048f, 2730.6667f, 3584f, 5461.3335f })
            {
                var surfaceObject = new GameObject("Canvas rect", typeof(RectTransform));
                var surface = (RectTransform)surfaceObject.transform;
                surface.sizeDelta = new Vector2(width, 1536);
                var controllerObject = new GameObject("Widescreen controller", typeof(WideScreenController));
                var controller = controllerObject.GetComponent<WideScreenController>();
                try
                {
                    RectTransform root = RaidMapControlsLayout.CreateRoot(surface);
                    float margin = width > 2731 ? (int)((width - 2730.6667f) / 2) + 1 : 0;
                    var buttons = new RectTransform[2];
                    for (int i = 0; i < buttons.Length; i++)
                    {
                        var buttonObject = new GameObject(i == 0 ? "UnderworldToggle" : "RaidMapScrollButton",
                            typeof(RectTransform), typeof(Image), typeof(Button));
                        buttons[i] = (RectTransform)buttonObject.transform;
                        RaidMapControlsLayout.AnchorNavigationButton(buttons[i], root, i == 0 ? -205 : -292);
                        var image = buttonObject.GetComponent<Image>();
                        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Generated/sprite_" + (i == 0 ? 0 : 2) + ".asset");
                        image.preserveAspect = true;
                        Require(image.sprite != null && image.sprite.vertices.Length == 4, "Native navigation sprite missing");
                    }
                    // Same post-Init operation that Scene.Awake performs on direct children.
                    if (margin > 0) inset.Invoke(controller, new object[] { root, margin });
                    foreach (RectTransform rect in buttons)
                    {
                        Require(Mathf.Abs(rect.rect.width - 102) < 0.001f && Mathf.Abs(rect.rect.height - 98) < 0.001f,
                            "Widescreen collapsed navigation button at width " + width);
                        Require(rect.anchoredPosition.x == -445, "Widescreen moved the fixed button");
                        var corners = new Vector3[4];
                        rect.GetWorldCorners(corners);
                        foreach (Vector3 corner in corners)
                            Require(root.rect.Contains(root.InverseTransformPoint(corner)), "Button outside visible map layer");
                        Require(rect.GetComponent<Button>().IsInteractable(), "Navigation button not interactable");
                        checks++;
                    }
                    if (margin > 0)
                    {
                        // Prove the former direct-child hierarchy reproduces the reported bug.
                        var legacy = new GameObject("Legacy direct canvas button", typeof(RectTransform));
                        var old = (RectTransform)legacy.transform;
                        RaidMapControlsLayout.AnchorNavigationButton(old, surface, -205);
                        inset.Invoke(controller, new object[] { old, margin });
                        Require(old.rect.width <= 0, "Expected old-layout regression not reproduced");
                        // Re-running the controller must not progressively move the new controls.
                        inset.Invoke(controller, new object[] { root, margin });
                        Require(buttons[0].rect.width == 102 && buttons[0].anchoredPosition.x == -445,
                            "Repeated widescreen pass changed the fixed controls");
                        checks++;
                    }
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(surfaceObject);
                    UnityEngine.Object.DestroyImmediate(controllerObject);
                }
            }
            Debug.Log("[RaidNavigationLayout] PASS " + checks +
                " cases: 4:3, 16:9, 21:9, 32:9; old negative-width bug reproduced; repaired controls stay 102x98, in bounds and interactable.");
            EditorApplication.Exit(0);
        }
        catch (Exception error)
        {
            Debug.LogException(error);
            EditorApplication.Exit(1);
        }
    }

    private static void Require(bool value, string message)
    {
        if (!value) throw new InvalidOperationException(message);
    }
}
