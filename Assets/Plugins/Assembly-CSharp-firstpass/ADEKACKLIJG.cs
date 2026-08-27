using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public abstract class ADEKACKLIJG : DOIKIDPKFKN, IStoreListener
{
	protected IStoreController FEHOHLMIEBP;

	protected IExtensionProvider OBANIIIEHDP;

	public virtual bool KIOGKAAIADJ
	{
		get
		{
			return LCFBJGONPBH();
		}
	}

	public Product[] LODOEPBHBGN
	{
		get
		{
			return NABJBCEKEHK();
		}
	}

	public Product[] LMLDPJFEPEL
	{
		get
		{
			return IBHEAHLJCKC();
		}
	}

	public Product[] PPPEOHKPMGG
	{
		get
		{
			return GIPILMHFHPN();
		}
	}

	public ADEKACKLIJG(Dictionary<string, object> PCJAKPJMKGN)
	{
	}

	public T MDMDFHPCOEI<T>() where T : ADEKACKLIJG
	{
		return this as T;
	}

	public bool CFEJGPGNOMM<T>() where T : ADEKACKLIJG
	{
		return this is T;
	}

	public virtual bool LCFBJGONPBH()
	{
		return FEHOHLMIEBP != null && OBANIIIEHDP != null;
	}

	public Product[] NABJBCEKEHK()
	{
		return (!LCFBJGONPBH()) ? null : FEHOHLMIEBP.products.all;
	}

	public Product[] IBHEAHLJCKC()
	{
		return MNPGMGILMEO((Product PANEMFIIOGB) => PANEMFIIOGB.definition.type == ProductType.Consumable);
	}

	public Product[] GIPILMHFHPN()
	{
		return MNPGMGILMEO((Product PANEMFIIOGB) => PANEMFIIOGB.definition.type != ProductType.Consumable);
	}

	public Product[] MNPGMGILMEO(Func<Product, bool> GDHANKIAAOP)
	{
		if (LCFBJGONPBH())
		{
			return null;
		}
		List<Product> list = new List<Product>();
		Product[] all = FEHOHLMIEBP.products.all;
		foreach (Product product in all)
		{
			if (GDHANKIAAOP(product))
			{
				list.Add(product);
			}
		}
		return list.ToArray();
	}

	public Product MGGAHKIPDKK(string HMDBGGEMICE)
	{
		return (!LCFBJGONPBH()) ? null : FEHOHLMIEBP.products.WithID(HMDBGGEMICE);
	}

	public virtual void BKFGAIHBCHL(params string[] PBJNMKIOCBD)
	{
		if (!LCFBJGONPBH())
		{
			EJLCKLCBBKM(InitializationFailureReason.PurchasingUnavailable);
			return;
		}
		HashSet<ProductDefinition> hashSet = new HashSet<ProductDefinition>();
		if (PBJNMKIOCBD == null || PBJNMKIOCBD.Length == 0)
		{
			foreach (Product item in FEHOHLMIEBP.products.set)
			{
				hashSet.Add(item.definition);
			}
		}
		else
		{
			foreach (string id in PBJNMKIOCBD)
			{
				Product product = FEHOHLMIEBP.products.WithID(id);
				if (product != null)
				{
					hashSet.Add(product.definition);
				}
			}
		}
		FEHOHLMIEBP.FetchAdditionalProducts(hashSet, ECLPBDKBGJL, EJLCKLCBBKM);
	}

	public virtual void BDAAKHOLPOF(string FDKNIPNGFNF)
	{
		if (!LCFBJGONPBH())
		{
			CJKHKGPHEOJ(FDKNIPNGFNF, PurchaseFailureReason.PurchasingUnavailable);
			return;
		}
		Debug.LogFormat("[Store] MakePurchase - {0}", FDKNIPNGFNF);
		PGICGKMBACN(FDKNIPNGFNF);
		FEHOHLMIEBP.InitiatePurchase(FDKNIPNGFNF);
	}

	public abstract void JDMELMJCKMN();

	public void PDEGOEPKGPK(string FDKNIPNGFNF)
	{
		Debug.LogFormat("[Store] ConfirmPendingPurchase - {0}", FDKNIPNGFNF);
		if (LCFBJGONPBH())
		{
			Product product = FEHOHLMIEBP.products.WithID(FDKNIPNGFNF);
			if (product == null)
			{
				Debug.LogErrorFormat("[Store] Product not found - {0}!", FDKNIPNGFNF);
			}
			else
			{
				FEHOHLMIEBP.ConfirmPendingPurchase(product);
			}
		}
	}

	public void NDDCENEGGEA(JLDHCFFAIPK PAENLDALDGB)
	{
		Debug.LogFormat("[Store] PaymentComplete: {0}", PAENLDALDGB);
		JEAJAJMDPNL(PAENLDALDGB.JLDEALIEEJI());
		JOFLHEEPJIB(PAENLDALDGB.JLDEALIEEJI(), PAENLDALDGB.KGNGCPEGMJP());
	}

	public void OnInitialized(IStoreController IMDFEPCMDHG, IExtensionProvider OCNLIHAMKNK)
	{
		Debug.Log("[Store] OnInitialized");
		FEHOHLMIEBP = IMDFEPCMDHG;
		OBANIIIEHDP = OCNLIHAMKNK;
		CJKMFPIJLDF(true);
	}

	public void OnInitializeFailed(InitializationFailureReason AJMILDEPOPO)
	{
		Debug.LogFormat("[Store] OnInitializeFailed - {0}!", AJMILDEPOPO);
		CJKMFPIJLDF(false);
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs CLMPIFFOHMG)
	{
		return GPNFELCBNPO(CLMPIFFOHMG);
	}

	public void OnPurchaseFailed(Product PANEMFIIOGB, PurchaseFailureReason ILDDNIBBANF)
	{
		CJKHKGPHEOJ(PANEMFIIOGB, ILDDNIBBANF);
	}

	protected virtual void CJKMFPIJLDF(bool NOGBHHLJECH)
	{
	}

	protected void ECLPBDKBGJL()
	{
		Debug.Log("[Store] RefreshProductsSuccess");
		CIIDFBBIICE();
	}

	protected void EJLCKLCBBKM(InitializationFailureReason ILDDNIBBANF)
	{
		Debug.Log("[Store] RefreshProductsFailed: " + ILDDNIBBANF);
		MNAMLEJHOFM(ILDDNIBBANF);
	}

	protected virtual PurchaseProcessingResult GPNFELCBNPO(PurchaseEventArgs CLMPIFFOHMG)
	{
		return PurchaseProcessingResult.Complete;
	}

	protected virtual void CJKHKGPHEOJ(Product ABDMPEAGHKG, PurchaseFailureReason DDDHNNADEEM)
	{
		CJKHKGPHEOJ(ABDMPEAGHKG.definition.id, DDDHNNADEEM);
	}

	private void CJKHKGPHEOJ(string FDKNIPNGFNF, PurchaseFailureReason DDDHNNADEEM)
	{
		Debug.LogFormat("[Store] PurchaseFailedEvent: {0}|{1}", FDKNIPNGFNF, DDDHNNADEEM);
		ENCIAJBEOEA(FDKNIPNGFNF, DDDHNNADEEM);
		JOFLHEEPJIB(FDKNIPNGFNF, string.Empty);
	}

	public void HBDJDPOHEDC(string FDKNIPNGFNF, string DNHKNDPBGNM)
	{
		AFBIACPALAJ(FDKNIPNGFNF, DNHKNDPBGNM);
	}

	public void BBBLEFEEMHG(string FDKNIPNGFNF, string DNHKNDPBGNM)
	{
		CFBBLIBNILI(FDKNIPNGFNF, DNHKNDPBGNM);
	}

	public void MOBENEMFFHG(string FDKNIPNGFNF, string DNHKNDPBGNM)
	{
		AFBIACPALAJ(FDKNIPNGFNF, DNHKNDPBGNM);
	}

	public void IIBFMAEJEOA(string FDKNIPNGFNF, string DNHKNDPBGNM)
	{
		CFBBLIBNILI(FDKNIPNGFNF, DNHKNDPBGNM);
	}

	public void KIKKOIHOLLN(string FDKNIPNGFNF)
	{
		DFOLPLOOOHK(FDKNIPNGFNF);
	}

	public void FLKCMDGDLJJ(string FDKNIPNGFNF)
	{
		OEIIAGKHMKN(FDKNIPNGFNF);
	}
}
