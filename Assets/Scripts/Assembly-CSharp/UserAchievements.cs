using System.Collections.Generic;
using System.Xml;

public class UserAchievements
{
	private XmlNode BEFBGGHPJFB;

	private XmlNode KMKPBELDMDA;

	private XmlNode FELLKEANCPA;

	private List<RosterAchievCounter> HFKGFPNAHAO = new List<RosterAchievCounter>();

	private List<RosterAchievement> EFAONOEAAIH = new List<RosterAchievement>();

	private List<RepostAchievement> KFMFEODLCOK = new List<RepostAchievement>();

	public List<RosterAchievCounter> FHGNFKGFNEL
	{
		get
		{
			return HOBHAAAEELG();
		}
	}

	public List<RosterAchievement> FOICCCGPCMJ
	{
		get
		{
			return NOJKMMJJPHF();
		}
	}

	public List<RepostAchievement> DKBINLMJIJG
	{
		get
		{
			return NEEDPAEIOLH();
		}
	}

	public int GNKJNICHNOM
	{
		get
		{
			return JKGGEMEBPCP();
		}
	}

	public List<RosterAchievCounter> HOBHAAAEELG()
	{
		return HFKGFPNAHAO;
	}

	public List<RosterAchievement> NOJKMMJJPHF()
	{
		return EFAONOEAAIH;
	}

	public List<RepostAchievement> NEEDPAEIOLH()
	{
		return KFMFEODLCOK;
	}

	public int JKGGEMEBPCP()
	{
		int num = 0;
		List<AchievCounter> mDNKEAFGAOB = GameUtils.HHLEKNNJGMJ.MDNKEAFGAOB;
		for (int i = 0; i < mDNKEAFGAOB.Count; i++)
		{
			List<Achievement> fOICCCGPCMJ = mDNKEAFGAOB[i].FOICCCGPCMJ;
			for (int j = 0; j < fOICCCGPCMJ.Count; j++)
			{
				if (fOICCCGPCMJ[j].DBHJGAGOLOB())
				{
					num++;
				}
			}
		}
		return num;
	}

	public void Parse(XmlNode node)
	{
		BEFBGGHPJFB = node["Counters"];
		if (BEFBGGHPJFB == null)
		{
			BEFBGGHPJFB = node.ACBPMPMPKJJ("Counters");
		}
		foreach (XmlNode childNode in BEFBGGHPJFB.ChildNodes)
		{
			HFKGFPNAHAO.Add(new RosterAchievCounter(childNode));
		}
		KMKPBELDMDA = node["Achievements"];
		if (KMKPBELDMDA == null)
		{
			KMKPBELDMDA = node.ACBPMPMPKJJ("Achievements");
		}
		foreach (XmlNode childNode2 in KMKPBELDMDA.ChildNodes)
		{
			HDDNEDBMAAA(new RosterAchievement(childNode2));
		}
		FELLKEANCPA = node["RepostAchievements"];
		if (FELLKEANCPA == null)
		{
			FELLKEANCPA = node.ACBPMPMPKJJ("RepostAchievements");
		}
		foreach (XmlNode childNode3 in FELLKEANCPA.ChildNodes)
		{
			KFMFEODLCOK.Add(new RepostAchievement(childNode3));
		}
	}

	public RosterAchievCounter KJPLIHEMLJL(string name)
	{
		for (int i = 0; i < HFKGFPNAHAO.Count; i++)
		{
			if (HFKGFPNAHAO[i].get_Name() == name)
			{
				return HFKGFPNAHAO[i];
			}
		}
		return null;
	}

