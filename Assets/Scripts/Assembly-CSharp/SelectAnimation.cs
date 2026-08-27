using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class SelectAnimation
{
	private class TriggerStruct
	{
		public Trigger FEDHCBGNJIM;

		public Model KJDFJPBIGJC;

		public TriggerStruct(Trigger CPBHKJFPFJB, Model ACENLMONNPA)
		{
			FEDHCBGNJIM = CPBHKJFPFJB;
			KJDFJPBIGJC = ACENLMONNPA;
		}
	}

	public class SelectInfo
	{
		public InfoAnimation FGICHADOEHF;

		public int GFHOIKMBNHF;

		public bool IsHit;

		public bool IsRandom;

		public int Index;

		public EventAnimation.EECEJKADLCK DJPLGDJCMPI;

		public EventAnimation AALMPLPGCHA;

		public SelectInfo()
		{
			FGICHADOEHF = null;
			GFHOIKMBNHF = 0;
			IsHit = false;
			IsRandom = false;
			Index = 0;
			AALMPLPGCHA = null;
			DJPLGDJCMPI = EventAnimation.EECEJKADLCK.EVENT_NONE;
		}
	}

	private int _FrameRound;

	private List<Model> BPIFJBJBKHA = new List<Model>();

	private List<Model> HKOBFBADDJN = new List<Model>();

	private List<ModelConditions> _ModelsConditions = new List<ModelConditions>();

	private List<EventModelDelayed> CFKGCLIKKOC = new List<EventModelDelayed>();

	private List<EventModelDelayed> KPHAPCNOPNP = new List<EventModelDelayed>();

	private List<TriggerStruct> IJIHPHBMEOI = new List<TriggerStruct>();

	private List<List<SelectInfo>> AGKKIGDCPOD = new List<List<SelectInfo>>();

	private List<List<SelectInfo>> BIEHAGCAHEL = new List<List<SelectInfo>>();

	private List<Model> _ExplicitBirthModels = new List<Model>();

	public List<Model> LNDLFINJHDB
	{
		set
		{
			set_Models(value);
		}
	}

	public SelectAnimation()
	{
		_FrameRound = 0;
	}

	public void set_Models(List<Model> value)
	{
		BPIFJBJBKHA.Clear();
		BPIFJBJBKHA.AddRange(value);
	}

	public void AddModel(Model ACENLMONNPA)
	{
		int count = BPIFJBJBKHA.Count;
		int num = BPIFJBJBKHA.AddIfNotExist(ACENLMONNPA);
		if (num == count)
		{
			ACENLMONNPA.AddEventListener(2, OnAnimationStart);
			ACENLMONNPA.AddEventListener(3, OnAnimationEnd);
			ACENLMONNPA.AddEventListener(0, OnIntervalStart);
			ACENLMONNPA.AddEventListener(1, OnIntervalEnd);
			ACENLMONNPA.AddEventListener(4, OnEveryFrame);
			ACENLMONNPA.AddEventListener(6, OnModelCreate);
			ACENLMONNPA.AddEventListener(10, OnKeyPress);
			ACENLMONNPA.AddEventListener(11, OnKeyRelease);
			ACENLMONNPA.UpdateAnimationParameters(BPIFJBJBKHA);
		}
	}

	public void FDBHLFMBECM()
	{
		BPIFJBJBKHA.Clear();
		CFKGCLIKKOC.Clear();
		KPHAPCNOPNP.Clear();
		_ExplicitBirthModels.Clear();
	}

	public void RemoveModel(Model ACENLMONNPA)
	{
		for (int num = BPIFJBJBKHA.Count - 1; num >= 0; num--)
		{
			if (BPIFJBJBKHA[num] == ACENLMONNPA)
			{
				BPIFJBJBKHA.Remove(ACENLMONNPA);
				HMOPJBLOKGB(ACENLMONNPA, CFKGCLIKKOC);
				HMOPJBLOKGB(ACENLMONNPA, KPHAPCNOPNP);
			}
		}
	}

	public void CheckEvent(EventAnimation.EECEJKADLCK LFLGCDNKNJI, Model.EventModel EGHPHELLOGO, bool HLEIILHFBKP = false)
	{
		EventModelDelayed gBEJMGCOCOJ = new EventModelDelayed();
		gBEJMGCOCOJ.Type = LFLGCDNKNJI;
		gBEJMGCOCOJ.Data = EGHPHELLOGO.Data;
		gBEJMGCOCOJ.KJDFJPBIGJC = ((LFLGCDNKNJI != EventAnimation.EECEJKADLCK.EVENT_STRIKE) ? EGHPHELLOGO.KJDFJPBIGJC : EGHPHELLOGO.GAIBPAGPEGK);
		gBEJMGCOCOJ.GAIBPAGPEGK = gBEJMGCOCOJ.KJDFJPBIGJC.KDAHHIMLJGG.GAIBPAGPEGK;
		gBEJMGCOCOJ.IsRandom = HLEIILHFBKP;
		if (LFLGCDNKNJI == EventAnimation.EECEJKADLCK.EVENT_INTERVAL_END)
		{
			KPHAPCNOPNP.Add(gBEJMGCOCOJ);
		}
		else
		{
			CFKGCLIKKOC.Add(gBEJMGCOCOJ);
		}
	}

	public void PKFPDKFLKBL(Model.EventModel EGHPHELLOGO)
	{
		CheckEvent(EventAnimation.EECEJKADLCK.EVENT_KEY_PRESSED, EGHPHELLOGO, true);
		foreach (Model item in BPIFJBJBKHA)
		{
			item.NPKHMEHKFMM = _FrameRound;
		}
	}

	public void UpdateConditions()
	{
		int count = BPIFJBJBKHA.Count;
		_ModelsConditions.Clear();
		_ModelsConditions.Capacity = count;
		for (int i = 0; i < count; i++)
		{
			_ModelsConditions.Add(BPIFJBJBKHA[i].EBABHGHPLFK());
			UpdateConditions(_ModelsConditions[i], BPIFJBJBKHA[i]);
		}
	}

	public void Render()
	{
		UpdateConditions();
		RenderEvent();
	}

	public void Reset()
	{
		KPHAPCNOPNP.Clear();
		CFKGCLIKKOC.Clear();
		_ExplicitBirthModels.Clear();
	}

	public void OnAnimationStart(object data)
	{
		CheckEvent(EventAnimation.EECEJKADLCK.EVENT_ANIMATION_START, (Model.EventModel)data);
	}

	public void OnAnimationEnd(object data)
	{
		CheckEvent(EventAnimation.EECEJKADLCK.EVENT_ANIMATION_END, (Model.EventModel)data);
	}

	public void OnIntervalStart(object data)
	{
		CheckEvent(EventAnimation.EECEJKADLCK.EVENT_INTERVAL_START, (Model.EventModel)data);
	}

	public void OnIntervalEnd(object data)
	{
		CheckEvent(EventAnimation.EECEJKADLCK.EVENT_INTERVAL_END, (Model.EventModel)data);
	}

	public void OnEveryFrame(object data)
	{
		CheckEvent(EventAnimation.EECEJKADLCK.EVENT_EVERY_FRAME, (Model.EventModel)data);
	}

	public void OnModelCreate(object data)
	{
		Model fGCODGKLHED = (Model)data;
		AddModel(fGCODGKLHED);
		HKOBFBADDJN.Add(fGCODGKLHED);
		if (fGCODGKLHED.HasExplicitBirthAnimation())
		{
			_ExplicitBirthModels.Add(fGCODGKLHED);
		}
		else
		{
			CheckEvent(EventAnimation.EECEJKADLCK.EVENT_BIRTH, fGCODGKLHED.KDAHHIMLJGG);
		}
	}

	public void OnKeyPress(object data)
	{
		CheckEvent(EventAnimation.EECEJKADLCK.EVENT_KEY_PRESSED, (Model.EventModel)data);
	}

	public void OnKeyRelease(object data)
	{
		CheckEvent(EventAnimation.EECEJKADLCK.EVENT_KEY_RELEASED, (Model.EventModel)data);
	}

	private void RenderEvent()
	{
		HKOBFBADDJN.Clear();
		CheckEventsForModels(BPIFJBJBKHA, AGKKIGDCPOD);
		foreach (EventModelDelayed item in CFKGCLIKKOC)
		{
			if (item.KJDFJPBIGJC != null)
			{
				item.KJDFJPBIGJC.OCPMJKIEPIG().PJDPCLCOGFP(item.Type);
			}
		}
		foreach (EventModelDelayed item2 in CFKGCLIKKOC)
		{
		}
		CFKGCLIKKOC.Clear();
		foreach (TriggerStruct item3 in IJIHPHBMEOI)
		{
			Model kJDFJPBIGJC = item3.KJDFJPBIGJC;
			Trigger fEDHCBGNJIM = item3.FEDHCBGNJIM;
			if (kJDFJPBIGJC != null && fEDHCBGNJIM != null)
			{
				kJDFJPBIGJC.GKDJBGMABDO(fEDHCBGNJIM.IDEMFOLJIFE.DJBAIAKOIHM);
			}
		}
		// Model-create listeners (camera, effects and renderer) have all completed
		// by this point, so an XML-requested start animation can safely emit its
		// first-frame actions. Fall back to legacy Birth selection if the named
		// move is unavailable.
		foreach (Model explicitBirthModel in _ExplicitBirthModels)
		{
			if (!explicitBirthModel.TryPlayExplicitBirthAnimation())
			{
				CheckEvent(EventAnimation.EECEJKADLCK.EVENT_BIRTH, explicitBirthModel.KDAHHIMLJGG);
			}
		}
		_ExplicitBirthModels.Clear();
		if (HKOBFBADDJN.Count > 0)
		{
			CheckEventsForModels(HKOBFBADDJN, BIEHAGCAHEL);
		}
		GIANMJNJFPL();
		ChooseAnimations(BPIFJBJBKHA, AGKKIGDCPOD);
		if (HKOBFBADDJN.Count > 0)
		{
			ChooseAnimations(HKOBFBADDJN, BIEHAGCAHEL);
		}
		if (0 < KPHAPCNOPNP.Count)
		{
			CFKGCLIKKOC.AddRange(KPHAPCNOPNP);
			KPHAPCNOPNP.Clear();
		}
	}

	private static bool IDLPMHIHDNO(InfoAnimation DBOLBEOCEME, List<SelectInfo> MAHEJFLCCHP)
	{
		foreach (SelectInfo item in MAHEJFLCCHP)
		{
			if (DBOLBEOCEME == item.FGICHADOEHF)
			{
				return true;
			}
		}
		return false;
	}

	private SelectInfo SelectAnimationWithWeights(Model ACENLMONNPA, List<SelectInfo> GBKDAGPNJLB)
	{
		int count = GBKDAGPNJLB.Count;
		if (0 < count)
		{
			List<InfoAnimation> list = new List<InfoAnimation>();
			foreach (SelectInfo item in GBKDAGPNJLB)
			{
				list.Add(item.FGICHADOEHF);
			}
			int num = ACENLMONNPA.EEIGOJBKFGE().SelectAnimationWithWeights(list);
			if (-1 < num && num < count)
			{
				return GBKDAGPNJLB[num];
			}
		}
		return null;
	}

	private void PlayAnimation(Model ACENLMONNPA, List<SelectInfo> MAHEJFLCCHP, bool HLEIILHFBKP = false)
	{
		List<SelectInfo> list = new List<SelectInfo>();
		int num = int.MinValue;
		SelectInfo nKDNDLNDFJH = null;
		List<SelectInfo> list2 = new List<SelectInfo>();
		SelectInfo nKDNDLNDFJH2 = null;
		for (int i = 0; i < MAHEJFLCCHP.Count; i++)
		{
			nKDNDLNDFJH2 = MAHEJFLCCHP[i];
			if (!HLEIILHFBKP && nKDNDLNDFJH2.IsRandom)
			{
				list.Add(nKDNDLNDFJH2);
			}
			int eBMPEMKCDGP = nKDNDLNDFJH2.FGICHADOEHF.Priority;
			if (eBMPEMKCDGP >= num)
			{
				if (eBMPEMKCDGP > num)
				{
					num = eBMPEMKCDGP;
					list2.Clear();
				}
				list2.Add(nKDNDLNDFJH2);
			}
		}
		int index = Random.Range(0, list2.Count);
		nKDNDLNDFJH = list2[index];
		if (list.Count > 0)
		{
			if (nKDNDLNDFJH != null)
			{
				list.Add(nKDNDLNDFJH);
			}
			PlayAnimationRandom(ACENLMONNPA, list);
		}
		else if (nKDNDLNDFJH != null)
		{
			if (!nKDNDLNDFJH.FGICHADOEHF.FBKGDALBNDJ)
			{
				SetTransitions(ACENLMONNPA, _ModelsConditions[nKDNDLNDFJH.Index], nKDNDLNDFJH.FGICHADOEHF, nKDNDLNDFJH.GFHOIKMBNHF);
			}
			else
			{
				ACENLMONNPA.IFDGGKPAHMC(nKDNDLNDFJH.FGICHADOEHF, nKDNDLNDFJH.IsHit);
			}
			ACENLMONNPA.DFLPNNBIFFN = nKDNDLNDFJH.FGICHADOEHF.Type;
			ACENLMONNPA.KMDKCFHMECJ = nKDNDLNDFJH.DJPLGDJCMPI;
		}
	}

	private void PlayAnimationRandom(Model ACENLMONNPA, List<SelectInfo> MAHEJFLCCHP)
	{
		int num = MAHEJFLCCHP.Count;
		if (0 < num)
		{
			int num2 = 0;
			for (int i = 0; i < MAHEJFLCCHP.Count; i++)
			{
				InfoAnimation.CapabilityTable iCANLHJKKNE = MAHEJFLCCHP[i].FGICHADOEHF.ICANLHJKKNE;
				bool flag = true;
				for (int j = 0; j < MAHEJFLCCHP.Count; j++)
				{
					if (!iCANLHJKKNE.IsThePriority(MAHEJFLCCHP[j].FGICHADOEHF))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					MAHEJFLCCHP[num2] = MAHEJFLCCHP[i];
					num2++;
				}
			}
			num = MAHEJFLCCHP.Count;
			if (num2 < num)
			{
				MAHEJFLCCHP.CPCAJIKOIEE(num2);
				num = num2;
			}
		}
		if (0 < num)
		{
			int num3 = 0;
			for (int l = 0; l < MAHEJFLCCHP.Count; l++)
			{
				SelectInfo item3 = MAHEJFLCCHP[l];
				if (item3.FGICHADOEHF.ODACDCDONJE.NIDNJFOGBFO.Count != 0)
				{
					ACENLMONNPA.EBABHGHPLFK().PDKPGKPBBIL = item3.FGICHADOEHF.FOLOOGCLPNE();
					ACENLMONNPA.EBABHGHPLFK().PCAOCHAIBJC = item3.FGICHADOEHF.CEDEDCLGJDE(ACENLMONNPA.EBABHGHPLFK(), ACENLMONNPA.OCPMJKIEPIG().KFCNPADAMHA());
					ACENLMONNPA.EBABHGHPLFK().FOIHIKCEBJF = (int)item3.FGICHADOEHF.ODACDCDONJE.ILOEBFFAEAN.OLBDPMKCJIF;
					if (item3.FGICHADOEHF.HPPGNJJCEGF(ACENLMONNPA.EBABHGHPLFK(), item3.FGICHADOEHF.ODACDCDONJE.NIDNJFOGBFO, item3.AALMPLPGCHA))
					{
						MAHEJFLCCHP[num3] = item3;
						num3++;
					}
				}
				else
				{
					MAHEJFLCCHP[num3] = item3;
					num3++;
				}
			}
			num = MAHEJFLCCHP.Count;
			if (num3 < num)
			{
				MAHEJFLCCHP.CPCAJIKOIEE(num3);
				num = num3;
			}
		}
		SelectInfo nKDNDLNDFJH = SelectAnimationWithWeights(ACENLMONNPA, MAHEJFLCCHP);
		if (nKDNDLNDFJH == null)
		{
			return;
		}
		List<SelectInfo> list = new List<SelectInfo>();
		ConditionKeys bHDEBDIHDFM = nKDNDLNDFJH.FGICHADOEHF.ILBCHANCOBP();
		for (int m = 0; m < MAHEJFLCCHP.Count; m++)
		{
			SelectInfo item4 = MAHEJFLCCHP[m];
			ConditionKeys bHDEBDIHDFM2 = item4.FGICHADOEHF.ILBCHANCOBP();
			if (bHDEBDIHDFM != null && bHDEBDIHDFM2 != null)
			{
				if (bHDEBDIHDFM2.IsEqual(bHDEBDIHDFM.FONEJOKEIEN, true))
				{
					list.Add(item4);
				}
			}
			else
			{
				list.Add(item4);
			}
		}
		PlayAnimation(ACENLMONNPA, list, true);
	}

	private static bool CGAJAFBPFAC(EventAnimation DOANBADPBGH, ModelConditions BCGJLLNBHJG, EventModelDelayed PEADINOKLKN)
	{
		bool result = false;
		if (PEADINOKLKN.Type == DOANBADPBGH.Type)
		{
			switch (PEADINOKLKN.Type)
			{
			case EventAnimation.EECEJKADLCK.EVENT_ROUND_STAGE:
				result = KLAJPEHFFAP(DOANBADPBGH, (StageType.FDBBPEGEGMK)PEADINOKLKN.Data);
				result = ((!DOANBADPBGH.IsNot) ? result : (!result));
				break;
			case EventAnimation.EECEJKADLCK.EVENT_KEY_PRESSED:
				result = NDAMNPDILFE(DOANBADPBGH);
				result = ((!DOANBADPBGH.IsNot) ? result : (!result));
				break;
			case EventAnimation.EECEJKADLCK.EVENT_KEY_RELEASED:
				result = IIJINHHFJNA(DOANBADPBGH);
				result = ((!DOANBADPBGH.IsNot) ? result : (!result));
				break;
			case EventAnimation.EECEJKADLCK.EVENT_ANIMATION_START:
				result = HMEMGEOKAGG(DOANBADPBGH, BCGJLLNBHJG, PEADINOKLKN);
				break;
			case EventAnimation.EECEJKADLCK.EVENT_ANIMATION_END:
				result = MCLDKKPMBGL(DOANBADPBGH, BCGJLLNBHJG, PEADINOKLKN);
				break;
			case EventAnimation.EECEJKADLCK.EVENT_INTERVAL_START:
				result = KBMLJHIEOIK(DOANBADPBGH, (IntervalAnimation)PEADINOKLKN.Data);
				result = ((!DOANBADPBGH.IsNot) ? result : (!result));
				break;
			case EventAnimation.EECEJKADLCK.EVENT_INTERVAL_END:
				result = KJHNKIAEHIM(DOANBADPBGH, (IntervalAnimation)PEADINOKLKN.Data);
				result = ((!DOANBADPBGH.IsNot) ? result : (!result));
				break;
			case EventAnimation.EECEJKADLCK.EVENT_HIT:
				result = IsHit(DOANBADPBGH, PEADINOKLKN, PEADINOKLKN.KJDFJPBIGJC.POCBCFMBKLO, PEADINOKLKN.KJDFJPBIGJC.EDJFLMILEBA());
				result = ((!DOANBADPBGH.IsNot) ? result : (!result));
				break;
			case EventAnimation.EECEJKADLCK.EVENT_STRIKE:
				result = IsHit(DOANBADPBGH, PEADINOKLKN, PEADINOKLKN.KJDFJPBIGJC.POCBCFMBKLO);
				result = ((!DOANBADPBGH.IsNot) ? result : (!result));
				break;
			case EventAnimation.EECEJKADLCK.EVENT_EVERY_FRAME:
				result = HDLPPBKLCBF(DOANBADPBGH);
				result = ((!DOANBADPBGH.IsNot) ? result : (!result));
				break;
			case EventAnimation.EECEJKADLCK.EVENT_BIRTH:
				result = HBNBGLGIEFF(DOANBADPBGH);
				result = ((!DOANBADPBGH.IsNot) ? result : (!result));
				break;
			case EventAnimation.EECEJKADLCK.EVENT_MOD_EXPIRES:
			{
				string gOHIIMFFFJI = (string)PEADINOKLKN.Data;
				result = DHFHPKJBJHB(DOANBADPBGH, gOHIIMFFFJI);
				result = ((!DOANBADPBGH.IsNot) ? result : (!result));
				break;
			}
			}
		}
		return result;
	}

	private static bool KLAJPEHFFAP(EventAnimation FOPOKALJIIJ, StageType.FDBBPEGEGMK LFLGCDNKNJI)
	{
		EventRoundStage gBIJAGPBADA = (EventRoundStage)FOPOKALJIIJ;
		return gBIJAGPBADA.IEDKJLFBCBK() == LFLGCDNKNJI;
	}

	private static bool NDAMNPDILFE(EventAnimation FOPOKALJIIJ)
	{
		return true;
	}

	private static bool IIJINHHFJNA(EventAnimation FOPOKALJIIJ)
	{
		return true;
	}

	private static bool HMEMGEOKAGG(EventAnimation FOPOKALJIIJ, ModelConditions conditions, EventModelDelayed PEADINOKLKN)
	{
		if (string.IsNullOrEmpty(FOPOKALJIIJ.LJICHLHMBFA))
		{
			return true;
		}
		bool flag = false;
		string lJICHLHMBFA = FOPOKALJIIJ.LJICHLHMBFA;
		if (lJICHLHMBFA != string.Empty)
		{
			List<string> list = null;
			switch (FOPOKALJIIJ.IHJJBIDMEMB)
			{
			case ModelType.KEIDBIOIFGA.MODEL_THIS:
				list = conditions.NNPJJLPCOHD;
				break;
			case ModelType.KEIDBIOIFGA.MODEL_OTHER:
				if (PEADINOKLKN.KJDFJPBIGJC.NJDJHGDMCIJ() != null)
				{
					return false;
				}
				list = conditions.MGFNFEHILNF;
				break;
			case ModelType.KEIDBIOIFGA.MODEL_PARENT:
				list = conditions.DHHADKMMOHP;
				break;
			case ModelType.KEIDBIOIFGA.MODEL_CHILD:
				list = conditions.NKPMIACBKDE;
				break;
			case ModelType.KEIDBIOIFGA.MODEL_BOTH:
				list = conditions.NNPJJLPCOHD;
				break;
			}
			List<string> list2 = null;
			if (lJICHLHMBFA == "$Move")
			{
				list2 = conditions.PDKPGKPBBIL;
				int count = list.Count;
				if (count != 1)
				{
					if (count > 1)
					{
						for (int num = count - 1; num >= 1; num--)
						{
							list.RemoveAt(num);
						}
					}
					else
					{
						for (int i = count; i < 1; i++)
						{
							list.Add(string.Empty);
						}
					}
				}
			}
			else
			{
				list2 = new List<string>();
				list2.Add(lJICHLHMBFA);
			}
			flag = IsNames(list, list2);
			return (!FOPOKALJIIJ.IsNot) ? flag : (!flag);
		}
		return true;
	}

	private static bool IsNames(List<string> NIKHAICFGNM, List<string> MGNOPLPBOHC)
	{
		string text = null;
		string text2 = null;
		int i = 0;
		for (int count = NIKHAICFGNM.Count; i < count; i++)
		{
			text = NIKHAICFGNM[i];
			int j = 0;
			for (int count2 = MGNOPLPBOHC.Count; j < count2; j++)
			{
				text2 = MGNOPLPBOHC[j];
				if (text == text2)
				{
					return true;
				}
			}
		}
		return false;
	}

	private static bool MCLDKKPMBGL(EventAnimation FOPOKALJIIJ, ModelConditions conditions, EventModelDelayed PEADINOKLKN)
	{
		return HMEMGEOKAGG(FOPOKALJIIJ, conditions, PEADINOKLKN);
	}

	private static bool KBMLJHIEOIK(EventAnimation FOPOKALJIIJ, IntervalAnimation CHCGJBLDPML)
	{
		IntervalAnimation.NGAJJDIEDGF nGAJJDIEDGF = IntervalAnimation.NGAJJDIEDGF.INTERVAL_NONE;
		if (FOPOKALJIIJ.LONCGFHLFKA == "Attack")
		{
			nGAJJDIEDGF = IntervalAnimation.NGAJJDIEDGF.INTERVAL_ATTACK;
		}
		else if (FOPOKALJIIJ.LONCGFHLFKA == "Block")
		{
			nGAJJDIEDGF = IntervalAnimation.NGAJJDIEDGF.INTERVAL_BLOCK;
		}
		else if (FOPOKALJIIJ.LONCGFHLFKA == "Invulnerable")
		{
			nGAJJDIEDGF = IntervalAnimation.NGAJJDIEDGF.INTERVAL_INVULNERABLE;
		}
		if ((nGAJJDIEDGF == IntervalAnimation.NGAJJDIEDGF.INTERVAL_NONE || nGAJJDIEDGF == CHCGJBLDPML.Type) && (FOPOKALJIIJ.LJICHLHMBFA == string.Empty || FOPOKALJIIJ.LJICHLHMBFA == CHCGJBLDPML.Name))
		{
			return true;
		}
		return false;
	}

	private static bool KJHNKIAEHIM(EventAnimation FOPOKALJIIJ, IntervalAnimation CHCGJBLDPML)
	{
		return KBMLJHIEOIK(FOPOKALJIIJ, CHCGJBLDPML);
	}

	private static bool IsHit(EventAnimation FOPOKALJIIJ, EventModelDelayed PEADINOKLKN, bool OOGIBOBMGJA = false, bool EPKEEMFHHFM = false)
	{
		if (string.IsNullOrEmpty(FOPOKALJIIJ.LONCGFHLFKA) || (FOPOKALJIIJ.LONCGFHLFKA == "Critical" && OOGIBOBMGJA) || (FOPOKALJIIJ.LONCGFHLFKA == "Shock" && EPKEEMFHHFM))
		{
			IntervalAnimation mNOIEOBBCMI = (IntervalAnimation)PEADINOKLKN.Data;
			IntervalAttack hFIIPNLCIEE = mNOIEOBBCMI as IntervalAttack;
			return string.IsNullOrEmpty(FOPOKALJIIJ.LJICHLHMBFA) || hFIIPNLCIEE == null || FOPOKALJIIJ.LJICHLHMBFA == hFIIPNLCIEE.GetReactionName(PEADINOKLKN.GAIBPAGPEGK.NODAINEDAKJ());
		}
		return false;
	}

	private static bool HDLPPBKLCBF(EventAnimation FOPOKALJIIJ)
	{
		return true;
	}

	private static bool HBNBGLGIEFF(EventAnimation FOPOKALJIIJ)
	{
		return true;
	}

	private static bool DHFHPKJBJHB(EventAnimation FOPOKALJIIJ, string name)
	{
		EventModExpires bEKAAGNGPFP = (EventModExpires)FOPOKALJIIJ;
		return bEKAAGNGPFP.CMKKGFDBBJF() == name;
	}

	private void UpdateConditions(ModelConditions conditions, Model ACENLMONNPA)
	{
		Fight gDBOMJODDEA = Fight.OHNKFOHIAKG();
		Model fGCODGKLHED = ACENLMONNPA.EGGEACCDAEK();
		Model fGCODGKLHED2 = ACENLMONNPA.NMGNPBMFJKP(ModelType.KEIDBIOIFGA.MODEL_PARENT);
		Model fGCODGKLHED3 = ACENLMONNPA.NMGNPBMFJKP(ModelType.KEIDBIOIFGA.MODEL_CHILD);
		if (gDBOMJODDEA != null)
		{
			gDBOMJODDEA.IEEGPNLEKHH().AINGCNFDFMM(ACENLMONNPA, conditions.LPGJIICFIKF);
			gDBOMJODDEA.IEEGPNLEKHH().KCEBAJBMJGF(ACENLMONNPA, conditions.FPFKABHOEHP);
			if (fGCODGKLHED != null)
			{
				gDBOMJODDEA.IEEGPNLEKHH().AINGCNFDFMM(fGCODGKLHED, conditions.CBMFGJHKKMJ);
				gDBOMJODDEA.IEEGPNLEKHH().AINGCNFDFMM(fGCODGKLHED, conditions.ENBHOAKMCIG);
			}
			else
			{
				conditions.CBMFGJHKKMJ.Clear();
				conditions.ENBHOAKMCIG.Clear();
			}
		}
		conditions.POBNMMADAJJ = ACENLMONNPA.KMMJCHDKBDO.NHBIJEEKALC;
		conditions.CFPLPALGCMK = ((fGCODGKLHED == null) ? null : fGCODGKLHED.KMMJCHDKBDO.NHBIJEEKALC);
		conditions.StrikeResult = ACENLMONNPA.GHHCDAFIKJE;
		conditions.FAHHBNIFAMB = ((fGCODGKLHED != null) ? true : false);
		conditions.GFHOIKMBNHF = ACENLMONNPA.KFCNPADAMHA();
		conditions.OLNDCCIPJAE = ((fGCODGKLHED == null) ? 1 : fGCODGKLHED.KFCNPADAMHA());
		conditions.CDPEPJDJIPK = ((fGCODGKLHED2 == null) ? 1 : fGCODGKLHED2.KFCNPADAMHA());
		conditions.CNNMAMCKCMO = ((fGCODGKLHED3 == null) ? 1 : fGCODGKLHED3.KFCNPADAMHA());
		conditions.BJACLIMKPAE = ACENLMONNPA.GetKeyDataBySign(conditions.GFHOIKMBNHF);
		conditions.Intervals = ACENLMONNPA.KPJAEBBJFEO();
		conditions.FJFOIEFFMEM = ((fGCODGKLHED == null) ? null : fGCODGKLHED.KPJAEBBJFEO());
		conditions.JLCFPNDDGCJ = ((fGCODGKLHED2 == null) ? null : fGCODGKLHED2.KPJAEBBJFEO());
		conditions.IsPlayer = ACENLMONNPA.EPCNJLEHJCB();
		conditions.FDELMAHAAJD = ACENLMONNPA.KIAFPPHPEEK();
		conditions.JMHJDHLBHLK = ACENLMONNPA.JMHJDHLBHLK;
		conditions.NCBPMBJCFBK = ACENLMONNPA.COBOFMDFLJO().EGNOOKHNFLK();
		conditions.EKFCILFBDPO = fGCODGKLHED != null && fGCODGKLHED.COBOFMDFLJO().EGNOOKHNFLK();
		conditions.LFLDHGKEDEH = fGCODGKLHED2 != null && fGCODGKLHED2.COBOFMDFLJO().EGNOOKHNFLK();
		conditions.KAKMANLHJOA = ACENLMONNPA.COBOFMDFLJO().PGOFHCBPLOE();
		conditions.BHHLEBHLBLH = ACENLMONNPA.KMMJCHDKBDO.BHHLEBHLBLH;
		conditions.IsWinner = ACENLMONNPA.KMMJCHDKBDO.IsWinner;
		conditions.EndRoundType = ACENLMONNPA.KMMJCHDKBDO.EndRoundType;
		conditions.IDCHHGHAENM = ACENLMONNPA.IDCHHGHAENM;
		conditions.BOECCPNHAII = (int)ACENLMONNPA.GHHCDAFIKJE.IIIDIKABLOJ.GILCBJJPKBK();
		conditions.BFLPOMAHPJD = (ObscuredFloat)(ACENLMONNPA.KMMJCHDKBDO.KKMCHCNOHMB());
		conditions.KGCJIBCACBH = ACENLMONNPA.KMMJCHDKBDO.CIDCNCDFONA;
		conditions.PKMHOICGDIM = ACENLMONNPA.GLEKCPCMINJ();
		conditions.JJDNDOLCMMN = ACENLMONNPA.LPOJKGLFMAL();
		conditions.KHDBLNPFDPE = ACENLMONNPA.CKAKLHDLHJO();
		conditions.AFLPHBDFMGA = ACENLMONNPA.OCPMJKIEPIG().CJELIBMCCMA();
		conditions.EJFOAKCDPHH = ((fGCODGKLHED == null) ? null : fGCODGKLHED.OCPMJKIEPIG().CJELIBMCCMA());
		conditions.CJELGCJHMHI = ((fGCODGKLHED2 == null) ? null : fGCODGKLHED2.OCPMJKIEPIG().CJELIBMCCMA());
		conditions.JMMGJGCDPGE = ((fGCODGKLHED3 == null) ? null : fGCODGKLHED3.OCPMJKIEPIG().CJELIBMCCMA());
		GLLMFHPLDNA(ref conditions.NNPJJLPCOHD, conditions.IHJJBIDMEMB, ACENLMONNPA);
		GLLMFHPLDNA(ref conditions.DHHADKMMOHP, conditions.JBNPEMEEMLK, fGCODGKLHED2);
		GLLMFHPLDNA(ref conditions.MGFNFEHILNF, conditions.GAIBPAGPEGK, fGCODGKLHED);
		GLLMFHPLDNA(ref conditions.NKPMIACBKDE, conditions.NECEKOMIPIB, fGCODGKLHED3);
	}

	private static void SetTransitions(Model ACENLMONNPA, ModelConditions conditions, InfoAnimation DBOLBEOCEME, int AOJJBKLCHJO)
	{
		bool hHJGACBCGBP = false;
		int bADKABIKMBD = -1;
		List<TransitionAnimation> eLFBPNOBDKC = DBOLBEOCEME.ODACDCDONJE.ELFBPNOBDKC;
		foreach (TransitionAnimation item in eLFBPNOBDKC)
		{
			if (item.HPPGNJJCEGF(conditions))
			{
				if (item.IsFrameShift)
				{
					hHJGACBCGBP = true;
				}
				bADKABIKMBD = item.FrameShift;
				break;
			}
		}
		ACENLMONNPA.PlayAnimationDelay(DBOLBEOCEME, AOJJBKLCHJO, hHJGACBCGBP, bADKABIKMBD);
	}

	private static bool AMECGJPMJBF(Model CEDPFKAOGHN, Model DBPIIMHNKNN, ModelType.KEIDBIOIFGA LFLGCDNKNJI)
	{
		return LFLGCDNKNJI == ModelType.KEIDBIOIFGA.MODEL_BOTH || (LFLGCDNKNJI == ModelType.KEIDBIOIFGA.MODEL_THIS && DBPIIMHNKNN == CEDPFKAOGHN) || (LFLGCDNKNJI == ModelType.KEIDBIOIFGA.MODEL_OTHER && DBPIIMHNKNN != CEDPFKAOGHN) || (LFLGCDNKNJI == ModelType.KEIDBIOIFGA.MODEL_PARENT && CEDPFKAOGHN == DBPIIMHNKNN.NJDJHGDMCIJ()) || (LFLGCDNKNJI == ModelType.KEIDBIOIFGA.MODEL_CHILD && CEDPFKAOGHN == DBPIIMHNKNN.NMGNPBMFJKP(ModelType.KEIDBIOIFGA.MODEL_CHILD));
	}

	private static void GLLMFHPLDNA(ref List<string> IPFMIJKPABH, ModelConditions.ModelPositions LJKGOKDLAKL, Model ACENLMONNPA)
	{
		if (ACENLMONNPA == null)
		{
			return;
		}
		if (!ACENLMONNPA.NLHFJIEHKMM())
		{
			InfoAnimation pJAHIOELGGD = ACENLMONNPA.FHBLLPCEAHG();
			if (pJAHIOELGGD != null)
			{
				IPFMIJKPABH = pJAHIOELGGD.FOLOOGCLPNE();
			}
		}
		else
		{
			List<string> list = ACENLMONNPA.KGHDFCKGAEO();
			if (list.Count > 0)
			{
				IPFMIJKPABH = list;
			}
		}
		LJKGOKDLAKL.CBAECAAKAIA = ACENLMONNPA.CLDMEJKGLBA();
		LJKGOKDLAKL.BOGHNBAKCEL.x = ACENLMONNPA.KJFIBMMOEPI();
		LJKGOKDLAKL.PCIBKEOCFAO.x = ACENLMONNPA.PHHHEGOBAPB();
	}

	private void CheckEventsForModels(List<Model> INNLAFHKJNI, List<List<SelectInfo>> GLEOPGKNDAO)
	{
		GLEOPGKNDAO.Clear();
		GLEOPGKNDAO.Capacity = INNLAFHKJNI.Count;
		for (int i = 0; i < INNLAFHKJNI.Count; i++)
		{
			GLEOPGKNDAO.Add(new List<SelectInfo>());
		}
		EventModelDelayed gBEJMGCOCOJ = null;
		Model fGCODGKLHED = null;
		for (int j = 0; j < CFKGCLIKKOC.Count; j++)
		{
			gBEJMGCOCOJ = CFKGCLIKKOC[j];
			for (int k = 0; k < INNLAFHKJNI.Count; k++)
			{
				fGCODGKLHED = INNLAFHKJNI[k];
				fGCODGKLHED.KMDKCFHMECJ = EventAnimation.EECEJKADLCK.EVENT_NONE;
				fGCODGKLHED.DFLPNNBIFFN = InfoAnimation.MGHNBEPCKIF.AnimationNone;
				List<InfoAnimation> mAHEJFLCCHP = fGCODGKLHED.CEOOLFLLIMC.NCNDKFCPLEH(gBEJMGCOCOJ.Type);
				CheckAnimations(gBEJMGCOCOJ, fGCODGKLHED, mAHEJFLCCHP, k, GLEOPGKNDAO);
				List<Trigger> cMHFKBKKKOK = fGCODGKLHED.NCGEHCHIBBH.KPMMHDGEBCB(gBEJMGCOCOJ.Type);
				GCIJFPECJMJ(gBEJMGCOCOJ, fGCODGKLHED, cMHFKBKKKOK, k);
			}
		}
	}

	private void CheckAnimations(EventModelDelayed PEADINOKLKN, Model ACENLMONNPA, List<InfoAnimation> MAHEJFLCCHP, int index, List<List<SelectInfo>> GLEOPGKNDAO)
	{
		InfoAnimation pJAHIOELGGD = null;
		EventAnimation nFCCFMOMPHG = null;
		string helperWeaponSubtype = string.Empty;
		bool hasSubtypeLockedBirthMove = false;
		if (PEADINOKLKN.Type == EventAnimation.EECEJKADLCK.EVENT_BIRTH && ACENLMONNPA is WeaponModel)
		{
			ItemInfo helperWeapon = ACENLMONNPA.KMMJCHDKBDO.KDABEFBJMOD("Weapon");
			if (helperWeapon != null)
			{
				helperWeaponSubtype = helperWeapon.MDPPNGIEJGD;
				for (int candidateIndex = 0; candidateIndex < MAHEJFLCCHP.Count; candidateIndex++)
				{
					if (MAHEJFLCCHP[candidateIndex].IsItemRequired("Weapon", helperWeaponSubtype))
					{
						hasSubtypeLockedBirthMove = true;
						break;
					}
				}
			}
		}
		for (int i = 0; i < MAHEJFLCCHP.Count; i++)
		{
			pJAHIOELGGD = MAHEJFLCCHP[i];
			// Old CreatePlayer entries do not always provide StartAnimation.  In
			// that case choose only birth moves whose XML lock explicitly names the
			// copied helper Weapon subtype.  This avoids equal-priority generic
			// projectile moves being selected at random.
			if (hasSubtypeLockedBirthMove && !pJAHIOELGGD.IsItemRequired("Weapon", helperWeaponSubtype))
			{
				continue;
			}
			for (int j = 0; j < pJAHIOELGGD.ODACDCDONJE.AJCMBMJGJEG.Count; j++)
			{
				nFCCFMOMPHG = pJAHIOELGGD.ODACDCDONJE.AJCMBMJGJEG[j];
				if (IDLPMHIHDNO(pJAHIOELGGD, GLEOPGKNDAO[index]) || !AMECGJPMJBF(PEADINOKLKN.KJDFJPBIGJC, ACENLMONNPA, nFCCFMOMPHG.IHJJBIDMEMB) || !CGAJAFBPFAC(nFCCFMOMPHG, _ModelsConditions[index], PEADINOKLKN))
				{
					continue;
				}
				_ModelsConditions[index].PDKPGKPBBIL = pJAHIOELGGD.FOLOOGCLPNE();
				_ModelsConditions[index].PCAOCHAIBJC = ((ACENLMONNPA.EGGEACCDAEK() == null) ? 1 : pJAHIOELGGD.CEDEDCLGJDE(_ModelsConditions[index], ACENLMONNPA.OCPMJKIEPIG().KFCNPADAMHA()));
				_ModelsConditions[index].FOIHIKCEBJF = (int)pJAHIOELGGD.ODACDCDONJE.ILOEBFFAEAN.OLBDPMKCJIF;
				if (PEADINOKLKN.Type == EventAnimation.EECEJKADLCK.EVENT_KEY_PRESSED && PEADINOKLKN.IsRandom)
				{
					_ModelsConditions[index].IDCHHGHAENM = false;
				}
				if (nFCCFMOMPHG.Type == EventAnimation.EECEJKADLCK.EVENT_HIT)
				{
					if (PEADINOKLKN.KJDFJPBIGJC.POCBCFMBKLO)
					{
						nFCCFMOMPHG.LONCGFHLFKA = "Critical";
						if (PEADINOKLKN.KJDFJPBIGJC.EDJFLMILEBA())
						{
							nFCCFMOMPHG.LONCGFHLFKA += "|Shock";
						}
					}
					else if (PEADINOKLKN.KJDFJPBIGJC.EDJFLMILEBA())
					{
						nFCCFMOMPHG.LONCGFHLFKA = "Shock";
					}
					else if (PEADINOKLKN.GAIBPAGPEGK.GHHCDAFIKJE.DFOHNJEBDED)
					{
						nFCCFMOMPHG.LONCGFHLFKA = "Block";
					}
				}
				if (pJAHIOELGGD.HPPGNJJCEGF(ACENLMONNPA, null, nFCCFMOMPHG))
				{
					if (PEADINOKLKN.IsRandom && pJAHIOELGGD.Type == InfoAnimation.MGHNBEPCKIF.AnimationAttack)
					{
						Model fGCODGKLHED = ACENLMONNPA.EGGEACCDAEK();
						if (ACENLMONNPA.FGKAFKFBFEM() && ACENLMONNPA.KMMJCHDKBDO.KMNLACDHAFE && fGCODGKLHED != null)
						{
							float num = (ObscuredFloat)(fGCODGKLHED.KMMJCHDKBDO.KKMCHCNOHMB());
							float cIDCNCDFONA = fGCODGKLHED.KMMJCHDKBDO.CIDCNCDFONA;
							float num2 = num / cIDCNCDFONA;
							if (num2 <= GameUtils.BJACOFCAHPD.BeginnerCheat)
							{
								continue;
							}
						}
					}
					List<SelectInfo> list = GLEOPGKNDAO[index];
					SelectInfo nKDNDLNDFJH = new SelectInfo();
					nKDNDLNDFJH.FGICHADOEHF = pJAHIOELGGD;
					nKDNDLNDFJH.GFHOIKMBNHF = _ModelsConditions[index].PCAOCHAIBJC;
					nKDNDLNDFJH.IsHit = PEADINOKLKN.Type == EventAnimation.EECEJKADLCK.EVENT_HIT;
					nKDNDLNDFJH.DJPLGDJCMPI = PEADINOKLKN.Type;
					nKDNDLNDFJH.IsRandom = PEADINOKLKN.IsRandom;
					nKDNDLNDFJH.Index = index;
					nKDNDLNDFJH.AALMPLPGCHA = nFCCFMOMPHG;
					list.Add(nKDNDLNDFJH);
				}
				if (nFCCFMOMPHG.Type == EventAnimation.EECEJKADLCK.EVENT_HIT)
				{
					nFCCFMOMPHG.LONCGFHLFKA = string.Empty;
				}
				if (PEADINOKLKN.Type == EventAnimation.EECEJKADLCK.EVENT_KEY_PRESSED && PEADINOKLKN.IsRandom)
				{
					_ModelsConditions[index].IDCHHGHAENM = true;
				}
			}
		}
	}

	private void GCIJFPECJMJ(EventModelDelayed PEADINOKLKN, Model ACENLMONNPA, List<Trigger> CMHFKBKKKOK, int index)
	{
		Trigger cPFMGFAFAFB = null;
		EventAnimation nFCCFMOMPHG = null;
		for (int i = 0; i < CMHFKBKKKOK.Count; i++)
		{
			cPFMGFAFAFB = CMHFKBKKKOK[i];
			for (int j = 0; j < cPFMGFAFAFB.IDEMFOLJIFE.AJCMBMJGJEG.Count; j++)
			{
				nFCCFMOMPHG = cPFMGFAFAFB.IDEMFOLJIFE.AJCMBMJGJEG[j];
				if (!AMECGJPMJBF(PEADINOKLKN.KJDFJPBIGJC, ACENLMONNPA, nFCCFMOMPHG.IHJJBIDMEMB) || !CGAJAFBPFAC(nFCCFMOMPHG, _ModelsConditions[index], PEADINOKLKN))
				{
					continue;
				}
				if (PEADINOKLKN.Type == EventAnimation.EECEJKADLCK.EVENT_KEY_PRESSED && PEADINOKLKN.IsRandom)
				{
					_ModelsConditions[index].IDCHHGHAENM = false;
				}
				if (nFCCFMOMPHG.Type == EventAnimation.EECEJKADLCK.EVENT_HIT)
				{
					if (PEADINOKLKN.KJDFJPBIGJC.POCBCFMBKLO)
					{
						if (PEADINOKLKN.KJDFJPBIGJC.EDJFLMILEBA())
						{
							nFCCFMOMPHG.LONCGFHLFKA = "Critical|Shock";
						}
						else
						{
							nFCCFMOMPHG.LONCGFHLFKA = "Critical";
						}
					}
					else if (PEADINOKLKN.KJDFJPBIGJC.EDJFLMILEBA())
					{
						nFCCFMOMPHG.LONCGFHLFKA = "Shock";
					}
					else if (PEADINOKLKN.KJDFJPBIGJC.GHHCDAFIKJE.DFOHNJEBDED)
					{
						nFCCFMOMPHG.LONCGFHLFKA = "Block";
					}
				}
				if (cPFMGFAFAFB.HPPGNJJCEGF(ACENLMONNPA, null, nFCCFMOMPHG))
				{
					cPFMGFAFAFB.HPPGNJJCEGF(ACENLMONNPA, null, nFCCFMOMPHG);
					COIEHEJNGKH(cPFMGFAFAFB, ACENLMONNPA);
				}
				if (nFCCFMOMPHG.Type == EventAnimation.EECEJKADLCK.EVENT_HIT)
				{
					nFCCFMOMPHG.LONCGFHLFKA = string.Empty;
				}
				if (PEADINOKLKN.Type == EventAnimation.EECEJKADLCK.EVENT_KEY_PRESSED && PEADINOKLKN.IsRandom)
				{
					_ModelsConditions[index].IDCHHGHAENM = true;
				}
			}
		}
	}

	private void ChooseAnimations(List<Model> INNLAFHKJNI, List<List<SelectInfo>> GLEOPGKNDAO)
	{
		int num = 0;
		foreach (List<SelectInfo> item in GLEOPGKNDAO)
		{
			if (0 < item.Count)
			{
				PlayAnimation(INNLAFHKJNI[num], item);
			}
			num++;
		}
	}

	private void COIEHEJNGKH(Trigger CPBHKJFPFJB, Model ACENLMONNPA)
	{
		TriggerStruct item = new TriggerStruct(CPBHKJFPFJB, ACENLMONNPA);
		IJIHPHBMEOI.Add(item);
	}

	private void GIANMJNJFPL()
	{
		foreach (TriggerStruct item in IJIHPHBMEOI)
		{
		}
		IJIHPHBMEOI.Clear();
	}

	private void HMOPJBLOKGB(Model ACENLMONNPA, List<EventModelDelayed> CDIELLOLINA)
	{
		for (int num = CDIELLOLINA.Count - 1; num >= 0; num--)
		{
			if (CDIELLOLINA[num].KJDFJPBIGJC == ACENLMONNPA)
			{
				CDIELLOLINA.RemoveAt(num);
			}
		}
	}
}
