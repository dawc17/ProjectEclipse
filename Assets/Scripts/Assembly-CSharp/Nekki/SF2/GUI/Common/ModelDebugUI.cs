using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Common
{
	public class ModelDebugUI : UIModule
	{
		[SerializeField]
		private Text _PlayerAnimName;

		[SerializeField]
		private Text _EnemyAnimName;

		private StringBuilder KCMLKPKALHF;

		private StringBuilder DKOMADOJIJG;

		protected override void Init()
		{
			base.Init();
			if (SceneManagerSF.EKFBDMBCDMB() != ScreenType.ModuleDojo && SceneManagerSF.EKFBDMBCDMB() != ScreenType.ModuleFight)
			{
				base.gameObject.SetActive(false);
			}
			KCMLKPKALHF = new StringBuilder();
			DKOMADOJIJG = new StringBuilder();
		}

		private void Update()
		{
			global::Fight gDBOMJODDEA = global::Fight.OHNKFOHIAKG();
			if (gDBOMJODDEA == null)
			{
				return;
			}
			KCMLKPKALHF.Clear();
			DKOMADOJIJG.Clear();
			List<Model> lNDLFINJHDB = gDBOMJODDEA.LNDLFINJHDB;
			for (int i = 0; i < lNDLFINJHDB.Count; i++)
			{
				Model fGCODGKLHED = lNDLFINJHDB[i];
				StringBuilder stringBuilder = ((!fGCODGKLHED.EPCNJLEHJCB()) ? DKOMADOJIJG : KCMLKPKALHF);
				if (fGCODGKLHED.KMMJCHDKBDO.HBFMBOHLKPJ != null && fGCODGKLHED.KMMJCHDKBDO.HBFMBOHLKPJ.get_Type() == Tactic.GKJKJFJALCA.TacticTabular)
				{
					stringBuilder.Append(AiData.GetTacticsTableName(fGCODGKLHED.EEIGOJBKFGE().get_ResultSource()));
					stringBuilder.Append("\n");
				}
			}
			for (int j = 0; j < lNDLFINJHDB.Count; j++)
			{
				Model fGCODGKLHED2 = lNDLFINJHDB[j];
				StringBuilder stringBuilder2 = ((!fGCODGKLHED2.EPCNJLEHJCB()) ? DKOMADOJIJG : KCMLKPKALHF);
				stringBuilder2.Append(CKAAKEHFAML(fGCODGKLHED2));
			}
			string text = KCMLKPKALHF.ToString();
			string text2 = DKOMADOJIJG.ToString();
			if (_PlayerAnimName.text != text)
			{
				_PlayerAnimName.text = text;
			}
			if (_EnemyAnimName.text != text2)
			{
				_EnemyAnimName.text = text2;
			}
		}

		private static string CKAAKEHFAML(Model ACENLMONNPA)
		{
			StringBuilder stringBuilder = new StringBuilder();
			InfoAnimation pJAHIOELGGD = ACENLMONNPA.FHBLLPCEAHG();
			int num = -1;
			if (ACENLMONNPA.NLHFJIEHKMM())
			{
				num = ACENLMONNPA.COBOFMDFLJO().PGOFHCBPLOE();
				List<string> list = ACENLMONNPA.KGHDFCKGAEO();
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					stringBuilder.Append(list[i]);
					if (i < count - 1)
					{
						stringBuilder.Append(" | ");
					}
				}
			}
			else
			{
				stringBuilder.Append((pJAHIOELGGD == null) ? "----" : pJAHIOELGGD.Name);
				num = ((pJAHIOELGGD == null) ? (-1) : ACENLMONNPA.OCPMJKIEPIG().LPFPGDJALED());
			}
			stringBuilder.Append("    ");
			if (num > -1)
			{
				stringBuilder.Append(num);
			}
			return stringBuilder.ToString();
		}
	}
}
