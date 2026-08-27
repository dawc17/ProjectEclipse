using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Location
{
	public const int ZDelta = -3;

	private const string JEFPPEDOMLA = "params.xml";

	private static string PGCOAFKLLCG;

	private static string ACIJLMFFGIA;

	private static string JGAGHHGDKNN;

	private string MFILBEJPGHO;

	private bool _preferCustomLayout;

	private static readonly Dictionary<string, string> MissingArtworkFallbacks = new Dictionary<string, string>
	{
		{ "emerald_forest_new", "emerald_forest" },
		{ "flying_rocks_small", "flying_rocks" },
		{ "ruins_village_small", "ruins_village" },
		{ "waterfall_small", "waterfall" },
		{ "flooded_village", "village" },
		{ "magic_rocks", "flying_rocks" },
		{ "road", "battlefield" },
		{ "spaceship_thorny", "spaceship" },
		{ "stone_dragon", "stone_forest" },
		{ "stone_forest_thorny", "stone_forest" }
	};

	public string name;

	private string PINIIFIOECE;

	public List<string> musics = new List<string>();

	public int gridSize;

	public float GBNPHCHGKDO;

	public float JMBOGPILDNM;

	public float MFAPMDDJBBL;

	public float JMLAKAKDBBL;

	public float FEIHFIPFNKF;

	public float IIMDMHKPJJN;

	public float DJKCICJKHNN;

	public Vector3f JJNMOJLLDEC = new Vector3f();

	public Vector3f CLGGLBHOMCE = new Vector3f();

	public Color modelsColor;

	public List<LocationSelector> layers;

	public LocationSelector gameLayer;

	public static string BDEIJGPCBMO
	{
		get
		{
			return KCCLMHNNMNH();
		}
	}

	public Vector3f BJBKBJMALJC
	{
		get
		{
			return GOEOFEIOAPC();
		}
	}

	public Vector2f LHFLKPKAOLM
	{
		get
		{
			return HIHNLFGMHAG();
		}
	}

	public string NPPIFKKLNCN
	{
		get
		{
			return MOADJJNKFKB();
		}
	}

	private string PNBLGOPFAOD
	{
		get
		{
			return BBNOJALBLKC();
		}
	}

	private string GPNPNHFACPO
	{
		get
		{
			return MKNEGEKDDKH();
		}
	}

	public Location()
	{
		name = GameUtils.NIPABEEAMHJ;
		gridSize = 0;
		GBNPHCHGKDO = 0f;
		JMBOGPILDNM = 0f;
		MFAPMDDJBBL = 0f;
		JMLAKAKDBBL = 0f;
		FEIHFIPFNKF = 0f;
		gameLayer = null;
		layers = null;
	}

	public Location(string JLEKBBJBLOE, string FGCHEGMCGPD, bool preferCustomLayout = false)
	{
		_preferCustomLayout = preferCustomLayout;
		LLLOJBFMONN.Write("Location:" + JLEKBBJBLOE);
		name = JLEKBBJBLOE;
		gridSize = 0;
		GBNPHCHGKDO = 0f;
		JMBOGPILDNM = 0f;
		MFAPMDDJBBL = 0f;
		JMLAKAKDBBL = 0f;
		FEIHFIPFNKF = 0f;
		PINIIFIOECE = FGCHEGMCGPD;
		gameLayer = null;
		layers = null;
	}

	public void init()
	{
		MFILBEJPGHO = name;
		string value;
		if (!_preferCustomLayout && MissingArtworkFallbacks.TryGetValue(MFILBEJPGHO, out value))
		{
			Debug.Log("[Location] Using installed artwork '" + value + "' for newer location '" + MFILBEJPGHO + "'.");
			MFILBEJPGHO = value;
		}
		// Recovered raid artwork belongs to the new combined-layer layouts.
		// The embedded legacy params split those layers into obsolete tiles.
		XmlDocument xmlDocument = _preferCustomLayout ?
			XmlUtils.OpenXMLDocument(BBNOJALBLKC(), string.Empty) : OpenInstalledLocationDocument();
		if (xmlDocument == null)
		{
			xmlDocument = _preferCustomLayout ? OpenInstalledLocationDocument() : XmlUtils.OpenXMLDocument(BBNOJALBLKC(), string.Empty);
		}
		if (xmlDocument == null || xmlDocument["Root"] == null)
		{
			Debug.LogWarning("[Location] Missing or invalid location '" + name + "'; using dojo fallback.");
			MFILBEJPGHO = "dojo";
			xmlDocument = OpenInstalledLocationDocument();
			if (xmlDocument == null)
			{
				xmlDocument = XmlUtils.OpenXMLDocument(BBNOJALBLKC(), string.Empty);
			}
		}
		if (xmlDocument == null || xmlDocument["Root"] == null)
		{
			Debug.LogError("[Location] Dojo fallback data is missing; location cannot be initialized.");
			return;
		}
		musics.Clear();
		if (PINIIFIOECE != string.Empty)
		{
			musics.Add(PINIIFIOECE);
		}
		else
		{
			string[] collection = xmlDocument["Root"].Attributes["Music"].CIPOICEEIBK().Split('|');
			musics = new List<string>(collection);
		}
		XmlAttribute cJBEMNNNHDM = xmlDocument["Root"].Attributes["FrictionForce"];
		PhysicsController.set_FrictionForce(cJBEMNNNHDM.ParseFloat(PhysicsController.EOBGEGHEPOA()));
		MFAPMDDJBBL = xmlDocument["Root"].Attributes["Wall"].ParseFloat();
		GBNPHCHGKDO = xmlDocument["Root"].Attributes["Floor"].ParseFloat();
		JMBOGPILDNM = xmlDocument["Root"].Attributes["PositionY"].ParseFloat();
		modelsColor = ColorUtils.DAAIIECAAFO(xmlDocument["Root"].Attributes["Color"].CIPOICEEIBK());
		JMLAKAKDBBL = xmlDocument["Root"].Attributes["Width"].ParseFloat();
		FEIHFIPFNKF = xmlDocument["Root"].Attributes["Height"].ParseFloat();
		IIMDMHKPJJN = xmlDocument["Root"].Attributes["MinWidth"].ParseFloat(JMLAKAKDBBL);
		DJKCICJKHNN = IIMDMHKPJJN / JMLAKAKDBBL;
		gridSize = xmlDocument["Root"].Attributes["GridSize"].ParseInt();
		layers = new List<LocationSelector>();
		XmlNode xmlNode = xmlDocument["Root"];
		int num = 0;
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			if (childNode.NodeType != XmlNodeType.Element || childNode.Name != "Layer")
				continue;
			ParseLayer(childNode, num);
			num += -3;
		}
	}

	private XmlDocument OpenInstalledLocationDocument()
	{
		string text = ResourceManager.GetBundledText(BBNOJALBLKC());
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		try
		{
			XmlDocument document = new XmlDocument();
			document.LoadXml(text);
			return document;
		}
		catch (XmlException exception)
		{
			Debug.LogWarning("[Location] Invalid bundled params for '" + MFILBEJPGHO + "': " + exception.Message);
			return null;
		}
	}

	public static string KCCLMHNNMNH()
	{
		return PGCOAFKLLCG;
	}

	public Vector3f GOEOFEIOAPC()
	{
		Vector3f eMAFACPEPDK = Vector3f.PHEFFKMOOCM(JJNMOJLLDEC, CLGGLBHOMCE);
		eMAFACPEPDK.Multiply(0.5f);
		return eMAFACPEPDK;
	}

	public Vector2f HIHNLFGMHAG()
	{
		return new Vector2f(JMLAKAKDBBL / 2f, FEIHFIPFNKF / 2f);
	}

	public string MOADJJNKFKB()
	{
		return NekkiMath.FGFBKJLIADI(musics);
	}

	public void Clear()
	{
	}

	private string BBNOJALBLKC()
	{
		return string.Format("{0}/{1}/{2}", SF2Paths.OCAKEHJCNCC(), MFILBEJPGHO, "params.xml");
	}

	private string MKNEGEKDDKH()
	{
		return string.Format("Textures/Locations/{0}", MFILBEJPGHO);
	}

	private string LKDJCCIJFMD(string FAAALPKKJID)
	{
		return "Textures/" + FAAALPKKJID.Trim('/');
	}

	private void ParseLayer(XmlNode node, int DFIDNHKKNMB)
	{
		int num = 0;
		int num2 = node.Attributes["Scaling"].ParseInt();
		LocationSelector hEOCIOGMDKG = new LocationSelector(DFIDNHKKNMB);
		string text = null;
		text = ((node.Attributes["Path"] == null) ? MKNEGEKDDKH() : LKDJCCIJFMD(node.Attributes["Path"].CIPOICEEIBK()));
		hEOCIOGMDKG.NLJHHPCLMBI(num2 > 0);
		hEOCIOGMDKG.set_Type(node.Attributes["Type"].ParseInt());
		hEOCIOGMDKG.FPFLDAMPALH(node.Attributes["Factor"].ParseFloat());
		hEOCIOGMDKG.LHPOLNGGAFA(node.Attributes["Atlas"].CIPOICEEIBK());
		if (!string.IsNullOrEmpty(hEOCIOGMDKG.EMNJEHHOBKG()))
		{
			hEOCIOGMDKG.NJPBFGMGCFC(CocosAnimationData.Create(string.Format("{0}/{1}_xml.xml", text, hEOCIOGMDKG.EMNJEHHOBKG()), true));
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			switch (childNode.Name)
			{
			case "ModelsViewer":
				JJNMOJLLDEC.Set(childNode.Attributes["PlayerPositionX"].ParseFloat(), childNode.Attributes["PlayerPositionY"].ParseFloat());
				CLGGLBHOMCE.Set(childNode.Attributes["EnemyPositionX"].ParseFloat(), childNode.Attributes["EnemyPositionY"].ParseFloat());
				gridSize = num;
				break;
			case "ParticleEffect":
				ParseParticleEffect(childNode, hEOCIOGMDKG, num, 0);
				break;
			case "NewParticleEffect":
				ParseParticleEffect(childNode, hEOCIOGMDKG, num, 1);
				break;
			case "Image":
			case "SpriteMask":
				ParseImage(childNode, text, hEOCIOGMDKG, num);
				break;
			case "SimpleEffect":
				ParseSimpleEffect(childNode, text, hEOCIOGMDKG, num);
				break;
			}
		}
		layers.Add(hEOCIOGMDKG);
		if (hEOCIOGMDKG.BBELALLBKHH())
		{
			gameLayer = hEOCIOGMDKG;
		}
	}

	private void ParseImage(XmlNode node, string PPAJIHNNNDG, LocationSelector IDHKNBECKKO, int EELGIMCJLAI)
	{
		string ODMCNMJPHFJ = node.Attributes["ClassName"].CIPOICEEIBK();
		Sprite sprite = LocationSpriteCache.PPBEKKDIJKC(PPAJIHNNNDG, ODMCNMJPHFJ, IDHKNBECKKO.EMNJEHHOBKG());
		if (sprite == null)
		{
			LLLOJBFMONN.Write("Pic: {0}", ODMCNMJPHFJ);
			return;
		}
		GameObject gameObject = new GameObject(ODMCNMJPHFJ);
		SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = sprite;
		spriteRenderer.flipX = node.Attributes["FlipX"].ParseInt() != 0;
		spriteRenderer.flipY = node.Attributes["FlipY"].ParseInt() != 0;
		SpriteMaskInteraction maskInteraction;
		if (System.Enum.TryParse(node.Attributes["MaskInteraction"].CIPOICEEIBK(), out maskInteraction))
			spriteRenderer.maskInteraction = maskInteraction;
		if (node.Name == "SpriteMask")
		{
			spriteRenderer.enabled = false;
			gameObject.AddComponent<SpriteMask>().sprite = sprite;
		}
		if (node.Attributes["Color"] != null)
		{
			spriteRenderer.color = ColorUtils.DAAIIECAAFO(node.Attributes["Color"].Value);
		}
		float num = node.Attributes["X"].ParseFloat();
		float num2 = 0f - node.Attributes["Y"].ParseFloat();
		float num3 = 0f;
		float num4 = 0f;
		float num5 = node.Attributes["Width"].ParseFloat();
		float num6 = node.Attributes["Height"].ParseFloat();
		float x = sprite.rect.size.x;
		float y = sprite.rect.size.y;
		bool flag = false;
		if (IDHKNBECKKO.ALLFLLFJIGC() != null)
		{
			CocosAnimationData.SpriteFrameCocos pBAHNJDFMBO = IDHKNBECKKO.ALLFLLFJIGC().BFJEFNHKPJI().Find((CocosAnimationData.SpriteFrameCocos DHDMNHCIPEH) => DHDMNHCIPEH.get_Name() == ODMCNMJPHFJ);
			if (pBAHNJDFMBO != null)
			{
				flag = pBAHNJDFMBO.KGFGOFBMCCG();
				num3 = pBAHNJDFMBO.LMJCBAFGAFL().x;
				num4 = pBAHNJDFMBO.LMJCBAFGAFL().y;
				x = pBAHNJDFMBO.PFIECJPOFFB().x;
				y = pBAHNJDFMBO.PFIECJPOFFB().y;
			}
		}
		Vector3 vector = default(Vector3);
		gameObject.transform.localPosition = new Vector3(num + num3, num2 + num4, 0f);
		if (flag)
		{
			gameObject.transform.Rotate(0f, 0f, 90f);
			vector = new Vector3(num6 / y, num5 / x, 1f);
		}
		else
		{
			vector = new Vector3(num5 / x, num6 / y, 1f);
		}
		gameObject.transform.localScale = vector;
		gameObject.transform.localPosition = new Vector3(num + num3 * vector.x, num2 + num4 * vector.y, 0f);
		IDHKNBECKKO.GDEDCJGMFDK(gameObject, EELGIMCJLAI);
	}

	private void ParseSimpleEffect(XmlNode node, string OKNJDIMPKCB, LocationSelector IDHKNBECKKO, int EELGIMCJLAI)
	{
		bool flag = false;
		string text = OKNJDIMPKCB;
		ChangingSprite fEMGGEAGICG = null;
		string text2 = node.Attributes["PictureLocation"].CIPOICEEIBK();
		if (text2 == "global")
		{
			text = string.Empty;
			text = "Textures/Location_effects/";
		}
		if (node.Attributes["Path"] != null)
		{
			text = "Textures/" + node.Attributes["Path"].CIPOICEEIBK();
		}
		string text3 = node.Attributes["Type"].CIPOICEEIBK();
		if (text3 == "Picture")
		{
			string ODMCNMJPHFJ = node.Attributes["ClassName"].CIPOICEEIBK();
			CocosAnimationData animationData = IDHKNBECKKO.ALLFLLFJIGC();
			CocosAnimationData.SpriteFrameCocos pIDBGGLFBCO = animationData == null ? null : animationData.BFJEFNHKPJI().Find((CocosAnimationData.SpriteFrameCocos DHDMNHCIPEH) => DHDMNHCIPEH.get_Name() == ODMCNMJPHFJ);
			if (LocationSpriteCache.PPBEKKDIJKC(text, ODMCNMJPHFJ, IDHKNBECKKO.EMNJEHHOBKG()) == null)
			{
				Debug.LogWarning("[Location] Missing picture effect '" + ODMCNMJPHFJ + "' in " + name);
				return;
			}
			fEMGGEAGICG = new ChangingSprite(ChangingSprite.MHDKGPHKHIE.PictureBased);
			fEMGGEAGICG.LDEAPJCKFMP(text, ODMCNMJPHFJ, IDHKNBECKKO.EMNJEHHOBKG(), pIDBGGLFBCO, node.Attributes["Width"].ParseFloat(), node.Attributes["Height"].ParseFloat());
		}
		if (text3 == "Sequention")
		{
			if (GameUtils.GBCMHICHIOI)
			{
				return;
			}
			fEMGGEAGICG = new ChangingSprite(ChangingSprite.MHDKGPHKHIE.AtlasBased);
			if (!fEMGGEAGICG.OMHFEGBJDHP(node.Attributes["ClassName"].CIPOICEEIBK(), text.TrimEnd('/') + "/Atlases/", node.Attributes["Speed"].ParseFloat(), node.Attributes["Offset"].ParseFloat(), node.Attributes["Width"].ParseFloat(), node.Attributes["Height"].ParseFloat()))
			{
				fEMGGEAGICG = null;
				return;
			}
			flag = true;
			fEMGGEAGICG.OGBLGCKOCLL(node.Attributes["Pause"].ParseFloat());
		}
		if (fEMGGEAGICG == null)
		{
			Debug.LogWarning("[Location] Unsupported effect type '" + text3 + "' in " + name);
			return;
		}
		fEMGGEAGICG.SetPosition(node.Attributes["X"].ParseFloat(), 0f - node.Attributes["Y"].ParseFloat());
		foreach (XmlNode childNode in node.ChildNodes)
		{
			string name = childNode.Name;
			if (name == "OscillationX")
			{
				fEMGGEAGICG.PBDEFHJGBML(childNode.Attributes["Offset"].ParseFloat());
				foreach (XmlNode childNode2 in childNode.ChildNodes)
				{
					fEMGGEAGICG.NOHGIBJKJNC(childNode2.Attributes["Period"].ParseFloat(), childNode2.Attributes["Value"].ParseFloat(), childNode2.Attributes["Ease"].ParseFloat());
				}
			}
			if (name == "OscillationY")
			{
				fEMGGEAGICG.INPLHCAAJKP(childNode.Attributes["Offset"].ParseFloat());
				foreach (XmlNode childNode3 in childNode.ChildNodes)
				{
					fEMGGEAGICG.HMLBMLMDLOP(childNode3.Attributes["Period"].ParseFloat(), childNode3.Attributes["Value"].ParseFloat(), childNode3.Attributes["Ease"].ParseFloat());
				}
			}
			if (name == "Transparency")
			{
				fEMGGEAGICG.CNECHMNCAHM(childNode.Attributes["Offset"].ParseFloat());
				foreach (XmlNode childNode4 in childNode.ChildNodes)
				{
					fEMGGEAGICG.KGJGDKNJPJH(childNode4.Attributes["Period"].ParseFloat(), childNode4.Attributes["Value"].ParseFloat(), childNode4.Attributes["Ease"].ParseFloat());
				}
			}
			if (name == "Rotation")
			{
				fEMGGEAGICG.MBGHNIKNNPJ(childNode.Attributes["Offset"].ParseFloat());
				foreach (XmlNode childNode5 in childNode.ChildNodes)
				{
					fEMGGEAGICG.KEOBIGPEGEO(childNode5.Attributes["Period"].ParseFloat(), childNode5.Attributes["Value"].ParseFloat(), childNode5.Attributes["Ease"].ParseFloat());
				}
			}
			if (name == "Speed")
			{
				fEMGGEAGICG.JNLCGHHDBBE(childNode.Attributes["X"].ParseFloat(), childNode.Attributes["Y"].ParseFloat());
			}
			if (name == "ReappearX")
			{
				fEMGGEAGICG.FDMODLLENAE(childNode.Attributes["Min"].ParseFloat(), childNode.Attributes["Max"].ParseFloat());
			}
			if (name == "ReappearY")
			{
				fEMGGEAGICG.FNPELDEJFGN(childNode.Attributes["Min"].ParseFloat(), childNode.Attributes["Max"].ParseFloat());
			}
		}
		IDHKNBECKKO.IFAMCLKHNMA(fEMGGEAGICG, EELGIMCJLAI);
	}

	private void ParseParticleEffect(XmlNode node, LocationSelector IDHKNBECKKO, int EELGIMCJLAI, int LFLGCDNKNJI)
	{
		if (!GameUtils.LEEIGNICAMN)
		{
			string jIPAAPBPNJM = "Textures/Location_effects/Particles/" + node.Attributes["ClassName"].CIPOICEEIBK();
			ChangingSprite fEMGGEAGICG = new ChangingSprite(ChangingSprite.MHDKGPHKHIE.ParticleBased);
			float fNDOOJNDJDC = node.Attributes["X"].ParseFloat();
			float num = node.Attributes["Y"].ParseFloat();
			int num2 = node.Attributes["MiddleColor"].ParseInt();
			bool flag = false;
			switch (LFLGCDNKNJI)
			{
			case 0:
				flag = fEMGGEAGICG.AFPMFHFIBBO(jIPAAPBPNJM, fNDOOJNDJDC, 0f - num);
				break;
			case 1:
				flag = fEMGGEAGICG.AFPMFHFIBBO(jIPAAPBPNJM, fNDOOJNDJDC, 0f - num);
				break;
			default:
				LLLOJBFMONN.Write("unknown type in parseParticleEffect");
				break;
			}
			if (flag)
			{
				IDHKNBECKKO.MHCEMJOAPCA(fEMGGEAGICG, EELGIMCJLAI);
			}
		}
	}
}
