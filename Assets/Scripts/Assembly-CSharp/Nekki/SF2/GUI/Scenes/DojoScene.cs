using Nekki.SF2.Core.Fights.Controller;
using Nekki.SF2.GUI.Common;
using Nekki.SF2.GUI.Menu;
using UnityEngine;

namespace Nekki.SF2.GUI.Scenes
{
	public class DojoScene : Scene<DojoScene>
	{
		[SerializeField]
		private MainMenu _mainMenu;

		[SerializeField]
		public GameController gameController;

		private PaymentUI ODCDHJGNPEM;

		public global::Fight fight;

		private bool OGKFKJFGOIE = true;

		public PaymentUI GKPMFKIEPPB
		{
			get
			{
				return get_PaymentUI();
			}
		}

		public override ScreenType PNAJHDBDDLP
		{
			get
			{
				return get_SceneId();
			}
		}

		public PaymentUI get_PaymentUI()
		{
			return ODCDHJGNPEM;
		}

		public override ScreenType get_SceneId()
		{
			return ScreenType.ModuleDojo;
		}

		protected override void Init(object data)
		{
			base.Init(data);
			ODCDHJGNPEM = IMDHIBMOAIG<PaymentUI>();
			_mainMenu.Init();
			FightList jDIPBIHBGPF = ListSF.MGABNFOMDGB().NIAMMNJLEFI(BattleType.FightNone)[0].OAJCBGAKHJJ(0);
			RosterFight pIGKOIFBOME = ListSF.CCDKHLAMKKO().DBMHOBPNIIA(jDIPBIHBGPF.BCKFACGMOKC);
			if (pIGKOIFBOME == null)
			{
				pIGKOIFBOME = ListSF.CCDKHLAMKKO().OBAFPDGJHNN(jDIPBIHBGPF.BCKFACGMOKC);
			}
			jDIPBIHBGPF.HOCFLEMFFKC(pIGKOIFBOME);
			fight = GameUtils.ABAIHGFPHMO(jDIPBIHBGPF, null, gameController);
		}

		protected override void PJNFHNFLNNO()
		{
			base.PJNFHNFLNNO();
			if (fight != null)
			{
				fight.ANIDBLANMIC();
			}
		}

		public override void UpdateScene(object data)
		{
		}

		private void FixedUpdate()
		{
			if ((OGKFKJFGOIE || Input.GetKeyDown(KeyCode.Equals)) && fight != null)
			{
				fight.Draw();
			}
			if (Input.GetKeyDown(KeyCode.Minus))
			{
				OGKFKJFGOIE = !OGKFKJFGOIE;
			}
		}
	}
}
