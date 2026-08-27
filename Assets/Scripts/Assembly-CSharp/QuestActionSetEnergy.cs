using System;
using System.Xml;

public class QuestActionSetEnergy : QuestAction
{
	private int _value;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		_value = EPKLCPOEELO.Attributes["Value"].ParseInt();
		if (_value < 0)
		{
			LLLOJBFMONN.Error("QuestActionSetEnergy::parse - wrong value: %i, setting to 0", _value);
			_value = 0;
		}
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		int oGLHGFJKMCO = nKGLHEGIKKP.OGLHGFJKMCO;
		nKGLHEGIKKP.DKAAELKJJOP(Math.Min(_value, oGLHGFJKMCO));
		OGIJONMKABB();
	}
}
