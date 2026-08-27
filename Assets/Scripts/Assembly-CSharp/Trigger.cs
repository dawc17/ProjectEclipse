using System.Collections.Generic;

public class Trigger
{
	public class TriggerInside
	{
		public List<EventAnimation> AJCMBMJGJEG = new List<EventAnimation>();

		public List<ConditionAnimation> JIFAHHGNPFH = new List<ConditionAnimation>();

		public List<ConditionAnimation> HIFPHBNGIPO = new List<ConditionAnimation>();

		public List<ActionAnimation> DJBAIAKOIHM = new List<ActionAnimation>();

		public EventAnimation OIGBIFNICBI(EventAnimation.EECEJKADLCK LFLGCDNKNJI)
		{
			foreach (EventAnimation item in AJCMBMJGJEG)
			{
				if (item.Type == LFLGCDNKNJI)
				{
					return item;
				}
			}
			return null;
		}
	}

	private List<string> _TemplateNames = new List<string>();

	public TriggerInside IDEMFOLJIFE;

	public string Name;

	public virtual List<string> LANPOMAOOIM
	{
		get
		{
			return FOLOOGCLPNE();
		}
	}

	public virtual ConditionKeys OHOPGOOAEOJ
	{
		get
		{
			return ILBCHANCOBP();
		}
	}

	public virtual List<ConditionKeys> ODCILHKPLAF
	{
		get
		{
			return MOPMGFIIFGA();
		}
	}

	public Trigger()
	{
		IDEMFOLJIFE = new TriggerInside();
	}

	public virtual void Init()
	{
	}

	public virtual void AddTemplateName(string name)
	{
	}

	public virtual bool JBALJDEOGNK(EventAnimation p_event)
	{
		foreach (EventAnimation item in IDEMFOLJIFE.AJCMBMJGJEG)
		{
			if (item.IsEqual(p_event))
			{
				return true;
			}
		}
		return false;
	}

	public virtual bool HPPGNJJCEGF(ModelConditions conditions, List<ConditionAnimation> JPGMNIFICDM = null, EventAnimation DOANBADPBGH = null)
	{
		List<ConditionAnimation> list = ((JPGMNIFICDM == null) ? IDEMFOLJIFE.JIFAHHGNPFH : JPGMNIFICDM);
		if (DOANBADPBGH != null)
		{
			conditions.HFCIDBJJINB = DOANBADPBGH;
			DOANBADPBGH.JIFAHHGNPFH = conditions;
		}
		foreach (ConditionAnimation item in list)
		{
			if (!item.IsEqual(conditions))
			{
				return false;
			}
		}
		return true;
	}

	public virtual bool HPPGNJJCEGF(Model ACENLMONNPA, List<ConditionAnimation> JPGMNIFICDM = null, EventAnimation DOANBADPBGH = null)
	{
		List<ConditionAnimation> list = ((JPGMNIFICDM == null) ? IDEMFOLJIFE.JIFAHHGNPFH : JPGMNIFICDM);
		foreach (ConditionAnimation item in list)
		{
			ModelType.KEIDBIOIFGA kEIDBIOIFGA = item.FHBAPKNECOM();
			Model fGCODGKLHED = item.DKDAKGDMHAL(ACENLMONNPA, kEIDBIOIFGA);
			if (fGCODGKLHED == null)
			{
				return false;
			}
			ModelConditions dGJJDPIAEAO = fGCODGKLHED.EBABHGHPLFK();
			if (DOANBADPBGH != null)
			{
				dGJJDPIAEAO.HFCIDBJJINB = DOANBADPBGH;
				DOANBADPBGH.JIFAHHGNPFH = dGJJDPIAEAO;
			}
			item.MJFKNEHGNMB(ModelType.KEIDBIOIFGA.MODEL_THIS);
			bool flag = false;
			if (item.Type == ConditionAnimation.DGAGKLODADD.LIST)
			{
				ConditionList eLFKOGJJNMN = item as ConditionList;
				if (eLFKOGJJNMN != null)
				{
					flag = eLFKOGJJNMN.DJEJMGCMPPH(ACENLMONNPA.EBABHGHPLFK(), ACENLMONNPA, DOANBADPBGH);
				}
			}
			else
			{
				flag = item.IsEqual(fGCODGKLHED.EBABHGHPLFK());
			}
			if (!flag)
			{
				item.GNPMNEDOFPB(kEIDBIOIFGA);
				return false;
			}
			item.GNPMNEDOFPB(kEIDBIOIFGA);
		}
		return true;
	}

	public virtual List<string> FOLOOGCLPNE()
	{
		return _TemplateNames;
	}

	public virtual ConditionKeys ILBCHANCOBP()
	{
		return DHBACBKLADO(IDEMFOLJIFE.JIFAHHGNPFH);
	}

	public virtual List<ConditionKeys> MOPMGFIIFGA()
	{
		List<ConditionKeys> list = new List<ConditionKeys>();
		CIEHMPCOKGK(IDEMFOLJIFE.JIFAHHGNPFH, list);
		return list;
	}

