using System;

internal interface IDeflater : IDisposable
{
	bool NeedsInput();

	void SetInput(byte[] MMFIPPNMIKJ, int CAILGDNIKJD, int count);

	int GetDeflateOutput(byte[] EKJJNOOPFNJ);

	bool Finish(byte[] EKJJNOOPFNJ, out int GJBPPJIGAIG);
}
