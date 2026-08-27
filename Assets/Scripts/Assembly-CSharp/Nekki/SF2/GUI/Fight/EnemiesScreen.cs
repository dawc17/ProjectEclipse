using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Fight
{
	public class EnemiesScreen : MonoBehaviour
	{
		[SerializeField]
		private Vector2 currentEnemyScale;

		[SerializeField]
		private Vector2 bossScale;

		[SerializeField]
		private Vector2 bossSize;

		[SerializeField]
		private float enemyAlpha;

		[SerializeField]
		private float timeEnemyPauseStart;

		[SerializeField]
		private float timeEnemyMove;

		[SerializeField]
		private float timeEnemyPause;

		[SerializeField]
		private float timeEnemyShade;

		[SerializeField]
		private float timeEnemyPauseFinish;

		[SerializeField]
		private GameObject displayModelPrefab;

		[SerializeField]
		private RectTransform enemiesPanel;

		[SerializeField]
		private HorizontalLayoutGroup layoutGroup;

		private float GLAMMHFCJPN;

		private List<DisplayModel> enemiesModels = new List<DisplayModel>();

		public float ADMLKNCMFLG
		{
			get
			{
				return get_AnimationTime();
			}
		}

		public float get_AnimationTime()
		{
			return GLAMMHFCJPN;
		}

		public void Init(List<ModelParameters> IDAAONBIBJM, int index, bool PPIJJHJCGGB)
		{
			if (displayModelPrefab == null)
			{
				LLLOJBFMONN.Error("EnemiesScreen.Init displayModelPrefab is null");
				return;
			}
			if (enemiesPanel == null)
			{
				LLLOJBFMONN.Error("EnemiesScreen.Init enemiesPanel is null");
				return;
			}
			if (layoutGroup == null)
			{
				LLLOJBFMONN.Error("EnemiesScreen.Init layoutGroup is null");
				return;
			}
			Vector2 sizeDelta = new Vector2(0f, 0f);
			int num = 0;
			foreach (ModelParameters item in IDAAONBIBJM)
			{
				DisplayModel component = Object.Instantiate(displayModelPrefab).GetComponent<DisplayModel>();
				component.transform.SetParent(enemiesPanel, false);
				component.SetAvatar(item.HNKFHGOOKEG);
				if (index > num)
				{
					component.Completed();
				}
				enemiesModels.Add(component);
				RectTransform rectTransform = component.transform as RectTransform;
				if (rectTransform != null)
				{
					sizeDelta.x += rectTransform.rect.width;
					if (sizeDelta.y < rectTransform.rect.height)
					{
						sizeDelta.y = rectTransform.rect.height;
					}
				}
				num++;
				if (IDAAONBIBJM.Count == num && PPIJJHJCGGB)
				{
					component.ScaleAvatar(bossScale);
					component.SetSizeDelta(bossSize);
				}
			}
			if (enemiesModels.Count > 1)
			{
				sizeDelta.x += layoutGroup.spacing * (float)(enemiesModels.Count - 1);
			}
			enemiesPanel.sizeDelta = sizeDelta;
			LayoutRebuilder.ForceRebuildLayoutImmediate(enemiesPanel);
			Vector2 vector = new Vector2(0f, 0f);
			if (enemiesModels.Count > index)
			{
				Vector3 position = enemiesModels[index].transform.position;
				Vector3 vector2 = base.transform.TransformPoint(new Vector2(0f, 0f));
				Vector3 vector3 = vector2 - position;
				vector = enemiesPanel.position + vector3;
			}
			DG.Tweening.Sequence s = DOTween.Sequence();
			s.AppendInterval(timeEnemyPauseStart);
			s.Append(enemiesPanel.DOMove(vector, timeEnemyMove));
			s.AppendInterval(timeEnemyPause);
			s.Append(enemiesModels[index].transform.DOScale(currentEnemyScale, timeEnemyShade));
			int num2 = 0;
			foreach (DisplayModel item2 in enemiesModels)
			{
				if (num2 != index)
				{
					s.Join(item2.get_Avatar().DOFade(enemyAlpha, timeEnemyShade));
					s.Join(item2.get_Complete().DOFade(enemyAlpha, timeEnemyShade));
				}
				num2++;
			}
			s.AppendInterval(timeEnemyPauseFinish);
			GLAMMHFCJPN = timeEnemyPauseStart + timeEnemyMove + timeEnemyPause + timeEnemyShade + timeEnemyPauseFinish;
		}
	}
}
