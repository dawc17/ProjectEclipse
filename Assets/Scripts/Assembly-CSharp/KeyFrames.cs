using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyFrames
{
	public class Frame
	{
		public List<Vector3f> Data;

		public int Size;
	}

	protected List<Frame> DCFPONJAING = new List<Frame>();

	private bool _IsInterruptFramesSeted;

	protected int BOAIMPGFINL;

	protected int BCFLAENGDHA;

	public int Size
	{
		get
		{
			return OLINNGEMHMG();
		}
	}

	public int DEHLAIGHHPD
	{
		get
		{
			return HEBNLOODNFE();
		}
	}

	public int KHAODKDIOHH
	{
		get
		{
			return FNEPPBAKIDP();
		}
	}

	public int OLINNGEMHMG()
	{
		return BOAIMPGFINL;
	}

	public int HEBNLOODNFE()
	{
		return BCFLAENGDHA;
	}

	public int FNEPPBAKIDP()
	{
		return BOAIMPGFINL - BCFLAENGDHA;
	}

	public void NEOLKFNMAHJ()
	{
		BCFLAENGDHA++;
	}

	public void InterruptFramesSeted(int FPDMCHPHFAJ)
	{
		BOAIMPGFINL = 2;
		_IsInterruptFramesSeted = true;
		if (DCFPONJAING.Count < BOAIMPGFINL)
		{
			NLMAIINFADE(2, FPDMCHPHFAJ);
		}
		for (int i = 0; i < BOAIMPGFINL; i++)
		{
			Frame cJMFONMNFBI = DCFPONJAING[i];
			if (cJMFONMNFBI.Size != FPDMCHPHFAJ)
			{
				if (cJMFONMNFBI.Size < FPDMCHPHFAJ)
				{
					cJMFONMNFBI.Data.CPCAJIKOIEE(FPDMCHPHFAJ);
				}
				cJMFONMNFBI.Size = FPDMCHPHFAJ;
			}
		}
	}

	public Frame KLNOLPIADNN(int DCHCFFFFLLK)
	{
		if (BOAIMPGFINL <= DCHCFFFFLLK)
		{
			return null;
		}
		return DCFPONJAING[DCHCFFFFLLK];
	}

	public Frame JGEBADMHJCP(int OCDKOFPGCHH)
	{
		return DCFPONJAING[BCFLAENGDHA + OCDKOFPGCHH];
	}

	public void Shift(float HLBMDDOPKKL, float ELAKEOGEDPN = 0f, float PIIFLHIBODE = 0f)
	{
		for (int i = (_IsInterruptFramesSeted ? 2 : 0); i < BOAIMPGFINL; i++)
		{
			for (int j = 0; j < DCFPONJAING[i].Size; j++)
			{
				DCFPONJAING[i].Data[j].Add(HLBMDDOPKKL, ELAKEOGEDPN, PIIFLHIBODE);
			}
		}
	}

	public void NKHEGNLGJIG()
	{
		for (int i = (_IsInterruptFramesSeted ? 2 : 0); i < BOAIMPGFINL; i++)
		{
			for (int j = 0; j < DCFPONJAING[i].Size; j++)
			{
				Vector3f eMAFACPEPDK = DCFPONJAING[i].Data[j];
				eMAFACPEPDK.JPFALPBDBAP(eMAFACPEPDK.GILCBJJPKBK() * -1f);
			}
		}
	}

	public void Reset()
	{
		BOAIMPGFINL = 0;
		BCFLAENGDHA = 0;
		_IsInterruptFramesSeted = false;
	}

	public void HAILLLEPCHP(int AMNCLCPADOO, int IFIOLDFCLIE, bool HOHEFHKJIOG, Vector3[][] GHDPPHAAPCA)
	{
		if (HOHEFHKJIOG)
		{
			int num = Math.Min(GHDPPHAAPCA.Length - 1, AMNCLCPADOO + 2);
			SetFrame(GHDPPHAAPCA[num]);
			SetFrame(GHDPPHAAPCA[num]);
		}
		for (int i = AMNCLCPADOO; i <= IFIOLDFCLIE; i++)
		{
			SetFrame(GHDPPHAAPCA[i]);
		}
	}

	public void SetFrame(Vector3[] GHDPPHAAPCA)
	{
		BOAIMPGFINL++;
		if (DCFPONJAING.Count < BOAIMPGFINL)
		{
			NLMAIINFADE(1, GHDPPHAAPCA.Length);
		}
		Frame cJMFONMNFBI = DCFPONJAING[BOAIMPGFINL - 1];
		if (cJMFONMNFBI.Size != GHDPPHAAPCA.Length)
		{
			if (cJMFONMNFBI.Size < GHDPPHAAPCA.Length)
			{
				cJMFONMNFBI.Data.CPCAJIKOIEE(GHDPPHAAPCA.Length);
			}
			cJMFONMNFBI.Size = GHDPPHAAPCA.Length;
		}
		for (int i = 0; i < GHDPPHAAPCA.Length; i++)
		{
			cJMFONMNFBI.Data[i].Set(GHDPPHAAPCA[i]);
		}
	}

	protected void NLMAIINFADE(int GNDPBMIJEMH, int DGHIGGGFNLP)
	{
		for (int i = 0; i < GNDPBMIJEMH; i++)
		{
			Frame cJMFONMNFBI = new Frame();
			cJMFONMNFBI.Data = new List<Vector3f>(DGHIGGGFNLP);
			DCFPONJAING.Add(cJMFONMNFBI);
			for (int j = 0; j < DGHIGGGFNLP; j++)
			{
				cJMFONMNFBI.Data.Add(new Vector3f());
			}
			cJMFONMNFBI.Size = DGHIGGGFNLP;
		}
	}
}