	public void BFCLLIKOJGD()
	{
		GameUtils.AchievementCounters oJNHPHEPFLI = GameUtils.OJNHPHEPFLI;
		bool flag = false;
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, Counter> item in oJNHPHEPFLI.LLMLCLKNAAN)
		{
			Counter value = item.Value;
			if (value.CompleteValue > 0)
			{
				list.Add(value.Name);
				int num = 0;
				RosterAchievCounter cKJBHGKBPPM = KJPLIHEMLJL(value.Name);
				if (cKJBHGKBPPM != null)
				{
					num = cKJBHGKBPPM.MCIPEJBLIDC() + value.CompleteValue;
					if (value.Type == "WinBattle")
					{
						num = ((num > 1) ? 1 : num);
					}
					cKJBHGKBPPM.set_Counter(num);
					flag = true;
				}
				else
				{
					num = value.CompleteValue;
					CreateRosterAchievCounter(value.Name, num);
					flag = true;
				}
			}
			value.CPMPOPHBFKJ();
		}
		List<global::Pair<Achievement, int>> cIMGCGDDKCE = GameUtils.HHLEKNNJGMJ.DLACNJLPKBK(list);
		GameUtils.NKGCBJAAJMA(cIMGCGDDKCE);
		if (flag)
		{
			ListSF.CCDKHLAMKKO().GGGEHAGCLGC();
		}
	}

	public RosterAchievement JABBCCJLOOC(string name)
	{
		for (int i = 0; i < EFAONOEAAIH.Count; i++)
		{
			if (EFAONOEAAIH[i].get_Name() == name)
			{
				return EFAONOEAAIH[i];
			}
		}
		return null;
	}

	public void CAEKPHDIGDA(RosterAchievement PGAGNLJABIE, bool POHFOGPKMMK, bool NLCCJEHMAOF = true)
	{
		if (PGAGNLJABIE.BLHBOBGKMBN() != POHFOGPKMMK)
		{
			PGAGNLJABIE.set_Reward(POHFOGPKMMK);
			if (NLCCJEHMAOF)
			{
				ListSF.CCDKHLAMKKO().GGGEHAGCLGC();
			}
		}
	}

	public void POKNGJJAHAL(Achievement NCCHENOEPNF, bool POHFOGPKMMK = true, bool NLCCJEHMAOF = true)
	{
		if (NCCHENOEPNF == null)
		{
			return;
		}
		string mENAJEAJJBE = NCCHENOEPNF.Name;
		for (int i = 0; i < EFAONOEAAIH.Count; i++)
		{
			RosterAchievement pMGCOHHMIIC = EFAONOEAAIH[i];
			if (mENAJEAJJBE == pMGCOHHMIIC.get_Name())
			{
				CAEKPHDIGDA(pMGCOHHMIIC, POHFOGPKMMK, NLCCJEHMAOF);
				return;
			}
		}
		string jLEKBBJBLOE = "Achievement";
		XmlNode hKPPBKPJOEO = KMKPBELDMDA.ACBPMPMPKJJ(jLEKBBJBLOE);
		RosterAchievement pMGCOHHMIIC2 = new RosterAchievement(hKPPBKPJOEO);
		pMGCOHHMIIC2.set_Name(mENAJEAJJBE);
		pMGCOHHMIIC2.set_Reward(POHFOGPKMMK);
		HDDNEDBMAAA(pMGCOHHMIIC2, NCCHENOEPNF);
		ArgsDict kEMMIFBFDPK = new ArgsDict();
		kEMMIFBFDPK["name"] = mENAJEAJJBE;
		StatisticsCollector.BPDGOKGHDHB(StatisticsEvent.JDNFFHILFAF.Achievement, kEMMIFBFDPK);
		if (NLCCJEHMAOF)
		{
			ListSF.CCDKHLAMKKO().GGGEHAGCLGC();
		}
	}

	public bool CreateRepostAchievement(string OGPJPGMBIHJ)
	{
		for (int i = 0; i < KFMFEODLCOK.Count; i++)
		{
			RepostAchievement aFOGJMECGBG = KFMFEODLCOK[i];
			if (aFOGJMECGBG.get_Name() == OGPJPGMBIHJ)
			{
				return false;
			}
		}
		KFMFEODLCOK.Add(new RepostAchievement(FELLKEANCPA, OGPJPGMBIHJ));
		return true;
	}

	public bool ANBCFNBEDMH(RepostAchievement NCCHENOEPNF)
	{
		int num = 0;
		foreach (XmlNode childNode in FELLKEANCPA.ChildNodes)
		{
			string text = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
			if (text == NCCHENOEPNF.get_Name())
			{
				FELLKEANCPA.RemoveChild(childNode);
				KFMFEODLCOK.RemoveAt(num);
				return true;
			}
			num++;
		}
		return false;
	}

	public void AddRepostAchievements(List<string> DODEADGDJCM)
	{
		bool flag = false;
		for (int i = 0; i < DODEADGDJCM.Count; i++)
		{
			flag = CreateRepostAchievement(DODEADGDJCM[i]);
		}
		if (flag)
		{
			ListSF.CCDKHLAMKKO().GGGEHAGCLGC();
		}
	}

	public void PEBJNEJLONK(List<RepostAchievement> MGNCKHDDHLE)
	{
		bool flag = false;
		for (int i = 0; i < MGNCKHDDHLE.Count; i++)
		{
			flag = ANBCFNBEDMH(MGNCKHDDHLE[i]);
		}
		if (flag)
		{
			ListSF.CCDKHLAMKKO().GGGEHAGCLGC();
		}
	}

	private void CreateRosterAchievCounter(string name, int value)
	{
		XmlNode hKPPBKPJOEO = BEFBGGHPJFB.ACBPMPMPKJJ("Counter");
		RosterAchievCounter cKJBHGKBPPM = new RosterAchievCounter(hKPPBKPJOEO);
		cKJBHGKBPPM.set_Name(name);
		cKJBHGKBPPM.set_Counter(value);
		HFKGFPNAHAO.Add(cKJBHGKBPPM);
	}

	private void HDDNEDBMAAA(RosterAchievement BCIJIDMGJLC, Achievement NCCHENOEPNF = null)
	{
		Achievement jNPIOKEKMII = ((NCCHENOEPNF == null) ? GameUtils.HHLEKNNJGMJ.ABNAODNDHDM(BCIJIDMGJLC.get_Name()) : NCCHENOEPNF);
		if (jNPIOKEKMII != null)
		{
			jNPIOKEKMII.HGMHEOGJDMM = true;
			jNPIOKEKMII.NMCBAKACIGK = BCIJIDMGJLC.BLHBOBGKMBN();
			jNPIOKEKMII.BEBDMOEIEJN(!BCIJIDMGJLC.BLHBOBGKMBN());
		}
		EFAONOEAAIH.Add(BCIJIDMGJLC);
	}
}
