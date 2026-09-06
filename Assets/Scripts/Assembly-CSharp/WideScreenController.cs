using System.Collections.Generic;
using UnityEngine;

public class WideScreenController : MonoBehaviour
{
	[SerializeField]
	private GameObject _LeftBorder;

	[SerializeField]
	private GameObject _RightBorder;

	[SerializeField]
	private bool _Flag;

    private const float MaximumAspect = 21f / 9f;
    private readonly Dictionary<RectTransform, Vector2[]> _originalOffsets = new Dictionary<RectTransform, Vector2[]>();
    private Vector2 _lastParentSize;
    private Vector2Int _lastScreenSize;

    public void Run()
    {
        var parent = transform.parent as RectTransform;
        if (parent == null || parent.rect.height <= 0f) return;
        _lastParentSize = parent.rect.size;
        _lastScreenSize = new Vector2Int(Screen.width, Screen.height);
        float contentWidth = Mathf.Min(parent.rect.width, parent.rect.height * MaximumAspect);
        float inset = Mathf.Max(0f, (parent.rect.width - contentWidth) * .5f);
        bool capped = inset > .01f;
        _LeftBorder.SetActive(capped);
        _RightBorder.SetActive(capped);
        if (capped)
        {
            transform.SetAsLastSibling();
            BEDKFGIICFL(contentWidth);
        }
        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i) as RectTransform;
            if (child == null || child == transform) continue;
            Vector2[] offsets;
            if (!_originalOffsets.TryGetValue(child, out offsets))
            {
                offsets = new[] { child.offsetMin, child.offsetMax };
                _originalOffsets.Add(child, offsets);
            }
            // Restore the authored layout when returning below the cap; never accumulate insets.
            child.offsetMin = offsets[0] + new Vector2(inset, 0f);
            child.offsetMax = offsets[1] - new Vector2(inset, 0f);
        }
    }

	private void BEDKFGIICFL(float DJFFDCFCNJM)
	{
		float num = DJFFDCFCNJM / 2f;
		Vector3 localPosition = _LeftBorder.transform.localPosition;
		localPosition.x = 0f - num;
		_LeftBorder.transform.localPosition = localPosition;
		localPosition = _RightBorder.transform.localPosition;
		localPosition.x = num;
		_RightBorder.transform.localPosition = localPosition;
	}

    private void LateUpdate()
    {
        var parent = transform.parent as RectTransform;
        if (_Flag || _lastScreenSize != new Vector2Int(Screen.width, Screen.height)
            || (parent != null && parent.rect.size != _lastParentSize))
        {
            _Flag = false;
            Run();
        }
    }
}
