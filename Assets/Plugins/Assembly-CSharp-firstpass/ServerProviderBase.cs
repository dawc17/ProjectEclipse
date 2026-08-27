using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Nekki.Social;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;

public abstract class ServerProviderBase : MonoBehaviour
{
	public class APKPDGMFGDL
	{
		private string _table;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string[] CIIOOOFFJHD;

		private DHBAMBIHOEC[] _conditions;

		private string[] POGAKCIFKDH;

		private int? _limit;

		public string[] JMKBOELCNPO
		{
			get
			{
				return CKOJIABCEBP();
			}
			private set
			{
				EOJFFGAAGOA(value);
			}
		}

		public APKPDGMFGDL(string BFGHBIMJHAK, string[] KHGIIFDIHHA, DHBAMBIHOEC[] conditions, string[] order, int? LOHCIKNKDEI)
		{
			_table = BFGHBIMJHAK;
			EOJFFGAAGOA(KHGIIFDIHHA);
			_conditions = conditions;
			POGAKCIFKDH = order;
			_limit = LOHCIKNKDEI;
		}

		public string[] CKOJIABCEBP()
		{
			return CIIOOOFFJHD;
		}

		private void EOJFFGAAGOA(string[] value)
		{
			CIIOOOFFJHD = value;
		}

		private Form HLKOMKMFAHH()
		{
			Form lBFANOCPALF = new Form();
			lBFANOCPALF.Add("table", _table);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[");
			for (int i = 0; i < CKOJIABCEBP().Length; i++)
			{
				stringBuilder.Append(string.Format("\"{0}\"", CKOJIABCEBP()[i]));
				if (i < CKOJIABCEBP().Length - 1)
				{
					stringBuilder.Append(",");
				}
			}
			stringBuilder.Append("]");
			lBFANOCPALF.Add("fields", stringBuilder.ToString());
			if (_conditions != null)
			{
				lBFANOCPALF.Add("where", DHBAMBIHOEC.HLKOMKMFAHH(_conditions));
			}
			if (POGAKCIFKDH != null)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append("[");
				for (int j = 0; j < POGAKCIFKDH.Length; j++)
				{
					stringBuilder2.Append(string.Format("\"{0}\"", POGAKCIFKDH[j]));
					if (j < POGAKCIFKDH.Length - 1)
					{
						stringBuilder2.Append(",");
					}
				}
				stringBuilder2.Append("]");
				lBFANOCPALF.Add("order", stringBuilder2.ToString());
			}
			int? eDEKKCMHGEN = _limit;
			if (eDEKKCMHGEN.HasValue)
			{
				lBFANOCPALF.Add("limit", _limit.Value);
			}
			return lBFANOCPALF;
		}

		[SpecialName]
		public static WWWForm op_Implicit(APKPDGMFGDL JHELEGOAKFH)
		{
			return Form.op_Implicit(JHELEGOAKFH.HLKOMKMFAHH());
		}
	}

	public class DHBAMBIHOEC
	{
		private string FEEOCAFHHFP;

		private string AOJJBKLCHJO;

		private string value;

		private DHBAMBIHOEC()
		{
		}

		public static DHBAMBIHOEC IAFFDNEHLFF(string FEEOCAFHHFP, object value)
		{
			DHBAMBIHOEC dHBAMBIHOEC = new DHBAMBIHOEC();
			dHBAMBIHOEC.FEEOCAFHHFP = FEEOCAFHHFP;
			dHBAMBIHOEC.AOJJBKLCHJO = ">";
			dHBAMBIHOEC.value = value.ToString();
			return dHBAMBIHOEC;
		}

		public static DHBAMBIHOEC LDCKIFIJOAH(string FEEOCAFHHFP, object value)
		{
			DHBAMBIHOEC dHBAMBIHOEC = new DHBAMBIHOEC();
			dHBAMBIHOEC.FEEOCAFHHFP = FEEOCAFHHFP;
			dHBAMBIHOEC.AOJJBKLCHJO = "<";
			dHBAMBIHOEC.value = value.ToString();
			return dHBAMBIHOEC;
		}

		public static DHBAMBIHOEC Equals(string FEEOCAFHHFP, object value)
		{
			DHBAMBIHOEC dHBAMBIHOEC = new DHBAMBIHOEC();
			dHBAMBIHOEC.FEEOCAFHHFP = FEEOCAFHHFP;
			dHBAMBIHOEC.AOJJBKLCHJO = "=";
			dHBAMBIHOEC.value = value.ToString();
			return dHBAMBIHOEC;
		}

