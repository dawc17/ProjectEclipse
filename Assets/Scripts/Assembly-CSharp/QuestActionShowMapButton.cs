using System.Xml;
using UnityEngine;

public class QuestActionShowMapButton : QuestAction
{
	private string _name = string.Empty;

	private string NCKCDCODNHA = string.Empty;

	private string KMFDBBKMLOO = string.Empty;

	private string GLBELKGODDB = string.Empty;

	private string BCNONLENDGG = string.Empty;

	private string _type = string.Empty;

	private string Atlas = string.Empty;

	private string AMEGCDJDGPB = string.Empty;

	private string JDDJEAGMNMP = string.Empty;

	private bool _AutoPosition;

	private string OAIIJFCMDJD = string.Empty;

	private float _anchorMinX = 0.5f;

	private float _anchorMaxX = 0.5f;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		_name = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK(string.Empty);
		NCKCDCODNHA = EPKLCPOEELO.Attributes["Image"].CIPOICEEIBK(string.Empty);
		KMFDBBKMLOO = EPKLCPOEELO.Attributes["Timer"].CIPOICEEIBK(string.Empty);
		_AutoPosition = EPKLCPOEELO.Attributes["X"] == null || EPKLCPOEELO.Attributes["Y"] == null;
		GLBELKGODDB = EPKLCPOEELO.Attributes["X"].CIPOICEEIBK(string.Empty);
		BCNONLENDGG = EPKLCPOEELO.Attributes["Y"].CIPOICEEIBK(string.Empty);
		_type = EPKLCPOEELO.Attributes["Type"].CIPOICEEIBK(string.Empty);
		Atlas = EPKLCPOEELO.Attributes["Atlas"].CIPOICEEIBK(string.Empty);
		AMEGCDJDGPB = EPKLCPOEELO.Attributes["Speed"].CIPOICEEIBK(string.Empty);
		JDDJEAGMNMP = EPKLCPOEELO.Attributes["Pause"].CIPOICEEIBK(string.Empty);
		OAIIJFCMDJD = EPKLCPOEELO.Attributes["ShowType"].CIPOICEEIBK("Both");
		_anchorMinX = EPKLCPOEELO.Attributes["AnchorMinX"].ParseFloat(0.5f);
		_anchorMaxX = EPKLCPOEELO.Attributes["AnchorMaxX"].ParseFloat(_anchorMinX);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		string name = string.Empty;
		string KHPKDMGDMAB = string.Empty;
		string timer = string.Empty;
		string LFLGCDNKNJI = string.Empty;
		string NBHBEJFPFBN = string.Empty;
		string CACHHLONJII = string.Empty;
		float ALCFJHNPDGL = 0f;
		float KCANPMPILKI = 0f;
		Vector3 AJMBPDGKMAF = default(Vector3);
		GetValues(ref name, ref KHPKDMGDMAB, ref timer, ref LFLGCDNKNJI, ref NBHBEJFPFBN, ref ALCFJHNPDGL, ref KCANPMPILKI, ref AJMBPDGKMAF, ref CACHHLONJII);
		MapButtonInfo dJDNMAOEFBD = new MapButtonInfo(name, KHPKDMGDMAB, timer, AJMBPDGKMAF, _AutoPosition, NBHBEJFPFBN, LFLGCDNKNJI, ALCFJHNPDGL, KCANPMPILKI, CACHHLONJII, _anchorMinX, _anchorMaxX);
		MapButtonController.ELEBLBJKDBI().GKIOOABOBFL(dJDNMAOEFBD);
		OGIJONMKABB();
	}

	private void GetValues(ref string name, ref string KHPKDMGDMAB, ref string timer, ref string LFLGCDNKNJI, ref string NBHBEJFPFBN, ref float ALCFJHNPDGL, ref float KCANPMPILKI, ref Vector3 AJMBPDGKMAF, ref string CACHHLONJII)
	{
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(PAJDEKLLFNJ);
		kKDGLNECFHA.MCPIOGALBMK(_name, lNIDLHOIHIM);
		name = lNIDLHOIHIM.ToString();
		lNIDLHOIHIM.Clear();
		kKDGLNECFHA.MCPIOGALBMK(NCKCDCODNHA, lNIDLHOIHIM);
		KHPKDMGDMAB = lNIDLHOIHIM.ToString();
		lNIDLHOIHIM.Clear();
		if (!string.IsNullOrEmpty(KMFDBBKMLOO))
		{
			kKDGLNECFHA.MCPIOGALBMK(KMFDBBKMLOO, lNIDLHOIHIM);
			timer = lNIDLHOIHIM.ToString();
		}
		lNIDLHOIHIM.Clear();
		if (!string.IsNullOrEmpty(_type))
		{
			kKDGLNECFHA.MCPIOGALBMK(_type, lNIDLHOIHIM);
			LFLGCDNKNJI = lNIDLHOIHIM.ToString();
		}
		lNIDLHOIHIM.Clear();
		if (!string.IsNullOrEmpty(Atlas))
		{
			kKDGLNECFHA.MCPIOGALBMK(Atlas, lNIDLHOIHIM);
			NBHBEJFPFBN = lNIDLHOIHIM.ToString();
		}
		lNIDLHOIHIM.Clear();
		if (!string.IsNullOrEmpty(AMEGCDJDGPB))
		{
			kKDGLNECFHA.MCPIOGALBMK(AMEGCDJDGPB, lNIDLHOIHIM);
			ALCFJHNPDGL = ((!lNIDLHOIHIM.INCOIAANDCO()) ? 0f : ((float)lNIDLHOIHIM.resultNumber));
		}
		lNIDLHOIHIM.Clear();
		if (!string.IsNullOrEmpty(JDDJEAGMNMP))
		{
			kKDGLNECFHA.MCPIOGALBMK(JDDJEAGMNMP, lNIDLHOIHIM);
			KCANPMPILKI = ((!lNIDLHOIHIM.INCOIAANDCO()) ? 0f : ((float)lNIDLHOIHIM.resultNumber));
		}
		lNIDLHOIHIM.Clear();
		if (!string.IsNullOrEmpty(GLBELKGODDB))
		{
			kKDGLNECFHA.MCPIOGALBMK(GLBELKGODDB, lNIDLHOIHIM);
			AJMBPDGKMAF.x = ((!lNIDLHOIHIM.INCOIAANDCO()) ? 0f : ((float)lNIDLHOIHIM.resultNumber));
		}
		lNIDLHOIHIM.Clear();
		if (!string.IsNullOrEmpty(BCNONLENDGG))
		{
			kKDGLNECFHA.MCPIOGALBMK(BCNONLENDGG, lNIDLHOIHIM);
			AJMBPDGKMAF.y = ((!lNIDLHOIHIM.INCOIAANDCO()) ? 0f : ((float)lNIDLHOIHIM.resultNumber));
		}
		lNIDLHOIHIM.Clear();
		kKDGLNECFHA.MCPIOGALBMK(OAIIJFCMDJD, lNIDLHOIHIM);
		CACHHLONJII = lNIDLHOIHIM.ToString();
	}
}
