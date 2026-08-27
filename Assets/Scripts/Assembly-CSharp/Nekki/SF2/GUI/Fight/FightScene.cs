using System;
using Nekki.SF2.Core.Fights.Controller;
using UnityEngine;

namespace Nekki.SF2.GUI.Fight
{
	public class FightScene : Scene<FightScene>
	{
		[SerializeField]
		private PreFight preFight;

		[SerializeField]
		public GameController gameController;

		public global::Fight Fight;

		private bool CEKGNACMGDB = true;

		public override ScreenType PNAJHDBDDLP
		{
			get
			{
				return get_SceneId();
			}
		}

		public override ScreenType get_SceneId()
		{
			return ScreenType.ModuleFight;
		}

		protected override void Init(object data)
		{
			base.Init(data);
			FightList jGMLAFOPBBC = (FightList)data;
			if (preFight == null)
			{
				LLLOJBFMONN.Error("FightHolder.Start preFight is null");
			}
			Fight = GameUtils.ABAIHGFPHMO(jGMLAFOPBBC, preFight, gameController);
			GC.Collect();
		}

		private void FixedUpdate()
		{
			if (Fight != null)
			{
				Fight.Draw();
			}
		}

		protected override void PJNFHNFLNNO()
		{
			base.PJNFHNFLNNO();
			if (Fight != null)
			{
				Fight.ANIDBLANMIC();
			}
		}

		public void RandomizeObscuredVars()
		{
			if (Fight != null)
			{
				Fight.RandomizeObscuredVars();
			}
		}
	}
}
