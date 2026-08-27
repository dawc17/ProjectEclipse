using System.Collections.Generic;
using System.IO;

public class TacticalTableHolder
{
	private class AnimationTablesForAnimation
	{
		public InfoAnimation FGICHADOEHF;

		public List<GroupTables> Container = new List<GroupTables>();
	}

	private List<GroupTables> MELLNDMKFLD = new List<GroupTables>();

	private List<IntervalNew> LLBNHMGFIAN;

	private List<TacticalTable> ILDNLDJLPBB;

	private List<Intervals> PNKLJFBOJII;

	private List<float> _distances;

	private List<int> _interframes;

	private List<AnimationTablesForAnimation> KMDJOANFEMG;

	private int _tableIndex;

	private string _weaponName;

	public void Load(byte[] buffer, int KOHGHADGPCE, string NBJPDCDOOKH)
	{
		int num = buffer.Length;
		if (0 >= num)
		{
			return;
		}
		List<InfoAnimation> list = new List<InfoAnimation>();
		List<string> list2 = new List<string>();
		using (MemoryStream input = new MemoryStream(buffer))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				FEAJDBHBNCC(binaryReader, list);
				ReadWeaponTypeList(binaryReader, list2);
				PMAKBKHCGBC(binaryReader, list, list2, KOHGHADGPCE, NBJPDCDOOKH);
				if (binaryReader.BaseStream.Length != binaryReader.BaseStream.Position)
				{
					LLLOJBFMONN.Error("pointer != buffer.end()");
				}
			}
		}
	}

	public void DLEINJHGIIL()
	{
		foreach (AnimationTablesForAnimation item in KMDJOANFEMG)
		{
			item.FGICHADOEHF.NLCLHLIPFFH()[_tableIndex].Add(new global::Pair<List<GroupTables>, string>(item.Container, _weaponName));
		}
	}

	public void Clear()
	{
		KMDJOANFEMG.Clear();
		MELLNDMKFLD.Clear();
		ILDNLDJLPBB.Clear();
		PNKLJFBOJII.Clear();
		_distances.Clear();
		_interframes.Clear();
	}

	public bool Empty()
	{
		if (MELLNDMKFLD.Count == 0 || ILDNLDJLPBB.Count == 0 || PNKLJFBOJII.Count == 0 || _distances.Count == 0)
		{
			LLLOJBFMONN.Write("Empty tables detected!");
			return true;
		}
		return false;
	}

	private static void FEAJDBHBNCC(BinaryReader NNGPBPLGEOK, List<InfoAnimation> MAHEJFLCCHP)
	{
		int num = (MAHEJFLCCHP.Capacity = NNGPBPLGEOK.ReadUInt16());
		byte[] array = NNGPBPLGEOK.ReadBytes(num);
		for (int i = 0; i < num; i++)
		{
			byte count = array[i];
			string gOHIIMFFFJI = new string(NNGPBPLGEOK.ReadChars(count));
			MAHEJFLCCHP.Add(AnimationData.BCIFKBJAFEC(gOHIIMFFFJI, AiData.get_IsShowErrorIfAnimationNotFound()));
		}
	}

	private static void ReadWeaponTypeList(BinaryReader NNGPBPLGEOK, List<string> NFMICLFEKJD)
	{
		short num = NNGPBPLGEOK.ReadInt16();
		byte[] array = NNGPBPLGEOK.ReadBytes(num);
		for (int i = 0; i < num; i++)
		{
			byte count = array[i];
			NFMICLFEKJD.Add(new string(NNGPBPLGEOK.ReadChars(count)));
		}
	}

	private void PMAKBKHCGBC(BinaryReader LEOMHBCGLKI, List<InfoAnimation> CHNJHIPHIHA, List<string> IMOCIDBCFBA, int GAMDIAAJJMC, string EILBAKBJCIJ)
	{
		_tableIndex = GAMDIAAJJMC;
		_weaponName = EILBAKBJCIJ;
		int num = (int)LEOMHBCGLKI.ReadUInt32();
		int num2 = (int)LEOMHBCGLKI.ReadUInt32();
		MELLNDMKFLD = new List<GroupTables>(num2);
		for (int i = 0; i < num2; i++)
		{
			MELLNDMKFLD.Add(new GroupTables());
		}
		int num3 = (int)LEOMHBCGLKI.ReadUInt32();
		ILDNLDJLPBB = new List<TacticalTable>(num3);
		for (int j = 0; j < num3; j++)
		{
			ILDNLDJLPBB.Add(new TacticalTable());
		}
		int num4 = (int)LEOMHBCGLKI.ReadUInt32();
		PNKLJFBOJII = new List<Intervals>(num4);
		for (int k = 0; k < num4; k++)
		{
			PNKLJFBOJII.Add(new Intervals());
		}
		int num5 = (int)LEOMHBCGLKI.ReadUInt32();
		LLBNHMGFIAN = new List<IntervalNew>(num5);
		for (int l = 0; l < num5; l++)
		{
			LLBNHMGFIAN.Add(new IntervalNew());
		}
		int num6 = (int)LEOMHBCGLKI.ReadUInt32();
		int num7 = (int)LEOMHBCGLKI.ReadUInt32();
		KMDJOANFEMG = new List<AnimationTablesForAnimation>(num);
		for (int m = 0; m < num; m++)
		{
			ushort num8 = LEOMHBCGLKI.ReadUInt16();
			KMDJOANFEMG.Add(new AnimationTablesForAnimation());
			if (num8 < CHNJHIPHIHA.Count)
			{
				KMDJOANFEMG[m].FGICHADOEHF = CHNJHIPHIHA[num8];
			}
			else
			{
				KMDJOANFEMG[m].FGICHADOEHF = null;
			}
		}
		short num9 = LEOMHBCGLKI.ReadInt16();
		_distances = new List<float>(num6);
		for (int n = 0; n < num6; n++)
		{
			_distances.Add((num9 == 0) ? LEOMHBCGLKI.ReadInt16() : (num9 * LEOMHBCGLKI.ReadInt16()));
		}
		_interframes = new List<int>(num7);
		for (int num10 = 0; num10 < num7; num10++)
		{
			_interframes.Add(LEOMHBCGLKI.ReadInt32());
		}
		List<InfoAnimation> list = new List<InfoAnimation>(num5);
		int num11 = 0;
		int num12 = 0;
		for (int num13 = num5; num12 < num13; num12++)
		{
			list.Add(CHNJHIPHIHA[LEOMHBCGLKI.ReadUInt16()]);
			if (list[num12] == null)
			{
				num11++;
			}
		}
		List<InfoAnimation> list2 = new List<InfoAnimation>(num11);
		for (int num14 = 0; num14 < num11; num14++)
		{
			list2.Add(new InfoAnimation());
		}
		int num15 = 0;
		int num16 = 0;
		int num17 = 0;
		int num18 = 0;
		int num19 = 0;
		int num20 = 0;
		int num21 = 0;
		foreach (AnimationTablesForAnimation item in KMDJOANFEMG)
		{
			ushort num22 = LEOMHBCGLKI.ReadUInt16();
			if (item.FGICHADOEHF == null)
			{
				item.FGICHADOEHF = new InfoAnimation();
				list2.Add(item.FGICHADOEHF);
			}
			item.Container = MELLNDMKFLD.GetRange(num15, num22);
			num15 += num22;
			foreach (GroupTables item2 in item.Container)
			{
				ushort index = LEOMHBCGLKI.ReadUInt16();
				item2.GroupLabel = IMOCIDBCFBA[index];
				num22 = LEOMHBCGLKI.ReadUInt16();
				item2.DOCMMNLEAMH = ILDNLDJLPBB.GetRange(num16, num22);
				num16 += num22;
				foreach (TacticalTable item3 in item2.DOCMMNLEAMH)
				{
					item3.Label = LNOMEMJCIAM(LEOMHBCGLKI);
					num22 = LEOMHBCGLKI.ReadUInt16();
					if (0 < num22)
					{
						item3.OCFKLCDIEBF = PNKLJFBOJII.GetRange(num17, num22);
						num17 += num22;
						item3.FirstFrameIndex = LEOMHBCGLKI.ReadInt16();
						foreach (Intervals item4 in item3.OCFKLCDIEBF)
						{
							ushort num23 = LEOMHBCGLKI.ReadUInt16();
							item4.MFFPCMPGEBK = LLBNHMGFIAN.GetRange(num21, num23);
							num21 += num23;
							if (0 >= num23)
							{
								continue;
							}
							foreach (IntervalNew item5 in item4.MFFPCMPGEBK)
							{
								item5.FGICHADOEHF = list[num19];
								num19++;
							}
							foreach (IntervalNew item6 in item4.MFFPCMPGEBK)
							{
								ushort num24 = LEOMHBCGLKI.ReadUInt16();
								item6.Distances = _distances.GetRange(num18, num24);
								num18 += num24;
							}
							foreach (IntervalNew item7 in item4.MFFPCMPGEBK)
							{
								ushort num25 = LEOMHBCGLKI.ReadUInt16();
								item7.Interframes = _interframes.GetRange(num20, num25);
								num20 += num25;
							}
						}
					}
					else
					{
						item3.OCFKLCDIEBF.Clear();
						item3.FirstFrameIndex = 0;
					}
				}
			}
		}
		if (num15 != MELLNDMKFLD.Count)
		{
			LLLOJBFMONN.Error("pGroups != _groups.end()");
		}
		if (num16 != ILDNLDJLPBB.Count)
		{
			LLLOJBFMONN.Error("pTables != _tables.end()");
		}
		if (num17 != PNKLJFBOJII.Count)
		{
			LLLOJBFMONN.Error("pFrames != _intervals.end()");
		}
		if (num18 != _distances.Count)
		{
			LLLOJBFMONN.Error("pDistances != _distances.end()");
		}
		if (num19 != list.Count)
		{
			LLLOJBFMONN.Error("pAnimations != _animations.end()");
		}
		if (num20 != _interframes.Count)
		{
			LLLOJBFMONN.Error("pInterframes != _interframes.end()");
		}
		if (num21 != LLBNHMGFIAN.Count)
		{
			LLLOJBFMONN.Error("pAnimIntervals != _animationsWithIntervals.end()");
		}
		foreach (InfoAnimation item8 in list2)
		{
			foreach (AnimationTablesForAnimation item9 in KMDJOANFEMG)
			{
				if (item9.FGICHADOEHF == item8)
				{
					KMDJOANFEMG.Remove(item9);
					LLLOJBFMONN.Write(string.Format("AnimationTablesForAnimation remove for %s", item8.Name));
					break;
				}
			}
		}
	}

	public static string LNOMEMJCIAM(BinaryReader NNGPBPLGEOK)
	{
		string text = string.Empty;
		for (char c = NNGPBPLGEOK.ReadChar(); c != 0; c = NNGPBPLGEOK.ReadChar())
		{
			text += c;
		}
		return text;
	}
}
