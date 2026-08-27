using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SceneConfig : MonoBehaviour
{
	[Serializable]
	public class BaseProperty
	{
		public string Name;

		public int Index;

		public BaseProperty()
		{
			Name = "Property";
		}

		public BaseProperty(string name)
		{
			Name = name;
		}
	}

	[Serializable]
	public class NoneProperty : BaseProperty
	{
		public int Value;
	}

	[Serializable]
	public class IntProperty : BaseProperty
	{
		public int Value;
	}

	[Serializable]
	public class FloatProperty : BaseProperty
	{
		public float Value;
	}

	[Serializable]
	public class StringProperty : BaseProperty
	{
		public string Value;
	}

	[Serializable]
	public class BoolProperty : BaseProperty
	{
		public bool Value;
	}

	[Serializable]
	public class IntArrayProperty : BaseProperty
	{
		[SerializeField]
		public List<int> Value;
	}

	[Serializable]
	public class FloatArrayProperty : BaseProperty
	{
		[SerializeField]
		public List<float> Value;
	}

	[Serializable]
	public class StringArrayProperty : BaseProperty
	{
		[SerializeField]
		public List<string> Value;
	}

	[Serializable]
	public class BoolArrayProperty : BaseProperty
	{
		[SerializeField]
		public List<bool> Value;
	}

	[Serializable]
	public class PropertyArrayProperty : BaseProperty
	{
		[SerializeField]
		public List<BaseProperty> Value;
	}

	public string SceneName;

	[SerializeField]
	public List<NoneProperty> NoneProperties;

	[SerializeField]
	public List<IntProperty> IntProperties;

	[SerializeField]
	public List<FloatProperty> FloatProperties;

	[SerializeField]
	public List<StringProperty> StringProperties;

	[SerializeField]
	public List<BoolProperty> BoolProperties;

	[SerializeField]
	public List<IntArrayProperty> IntArrayProperties;

	[SerializeField]
	public List<FloatArrayProperty> FloatArrayProperties;

	[SerializeField]
	public List<StringArrayProperty> StringArrayProperties;

	[SerializeField]
	public List<BoolArrayProperty> BoolArrayProperties;

	[SerializeField]
	public List<PropertyArrayProperty> PropertyArrayProperties;

	public bool IsConfig;

	private static SceneConfig EDAPJLKMFPC;

	private float CDLEAEBCKDN;

	private float BJCBBBHBOID;

	private float ECICHHBFELI;

	private Vector3 OFELPAHFIOJ;

	private Vector3 IFALGIHJHLN;

	private float JHPCPBHPDNJ;

	private float FNICPCKHHAC;

	private float BNJCCKOMHJH;

	private float LEGKPOHHMEO;

	public static SceneConfig BPCBBHAKFDM
	{
		get
		{
			return get_Instance();
		}
	}

	public static bool ELFPNFCHEIL
	{
		get
		{
			return get_IsPresent();
		}
	}

	public static float LDNACACPMFG
	{
		get
		{
			return get_LeftBorderX();
		}
		set
		{
			set_LeftBorderX(value);
		}
	}

	public static float JHPCKJNHHDE
	{
		get
		{
			return get_RightBorderX();
		}
		set
		{
			set_RightBorderX(value);
		}
	}

	public static float HLIGLIEFMGL
	{
		get
		{
			return get_CenterX();
		}
		private set
		{
			HEPEBCNHAPD(value);
		}
	}

	public static Vector3 BNGJNHJPHGG
	{
		get
		{
			return get_SpawnPointEnemy();
		}
		private set
		{
			LENIOMGPDNB(value);
		}
	}

	public static Vector3 DLEJLEOJGCE
	{
		get
		{
			return get_SpawnPointPlayer();
		}
		private set
		{
			PEIPFPAOCHB(value);
		}
	}

	public static float CJCICEMEKJI
	{
		get
		{
			return get_PointFloor();
		}
	}

	public static float PBDOAMOFMHI
	{
		get
		{
			return get_MaxDistBetweenModels();
		}
		private set
		{
			GLBMAFELMDI(value);
		}
	}

	public static float FPLFNIJIICP
	{
		get
		{
			return get_LocationRightBorder();
		}
		private set
		{
			EBNMJIGMMOP(value);
		}
	}

	public static float NBHMMCCAFFL
	{
		get
		{
			return get_LocationLeftBorder();
		}
		private set
		{
			HPKGOFCOGGM(value);
		}
	}

	public static float MCHJFJGLGNC
	{
		get
		{
			return get_CamZOffset();
		}
		private set
		{
			IECJPIDEPCD(value);
		}
	}

	public int GetInt(string JLCGLCLEGBD)
	{
		for (int i = 0; i < IntProperties.Count; i++)
		{
			if (IntProperties[i].Name.Equals(JLCGLCLEGBD))
			{
				return IntProperties[i].Value;
			}
		}
		return 0;
	}

	public float GetFloat(string JLCGLCLEGBD)
	{
		for (int i = 0; i < FloatProperties.Count; i++)
		{
			if (FloatProperties[i].Name.Equals(JLCGLCLEGBD))
			{
				return FloatProperties[i].Value;
			}
		}
		return 0f;
	}

	public string GetString(string JLCGLCLEGBD)
	{
		for (int i = 0; i < StringProperties.Count; i++)
		{
			if (StringProperties[i].Name.Equals(JLCGLCLEGBD))
			{
				return StringProperties[i].Value;
			}
		}
		return string.Empty;
	}

	public bool GetBool(string JLCGLCLEGBD)
	{
		for (int i = 0; i < BoolProperties.Count; i++)
		{
			if (BoolProperties[i].Name.Equals(JLCGLCLEGBD))
			{
				return BoolProperties[i].Value;
			}
		}
		return false;
	}

	public List<int> GetIntArray(string JLCGLCLEGBD)
	{
		for (int i = 0; i < IntArrayProperties.Count; i++)
		{
			if (IntArrayProperties[i].Name.Equals(JLCGLCLEGBD))
			{
				return IntArrayProperties[i].Value;
			}
		}
		return new List<int>();
	}

	public List<float> GetFloatArray(string JLCGLCLEGBD)
	{
		for (int i = 0; i < FloatArrayProperties.Count; i++)
		{
			if (FloatArrayProperties[i].Name.Equals(JLCGLCLEGBD))
			{
				return FloatArrayProperties[i].Value;
			}
		}
		return new List<float>();
	}

	public List<string> GetStringArray(string JLCGLCLEGBD)
	{
		for (int i = 0; i < StringArrayProperties.Count; i++)
		{
			if (StringArrayProperties[i].Name.Equals(JLCGLCLEGBD))
			{
				return StringArrayProperties[i].Value;
			}
		}
		return new List<string>();
	}

	public List<bool> GetBoolArray(string JLCGLCLEGBD)
	{
		for (int i = 0; i < BoolArrayProperties.Count; i++)
		{
			if (BoolArrayProperties[i].Name.Equals(JLCGLCLEGBD))
			{
				return BoolArrayProperties[i].Value;
			}
		}
		return new List<bool>();
	}

	public static SceneConfig get_Instance()
	{
		return EDAPJLKMFPC;
	}

	public static bool get_IsPresent()
	{
		return EDAPJLKMFPC;
	}

	public static float get_LeftBorderX()
	{
		return EDAPJLKMFPC.CDLEAEBCKDN;
	}

	public static void set_LeftBorderX(float value)
	{
		EDAPJLKMFPC.CDLEAEBCKDN = value;
	}

	public static float get_RightBorderX()
	{
		return EDAPJLKMFPC.BJCBBBHBOID;
	}

	public static void set_RightBorderX(float value)
	{
		EDAPJLKMFPC.BJCBBBHBOID = value;
	}

	public static float get_CenterX()
	{
		return EDAPJLKMFPC.ECICHHBFELI;
	}

	private static void HEPEBCNHAPD(float value)
	{
		EDAPJLKMFPC.ECICHHBFELI = value;
	}

	public static Vector3 get_SpawnPointEnemy()
	{
		return EDAPJLKMFPC.OFELPAHFIOJ;
	}

	private static void LENIOMGPDNB(Vector3 value)
	{
		EDAPJLKMFPC.OFELPAHFIOJ = value;
	}

	public static Vector3 get_SpawnPointPlayer()
	{
		return EDAPJLKMFPC.IFALGIHJHLN;
	}

	private static void PEIPFPAOCHB(Vector3 value)
	{
		EDAPJLKMFPC.IFALGIHJHLN = value;
	}

	public static float get_PointFloor()
	{
		return EDAPJLKMFPC.IFALGIHJHLN.y;
	}

	public static float get_MaxDistBetweenModels()
	{
		return EDAPJLKMFPC.JHPCPBHPDNJ;
	}

	private static void GLBMAFELMDI(float value)
	{
		EDAPJLKMFPC.JHPCPBHPDNJ = value;
	}

	public static float get_LocationRightBorder()
	{
		return EDAPJLKMFPC.FNICPCKHHAC;
	}

	private static void EBNMJIGMMOP(float value)
	{
		EDAPJLKMFPC.FNICPCKHHAC = value;
	}

	public static float get_LocationLeftBorder()
	{
		return EDAPJLKMFPC.BNJCCKOMHJH;
	}

	private static void HPKGOFCOGGM(float value)
	{
		EDAPJLKMFPC.BNJCCKOMHJH = value;
	}

	public static float get_CamZOffset()
	{
		return EDAPJLKMFPC.LEGKPOHHMEO;
	}

	private static void IECJPIDEPCD(float value)
	{
		EDAPJLKMFPC.LEGKPOHHMEO = value;
	}

	private void Awake()
	{
		EDAPJLKMFPC = this;
		if (!IsConfig)
		{
			float x = base.transform.Find(GetString("LeftBorder")).position.x;
			set_LeftBorderX(x);
			HPKGOFCOGGM(x);
			x = base.transform.Find(GetString("RightBorder")).position.x;
			set_RightBorderX(x);
			EBNMJIGMMOP(x);
			HEPEBCNHAPD((get_RightBorderX() + get_LeftBorderX()) / 2f);
			LENIOMGPDNB(base.transform.Find(GetString("SpawnPointA")).position);
			PEIPFPAOCHB(base.transform.Find(GetString("SpawnPointB")).position);
			GLBMAFELMDI(GetFloat("MaxDistBetweenModels"));
			IECJPIDEPCD(GetFloat("CamZOffset"));
		}
	}
}
