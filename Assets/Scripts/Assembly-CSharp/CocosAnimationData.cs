using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using UnityEngine;

public class CocosAnimationData
{
	private static readonly HashSet<string> CompatibilityEffectWarnings = new HashSet<string>();

	private static string ResolveCompatibilityEffect(string path)
	{
		string replacement = null;
		if (path.EndsWith("mgc_surge_time_effec_xml") || path.EndsWith("mgc_surge_time_effec_xml.xml"))
			replacement = "mgc_effect_time_bomb_xml";
		else if (path.EndsWith("mgc_effect_prediction_start_xml") ||
			path.EndsWith("mgc_effect_prediction_loop_xml") ||
			path.EndsWith("mgc_effect_prediction_end_xml"))
			replacement = "mgc_effect_green_aura_xml";
		if (replacement == null)
			return path;
		int separator = path.LastIndexOf('/');
		string resolved = (separator < 0 ? string.Empty : path.Substring(0, separator + 1)) + replacement;
		if (CompatibilityEffectWarnings.Add(path))
			Debug.LogWarning("[Effects] Missing newer sequence '" + path + "'; using '" + resolved + "'.");
		return resolved;
	}

	public class SpriteFrameCocos
	{
		private class PJGMEEGMPFF
		{
			public int NPKMJMCLDAH;

			public int IHAHIEHHNCG;

			public int BLFBMIOIPOI;

			public int CCIPIPKPHGB;
		}

		private string _Name;

		private Sprite _Sprite;

		private PJGMEEGMPFF CGAMKFFFCMO;

		private Vector2 KBOIHCPHFJL;

		private bool _Rotated;

		private Vector2 MHNOMBDDNLE;

		public Sprite MNMCAEJGDGG
		{
			get
			{
				return HJADPLOLOBH();
			}
			set
			{
				set_Sprite(value);
			}
		}

		public Vector2 AMAEBLJHMGG
		{
			get
			{
				return LMJCBAFGAFL();
			}
		}

		public bool GHEODOHGKPG
		{
			get
			{
				return KGFGOFBMCCG();
			}
			set
			{
				set_Rotated(value);
			}
		}

		public Vector2 PLCPNLBCPCC
		{
			get
			{
				return PFIECJPOFFB();
			}
		}

		public void set_Name(string value)
		{
			_Name = value;
		}

		public string get_Name()
		{
			return _Name;
		}

		public Sprite HJADPLOLOBH()
		{
			return _Sprite;
		}

		public void set_Sprite(Sprite value)
		{
			_Sprite = value;
		}

		public void SetFrame(string LIAILCGJBDK)
		{
			int num = LIAILCGJBDK.IndexOf('}');
			string[] array = LIAILCGJBDK.Substring(2, num - 2).Split(',');
			string[] array2 = LIAILCGJBDK.Substring(num + 3, LIAILCGJBDK.Length - (num + 5)).Split(',');
			CGAMKFFFCMO = new PJGMEEGMPFF();
			CGAMKFFFCMO.NPKMJMCLDAH = int.Parse(array[0]);
			CGAMKFFFCMO.IHAHIEHHNCG = int.Parse(array[1]);
			CGAMKFFFCMO.BLFBMIOIPOI = int.Parse(array2[0]);
			CGAMKFFFCMO.CCIPIPKPHGB = int.Parse(array2[1]);
		}

		public Vector2 LMJCBAFGAFL()
		{
			return KBOIHCPHFJL;
		}

		public void CEDNGLNABAJ(string LIAILCGJBDK)
		{
			KBOIHCPHFJL = ParseVector(LIAILCGJBDK);
		}

		public void set_Rotated(bool value)
		{
			_Rotated = value;
		}

		public bool KGFGOFBMCCG()
		{
			return _Rotated;
		}

		public Vector2 PFIECJPOFFB()
		{
			return MHNOMBDDNLE;
		}

		public void AAHNBCAFBMG(string LIAILCGJBDK)
		{
			MHNOMBDDNLE = ParseVector(LIAILCGJBDK);
		}

