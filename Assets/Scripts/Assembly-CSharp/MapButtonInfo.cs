using UnityEngine;

public class MapButtonInfo
{
	public enum FOMHAJMHHJK
	{
		IMAGE = 0,
		SEQUENCE = 1
	}

	public enum HNEJAKIGDBA
	{
		Both = 0,
		Story = 1,
		Raid = 2
	}

	public string Name = string.Empty;

	public string NHKMCLPOMFK = string.Empty;

	public string Timer = string.Empty;

	public string HFBFPBGLBOM = string.Empty;

	public string KMEDBHDDDJA = string.Empty;

	public Vector2 BIJFFONMDBC = default(Vector2);

	// Newer gamedata positions some buttons relative to a canvas edge.  The
	// decompiled runtime had lost these XML fields and treated every position as
	// canvas-centred, which moved the Eclipse switch far to the left.
	public float AnchorMinX = 0.5f;

	public float AnchorMaxX = 0.5f;

	public bool NEOIMNAHLAN;

	public float BOEJEFCDIAD;

	public float Pause;

	public string MLKPBAALMBC = string.Empty;

	public MapButtonInfo()
	{
	}

	public MapButtonInfo(string _name, string NCKCDCODNHA, string KMFDBBKMLOO, Vector2 LGDMCAAHPOC, bool _AutoPosition = false, string Atlas = "", string IOKOBBFCIGE = "IMAGE", float AMEGCDJDGPB = 0f, float JDDJEAGMNMP = 0f, string BFBFKHHANJG = "Story", float anchorMinX = 0.5f, float anchorMaxX = 0.5f)
	{
		Name = _name;
		NHKMCLPOMFK = NCKCDCODNHA;
		Timer = KMFDBBKMLOO;
		HFBFPBGLBOM = Atlas;
		KMEDBHDDDJA = IOKOBBFCIGE;
		BIJFFONMDBC = LGDMCAAHPOC;
		NEOIMNAHLAN = _AutoPosition;
		BOEJEFCDIAD = AMEGCDJDGPB;
		Pause = JDDJEAGMNMP;
		MLKPBAALMBC = BFBFKHHANJG;
		AnchorMinX = anchorMinX;
		AnchorMaxX = anchorMaxX;
	}

	public FOMHAJMHHJK KCADIADFGDJ()
	{
		FOMHAJMHHJK result = FOMHAJMHHJK.IMAGE;
		if (KMEDBHDDDJA == "Sequence")
		{
			result = FOMHAJMHHJK.SEQUENCE;
		}
		return result;
	}

	public HNEJAKIGDBA EDMILHNJFAA()
	{
		HNEJAKIGDBA result = HNEJAKIGDBA.Both;
		if (MLKPBAALMBC == "Story")
		{
			result = HNEJAKIGDBA.Story;
		}
		if (MLKPBAALMBC == "Raid")
		{
			result = HNEJAKIGDBA.Raid;
		}
		return result;
	}
}