		public static DHBAMBIHOEC JFMBIGDDPNG(string FEEOCAFHHFP, string LADOAFMFCGL, object value)
		{
			DHBAMBIHOEC dHBAMBIHOEC = new DHBAMBIHOEC();
			dHBAMBIHOEC.FEEOCAFHHFP = FEEOCAFHHFP;
			dHBAMBIHOEC.AOJJBKLCHJO = LADOAFMFCGL;
			dHBAMBIHOEC.value = value.ToString();
			return dHBAMBIHOEC;
		}

		public string HLKOMKMFAHH()
		{
			return string.Format("[\"{0}\",\"{1}\",\"{2}\"]", FEEOCAFHHFP, AOJJBKLCHJO, value);
		}

		public static string HLKOMKMFAHH(DHBAMBIHOEC[] conditions)
		{
			string text = "[";
			for (int i = 0; i < conditions.Length; i++)
			{
				text += conditions[i].HLKOMKMFAHH();
				if (i != conditions.Length - 1)
				{
					text += ",";
				}
			}
			return text + "]";
		}
	}

	public class FGHAAKOBGGN
	{
		private readonly string _value;

		public string CIPOICEEIBK
		{
			get
			{
				return AKMCEMFFOBE();
			}
		}

		public float ParseFloat
		{
			get
			{
				return DLHFMAMPMPM();
			}
		}

		public int ParseInt
		{
			get
			{
				return MKAAOLMHECH();
			}
		}

		public FGHAAKOBGGN(string value)
		{
			_value = value ?? string.Empty;
		}

		public string AKMCEMFFOBE()
		{
			return _value;
		}

		public float DLHFMAMPMPM()
		{
			return float.Parse(_value);
		}

		public int MKAAOLMHECH()
		{
			return int.Parse(_value);
		}

		public override string ToString()
		{
			return AKMCEMFFOBE();
		}
	}

	[DefaultMember("Item")]
	public class BKPEDCFDPNN
	{
		private readonly Dictionary<string, FGHAAKOBGGN> GCGGIJDKKKO;

		// C# has no syntax for parameterized property 'DLKPBAJDHBO'.
		public FGHAAKOBGGN get_DLKPBAJDHBO(string index)
		{
			return get_Item(index);
		}

		public BKPEDCFDPNN(Dictionary<string, FGHAAKOBGGN> DMNBDBJNKME)
		{
			GCGGIJDKKKO = DMNBDBJNKME;
		}

		public FGHAAKOBGGN get_Item(string index)
		{
			if (!GCGGIJDKKKO.ContainsKey(index.ToLower()))
			{
				AdvLog.LOPHFKMOPAA("there no data for key " + index.ToLower());
			}
			return GCGGIJDKKKO[index.ToLower()];
		}
	}

	[DefaultMember("Item")]
	public class NNFJBMBACHB
	{
		private readonly List<BKPEDCFDPNN> _data = new List<BKPEDCFDPNN>();

		// C# has no syntax for parameterized property 'DLKPBAJDHBO'.
		public BKPEDCFDPNN get_DLKPBAJDHBO(int index)
		{
			return get_Item(index);
		}

		public int Count
		{
			get
			{
				return OFOPFCJNEBL();
			}
		}

		public bool Empty
		{
			get
			{
				return KLNLNKBIDGD();
			}
		}

		public NNFJBMBACHB(JSONArray EMDHMHOKGFP, APKPDGMFGDL JHELEGOAKFH)
		{
			for (int i = 0; i < EMDHMHOKGFP.Count; i++)
			{
				Dictionary<string, FGHAAKOBGGN> dictionary = new Dictionary<string, FGHAAKOBGGN>();
				for (int j = 0; j < JHELEGOAKFH.CKOJIABCEBP().Length; j++)
				{
					dictionary.Add(JHELEGOAKFH.CKOJIABCEBP()[j].ToLower(), new FGHAAKOBGGN(EMDHMHOKGFP[i].AsArray[j]));
				}
				_data.Add(new BKPEDCFDPNN(dictionary));
			}
		}

		public BKPEDCFDPNN get_Item(int index)
		{
			return _data[index];
		}

		public int OFOPFCJNEBL()
		{
			return _data.Count;
		}