	public static ConditionKeys JEELAPHJLOE(ConditionAnimation IOFGGOCEIAM)
	{
		if (IOFGGOCEIAM.Type == ConditionAnimation.DGAGKLODADD.KEYS)
		{
			return IOFGGOCEIAM as ConditionKeys;
		}
		return null;
	}

	public virtual void PreloadEffects(List<string> MNDEJPFJODO = null)
	{
		string text = "Textures/Effects/Magic/";
		foreach (ActionAnimation item in IDEMFOLJIFE.DJBAIAKOIHM)
		{
			if (item.get_Type() == ActionAnimation.FADAJCEEKIO.EFFECT)
			{
				ActionEffect jFJGGMEJDPG = (ActionEffect)item;
				string oNNKJLOGHGH = text + jFJGGMEJDPG.EPDMGFELIMC();
				LocationSpriteCache.ENFOJMFEGJH(oNNKJLOGHGH);
			}
		}
	}

	public virtual void PreloadSounds()
	{
		foreach (ActionAnimation item in IDEMFOLJIFE.DJBAIAKOIHM)
		{
			if (item.get_Type() == ActionAnimation.FADAJCEEKIO.SOUND)
			{
				ActionSound nMLKJLJHCIA = (ActionSound)item;
				Sound.IOIEJHLMBLI(nMLKJLJHCIA.get_Name());
			}
		}
	}

	public virtual void NHAEHLFMPNK(TriggerInside KECIIKEIJBH)
	{
		if (IDEMFOLJIFE != null)
		{
			ACGIFMKPBGC(KECIIKEIJBH.AJCMBMJGJEG);
			EGGLLLLFMCO(KECIIKEIJBH.JIFAHHGNPFH);
			FGAEEJBEGEJ(KECIIKEIJBH.DJBAIAKOIHM);
			OFMGLKAGCGO(KECIIKEIJBH.HIFPHBNGIPO);
		}
	}

	public virtual void BPHNHFJCFCD(ModelObject OECPEDPMKCD, bool EKBOGDKIHIH, bool PHADJMAONJG, ModelObject MJCGOJBGFIE)
	{
		ModelNode aECCPADGGPG = null;
		UpdateConditions(IDEMFOLJIFE.JIFAHHGNPFH, OECPEDPMKCD, EKBOGDKIHIH, PHADJMAONJG, MJCGOJBGFIE, aECCPADGGPG);
		foreach (ActionAnimation item in IDEMFOLJIFE.DJBAIAKOIHM)
		{
			if (item.get_Type() == ActionAnimation.FADAJCEEKIO.EFFECT)
			{
				ActionEffect jFJGGMEJDPG = (ActionEffect)item;
				jFJGGMEJDPG.KJHPCLOFDJB(OECPEDPMKCD, EKBOGDKIHIH, null, PHADJMAONJG, MJCGOJBGFIE);
			}
		}
	}

	public void CJAPHCKAOIE(List<ConditionAnimation> AIDMEPEKEOL)
	{
		foreach (ConditionAnimation item in AIDMEPEKEOL)
		{
			if (item.Type == ConditionAnimation.DGAGKLODADD.DISTANCE)
			{
				ConditionDistance jNPIBKBDJAN = item as ConditionDistance;
				if (jNPIBKBDJAN != null)
				{
					jNPIBKBDJAN.ABNCNNHMLII();
				}
				else
				{
					LLLOJBFMONN.Error("conditionDistance is null");
				}
			}
			else if (item.Type == ConditionAnimation.DGAGKLODADD.DIRECTION)
			{
				ConditionDirection cFCGJLJBOKI = item as ConditionDirection;
				if (cFCGJLJBOKI != null)
				{
					cFCGJLJBOKI.ABNCNNHMLII();
				}
				else
				{
					LLLOJBFMONN.Error("conditionDistance is null");
				}
			}
			else if (item.Type == ConditionAnimation.DGAGKLODADD.LIST)
			{
				ConditionList eLFKOGJJNMN = item as ConditionList;
				if (eLFKOGJJNMN != null)
				{
					List<ConditionAnimation> aIDMEPEKEOL = eLFKOGJJNMN.KJILOMLMMEN();
					CJAPHCKAOIE(aIDMEPEKEOL);
				}
				else
				{
					LLLOJBFMONN.Error("conditions is null");
				}
			}
		}
	}

	public virtual void ABNCNNHMLII()
	{
		CJAPHCKAOIE(IDEMFOLJIFE.JIFAHHGNPFH);
		foreach (ActionAnimation item in IDEMFOLJIFE.DJBAIAKOIHM)
		{
			if (item.get_Type() == ActionAnimation.FADAJCEEKIO.EFFECT)
			{
				ActionEffect jFJGGMEJDPG = (ActionEffect)item;
				jFJGGMEJDPG.MGCNPBCBMHB();
			}
		}
	}

	public virtual bool CNPFHBMGDFP(string name)
	{
		return Name == name || LPPIKDGABOL(name);
	}

