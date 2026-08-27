using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ModelShiftTable : List<List<float>>
{
	private const string CGKGCCGKKNI = "assets/tactics/shift/";

	private const string HBGKLIGDCKI = ".stb";

	private List<string> _NodeNames = new List<string>();

	private InfoAnimation _Animation;

	public List<string> IFOCBIGHBBP
	{
		get
		{
			return OJCPLCLNLPI();
		}
	}

	public InfoAnimation FGICHADOEHF
	{
		get
		{
			return NNMAFFCCMHC();
		}
	}

	public ModelShiftTable()
	{
		_Animation = null;
	}

	public List<string> OJCPLCLNLPI()
	{
		return _NodeNames;
	}

	public InfoAnimation NNMAFFCCMHC()
	{
		return _Animation;
	}

	public int GetNodeId(string IMGCANJHPND)
	{
		int num = 0;
		for (int i = 0; i < _NodeNames.Count; i++)
		{
			if (IMGCANJHPND == _NodeNames[i])
			{
				return num;
			}
			num++;
		}
		if (_Animation != null)
		{
			LLLOJBFMONN.Error("heel {0} not found in shift table for {1}", IMGCANJHPND, _Animation.Name);
		}
		return -1;
	}

	public void LoadFromFile(InfoAnimation DBOLBEOCEME, BinaryReader buffer)
	{
		_Animation = DBOLBEOCEME;
		uint pEEOEOMEBFG = buffer.ReadUInt32();
		LoadFromFile(buffer, (int)pEEOEOMEBFG);
	}

	private void LoadFromFile(BinaryReader buffer, int PEEOEOMEBFG)
	{
		if (0 < PEEOEOMEBFG)
		{
			int num = ParseTableHeader(buffer);
			int count = _NodeNames.Count;
			int num2 = PEEOEOMEBFG - num;
			if (num2 % 4 != 0)
			{
				LLLOJBFMONN.Error("count % 4 != 0");
			}
			if (num2 / 4 % count != 0)
			{
				LLLOJBFMONN.Error("count % nodeCount != 0");
			}
			int num3 = num2 / 4;
			List<float> list = new List<float>(num3);
			for (int i = 0; i < num3; i++)
			{
				list.Add(buffer.ReadSingle());
			}
			num2 = num3 / count;
			base.Capacity = num2;
			int num4 = 0;
			for (int j = 0; j < base.Count; j++)
			{
				Add(list.GetRange(num4, count));
				num4 += count;
			}
			list = null;
		}
	}

	private int ParseTableHeader(BinaryReader buffer)
	{
		int num = 4;
		uint num2 = buffer.ReadUInt32();
		_NodeNames.Clear();
		for (int i = 0; i < num2; i++)
		{
			string text = TacticalTableHolder.LNOMEMJCIAM(buffer);
			num += text.Length + 1;
			_NodeNames.Add(text);
		}
		return num;
	}

	public float GetDistance(int JAPBDIJOKDJ, string IMGCANJHPND)
	{
		if (JAPBDIJOKDJ < base.Count)
		{
			int num = GetNodeId(IMGCANJHPND);
			if (-1 < num)
			{
				return this.ElementAt(JAPBDIJOKDJ)[num];
			}
			LLLOJBFMONN.Error("node {1} not found", IMGCANJHPND);
		}
		return 0f;
	}
}
