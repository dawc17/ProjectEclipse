using System;

internal class DeflaterManaged : IDisposable, IDeflater
{
	private enum FKCGLKODHMG
	{
		NotStarted = 0,
		SlowDownForIncompressible1 = 1,
		SlowDownForIncompressible2 = 2,
		StartingSmallData = 3,
		CompressThenCheck = 4,
		CheckingForIncompressible = 5,
		HandlingSmallData = 6
	}

	private const int HHDHGAJOJDG = 256;

	private const int CFLAPKCCHDK = 120;

	private const int NHPIKMKOLHB = 8072;

	private const double BadCompressionThreshold = 1.0;

	private FastEncoder LIKCODMHOFC;

	private CopyEncoder EHKLJCLGGKN;

	private DeflateInput NILNDHEKNLJ;

	private OutputBuffer output;

	private FKCGLKODHMG LBINABAOCOB;

	private DeflateInput BJOMPEOMAEB;

	internal DeflaterManaged()
	{
		LIKCODMHOFC = new FastEncoder();
		EHKLJCLGGKN = new CopyEncoder();
		NILNDHEKNLJ = new DeflateInput();
		output = new OutputBuffer();
		LBINABAOCOB = FKCGLKODHMG.NotStarted;
	}

	private bool NeedsInput()
	{
		return ((IDeflater)this).NeedsInput();
	}

	bool IDeflater.NeedsInput()
	{
		return NILNDHEKNLJ.OFOPFCJNEBL() == 0 && LIKCODMHOFC.EHHKMDHLKHJ() == 0;
	}

	void IDeflater.SetInput(byte[] MMFIPPNMIKJ, int CAILGDNIKJD, int count)
	{
		NILNDHEKNLJ.set_Buffer(MMFIPPNMIKJ);
		NILNDHEKNLJ.CHILOKHFALD(count);
		NILNDHEKNLJ.MOFAGMEDPNM(CAILGDNIKJD);
		if (count > 0 && count < 256)
		{
			switch (LBINABAOCOB)
			{
			case FKCGLKODHMG.NotStarted:
			case FKCGLKODHMG.CheckingForIncompressible:
				LBINABAOCOB = FKCGLKODHMG.StartingSmallData;
				break;
			case FKCGLKODHMG.CompressThenCheck:
				LBINABAOCOB = FKCGLKODHMG.HandlingSmallData;
				break;
			}
		}
	}

	int IDeflater.GetDeflateOutput(byte[] EKJJNOOPFNJ)
	{
		output.UpdateBuffer(EKJJNOOPFNJ);
		switch (LBINABAOCOB)
		{
		case FKCGLKODHMG.NotStarted:
		{
			DeflateInput.BKLHEBEBFFD pIFKPLHIOFJ3 = NILNDHEKNLJ.ENBODKKOALL();
			OutputBuffer.LHFANIPMGPA pIFKPLHIOFJ4 = output.ENBODKKOALL();
			LIKCODMHOFC.ILPKALKNGIP(output);
			LIKCODMHOFC.IOKJILLFPDI(NILNDHEKNLJ, output);
			if (!UseCompressed(LIKCODMHOFC.LMLAIGGBPFL()))
			{
				NILNDHEKNLJ.BIDLPPIPACF(pIFKPLHIOFJ3);
				output.BIDLPPIPACF(pIFKPLHIOFJ4);
				EHKLJCLGGKN.GetBlock(NILNDHEKNLJ, output, false);
				MFMCDOAMEHP();
				LBINABAOCOB = FKCGLKODHMG.CheckingForIncompressible;
			}
			else
			{
				LBINABAOCOB = FKCGLKODHMG.CompressThenCheck;
			}
			break;
		}
		case FKCGLKODHMG.CompressThenCheck:
			LIKCODMHOFC.IOKJILLFPDI(NILNDHEKNLJ, output);
			if (!UseCompressed(LIKCODMHOFC.LMLAIGGBPFL()))
			{
				LBINABAOCOB = FKCGLKODHMG.SlowDownForIncompressible1;
				BJOMPEOMAEB = LIKCODMHOFC.EGHDOBABAFB();
			}
			break;
		case FKCGLKODHMG.SlowDownForIncompressible1:
			LIKCODMHOFC.ADMOOJIAFEI(output);
			LBINABAOCOB = FKCGLKODHMG.SlowDownForIncompressible2;
			goto case FKCGLKODHMG.SlowDownForIncompressible2;
		case FKCGLKODHMG.SlowDownForIncompressible2:
			if (BJOMPEOMAEB.OFOPFCJNEBL() > 0)
			{
				EHKLJCLGGKN.GetBlock(BJOMPEOMAEB, output, false);
			}
			if (BJOMPEOMAEB.OFOPFCJNEBL() == 0)
			{
				LIKCODMHOFC.BMLBPABODCO();
				LBINABAOCOB = FKCGLKODHMG.CheckingForIncompressible;
			}
			break;
		case FKCGLKODHMG.CheckingForIncompressible:
		{
			DeflateInput.BKLHEBEBFFD pIFKPLHIOFJ = NILNDHEKNLJ.ENBODKKOALL();
			OutputBuffer.LHFANIPMGPA pIFKPLHIOFJ2 = output.ENBODKKOALL();
			LIKCODMHOFC.GetBlock(NILNDHEKNLJ, output, 8072);
			if (!UseCompressed(LIKCODMHOFC.LMLAIGGBPFL()))
			{
				NILNDHEKNLJ.BIDLPPIPACF(pIFKPLHIOFJ);
				output.BIDLPPIPACF(pIFKPLHIOFJ2);
				EHKLJCLGGKN.GetBlock(NILNDHEKNLJ, output, false);
				MFMCDOAMEHP();
			}
			break;
		}
		case FKCGLKODHMG.StartingSmallData:
			LIKCODMHOFC.ILPKALKNGIP(output);
			LBINABAOCOB = FKCGLKODHMG.HandlingSmallData;
			goto case FKCGLKODHMG.HandlingSmallData;
		case FKCGLKODHMG.HandlingSmallData:
			LIKCODMHOFC.IOKJILLFPDI(NILNDHEKNLJ, output);
			break;
		}
		return output.GEBLFKFACKO();
	}

	bool IDeflater.Finish(byte[] EKJJNOOPFNJ, out int GJBPPJIGAIG)
	{
		if (LBINABAOCOB == FKCGLKODHMG.NotStarted)
		{
			GJBPPJIGAIG = 0;
			return true;
		}
		output.UpdateBuffer(EKJJNOOPFNJ);
		if (LBINABAOCOB == FKCGLKODHMG.CompressThenCheck || LBINABAOCOB == FKCGLKODHMG.HandlingSmallData || LBINABAOCOB == FKCGLKODHMG.SlowDownForIncompressible1)
		{
			LIKCODMHOFC.ADMOOJIAFEI(output);
		}
		PLDEOFFHGDO();
		GJBPPJIGAIG = output.GEBLFKFACKO();
		return true;
	}

	void IDisposable.Dispose()
	{
	}

	protected void Dispose(bool KLCPNDHEBGP)
	{
	}

	private bool UseCompressed(double ratio)
	{
		return ratio <= 1.0;
	}

	private void MFMCDOAMEHP()
	{
		LIKCODMHOFC.BMLBPABODCO();
	}

	private void PLDEOFFHGDO()
	{
		EHKLJCLGGKN.GetBlock(null, output, true);
	}
}
