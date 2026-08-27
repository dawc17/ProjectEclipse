using System.Collections;
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
			string lNGIMAAHIFE = AEFFHJGMNFI.PDLPHLNCOMJ(AEFFHJGMNFI.MGNNJPBCOGD);
			FLAPNMIDCAM.SetText(AEFFHJGMNFI.HBCNKNFPAIM, lNGIMAAHIFE);
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
			FLAPNMIDCAM.gameObject.SetActive(false);
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
			if (showingHint && (Input.touchCount > 0 || Input.anyKeyDown) && DBEKMNDHBCG != EventSystem.current.currentSelectedGameObject)
			{
				HideHintAndStopCorutine();
			}
		}
	}
}