	public virtual bool LPPIKDGABOL(string IJBOAGICOON)
	{
		foreach (string item in _TemplateNames)
		{
			if (item == IJBOAGICOON)
			{
				return true;
			}
		}
		return false;
	}

	protected static ConditionKeys DHBACBKLADO(List<ConditionAnimation> conditions)
	{
		foreach (ConditionAnimation item in conditions)
		{
			if (item.Type == ConditionAnimation.DGAGKLODADD.LIST)
			{
				ConditionList eLFKOGJJNMN = item as ConditionList;
				if (eLFKOGJJNMN != null)
				{
					List<ConditionAnimation> kDOGKKGDOBK = eLFKOGJJNMN.KJILOMLMMEN();
					ConditionKeys bHDEBDIHDFM = DHBACBKLADO(kDOGKKGDOBK);
					if (bHDEBDIHDFM != null)
					{
						return bHDEBDIHDFM;
					}
				}
				else
				{
					LLLOJBFMONN.Error("conditionList is null");
				}
			}
			else
			{
				ConditionKeys bHDEBDIHDFM2 = InfoAnimation.JEELAPHJLOE(item);
				if (bHDEBDIHDFM2 != null)
				{
					return bHDEBDIHDFM2;
				}
			}
		}
		return null;
	}

	protected static void CIEHMPCOKGK(List<ConditionAnimation> conditions, List<ConditionKeys> GKHEPKGMEFI)
	{
		foreach (ConditionAnimation item in conditions)
		{
			if (item.Type == ConditionAnimation.DGAGKLODADD.LIST)
			{
				ConditionList eLFKOGJJNMN = item as ConditionList;
				if (eLFKOGJJNMN != null)
				{
					List<ConditionAnimation> kDOGKKGDOBK = eLFKOGJJNMN.KJILOMLMMEN();
					CIEHMPCOKGK(kDOGKKGDOBK, GKHEPKGMEFI);
				}
				else
				{
					LLLOJBFMONN.Error("conditionList is null");
				}
			}
			else
			{
				ConditionKeys bHDEBDIHDFM = InfoAnimation.JEELAPHJLOE(item);
				if (bHDEBDIHDFM != null)
				{
					GKHEPKGMEFI.Add(bHDEBDIHDFM);
				}
			}
		}
	}

	protected virtual void UpdateConditions(List<ConditionAnimation> conditions, ModelObject OECPEDPMKCD, bool EKBOGDKIHIH, bool PHADJMAONJG, ModelObject MJCGOJBGFIE, ModelNode AECCPADGGPG)
	{
		foreach (ConditionAnimation item in conditions)
		{
			if (item == null)
			{
				continue;
			}
			if (item.Type == ConditionAnimation.DGAGKLODADD.DISTANCE)
			{
				ConditionDistance jNPIBKBDJAN = ((item == null) ? null : (item as ConditionDistance));
				if (jNPIBKBDJAN != null)
				{
					jNPIBKBDJAN.KJHPCLOFDJB(OECPEDPMKCD, EKBOGDKIHIH, AECCPADGGPG, PHADJMAONJG, MJCGOJBGFIE);
				}
				else
				{
					LLLOJBFMONN.Error("subcondition is null");
				}
			}
			if (item.Type == ConditionAnimation.DGAGKLODADD.DIRECTION)
			{
				ConditionDirection cFCGJLJBOKI = item as ConditionDirection;
				if (cFCGJLJBOKI != null)
				{
					cFCGJLJBOKI.KJHPCLOFDJB(OECPEDPMKCD, EKBOGDKIHIH, AECCPADGGPG, PHADJMAONJG, MJCGOJBGFIE);
				}
				else
				{
					LLLOJBFMONN.Error("subcondition is null");
				}
			}
			else if (item.Type == ConditionAnimation.DGAGKLODADD.LIST)
			{
				ConditionList eLFKOGJJNMN = item as ConditionList;
				if (eLFKOGJJNMN != null)
				{
					List<ConditionAnimation> kDOGKKGDOBK = eLFKOGJJNMN.KJILOMLMMEN();
					UpdateConditions(kDOGKKGDOBK, OECPEDPMKCD, EKBOGDKIHIH, PHADJMAONJG, MJCGOJBGFIE, AECCPADGGPG);
				}
				else
				{
					LLLOJBFMONN.Error("subconditions is null");
				}
			}
		}
	}

	protected virtual void ACGIFMKPBGC(List<EventAnimation> value)
	{
		IDEMFOLJIFE.AJCMBMJGJEG.AddRange(value);
	}

	protected virtual void EGGLLLLFMCO(List<ConditionAnimation> value)
	{
		IDEMFOLJIFE.JIFAHHGNPFH.AddRange(value);
	}

	protected virtual void OFMGLKAGCGO(List<ConditionAnimation> value)
	{
		IDEMFOLJIFE.HIFPHBNGIPO.AddRange(value);
	}

	protected virtual void FGAEEJBEGEJ(List<ActionAnimation> value)
	{
		IDEMFOLJIFE.DJBAIAKOIHM.AddRange(value);
	}
}
