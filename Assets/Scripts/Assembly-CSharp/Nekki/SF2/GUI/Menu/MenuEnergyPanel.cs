using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Menu
{
	public class MenuEnergyPanel : SFMonoBehaviour<object>
	{
		public enum PGDGDNCJEIC
		{
			onBarClicked = 0
		}

		private enum COILLEPJHCP
		{
			InvisibleView = 0,
			NormalView = 1,
			UnlimitedView = 2
		}

		private COILLEPJHCP EDEPKCAONJK;

		[SerializeField]
		private Image _icon;

		[SerializeField]
		private Image _iconMax;

		[SerializeField]
		private StepBar _bar;

		[SerializeField]
		private Button _dialogButton;

		public void Init()
		{
			int num = GameUtils.NAMEDMHAFKA();
			float num2 = 100f / (float)num;
			List<int> list = new List<int>();
			for (int i = 0; i <= num; i++)
			{
				list.Add((int)(num2 * (float)i));
			}
			_bar.Init();
			_bar.SetPercent(list);
			if (ListSF.CCDKHLAMKKO().ADKHNLAMDJP)
			{
				_bar.gameObject.SetActive(false);
			}
			_dialogButton.onClick.AddListener(() =>
			{
				HCADFMMDHOF();
			});
			UpdateView();
		}

		private void CHILAIJNEHG()
		{
			_dialogButton.onClick.RemoveListener(() =>
			{
				HCADFMMDHOF();
			});
		}

		public void UpdateView()
		{
			if (!ListSF.CCDKHLAMKKO().ADKHNLAMDJP)
			{
				_icon.gameObject.SetActive(true);
				_bar.gameObject.SetActive(true);
				_iconMax.gameObject.SetActive(false);
				UpdateBar();
			}
			else
			{
				_icon.gameObject.SetActive(false);
				_bar.gameObject.SetActive(false);
				_iconMax.gameObject.SetActive(true);
			}
		}

		public void UpdateBar()
		{
			int num = ListSF.CCDKHLAMKKO().NHKMGNPADKI();
			if (_bar.GetValue() != (float)num)
			{
				_bar.SetValue(num);
			}
		}

		public void SetDialogBtnPressType(NFOGOFFAPPP.HHGPKAJENGF LFLGCDNKNJI, bool GHJGPAEDIHG)
		{
			_dialogButton.OFPNNIBBNCE(LFLGCDNKNJI, GHJGPAEDIHG);
		}

		public virtual void SetTouchEnabled(bool value)
		{
			_dialogButton.gameObject.SetActive(value);
		}

		private void HCADFMMDHOF()
		{
			CallEvent(0, 0);
		}
	}
}
