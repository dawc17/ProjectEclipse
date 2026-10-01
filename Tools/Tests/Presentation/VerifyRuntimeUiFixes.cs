
var checks = new System.Collections.Generic.List<string>();
Action<bool,string> check = (ok, label) => { if (!ok) throw new Exception(label); checks.Add(label); };
var root = new GameObject("Runtime UI regression fixture", typeof(RectTransform), typeof(Canvas));
root.hideFlags = HideFlags.HideAndDontSave;
try
{
    var life = new Eclipse.Underworld.UI.UnderworldRaidLifeBarTransition();
    life.Reset(1);
    var lethal = life.Update(0, 0, 1, 1, 1, 1);
    check(lethal.Handled && !lethal.ResetLife && lethal.SetHitBar && lethal.HitBarFrames > 0, "Single-bar lethal hit drains yellow bar");
    check(life.Update(0,0,1,0,.5f,0).Handled, "Yellow drain survives subsequent zero-health updates");
    life.Reset(4);
    check(life.Update(0,0,4,1,1,1).HitBarFrames > 0, "Multi-bar one-shot drains yellow bar");

    foreach (var name in new[] { "FightPause.PauseLeft", "FightPause.PauseRight", "Logo.left", "Logo.right" })
    {
        var child = new GameObject(name, typeof(RectTransform), typeof(UnityEngine.UI.Image));
        child.transform.SetParent(root.transform, false);
        var image = child.GetComponent<UnityEngine.UI.Image>();
        var sprite = Nekki.SF2.GUI.ResolutionImage.GetSprite(name.StartsWith("Logo") ? "Textures/Logos/" : "UI/Atlases/", name);
        check(sprite != null, name + " resolves");
        image.sprite = sprite;
        var r = image.rectTransform;
        r.sizeDelta = name.StartsWith("Logo") ? new Vector2(name.EndsWith("left") ? 842 : 826, 494) : new Vector2(400,192);
        Eclipse.UI.SplitImageLayout.Apply(image, name);
        var size = r.sizeDelta; var position = r.anchoredPosition;
        check(Mathf.Abs(size.y - sprite.rect.height) < .01f, name + " preserves trimmed height");
        Eclipse.UI.SplitImageLayout.Apply(image, name);
        check(r.sizeDelta == size && r.anchoredPosition == position, name + " layout is idempotent");
    }

    var scrollObject = new GameObject("Wheel", typeof(RectTransform), typeof(Nekki.SF2.GUI.SFScrollRect));
    scrollObject.transform.SetParent(root.transform, false);
    ((RectTransform)scrollObject.transform).sizeDelta = new Vector2(400,200);
    var content = new GameObject("Content", typeof(RectTransform)).GetComponent<RectTransform>();
    content.SetParent(scrollObject.transform, false); content.sizeDelta = new Vector2(1800,200);
    var scroll = scrollObject.GetComponent<Nekki.SF2.GUI.SFScrollRect>();
    scroll.set_content(content); scroll.set_horizontal(true); scroll.set_vertical(false);
    scroll.set_movementType(Nekki.SF2.GUI.SFScrollRect.MDMLKCMBBPA.Clamped);
    scroll.set_horizontalNormalizedPosition(.5f);
    float before = content.anchoredPosition.x;
    scroll.OnScroll(new UnityEngine.EventSystems.PointerEventData(null) { scrollDelta = new Vector2(0,-1) });
    check(Mathf.Abs(content.anchoredPosition.x-before) >= 99, "Wheel moves horizontal lists visibly");
    Eclipse.UI.DesktopScrollbars.Attach(scroll, null);
    check(scroll.get_horizontalScrollbar() != null, "Horizontal scrollbar is attached");
    scroll.get_horizontalScrollbar().value = .2f;
    check(Mathf.Abs(scroll.get_horizontalNormalizedPosition()-.2f) < .001f, "Scrollbar controls content position");

    var labelObject = new GameObject("Inline icons", typeof(RectTransform), typeof(Nekki.SF2.GUI.TextPic));
    labelObject.transform.SetParent(root.transform, false);
    var label = labelObject.GetComponent<Nekki.SF2.GUI.TextPic>();
    label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    label.rectTransform.sizeDelta = new Vector2(800,200);
    label.text = "<b>A</b><quad name=UI/Atlases/FightPause.PauseLeft size=25 width=1 />B<quad name=UI/Atlases/FightPause.PauseRight size=25 width=1 />";
    var icons = label.GetComponentsInChildren<Nekki.SF2.GUI.ResolutionImage>();
    check(icons.Length == 2 && icons[0].sprite != null && icons[1].sprite != null, "Qualified inline dialogue textures resolve");
    using (var vertices = new UnityEngine.UI.VertexHelper())
    {
        typeof(Nekki.SF2.GUI.TextPic).GetMethod("OnPopulateMesh", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new[] { typeof(UnityEngine.UI.VertexHelper) }, null).Invoke(label, new object[] { vertices });
        check(icons[1].rectTransform.localPosition.x > icons[0].rectTransform.localPosition.x, "Multiple inline icons follow rendered text positions");
    }
    label.text = "Plain text";
    check(!icons[0].gameObject.activeSelf && !icons[1].gameObject.activeSelf, "Replaced dialogue releases obsolete icons");

    return new { passed = checks.Count, checks };
}
finally { UnityEngine.Object.DestroyImmediate(root); }