		public bool KLNLNKBIDGD()
		{
			return _data.Count == 0;
		}
	}

	public class FileData
	{
		private string IPJFPMMAKMB;

		private string _source;

		private string PCOGLHJCLEG;

		public string GLJIAOAHJNE
		{
			get
			{
				return DIHKMAKOHGN();
			}
		}

		public string NNPFICBIAKI
		{
			get
			{
				return EOHCHEKOMFB();
			}
		}

		public string GNDLHEMMGPH
		{
			get
			{
				return JGNPLINLCGC();
			}
		}

		public FileData(string BBNKIBKPBLO, string NOLDJLJIPOG, string GNIBJBFNGAD)
		{
			_source = BBNKIBKPBLO;
			PCOGLHJCLEG = NOLDJLJIPOG;
			IPJFPMMAKMB = GNIBJBFNGAD;
		}

		public string DIHKMAKOHGN()
		{
			return IPJFPMMAKMB;
		}

		public string EOHCHEKOMFB()
		{
			return PCOGLHJCLEG;
		}

		public string JGNPLINLCGC()
		{
			return _source;
		}
	}

	public class Form
	{
		private readonly List<KeyValuePair<string, string>> _data = new List<KeyValuePair<string, string>>();

		private readonly List<KeyValuePair<string, FileData>> LKLAKNDFCKG = new List<KeyValuePair<string, FileData>>();

		private static string _key = "DGgim7dg7cbknRCxVOAlXfGVtjOPyZls";

		public static string ENFBNOGCCBH
		{
			get
			{
				return AENLBNDAEKB();
			}
			set
			{
				set_Key(value);
			}
		}

		public Form()
		{
			Add("rand", UnityEngine.Random.Range(0, int.MaxValue));
		}

		public static string AENLBNDAEKB()
		{
			return _key;
		}

		public static void set_Key(string value)
		{
			_key = value;
		}

		public void Add(string KGBGENDIMBC, object value)
		{
			_data.Add(new KeyValuePair<string, string>(KGBGENDIMBC, value.ToString()));
		}

		public void HIIBLOGOILG(string KGBGENDIMBC, FileData OONGGDBLOHH)
		{
			KeyValuePair<string, FileData> item = new KeyValuePair<string, FileData>(KGBGENDIMBC, OONGGDBLOHH);
			LKLAKNDFCKG.Add(item);
		}

		[SpecialName]
		public static WWWForm op_Implicit(Form HOELLMLEBAK)
		{
			HOELLMLEBAK._data.Sort(MJKNBEKEJLP);
			WWWForm wWWForm = new WWWForm();
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < HOELLMLEBAK._data.Count; i++)
			{
				stringBuilder.Append(string.Format("{0}={1}", HOELLMLEBAK._data[i].Key, HOELLMLEBAK._data[i].Value));
				wWWForm.AddField(HOELLMLEBAK._data[i].Key, HOELLMLEBAK._data[i].Value);
			}
			foreach (KeyValuePair<string, FileData> item in HOELLMLEBAK.LKLAKNDFCKG)
			{
				string key = item.Key;
				string iFKJHHPJPLP = item.Value.JGNPLINLCGC();
				byte[] contents = HCEPBIAOJKG.OEPBCILIGPI(iFKJHHPJPLP);
				string fileName = item.Value.EOHCHEKOMFB();
				string mimeType = item.Value.DIHKMAKOHGN();
				wWWForm.AddBinaryData(key, contents, fileName, mimeType);
			}
			stringBuilder.Append(_key);
			MD5 mD = MD5.Create();
			byte[] array = mD.ComputeHash(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
			wWWForm.AddField("sig", BitConverter.ToString(array).Replace("-", string.Empty).ToLower());
			return wWWForm;
		}

		private static int MJKNBEKEJLP(KeyValuePair<string, string> DIHJILMHNGB, KeyValuePair<string, string> KBEKLNMPDDE)
		{
			return string.Compare(DIHJILMHNGB.Key, KBEKLNMPDDE.Key, StringComparison.Ordinal);
		}

