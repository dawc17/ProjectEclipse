using System;
using System.Collections.Generic;
using Nekki.SF2.Core.Network;
using SimpleJSON;

public class GiveLogin
{
	public bool DCHJDPCEODD;

	public bool GMBOPFIPNAE;

	public long OHHLCBPGOIM;

	public long JDPAGMPKLHB;

	public List<GiveItemLogin> OJIAKDDCGLB = new List<GiveItemLogin>();

	public void Parse(JSONNode value)
	{
		JSONNode jSONNode = null;
		JSONNode jSONNode2 = value["data"];
		if (jSONNode2 != null && jSONNode2.Value.Equals("user"))
		{
			JSONNode jSONNode3 = value["value"];
			if (jSONNode3 != null)
			{
				jSONNode = jSONNode3["gives"];
			}
		}
		if (!(jSONNode != null) || jSONNode.Count <= 0)
		{
			return;
		}
		JSONNode mEEAKLDGLDF = jSONNode["Bonus"];
		JSONNode mEEAKLDGLDF2 = jSONNode["Money"];
		JSONNode jSONNode4 = jSONNode["Items"];
		long oHHLCBPGOIM = mEEAKLDGLDF.ParseLong(0L);
		long jDPAGMPKLHB = mEEAKLDGLDF2.ParseLong(0L);
		int num = ((jSONNode4 != null) ? jSONNode4.Count : 0);
		int i = 0;
		for (int num2 = num; i < num2; i++)
		{
			JSONNode jSONNode5 = jSONNode4[i];
			JSONNode jSONNode6 = jSONNode5["Item"];
			JSONNode mEEAKLDGLDF3 = jSONNode5["UpgradeLevel"];
			JSONNode mEEAKLDGLDF4 = jSONNode5["Count"];
			JSONNode mEEAKLDGLDF5 = jSONNode5["Equip"];
			if (jSONNode6 != null && jSONNode6.Value != null)
			{
				string valueText = jSONNode6.Value;
				int aKKLOMFOLNO = mEEAKLDGLDF3.ParseInt();
				int num3 = mEEAKLDGLDF4.ParseInt(1);
				int num4 = mEEAKLDGLDF5.ParseInt();
				if (num3 > 0)
				{
					GiveItemLogin item = new GiveItemLogin
					{
						Name = valueText,
						AKKLOMFOLNO = aKKLOMFOLNO,
						Count = num3,
						Equip = (num4 > 0)
					};
					OJIAKDDCGLB.Add(item);
				}
			}
		}
		OHHLCBPGOIM = oHHLCBPGOIM;
		JDPAGMPKLHB = jDPAGMPKLHB;
		DCHJDPCEODD = true;
	}

	public void PGAJKMOPDIJ()
	{
		GMBOPFIPNAE = false;
		if (DCHJDPCEODD)
		{
			ServerProvider.get_Instance().SendGiveLogin(MELKJFNJGGP);
		}
	}

	private void MELKJFNJGGP(bool DCJLKCFKCOM, string data, object IEHMCKBJCAK)
	{
		if (DCJLKCFKCOM)
		{
			JSONNode jSONNode = JSON.Parse(data)["data"];
			if (jSONNode != null && jSONNode.Value.Equals("success"))
			{
				OEJJNNMGOHO();
			}
		}
	}

	private void OEJJNNMGOHO()
	{
		Roster GJJHILBJOGF = ListSF.CCDKHLAMKKO();
		if (JDPAGMPKLHB != 0)
		{
			GJJHILBJOGF.OIOOMAKNIOB(Math.Max(0L, GJJHILBJOGF.BFBOEGMAMNF() + JDPAGMPKLHB));
		}
		if (OHHLCBPGOIM != 0)
		{
			GJJHILBJOGF.LLNELLFMMBB(Math.Max(0L, GJJHILBJOGF.EHFJHFDACMP() + OHHLCBPGOIM), Roster.HPOIJPGPOCF.CHANGE_SERVER_GIVE);
		}
		if (JDPAGMPKLHB != 0 || OHHLCBPGOIM != 0)
		{
			MenuController.IAMGKKOINFC();
		}
		foreach (GiveItemLogin item in OJIAKDDCGLB)
		{
			ListSF.DJBOFEEKJMP().CKCMJAJAELO(item.Name).ForEach((ItemInfo PJDAGCBPLJE) =>
			{
				if (item.Equip)
				{
					ListSF.FAAAGBACKAE(PJDAGCBPLJE);
				}
				ListSF.GEFDJDIINND(PJDAGCBPLJE, item.Count, 0L, item.Equip);
				if (PJDAGCBPLJE.MHGODOLNDLE <= GJJHILBJOGF.PINDEKDNCNL())
				{
					PJDAGCBPLJE.BEBDMOEIEJN(true);
				}
				if (item.AKKLOMFOLNO > 0)
				{
					PJDAGCBPLJE.OBJDGBBFJOO = item.AKKLOMFOLNO;
					ItemInfo HDMHCCKLLGK = null;
					ItemInfo JLNLOCNBGEK = null;
					PJDAGCBPLJE.NHJAHNDOLAE(GJJHILBJOGF.PINDEKDNCNL(), item.AKKLOMFOLNO, ref HDMHCCKLLGK, ref JLNLOCNBGEK);
				}
			});
		}
		DCHJDPCEODD = false;
		GMBOPFIPNAE = OHHLCBPGOIM != 0 || JDPAGMPKLHB != 0 || OJIAKDDCGLB.Count > 0;
		if (ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SERVER_CURRENCY))
		{
			ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
		}
	}
}
