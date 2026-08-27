using System;
using UnityEngine;

internal class DemoHubSample : MonoBehaviour
{
	private readonly Uri URI = new Uri("http://besthttpsignalr.azurewebsites.net/signalr");

	private Connection FJGOJHMELAH;

	private DemoHub LHDMOALIFHL;

	private TypedDemoHub BHEJFCJCNJO;

	private Hub NEEINNIJJBL;

	private string vbReadStateResult = string.Empty;

	private Vector2 scrollPos;

	private void Start()
	{
		LHDMOALIFHL = new DemoHub();
		BHEJFCJCNJO = new TypedDemoHub();
		NEEINNIJJBL = new Hub("vbdemo");
		FJGOJHMELAH = new Connection(URI, LHDMOALIFHL, BHEJFCJCNJO, NEEINNIJJBL);
		FJGOJHMELAH.LPEPILDNMNE(new PEELJCOAGOH());
		FJGOJHMELAH.FJBEHFPIAHI((Connection MDGFGCDPGFI) =>
		{
			var anon = new
			{
				MENAJEAJJBE = "Foo",
				IDGJLEBFHOK = 20,
				IJMOLOMMEBG = new
				{
					IKGHGDEDKPC = "One Microsoft Way",
					DDIDIMMDPDN = "98052"
				}
			};
			LHDMOALIFHL.ReportProgress("Long running job!");
			LHDMOALIFHL.MDGBAIHKMPE();
			LHDMOALIFHL.GetValue();
			LHDMOALIFHL.GEEDKDFKMOI();
			LHDMOALIFHL.MBDPOHEEJGO();
			LHDMOALIFHL.FBDHPAIGJBG();
			LHDMOALIFHL.LACEDJJCILL();
			LHDMOALIFHL.ILAAHBOFIIM(anon);
			LHDMOALIFHL.SimpleArray(new int[3] { 5, 5, 6 });
			LHDMOALIFHL.MJLIGOHOKLH(anon);
			LHDMOALIFHL.ComplexArray(new object[3] { anon, anon, anon });
			LHDMOALIFHL.Overload();
			LHDMOALIFHL.FLBBFDNHJAJ()["name"] = "Testing state!";
			LHDMOALIFHL.LLBPNJCDCBB();
			LHDMOALIFHL.IHIBHBFIMCJ();
			LHDMOALIFHL.PJKALCNNLBD();
			BHEJFCJCNJO.OBHMNCMCEIO("Typed echo callback");
			NEEINNIJJBL.Call("readStateValue", (Hub CGFIJCNNCKP, ClientMessage CKEHOEGLMBM, ResultMessage DCJLKCFKCOM) =>
			{
				vbReadStateResult = string.Format("Read some state from VB.NET! => {0}", (DCJLKCFKCOM.LBAIENGDLDJ() != null) ? DCJLKCFKCOM.LBAIENGDLDJ().ToString() : "undefined");
			});
		});
		FJGOJHMELAH.LAJCMNNNIIM();
	}

	private void OnDestroy()
	{
		FJGOJHMELAH.Close();
	}

	private void OnGUI()
	{
		GUIHelper.ECMOBPFHNPN(GUIHelper.ClientArea, true, () =>
		{
			scrollPos = GUILayout.BeginScrollView(scrollPos, false, false);
			GUILayout.BeginVertical();
			LHDMOALIFHL.MCAIPGEPMDE();
			BHEJFCJCNJO.MCAIPGEPMDE();
			GUILayout.Label("Read State Value");
			GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			GUILayout.Label(vbReadStateResult);
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		});
	}
}
