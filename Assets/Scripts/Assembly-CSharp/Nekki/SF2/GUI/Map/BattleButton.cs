using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Map
{
	public class BattleButton : ResolutionButton
	{
		[SerializeField]
		private string _battleName = string.Empty;

		private Battle LDHBJAHPENM;

		public bool Locked;

		private bool INMFGOMPJEO;

		[SerializeField]
		private LabelAlias _lblName;

		private float _CurrentAlpha = 1f;

		private Tween _tween;

		public Battle EDHMHFONDAI
		{
			get
			{
				return get_Battle();
			}
			set
			{
				set_Battle(value);
			}
		}

		public bool GDCBBAHKCIE
		{
			get
			{
				return get_Hidden();
			}
			set
			{
				set_Hidden(value);
			}
		}

		public Battle get_Battle()
		{
			return LDHBJAHPENM;
		}

		public void set_Battle(Battle value)
		{
			LDHBJAHPENM = value;
			_battleName = LDHBJAHPENM.get_Name();
		}

		public bool get_Hidden()
		{
			return INMFGOMPJEO;
		}

		public void set_Hidden(bool value)
		{
			INMFGOMPJEO = value;
			base.gameObject.SetActive(get_Battle().DCHJDPCEODD && !INMFGOMPJEO);
		}

		public void Init(string LPCAHLHLBJE, string KEIJPCJFLEO, string HHBECAKNFHD, string NGHGFJCOMIP, bool NIBIMBDBPMI, string iconAtlas = "")
		{
			// Zones 1-6 still tag their original Tournament/Lynx map entries with
			// the legacy BattleBtnStart atlas. The modern art bundle no longer ships
			// that atlas and uses BattleBtnBase instead, but those old map slots were
			// authored for a 150x150 icon footprint rather than the normal 300x300
			// battle button. Keep the modern bundle art while preserving that layout
			// contract. The label is a child with its own size, so only the image/button
			// rect shrinks and the map text remains at the intended scale.
			if (iconAtlas == "BattleBtnStart")
			{
				GetComponent<RectTransform>().sizeDelta = new Vector2(150f, 150f);
				iconAtlas = string.Empty;
			}
			bool raidDefault = iconAtlas == "BattleBtn_raid";
			string baseAtlas = string.IsNullOrEmpty(iconAtlas) ? "BattleBtnBase" :
				(raidDefault ? "BattleBtnBase_raid" : iconAtlas + "Base");
			string activeAtlas = string.IsNullOrEmpty(iconAtlas) ? "BattleBtnActive" :
				(raidDefault ? "BattleBtnActive_raid" : iconAtlas + "Active");
			string lockAtlas = string.IsNullOrEmpty(iconAtlas) ? "BattleBtnLock" :
				(raidDefault ? "BattleBtnLock_raid" : iconAtlas + "Lock");
			string lockActiveAtlas = string.IsNullOrEmpty(iconAtlas) ? "BattleBtnLockActive" :
				(raidDefault ? "BattleBtnLockActive_raid" : iconAtlas + "LockActive");
			// Some raid pages intentionally reuse classic icons. Prefer the raid or
			// event atlas, but gracefully retain those classic entries.
			if (ResolutionImage.GetSprite("UI/Atlases/", baseAtlas + "." + LPCAHLHLBJE) == null)
			{
				baseAtlas = "BattleBtnBase";
			}
			if (ResolutionImage.GetSprite("UI/Atlases/", activeAtlas + "." + KEIJPCJFLEO) == null)
			{
				activeAtlas = "BattleBtnActive";
			}
			if (ResolutionImage.GetSprite("UI/Atlases/", lockAtlas + "." + HHBECAKNFHD) == null)
			{
				lockAtlas = "BattleBtnLock";
			}
			if (ResolutionImage.GetSprite("UI/Atlases/", lockActiveAtlas + "." + NGHGFJCOMIP) == null)
			{
				lockActiveAtlas = "BattleBtnLockActive";
			}
			if (!NIBIMBDBPMI)
			{
				SetNormalSprite("UI/Atlases/", baseAtlas + "." + LPCAHLHLBJE);
				SetDisabledSprite("UI/Atlases/", activeAtlas + "." + KEIJPCJFLEO);
			}
			else
			{
				SetNormalSprite("UI/Atlases/", lockAtlas + "." + HHBECAKNFHD);
				SetDisabledSprite("UI/Atlases/", lockActiveAtlas + "." + NGHGFJCOMIP);
			}
		}

		public void CorrectLabel()
		{
		}

		public void SetText(string JMLFOFKBGPE)
		{
			_lblName.set_text(JMLFOFKBGPE);
		}

		public void SetAlias(string HCCJDBGBCNC)
		{
			_lblName.SetAlias(HCCJDBGBCNC);
		}

		public void SetActiveBattle(bool isActive)
		{
			base.interactable = !isActive;
			base.transition = (base.interactable ? Transition.ColorTint : Transition.SpriteSwap);
		}

		public void SetAlpha(float PGFIPOJBNFC, float time = 0f)
		{
			KillTween();
			if (!base.gameObject.activeSelf || time <= 0f)
			{
				COAKLBIEPLH(PGFIPOJBNFC);
				return;
			}
			_tween = DOTween.To(() => _CurrentAlpha, (float DHDMNHCIPEH) =>
			{
				COAKLBIEPLH(DHDMNHCIPEH);
			}, PGFIPOJBNFC, time);
		}

		private void COAKLBIEPLH(float PGFIPOJBNFC)
		{
			if (_CurrentAlpha != PGFIPOJBNFC)
			{
				_CurrentAlpha = Mathf.Clamp(PGFIPOJBNFC, 0f, 1f);
				GetComponent<CanvasGroup>().alpha = _CurrentAlpha;
			}
		}

		private void KillTween()
		{
			if (_tween != null)
			{
				_tween.Kill();
				_tween = null;
			}
		}
	}
}
