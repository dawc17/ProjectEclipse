using System.IO;

public class OutBuffer
{
	private byte[] m_Buffer;

	private uint AIJNPGBGLBN;

	private uint FAAGKBAPFOM;

	private Stream m_Stream;

	private ulong m_ProcessedSize;

	public OutBuffer(uint KOGACKBGCFP)
	{
		m_Buffer = new byte[KOGACKBGCFP];
		FAAGKBAPFOM = KOGACKBGCFP;
	}

	public void SetStream(Stream ABJIEFMMIEK)
	{
		m_Stream = ABJIEFMMIEK;
	}

	public void PDFBMGAJEHM()
	{
		m_Stream.Flush();
	}

	public void GKKOGGMCJEC()
	{
		m_Stream.Close();
	}

	public void IAIFCIAAHOE()
	{
		m_Stream = null;
	}

	public void Init()
	{
		m_ProcessedSize = 0uL;
		AIJNPGBGLBN = 0u;
	}

	public void WriteByte(byte AAOIAEJJINO)
	{
		m_Buffer[AIJNPGBGLBN++] = AAOIAEJJINO;
		if (AIJNPGBGLBN >= FAAGKBAPFOM)
		{
			DMHMONMENHH();
		}
	}

	public void DMHMONMENHH()
	{
		if (AIJNPGBGLBN != 0)
		{
			m_Stream.Write(m_Buffer, 0, (int)AIJNPGBGLBN);
			AIJNPGBGLBN = 0u;
		}
	}

	public ulong GBFBDFIGOJE()
	{
		return m_ProcessedSize + AIJNPGBGLBN;
	}
}
