using System.Collections;
using Eclipse.Modding;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Nekki.SF2.GUI.Shop
{
	public class HintPanel : MonoBehaviour
	{
		[SerializeField]
		private float hintBoxWidth = 650f;

		[SerializeField]
		private float hintBoxHeight = 200f;

		[SerializeField]
		private bool showingHint;

		[SerializeField]
		private float timeToHide = 5f;

		[SerializeField]
		private GameObject hintBoxPrefab;

		private GameObject DBEKMNDHBCG;

		private IEnumerator OPCLGOHELNO;

		private HintBox FLAPNMIDCAM;

		private GameObject listAnchor;

		public void Init()
		{
			if (hintBoxPrefab != null)
			{
				GameObject gameObject = Object.Instantiate(hintBoxPrefab);
				FLAPNMIDCAM = gameObject.GetComponent<HintBox>();
				RectTransform rectTransform = FLAPNMIDCAM.transform as RectTransform;
				gameObject.transform.SetParent(base.transform, false);
				if (FLAPNMIDCAM != null && rectTransform != null)
				{
					rectTransform.sizeDelta = new Vector2(hintBoxWidth, hintBoxHeight);
					FLAPNMIDCAM.Init();
				}
				gameObject.SetActive(false);
				showingHint = false;
			}
		}

		public void ShowPerkHint(PerkInfoItem AEFFHJGMNFI, Vector2 MGMMDGFPBLP, Vector2 IPCOBJBKNAO, GameObject AOMLCBHAJJH)
		{
			if (AEFFHJGMNFI == null || false || FLAPNMIDCAM == null)
			{
				return;
			}
			if (DBEKMNDHBCG == AOMLCBHAJJH)
			{
				HideHintAndStopCorutine();
				return;
			}
			if (DBEKMNDHBCG != null && DBEKMNDHBCG != AOMLCBHAJJH && showingHint)
			{
				HideHintAndStopCorutine();
			}
				DBEKMNDHBCG = AOMLCBHAJJH;
				FLAPNMIDCAM.gameObject.SetActive(true);
				string title = AEFFHJGMNFI.HBCNKNFPAIM;
				string description = AEFFHJGMNFI.PDLPHLNCOMJ(AEFFHJGMNFI.MGNNJPBCOGD);
				// Public mod presentation belongs to the Eclipse definition, not to the
				// recovered PerkInfoItem compatibility projection. Saved/cloned enchantment
				// instances can carry legacy presentation metadata, so prefer the canonical
				// registry keys for qualified external perks/enchantments.
				string modTitle;
				string modDescription;
				if (ModRuntime.TryGetExternalEffectPresentation(AEFFHJGMNFI.Name, out modTitle, out modDescription))
				{
					title = modTitle;
					description = modDescription;
				}
				FLAPNMIDCAM.SetText(title, description);
			if (AnchorToIcon(AOMLCBHAJJH))
			{
				showingHint = true;
				OPCLGOHELNO = WaitAndHideHint();
				StartCoroutine(OPCLGOHELNO);
				return;
			}
			bool flag = false;
			RectTransform component = base.transform.root.GetComponent<RectTransform>();
			if (component != null)
			{
				Vector2 vector = new Vector2(0f, (0f - component.sizeDelta.y) * 0.5f);
				Vector2 vector2 = MGMMDGFPBLP + IPCOBJBKNAO;
				vector2 = base.transform.InverseTransformPoint(vector2);
				flag = Mathf.Abs((vector - vector2).y) < FLAPNMIDCAM.get_RectTransform().sizeDelta.y;
			}
			if (flag)
			{
				FLAPNMIDCAM.transform.position = MGMMDGFPBLP - IPCOBJBKNAO;
				FLAPNMIDCAM.Flip();
			}
			else
			{
				FLAPNMIDCAM.transform.position = MGMMDGFPBLP + IPCOBJBKNAO;
				FLAPNMIDCAM.ResetFlip();
			}
			showingHint = true;
			OPCLGOHELNO = WaitAndHideHint();
			StartCoroutine(OPCLGOHELNO);
		}

		public void ShowListHint(string titleAlias, string content, GameObject source, GameObject anchor)
		{
			if (FLAPNMIDCAM == null || source == null || anchor == null) return;
			bool toggleOff = showingHint && DBEKMNDHBCG == source;
			HideHintAndStopCorutine();
			if (toggleOff) return;
			DBEKMNDHBCG = source;
			listAnchor = anchor;
			FLAPNMIDCAM.ResetFlip();
			FLAPNMIDCAM.gameObject.SetActive(true);
			FLAPNMIDCAM.SetListContent(titleAlias, content);
			AnchorToIcon(anchor);
			showingHint = true;
			OPCLGOHELNO = WaitAndHideHint();
			StartCoroutine(OPCLGOHELNO);
		}

		// Called after the forge drawer docks during canvas layout.
		public void UpdateListHintPosition()
		{
			if (!showingHint || listAnchor == null) return;
			if (!listAnchor.activeInHierarchy) HideHintAndStopCorutine();
			else AnchorToIcon(listAnchor);
		}

		// Eclipse: the recovered placement added a fixed (0,-25) world-space offset to the icon
		// centre, which on the scaled canvas left the hint box covering the icon. Hang the box's
		// arrow off the icon's own edge instead: below it, or above it when there is no room,
		// kept inside the screen with the arrow still pointing at the icon.
		private const float ArrowReach = 12f;

		private bool AnchorToIcon(GameObject icon)
		{
			RectTransform iconRect = (icon != null) ? (icon.transform as RectTransform) : null;
			RectTransform root = base.transform.root as RectTransform;
			RectTransform box = FLAPNMIDCAM.get_RectTransform();
			if (iconRect == null || root == null || box == null)
			{
				return false;
			}
			Rect iconBounds = LocalBounds(iconRect);
			Rect screen = LocalBounds(root);
			Vector2 size = box.rect.size;
			float below = iconBounds.yMin - ArrowReach;
			bool flip = below - size.y < screen.yMin && iconBounds.yMax + ArrowReach + size.y <= screen.yMax;
			float y = flip ? (iconBounds.yMax + ArrowReach) : below;
			float x = iconBounds.center.x;
			float half = size.x * 0.5f;
			if (screen.width > size.x)
			{
				x = Mathf.Clamp(x, screen.xMin + half, screen.xMax - half);
			}
			if (flip)
			{
				FLAPNMIDCAM.Flip();
			}
			else
			{
				FLAPNMIDCAM.ResetFlip();
			}
			box.localPosition = new Vector3(x, y, box.localPosition.z);
			float arrowShift = iconBounds.center.x - x;
			FLAPNMIDCAM.SetArrowOffset(flip ? -arrowShift : arrowShift, half - 60f);
			return true;
		}

		private Rect LocalBounds(RectTransform target)
		{
			Vector3[] corners = new Vector3[4];
			target.GetWorldCorners(corners);
			Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
			Vector2 max = new Vector2(float.MinValue, float.MinValue);
			for (int i = 0; i < 4; i++)
			{
				Vector2 local = base.transform.InverseTransformPoint(corners[i]);
				min = Vector2.Min(min, local);
				max = Vector2.Max(max, local);
			}
			return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
		}

		public void HideHintAndStopCorutine()
		{
			if (OPCLGOHELNO != null)
			{
				StopCoroutine(OPCLGOHELNO);
			}
			HideHint();
		}

		public void HideHint()
		{
			showingHint = false;
			if (FLAPNMIDCAM != null) FLAPNMIDCAM.gameObject.SetActive(false);
			listAnchor = null;
			DBEKMNDHBCG = null;
			OPCLGOHELNO = null;
		}

		public IEnumerator WaitAndHideHint()
		{
			yield return new WaitForSeconds(timeToHide);
			HideHint();
		}

		public void Update()
		{
			if (showingHint && (Input.touchCount > 0 || Input.anyKeyDown) && (EventSystem.current == null || DBEKMNDHBCG != EventSystem.current.currentSelectedGameObject))
			{
				HideHintAndStopCorutine();
			}
		}
	}
}