		private static Vector2 ParseVector(string value)
		{
			// An earlier recovery tool emitted an extra brace pair. Accept that
			// representation as well as TexturePacker's canonical {x,y}, without
			// letting the machine's decimal separator change the result.
			string[] parts = (value ?? string.Empty).Trim().Trim('{', '}', ' ').Split(',');
			float x, y;
			if (parts.Length != 2 ||
				!float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out x) ||
				!float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out y) ||
				float.IsNaN(x) || float.IsInfinity(x) || float.IsNaN(y) || float.IsInfinity(y))
			{
				throw new FormatException("Invalid Cocos vector '" + value + "'.");
			}
			return new Vector2(x, y);
		}

		public void GCDLICFMMAL()
		{
			if (CGAMKFFFCMO.CCIPIPKPHGB <= 2 && CGAMKFFFCMO.BLFBMIOIPOI <= 2)
			{
			}
		}
	}

	private List<SpriteFrameCocos> _Frames = new List<SpriteFrameCocos>();

	private int _TextureH;

	private string _Path;

	private static Dictionary<string, CocosAnimationData> LECBCNDHDMP = new Dictionary<string, CocosAnimationData>();

	public List<SpriteFrameCocos> OCFKLCDIEBF
	{
		get
		{
			return BFJEFNHKPJI();
		}
	}

	private CocosAnimationData(XmlDocument GPIBAMAMGKD, string ONEIGMLOGDC)
	{
		_Path = ONEIGMLOGDC.ToLower();
		XmlNode xmlNode = GPIBAMAMGKD["plist"]["dict"];
		string text = null;
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			if (childNode.Name == "key")
			{
				text = childNode.FirstChild.Value;
				continue;
			}
			if (text == "frames")
			{
				IPIEHGHEKOK(childNode, _Frames);
			}
			if (!(text == "metadata"))
			{
				continue;
			}
			foreach (XmlNode childNode2 in childNode.ChildNodes)
			{
				if (childNode2.Name == "key")
				{
					text = childNode2.FirstChild.Value;
				}
				else if (text == "size")
				{
					string value = childNode2.FirstChild.Value;
					string[] array = value.Substring(1, value.Length - 2).Split(',');
					_TextureH = int.Parse(array[1]);
				}
			}
		}
	}

	public List<SpriteFrameCocos> BFJEFNHKPJI()
	{
		return _Frames;
	}

	public string GetResourcePath()
	{
		return _Path;
	}

	public static void DECIILEPLDM()
	{
		LECBCNDHDMP.Clear();
	}

	public static CocosAnimationData Create(string ONEIGMLOGDC, bool MPMHHEMGHOJ = false)
	{
		ONEIGMLOGDC = ResolveCompatibilityEffect(ONEIGMLOGDC);
		if (LECBCNDHDMP.ContainsKey(ONEIGMLOGDC))
		{
			return LECBCNDHDMP[ONEIGMLOGDC];
		}
		XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(ONEIGMLOGDC, string.Empty, MPMHHEMGHOJ ? XmlUtils.EBLFEPIOMOL.ForcedResourced : XmlUtils.EBLFEPIOMOL.Normal);
		if (xmlDocument == null)
		{
			return null;
		}
		CocosAnimationData nIHINKFPFLM = new CocosAnimationData(xmlDocument, ONEIGMLOGDC);
		LECBCNDHDMP.Add(ONEIGMLOGDC, nIHINKFPFLM);
		return nIHINKFPFLM;
	}

	private void IPIEHGHEKOK(XmlNode OPPGGBFCIJA, List<SpriteFrameCocos> GFIODDEBNHM)
	{
		string jLEKBBJBLOE = null;
		foreach (XmlNode childNode in OPPGGBFCIJA.ChildNodes)
		{
			if (childNode.Name == "key")
			{
				jLEKBBJBLOE = childNode.FirstChild.Value;
			}
			else
			{
				GFIODDEBNHM.Add(AFHOCALJBEE(childNode, jLEKBBJBLOE));
			}
		}
	}

	private SpriteFrameCocos AFHOCALJBEE(XmlNode EBBAHEDDHFO, string JLEKBBJBLOE)
	{
		SpriteFrameCocos pBAHNJDFMBO = new SpriteFrameCocos();
		pBAHNJDFMBO.set_Name(JLEKBBJBLOE.Replace(".png", string.Empty));
		string text = null;
		foreach (XmlNode childNode in EBBAHEDDHFO.ChildNodes)
		{
			if (childNode.Name == "key")
			{
				text = childNode.FirstChild.Value;
				continue;
			}
			switch (text)
			{
			case "frame":
				pBAHNJDFMBO.SetFrame(childNode.FirstChild.Value);
				break;
			case "offset":
				pBAHNJDFMBO.CEDNGLNABAJ(childNode.FirstChild.Value);
				break;
			case "rotated":
				pBAHNJDFMBO.set_Rotated(childNode.Name == "true");
				break;
			case "sourceSize":
				pBAHNJDFMBO.AAHNBCAFBMG(childNode.FirstChild.Value);
				break;
			}
		}
		pBAHNJDFMBO.GCDLICFMMAL();
		return pBAHNJDFMBO;
	}

	public void AIFNJAPCCII()
	{
		int num = _Path.IndexOf("resources");
		string oNEIGMLOGDC = ((num != -1) ? _Path.Substring(num) : _Path).Replace("_xml", string.Empty).Replace(".xml", string.Empty);
		Sprite[] array = ResourcesAndBundles.BNCMBJOICHI<Sprite>(oNEIGMLOGDC);
		Dictionary<string, Sprite> dictionary = new Dictionary<string, Sprite>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				dictionary[array[i].name] = array[i];
			}
		}

		int num2 = oNEIGMLOGDC.LastIndexOf('/');
		string text = (num2 >= 0) ? oNEIGMLOGDC.Substring(0, num2 + 1) : string.Empty;
		for (int j = 0; j < _Frames.Count; j++)
		{
			Sprite value;
			if (!dictionary.TryGetValue(_Frames[j].get_Name(), out value))
			{
				// The exported project stores atlas frames as individual Sprite
				// resources (hit_1.asset, block_1.asset, etc.) rather than as
				// sub-assets of the source PNG. Load those standalone frames when
				// LoadAll cannot find a matching embedded sprite.
				value = ResourcesAndBundles.Load<Sprite>(text + _Frames[j].get_Name());
			}
			_Frames[j].set_Sprite(value);
		}
	}

	public void JBPCHMAGDMI()
	{
		// Atlas dictionaries are commonly emitted in lexical order, which puts
		// frame_10 before frame_2.  The decompiled sorter only recognized a
		// one-character suffix and therefore shuffled every effect over 9 frames.
		// Newer magic effects routinely contain 30-80 frames, so sort by the full
		// trailing integer while retaining a deterministic fallback for names that
		// do not end in a frame number.
		_Frames.Sort(delegate(SpriteFrameCocos left, SpriteFrameCocos right)
		{
			int leftNumber;
			int rightNumber;
			bool leftHasNumber = TryGetFrameNumber(left.get_Name(), out leftNumber);
			bool rightHasNumber = TryGetFrameNumber(right.get_Name(), out rightNumber);
			if (leftHasNumber && rightHasNumber)
			{
				int comparison = leftNumber.CompareTo(rightNumber);
				if (comparison != 0)
					return comparison;
			}
			else if (leftHasNumber != rightHasNumber)
			{
				return leftHasNumber ? -1 : 1;
			}
			return string.CompareOrdinal(left.get_Name(), right.get_Name());
		});
	}

	private static bool TryGetFrameNumber(string name, out int frameNumber)
	{
		frameNumber = 0;
		if (string.IsNullOrEmpty(name))
			return false;
		int separator = name.LastIndexOf('_');
		return separator >= 0 && separator + 1 < name.Length &&
			int.TryParse(name.Substring(separator + 1), out frameNumber);
	}
}
