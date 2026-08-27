using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class PEILCKGHHDP : ADEKACKLIJG
{
	public PEILCKGHHDP(ProductDefinition[] OCMDJBDPLJK, Dictionary<string, object> PCJAKPJMKGN)
		: base(PCJAKPJMKGN)
	{
		Debug.Log("[Store_Android] Init");
		Debug.Log("[Store_Android] Purchasing is disabled in this build.");
	}

	public override void JDMELMJCKMN()
	{
		GMKLFLAKKOJ();
	}

	protected override PurchaseProcessingResult GPNFELCBNPO(PurchaseEventArgs CLMPIFFOHMG)
	{
		Debug.LogFormat("[Store_Android] PurchaseSuccessEvent: {0}|{1}", CLMPIFFOHMG.purchasedProduct.definition.id, CLMPIFFOHMG.purchasedProduct.transactionID);
		FIKEKJBAKBO fIKEKJBAKBO = CLMPIFFOHMG.purchasedProduct.HMBLNIANJDN();
		if (fIKEKJBAKBO == null)
		{
			Debug.Log("UnityReceipt == null!");
			CJKHKGPHEOJ(CLMPIFFOHMG.purchasedProduct, PurchaseFailureReason.SignatureInvalid);
			return PurchaseProcessingResult.Complete;
		}
		if (ICFMIHIKGOD.JBLCINPOOEM(fIKEKJBAKBO.JICEOHCLPJP()) || ICFMIHIKGOD.MNONELGNFNM(fIKEKJBAKBO.JICEOHCLPJP()))
		{
			Debug.Log("Try to purchase already pending/purchased transaction!");
			CJKHKGPHEOJ(CLMPIFFOHMG.purchasedProduct, PurchaseFailureReason.ExistingPurchasePending);
			return PurchaseProcessingResult.Complete;
		}
		if (fIKEKJBAKBO.ILOONNDHLLI().type == ProductType.NonConsumable)
		{
			if (ICFMIHIKGOD.JBLCINPOOEM(fIKEKJBAKBO.JLDEALIEEJI()))
			{
				Debug.Log("Try to purchase already pending nonconsumable product!");
				CJKHKGPHEOJ(CLMPIFFOHMG.purchasedProduct, PurchaseFailureReason.ExistingPurchasePending);
				return PurchaseProcessingResult.Complete;
			}
			if (ICFMIHIKGOD.MNONELGNFNM(fIKEKJBAKBO.JLDEALIEEJI()))
			{
				Debug.Log("Try to purchase already purchased nonconsumable product!");
				CJKHKGPHEOJ(CLMPIFFOHMG.purchasedProduct, PurchaseFailureReason.DuplicateTransaction);
				return PurchaseProcessingResult.Complete;
			}
		}
		KMAENHJICNF.DCNPKBNNKHD(fIKEKJBAKBO);
		return PurchaseProcessingResult.Pending;
	}
}
