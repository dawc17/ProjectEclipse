using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class MapButtonController : global::EventDispatcher<MapButtonInfo>
{
	public enum HNOEGLCDHJF
	{
		MAP_BUTTON_INFO_ADD = 0,
		MAP_BUTTON_INFO_REMOVE = 1
	}

	private static MapButtonController _instance;

	private XmlNode _node;

	private List<MapButtonInfo> ECMMPBEMCPE = new List<MapButtonInfo>();

	public static MapButtonController BPCBBHAKFDM
	{
		get
		{
			return ELEBLBJKDBI();
		}
	}

	public List<MapButtonInfo> KIHLMIHLGHC
	{
		get
		{
			return MEPCBPIJLGB();
		}
	}

	public static MapButtonController ELEBLBJKDBI()
	{
		if (_instance == null)
		{
			_instance = new MapButtonController();
		}
		return _instance;
	}

	public List<MapButtonInfo> MEPCBPIJLGB()
	{
		return ECMMPBEMCPE.FindAll(OHGCDIHFJIJ);
	}

	public bool OHGCDIHFJIJ(MapButtonInfo KLNKEPMAGKF)
	{
		MapButtonInfo.HNEJAKIGDBA hNEJAKIGDBA = KLNKEPMAGKF.EDMILHNJFAA();
		return hNEJAKIGDBA == MapButtonInfo.HNEJAKIGDBA.Story || hNEJAKIGDBA == MapButtonInfo.HNEJAKIGDBA.Both;
	}

	public void GKIOOABOBFL(MapButtonInfo DJDNMAOEFBD)
	{
		if (DJDNMAOEFBD != null)
		{
			MapButtonInfo eBMMANKELOA = ECMMPBEMCPE.Find((MapButtonInfo DHDMNHCIPEH) => DHDMNHCIPEH.Name.Equals(DJDNMAOEFBD.Name));
			if (eBMMANKELOA == null)
			{
				IPLIFIPBFAD(DJDNMAOEFBD);
				ECMMPBEMCPE.Add(DJDNMAOEFBD);
				CallEvent(0, DJDNMAOEFBD);
			}
		}
	}

	public void DMCBGLJHBPA(MapButtonInfo DJDNMAOEFBD)
	{
		if (DJDNMAOEFBD != null)
		{
			DMCBGLJHBPA(DJDNMAOEFBD.Name);
		}
	}

	public void DMCBGLJHBPA(string name)
	{
		MapButtonInfo eBMMANKELOA = ECMMPBEMCPE.Find((MapButtonInfo DHDMNHCIPEH) => DHDMNHCIPEH.Name.Equals(name));
		if (eBMMANKELOA != null)
		{
			KFEBGKAALIA(name);
			ECMMPBEMCPE.Remove(eBMMANKELOA);
			CallEvent(1, eBMMANKELOA);
		}
	}

	public void Parse(XmlNode node)
	{
		JNIIGKNBCCL(node);
		Clear();
		foreach (XmlNode childNode in _node.ChildNodes)
		{
			MapButtonInfo eBMMANKELOA = new MapButtonInfo();
			bool nEOIMNAHLAN = childNode.Attributes["X"].Empty() || childNode.Attributes["Y"].Empty();
			float x = childNode.Attributes["X"].ParseFloat();
			float y = childNode.Attributes["Y"].ParseFloat();
			eBMMANKELOA.Name = childNode.Attributes["Name"].CIPOICEEIBK();
			eBMMANKELOA.NHKMCLPOMFK = childNode.Attributes["Image"].CIPOICEEIBK();
			eBMMANKELOA.Timer = childNode.Attributes["Timer"].CIPOICEEIBK();
			eBMMANKELOA.KMEDBHDDDJA = childNode.Attributes["Type"].CIPOICEEIBK("Image");
			eBMMANKELOA.HFBFPBGLBOM = childNode.Attributes["Atlas"].CIPOICEEIBK();
			eBMMANKELOA.BOEJEFCDIAD = childNode.Attributes["Speed"].ParseFloat();
			eBMMANKELOA.Pause = childNode.Attributes["Pause"].ParseFloat();
			eBMMANKELOA.BIJFFONMDBC = new Vector2(x, y);
			float defaultAnchorX = (eBMMANKELOA.Name == "EclipseModeOn" || eBMMANKELOA.Name == "EclipseModeOff") ? 1f : 0.5f;
			eBMMANKELOA.AnchorMinX = childNode.Attributes["AnchorMinX"].ParseFloat(defaultAnchorX);
			eBMMANKELOA.AnchorMaxX = childNode.Attributes["AnchorMaxX"].ParseFloat(eBMMANKELOA.AnchorMinX);
			eBMMANKELOA.NEOIMNAHLAN = nEOIMNAHLAN;
			eBMMANKELOA.MLKPBAALMBC = childNode.Attributes["ShowType"].CIPOICEEIBK("Story");
			ECMMPBEMCPE.Add(eBMMANKELOA);
		}
	}

	private void Clear()
	{
		ECMMPBEMCPE.Clear();
	}

	private void JNIIGKNBCCL(XmlNode node)
	{
		if (node != null)
		{
			_node = node["MapButtons"];
			if (_node == null)
			{
				node.ACBPMPMPKJJ("MapButtons");
				_node = node["MapButtons"];
				ListSF.ELEBLBJKDBI().EJANJEEGOOE();
			}
		}
	}

	private void IPLIFIPBFAD(MapButtonInfo DJDNMAOEFBD)
	{
		XmlNode mEEAKLDGLDF = _node.ACBPMPMPKJJ("Button");
		mEEAKLDGLDF.LLIKNHNLGJJ("Name").Value = DJDNMAOEFBD.Name;
		mEEAKLDGLDF.LLIKNHNLGJJ("Image").Value = DJDNMAOEFBD.NHKMCLPOMFK;
		mEEAKLDGLDF.LLIKNHNLGJJ("Type").Value = DJDNMAOEFBD.KMEDBHDDDJA;
		if (DJDNMAOEFBD.BOEJEFCDIAD > 0f)
		{
			mEEAKLDGLDF.LLIKNHNLGJJ("Speed").Value = DJDNMAOEFBD.BOEJEFCDIAD.ToString();
		}
		if (DJDNMAOEFBD.Pause > 0f)
		{
			mEEAKLDGLDF.LLIKNHNLGJJ("Pause").Value = DJDNMAOEFBD.Pause.ToString();
		}
		if (!DJDNMAOEFBD.NEOIMNAHLAN)
		{
			mEEAKLDGLDF.LLIKNHNLGJJ("X").Value = DJDNMAOEFBD.BIJFFONMDBC.x.ToString();
			mEEAKLDGLDF.LLIKNHNLGJJ("Y").Value = DJDNMAOEFBD.BIJFFONMDBC.y.ToString();
			mEEAKLDGLDF.LLIKNHNLGJJ("AnchorMinX").Value = DJDNMAOEFBD.AnchorMinX.ToString();
			mEEAKLDGLDF.LLIKNHNLGJJ("AnchorMaxX").Value = DJDNMAOEFBD.AnchorMaxX.ToString();
		}
		if (!string.IsNullOrEmpty(DJDNMAOEFBD.HFBFPBGLBOM))
		{
			mEEAKLDGLDF.LLIKNHNLGJJ("Atlas").Value = DJDNMAOEFBD.HFBFPBGLBOM;
		}
		if (!string.IsNullOrEmpty(DJDNMAOEFBD.Timer))
		{
			mEEAKLDGLDF.LLIKNHNLGJJ("Timer").Value = DJDNMAOEFBD.Timer;
		}
		mEEAKLDGLDF.LLIKNHNLGJJ("ShowType").Value = DJDNMAOEFBD.MLKPBAALMBC;
		ListSF.ELEBLBJKDBI().EJANJEEGOOE();
	}

	private void KFEBGKAALIA(MapButtonInfo DJDNMAOEFBD)
	{
		KFEBGKAALIA(DJDNMAOEFBD.Name);
	}

	private void KFEBGKAALIA(string name)
	{
		XmlNode xmlNode = _node.LJGLMGNAFHJ("Button", "Name", name);
		if (xmlNode != null)
		{
			_node.RemoveChild(xmlNode);
			ListSF.ELEBLBJKDBI().EJANJEEGOOE();
		}
	}
}