		public JSONClass LINEPHBFDFM()
		{
			JSONClass jSONClass = new JSONClass();
			for (int i = 0; i < _data.Count; i++)
			{
				jSONClass[_data[i].Key] = _data[i].Value;
			}
			return jSONClass;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < _data.Count; i++)
			{
				stringBuilder.Append(string.Format("{0}={1}\n", _data[i].Key, _data[i].Value));
			}
			return stringBuilder.ToString();
		}
	}

	protected class FFCINPEAEBE
	{
		public string GIHDDAKBMHE;

		public string JDONBAPIJCG;

		public static FFCINPEAEBE Get(string EMDHMHOKGFP)
		{
			return JsonConvert.DeserializeObject<FFCINPEAEBE>(EMDHMHOKGFP);
		}
	}

	private static GameObject _nestedObject;

	private readonly List<IEnumerator> _holdRoutine = new List<IEnumerator>();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static ServerProviderBase OGKMDFDNIEN;

	protected static GameObject GLOLDAJOGNK
	{
		get
		{
			return CCIJPFJNGCE();
		}
	}

	public static ServerProviderBase BPCBBHAKFDM
	{
		get
		{
			return get_Instance();
		}
		protected set
		{
			set_Instance(value);
		}
	}

	protected abstract string NFKOPHMCLFF();

	protected static GameObject CCIJPFJNGCE()
	{
		return _nestedObject;
	}

	protected void OnDestroy()
	{
		_nestedObject = null;
	}

	internal void Update()
	{
		if (_holdRoutine.Count > 0)
		{
			for (int i = 0; i < _holdRoutine.Count; i++)
			{
				StartCoroutine(_holdRoutine[i]);
			}
			_holdRoutine.Clear();
		}
	}

	public static ServerProviderBase get_Instance()
	{
		return OGKMDFDNIEN;
	}

	protected static void set_Instance(ServerProviderBase value)
	{
		OGKMDFDNIEN = value;
	}

	protected static T Init<T>() where T : ServerProviderBase
	{
		_nestedObject = GameObject.Find("_server");
		if (_nestedObject == null)
		{
			_nestedObject = new GameObject("_server");
			UnityEngine.Object.DontDestroyOnLoad(_nestedObject);
		}
		T val = _nestedObject.GetComponent<T>();
		if (val == null)
		{
			val = _nestedObject.AddComponent<T>();
		}
		set_Instance(val);
		return val;
	}

	protected abstract void Init();

	protected bool Check()
	{
		Init();
		if (!_nestedObject)
		{
			AdvLog.CCOFFJPPAKC("ServerCall terminated. you should call Init<T>() from your Init() method! (where T is your inherited class)");
			return false;
		}
		return true;
	}

	public virtual void Join(Action<bool> onDone, Action<string> onError)
	{
		if (Check())
		{
			IEnumerator enumerator = JoinRoutine(onDone, onError);
			if (!UserInfo.op_Implicit(Social.NLEKLPFPLPC()))
			{
				AdvLog.LOPHFKMOPAA("ServerProvider.Join(..) hold while SocialWrapper.CurrentUser is null");
				_holdRoutine.Add(enumerator);
			}
			else
			{
				StartCoroutine(enumerator);
			}
		}
	}

	protected virtual IEnumerator JoinRoutine(Action<bool> onDone, Action<string> onError)
	{
		Form lBFANOCPALF = new Form();
		lBFANOCPALF.Add("uid", Social.NLEKLPFPLPC().NDLJPNCIJIP());
		lBFANOCPALF.Add("photo", Social.NLEKLPFPLPC().CIHLLDHJLON());
		lBFANOCPALF.Add("fname", Social.NLEKLPFPLPC().FJANLLCDPCP());
		lBFANOCPALF.Add("lname", Social.NLEKLPFPLPC().GKKFLFIACMN());
		WWW wWW = new WWW(string.Format("{0}/join.php", NFKOPHMCLFF()), Form.op_Implicit(lBFANOCPALF));
		yield return wWW;
		if (!string.IsNullOrEmpty(wWW.error))
		{
			onError(wWW.error);
			yield break;
		}
		try
		{
			FFCINPEAEBE fFCINPEAEBE = FFCINPEAEBE.Get(wWW.text);
			if (string.IsNullOrEmpty(fFCINPEAEBE.JDONBAPIJCG))
			{
				onDone(fFCINPEAEBE.GIHDDAKBMHE.Equals("new"));
			}
			else
			{
				onError(fFCINPEAEBE.JDONBAPIJCG ?? string.Empty);
			}
		}
		catch (Exception ex)
		{
			onError(ex.Message);
		}
	}

	public virtual void SaveData(string LOKLDPLAPOL, string data, Action onDone, Action<string> onError)
	{
		if (Check())
		{
			IEnumerator enumerator = SaveDataRoutine(LOKLDPLAPOL, data, onDone, onError);
			if (!UserInfo.op_Implicit(Social.NLEKLPFPLPC()))
			{
				AdvLog.LOPHFKMOPAA("ServerProvider.SaveData(..) hold while SocialWrapper.CurrentUser is null");
				_holdRoutine.Add(enumerator);
			}
			else
			{
				StartCoroutine(enumerator);
			}
		}
	}

	protected virtual IEnumerator SaveDataRoutine(string LOKLDPLAPOL, string data, Action onDone, Action<string> onError)
	{
		Form lBFANOCPALF = new Form();
		lBFANOCPALF.Add("uid", SocialWrapper.NLEKLPFPLPC().NDLJPNCIJIP());
		lBFANOCPALF.Add("alias", LOKLDPLAPOL);
		lBFANOCPALF.Add("data", data);
		WWW wWW = new WWW(string.Format("{0}/put.php", NFKOPHMCLFF()), Form.op_Implicit(lBFANOCPALF));
		yield return wWW;
		if (!string.IsNullOrEmpty(wWW.error))
		{
			onError(wWW.error);
			yield break;
		}
		try
		{
			FFCINPEAEBE fFCINPEAEBE = FFCINPEAEBE.Get(wWW.text);
			if (string.IsNullOrEmpty(fFCINPEAEBE.JDONBAPIJCG))
			{
				onDone();
			}
			else
			{
				onError(fFCINPEAEBE.JDONBAPIJCG ?? string.Empty);
			}
		}
		catch (Exception ex)
		{
			onError(ex.Message);
		}
	}

	public virtual void LoadData(string LOKLDPLAPOL, Action<string> onDone, Action<string> onError)
	{
		if (Check())
		{
			IEnumerator enumerator = LoadDataRoutine(LOKLDPLAPOL, onDone, onError);
			if (!UserInfo.op_Implicit(Social.NLEKLPFPLPC()))
			{
				AdvLog.LOPHFKMOPAA("ServerProvider.LoadData(..) holden while SocialWrapper.CurrentUser is null");
				_holdRoutine.Add(enumerator);
			}
			else
			{
				StartCoroutine(enumerator);
			}
		}
	}

	protected virtual IEnumerator LoadDataRoutine(string LOKLDPLAPOL, Action<string> onDone, Action<string> onError)
	{
		Form lBFANOCPALF = new Form();
		lBFANOCPALF.Add("uid", Social.NLEKLPFPLPC().NDLJPNCIJIP());
		lBFANOCPALF.Add("alias", LOKLDPLAPOL);
		WWW wWW = new WWW(string.Format("{0}/get.php", NFKOPHMCLFF()), Form.op_Implicit(lBFANOCPALF));
		yield return wWW;
		if (!string.IsNullOrEmpty(wWW.error))
		{
			onError(wWW.error);
			yield break;
		}
		try
		{
			FFCINPEAEBE fFCINPEAEBE = FFCINPEAEBE.Get(wWW.text);
			if (string.IsNullOrEmpty(fFCINPEAEBE.JDONBAPIJCG))
			{
				onDone(Unescape(fFCINPEAEBE.GIHDDAKBMHE));
			}
			else
			{
				onError(fFCINPEAEBE.JDONBAPIJCG ?? string.Empty);
			}
		}
		catch (Exception ex)
		{
			onError(ex.Message);
		}
	}

	protected string Unescape(string DCJLKCFKCOM)
	{
		if (string.IsNullOrEmpty(DCJLKCFKCOM))
		{
			return string.Empty;
		}
		return DCJLKCFKCOM.Replace("\\/", "/").Replace("\\\"", "\"").Replace("\\r\\n", "\n");
	}

	public virtual void TimeSync(Action<long> onDone, Action<string> onError)
	{
		if (Check())
		{
			IEnumerator routine = TimeSyncRoutine(onDone, onError);
			StartCoroutine(routine);
		}
	}

	protected virtual IEnumerator TimeSyncRoutine(Action<long> onDone, Action<string> onError)
	{
		WWW wWW = new WWW(form: Form.op_Implicit(new Form()), url: string.Format("{0}/time.php", NFKOPHMCLFF()));
		yield return wWW;
		if (!string.IsNullOrEmpty(wWW.error))
		{
			onError(wWW.error);
			yield break;
		}
		try
		{
			FFCINPEAEBE fFCINPEAEBE = FFCINPEAEBE.Get(wWW.text);
			if (string.IsNullOrEmpty(fFCINPEAEBE.JDONBAPIJCG))
			{
				onDone(long.Parse(fFCINPEAEBE.GIHDDAKBMHE.Trim()));
			}
			else
			{
				onError(fFCINPEAEBE.JDONBAPIJCG ?? string.Empty);
			}
		}
		catch (Exception ex)
		{
			onError(ex.Message);
		}
	}

	public virtual void WipeUser(Action onDone, Action<string> onError)
	{
		if (Check())
		{
			IEnumerator enumerator = WipeUserRoutine(onDone, onError);
			if (!UserInfo.op_Implicit(Social.NLEKLPFPLPC()))
			{
				AdvLog.LOPHFKMOPAA("ServerProvider.WipeUser(..) hold while SocialWrapper.CurrentUser is null");
				_holdRoutine.Add(enumerator);
			}
			else
			{
				StartCoroutine(enumerator);
			}
		}
	}

	protected virtual IEnumerator WipeUserRoutine(Action onDone, Action<string> onError)
	{
		Form lBFANOCPALF = new Form();
		lBFANOCPALF.Add("uid", Social.NLEKLPFPLPC().NDLJPNCIJIP());
		WWW wWW = new WWW(string.Format("{0}/wipe.php", NFKOPHMCLFF()), Form.op_Implicit(lBFANOCPALF));
		yield return wWW;
		if (!string.IsNullOrEmpty(wWW.error))
		{
			onError(wWW.error);
			yield break;
		}
		try
		{
			FFCINPEAEBE fFCINPEAEBE = FFCINPEAEBE.Get(wWW.text);
			if (string.IsNullOrEmpty(fFCINPEAEBE.JDONBAPIJCG))
			{
				onDone();
			}
			else
			{
				onError(fFCINPEAEBE.JDONBAPIJCG ?? string.Empty);
			}
		}
		catch (Exception ex)
		{
			onError(ex.Message);
		}
	}

	public virtual NNFJBMBACHB Query(APKPDGMFGDL KOGEDGJJMPO)
	{
		WWW wWW = new WWW(string.Format("{0}/query.php", NFKOPHMCLFF()), APKPDGMFGDL.op_Implicit(KOGEDGJJMPO));
		while (!wWW.isDone && string.IsNullOrEmpty(wWW.error))
		{
		}
		if (!string.IsNullOrEmpty(wWW.error))
		{
			throw new Exception(wWW.error);
		}
		string aJSON = wWW.text.Replace("\\\"", "\"");
		JSONNode jSONNode = JSON.Parse(aJSON);
		if (string.IsNullOrEmpty(jSONNode["error"]))
		{
			return new NNFJBMBACHB(jSONNode["response"].AsArray, KOGEDGJJMPO);
		}
		throw new Exception(jSONNode["error"]);
	}

	public virtual void Query(APKPDGMFGDL KOGEDGJJMPO, Action<NNFJBMBACHB> onDone, Action<string> onError)
	{
		StartCoroutine(LGDPJMAFNJH(KOGEDGJJMPO, onDone, onError));
	}

	protected virtual IEnumerator LGDPJMAFNJH(APKPDGMFGDL KOGEDGJJMPO, Action<NNFJBMBACHB> onDone, Action<string> onError)
	{
		WWW wWW = new WWW(string.Format("{0}/query.php", NFKOPHMCLFF()), APKPDGMFGDL.op_Implicit(KOGEDGJJMPO));
		yield return wWW;
		if (!string.IsNullOrEmpty(wWW.error))
		{
			onError(wWW.error);
			yield break;
		}
		try
		{
			string aJSON = wWW.text.Replace("\\\"", "\"");
			JSONNode jSONNode = JSON.Parse(aJSON);
			if (string.IsNullOrEmpty(jSONNode["error"]))
			{
				onDone(new NNFJBMBACHB(jSONNode["response"].AsArray, KOGEDGJJMPO));
			}
			else
			{
				onError(jSONNode["error"]);
			}
		}
		catch (Exception ex)
		{
			onError(ex.Message);
		}
	}
}
