using System.Collections.Generic;

public sealed class WWWAuthenticateHeaderParser : KeyValuePairList
{
	public WWWAuthenticateHeaderParser(string PNJNBBFLCAH)
	{
		CAAHOEMAAAL(FFCAJBGFMPD(PNJNBBFLCAH));
	}

	private List<KeyValuePair> FFCAJBGFMPD(string IGGFGLLIGCG)
	{
		List<KeyValuePair> list = new List<KeyValuePair>();
		if (IGGFGLLIGCG != null)
		{
			int LCCLEFMKLPB = 0;
			string kGBGENDIMBC = IGGFGLLIGCG.Read(ref LCCLEFMKLPB, (char KDFCGMMKAME) => !char.IsWhiteSpace(KDFCGMMKAME) && !char.IsControl(KDFCGMMKAME)).JONPEPOKJFC();
			list.Add(new KeyValuePair(kGBGENDIMBC));
			while (LCCLEFMKLPB < IGGFGLLIGCG.Length)
			{
				string kGBGENDIMBC2 = IGGFGLLIGCG.Read(ref LCCLEFMKLPB, '=').JONPEPOKJFC();
				KeyValuePair gGCJLGPPHKP = new KeyValuePair(kGBGENDIMBC2);
				IGGFGLLIGCG.SkipWhiteSpace(ref LCCLEFMKLPB);
				gGCJLGPPHKP.set_Value(IGGFGLLIGCG.ReadQuotedText(ref LCCLEFMKLPB));
				list.Add(gGCJLGPPHKP);
			}
		}
		return list;
	}
}
