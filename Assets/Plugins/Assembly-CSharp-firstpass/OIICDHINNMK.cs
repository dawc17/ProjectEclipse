using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class OIICDHINNMK : ADEKACKLIJG
{
	private static float NAJBMEJHEAF = 2f;

	public override bool KIOGKAAIADJ
	{
		get
		{
			return LCFBJGONPBH();
		}
	}

	public OIICDHINNMK(ProductDefinition[] OCMDJBDPLJK, Dictionary<string, object> PCJAKPJMKGN)
		: base(PCJAKPJMKGN)
	{
		Debug.Log("[Store_Emulator] Init");
	}

	public override bool LCFBJGONPBH()
	{
		return true;
	}

	public override void BKFGAIHBCHL(params string[] PBJNMKIOCBD)
	{
		if (NAJBMEJHEAF > 0f)
		{
			CoroutineManager.get_Current().StartRoutine(DIKGCJEOPEK(base.ECLPBDKBGJL));
		}
		else
		{
			ECLPBDKBGJL();
		}
	}

	public override void BDAAKHOLPOF(string FDKNIPNGFNF)
	{
		Debug.LogFormat("[Store] MakePurchase: {0}", FDKNIPNGFNF);
		PGICGKMBACN(FDKNIPNGFNF);
		if (NAJBMEJHEAF > 0f)
		{
			CoroutineManager.get_Current().StartRoutine(DIKGCJEOPEK(() =>
			{
				GPNFELCBNPO(FDKNIPNGFNF);
			}));
		}
		else
		{
			GPNFELCBNPO(FDKNIPNGFNF);
		}
	}

	public override void JDMELMJCKMN()
	{
		if (NAJBMEJHEAF > 0f)
		{
			CoroutineManager.get_Current().StartRoutine(DIKGCJEOPEK(LLCKDACHBIM));
		}
		else
		{
			LLCKDACHBIM();
		}
	}

	private void GPNFELCBNPO(string FDKNIPNGFNF)
	{
		Debug.Log("[Store_Emulator] PaymentSuccess: " + FDKNIPNGFNF);
		KMAENHJICNF.DCNPKBNNKHD(FDKNIPNGFNF, string.Empty, string.Empty, null);
	}

	private void LLCKDACHBIM()
	{
		Debug.Log("[Store_Emulator] RestoreTransactionsEvent");
		GMKLFLAKKOJ();
	}

	private IEnumerator DIKGCJEOPEK(Action AKEMCGIHDDM)
	{
		yield return new WaitForSeconds(NAJBMEJHEAF);
		AKEMCGIHDDM();
	}
}
