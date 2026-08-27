using System.Collections.Generic;

public class EventTriggers
{
	public List<Trigger> GIFPBBKCKIK;

	public List<Trigger> POJOOBPNEAK;

	public List<Trigger> ILNELINMJEG;

	public List<Trigger> EMKHLEINEBI;

	public List<Trigger> KHMAPLHHBDI;

	public List<Trigger> LNHBEDPOGBI;

	public List<Trigger> PADGFKKHGFF;

	public List<Trigger> ECOIADPIBJJ;

	public List<Trigger> IFDEGOAJDBP;

	public List<Trigger> GCKHGBKLPHM;

	public List<Trigger> JDGIMJPEGON;

	public List<Trigger> NIMANOOEBAJ;

	public List<Trigger> KMAAHHEBKMG;

	public void NKKOAAKHINN()
	{
		GIFPBBKCKIK.Clear();
		POJOOBPNEAK.Clear();
		ILNELINMJEG.Clear();
		EMKHLEINEBI.Clear();
		KHMAPLHHBDI.Clear();
		LNHBEDPOGBI.Clear();
		PADGFKKHGFF.Clear();
		ECOIADPIBJJ.Clear();
		IFDEGOAJDBP.Clear();
		GCKHGBKLPHM.Clear();
		JDGIMJPEGON.Clear();
		NIMANOOEBAJ.Clear();
		KMAAHHEBKMG.Clear();
	}

	public void HGPNHBMHIKH(List<Trigger> JJAEKPONOBM)
	{
		foreach (Trigger item in JJAEKPONOBM)
		{
			List<EventAnimation> aJCMBMJGJEG = item.IDEMFOLJIFE.AJCMBMJGJEG;
			foreach (EventAnimation item2 in aJCMBMJGJEG)
			{
				List<Trigger> list = KPMMHDGEBCB(item2.Type);
				int count = list.Count;
				if (count == 0 || list[count - 1] != item)
				{
					list.Add(item);
				}
			}
			GIFPBBKCKIK.Add(item);
		}
	}

	private List<Trigger> KPMMHDGEBCB(EventAnimation.EECEJKADLCK LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case EventAnimation.EECEJKADLCK.EVENT_ROUND_STAGE:
			return POJOOBPNEAK;
		case EventAnimation.EECEJKADLCK.EVENT_KEY_PRESSED:
			return ILNELINMJEG;
		case EventAnimation.EECEJKADLCK.EVENT_KEY_RELEASED:
			return EMKHLEINEBI;
		case EventAnimation.EECEJKADLCK.EVENT_ANIMATION_START:
			return KHMAPLHHBDI;
		case EventAnimation.EECEJKADLCK.EVENT_ANIMATION_END:
			return LNHBEDPOGBI;
		case EventAnimation.EECEJKADLCK.EVENT_INTERVAL_START:
			return PADGFKKHGFF;
		case EventAnimation.EECEJKADLCK.EVENT_INTERVAL_END:
			return ECOIADPIBJJ;
		case EventAnimation.EECEJKADLCK.EVENT_HIT:
			return IFDEGOAJDBP;
		case EventAnimation.EECEJKADLCK.EVENT_STRIKE:
			return GCKHGBKLPHM;
		case EventAnimation.EECEJKADLCK.EVENT_EVERY_FRAME:
			return JDGIMJPEGON;
		case EventAnimation.EECEJKADLCK.EVENT_BIRTH:
			return NIMANOOEBAJ;
		case EventAnimation.EECEJKADLCK.EVENT_MOD_EXPIRES:
			return KMAAHHEBKMG;
		default:
			return GIFPBBKCKIK;
		}
	}
}
