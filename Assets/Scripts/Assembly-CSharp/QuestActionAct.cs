using System.Collections.Generic;
using System.Xml;

public class QuestActionAct : QuestAction
{
	private string _text = string.Empty;

	private List<KeyValuePair<string, int>> GOJABLKPFJM = new List<KeyValuePair<string, int>>();

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		_text = EPKLCPOEELO.Attributes["Text"].CIPOICEEIBK(string.Empty);
		EPAKNMNEIDK(EPKLCPOEELO);
	}

	public void EPAKNMNEIDK(XmlNode EPKLCPOEELO)
	{
		foreach (XmlNode childNode in EPKLCPOEELO.ChildNodes)
		{
			string key = childNode.Attributes["Text"].CIPOICEEIBK(string.Empty);
			int value = childNode.Attributes["Frames"].ParseInt();
			GOJABLKPFJM.Add(new KeyValuePair<string, int>(key, value));
		}
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		if (GOJABLKPFJM.Count > 0)
		{
			HJLLBHDCBFE(GFIHPBCEEOB);
		}
		else
		{
			CHBCAEBBOPJ(GFIHPBCEEOB);
		}
	}

	public void HJLLBHDCBFE(QuestParameters GFIHPBCEEOB)
	{
		List<KeyValuePair<string, int>> KPKPFFGEFGI = new List<KeyValuePair<string, int>>();
		GOJABLKPFJM.ForEach((KeyValuePair<string, int> DHDMNHCIPEH) =>
		{
			string key = ABMMAALFNFD.KGIEIAJLAGI(DHDMNHCIPEH.Key, GFIHPBCEEOB);
			KPKPFFGEFGI.Add(new KeyValuePair<string, int>(key, DHDMNHCIPEH.Value));
		});
		GameUtils.ShowEnterScreen(KPKPFFGEFGI, GCKKOOHDJMI);
	}

	public void CHBCAEBBOPJ(QuestParameters GFIHPBCEEOB)
	{
		string hCPNFPMHFCM = ABMMAALFNFD.KGIEIAJLAGI(_text, GFIHPBCEEOB);
		GameUtils.ShowEnterScreen(hCPNFPMHFCM, GCKKOOHDJMI);
		string text = ABMMAALFNFD.KGIEIAJLAGI(_text, GFIHPBCEEOB);
	}

	private void GCKKOOHDJMI()
	{
		GameUtils.OFOKPNFGDMD("Chapter completed");
		OGIJONMKABB();
	}
}
