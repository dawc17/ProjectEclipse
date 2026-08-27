using System.IO;

public class InBuffer
{
	private byte[] m_Buffer;

	private uint AIJNPGBGLBN;

	private uint LCEJOIDPMEK;

	private uint FAAGKBAPFOM;

	private Stream m_Stream;

	private bool m_StreamWasExhausted;

	private ulong m_ProcessedSize;

	public InBuffer(uint KOGACKBGCFP)
	{
		m_Buffer = new byte[KOGACKBGCFP];
		FAAGKBAPFOM = KOGACKBGCFP;
	}

	public void Init(Stream ABJIEFMMIEK)
	{
		m_Stream = ABJIEFMMIEK;
		m_ProcessedSize = 0uL;
		LCEJOIDPMEK = 0u;
		AIJNPGBGLBN = 0u;
		m_StreamWasExhausted = false;
	}

	public bool ONDIFDJPMDM()
	{
		if (m_StreamWasExhausted)
		{
			return false;
		}
		m_ProcessedSize += AIJNPGBGLBN;
		int num = m_Stream.Read(m_Buffer, 0, (int)FAAGKBAPFOM);
		AIJNPGBGLBN = 0u;
		LCEJOIDPMEK = (uint)num;
		m_StreamWasExhausted = num == 0;
		return !m_StreamWasExhausted;
	}

	public void IAIFCIAAHOE()
	{
		m_Stream = null;
	}

	public bool ReadByte(byte AAOIAEJJINO)
	{
		if (AIJNPGBGLBN >= LCEJOIDPMEK && !ONDIFDJPMDM())
		{
			return false;
		}
		AAOIAEJJINO = m_Buffer[AIJNPGBGLBN++];
		return true;
	}

	public byte ReadByte()
	{
		if (AIJNPGBGLBN >= LCEJOIDPMEK && !ONDIFDJPMDM())
		{
			return byte.MaxValue;
		}
		return m_Buffer[AIJNPGBGLBN++];
	}

	public ulong GBFBDFIGOJE()
	{
		return m_ProcessedSize + AIJNPGBGLBN;
	}
}
