using System.Collections.Generic;
using UnityEngine;

public class TexturesUtils
{
	private static readonly Dictionary<string, Sprite[]> AtlasesCache = new Dictionary<string, Sprite[]>();

	private static readonly Dictionary<string, string> AtlasesNames = new Dictionary<string, string>();

	private static readonly Dictionary<int, int> NJHEFKILICK = new Dictionary<int, int>();

	private static readonly List<Texture> NHNONLFMFDC = new List<Texture>();

	public static void Init()
	{
		Routiner.AddUpdate(JLPMOKPFECK);
	}

	public static Sprite CreateSprite(Texture2D texture)
	{
		return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
	}

	public static int GetCountTexture(Texture texture)
	{
		if (texture != null)
		{
			int instanceID = texture.GetInstanceID();
			if (NJHEFKILICK.ContainsKey(instanceID))
			{
				return NJHEFKILICK[instanceID];
			}
			if (NHNONLFMFDC.Contains(texture))
			{
				return 0;
			}
		}
		return -1;
	}

	public static void NOABEDJAGHG(Texture texture)
	{
		if (texture == null)
		{
			return;
		}
		int instanceID = texture.GetInstanceID();
		if (NJHEFKILICK.ContainsKey(instanceID))
		{
			NJHEFKILICK[instanceID]++;
		}
		else
		{
			if (NHNONLFMFDC.Contains(texture))
			{
				NHNONLFMFDC.Remove(texture);
			}
			NJHEFKILICK.Add(instanceID, 1);
		}
		Log("AddTexture " + texture.name + " " + GetCountTexture(texture));
	}

	public static void OFEDABNDEAF(Texture texture)
	{
		if (texture == null)
		{
			return;
		}
		int instanceID = texture.GetInstanceID();
		if (NJHEFKILICK.ContainsKey(instanceID))
		{
			if (NJHEFKILICK[instanceID] > 1)
			{
				NJHEFKILICK[instanceID]--;
			}
			else
			{
				NJHEFKILICK.Remove(instanceID);
				NHNONLFMFDC.Add(texture);
			}
			Log("ReleaseTexture " + texture.name + " " + GetCountTexture(texture));
		}
	}

	private static void JLPMOKPFECK()
	{
		if (NHNONLFMFDC.Count <= 0)
		{
			return;
		}
		Texture texture = NHNONLFMFDC[0];
		if (texture != null)
		{
			if (AtlasesNames.ContainsKey(texture.name))
			{
				AtlasesCache.Remove(AtlasesNames[texture.name]);
				AtlasesNames.Remove(texture.name);
				Log("UnloadAtlas " + texture.name);
			}
			Log("DestroyTexture " + texture.name);
			GlobalLoad.BPEDLFOKKNN(texture);
		}
		NHNONLFMFDC.Remove(texture);
	}

	public static Sprite GetSpriteFromAtlas(string NJKCBALJDMM, string KIKMPCLOBCK, string JGIGOMLGLPN)
	{
		return GetSpriteFromAtlas(LoadAtlas(NJKCBALJDMM, KIKMPCLOBCK), JGIGOMLGLPN);
	}

	public static Sprite GetSpriteFromAtlas(string KIKMPCLOBCK, string JGIGOMLGLPN)
	{
		return GetSpriteFromAtlas(LoadAtlas(KIKMPCLOBCK, string.Empty), JGIGOMLGLPN);
	}

	private static Sprite GetSpriteFromAtlas(Sprite[] KIPBMMFEBEE, string JGIGOMLGLPN)
	{
		if (KIPBMMFEBEE != null && KIPBMMFEBEE.Length > 0)
		{
			foreach (Sprite sprite in KIPBMMFEBEE)
			{
				if (sprite.name.Equals(JGIGOMLGLPN))
				{
					return sprite;
				}
			}
		}
		Log("Sprite From Atlas Not Found  - " + JGIGOMLGLPN);
		return GlobalLoad.IHDKNNHOPFJ();
	}

	private static Sprite[] LoadAtlas(string path, string name = "")
	{
		if (!AtlasesCache.ContainsKey(path))
		{
			Sprite[] array = ((!name.BKOIKMEEHDK()) ? GlobalLoad.GetLoadObjectsInternal<Sprite>(path, name) : GlobalLoad.DKIOHNLLACG<Sprite>(path));
			if (array != null && array.Length > 0)
			{
				AtlasesCache.Add(path, array);
				AtlasesNames.Add(array[0].texture.name, path);
				Log("LoadAtlas " + path);
			}
			return array;
		}
		return AtlasesCache[path];
	}

	private static void Log(string LIOGIBJBHAH)
	{
	}
}
