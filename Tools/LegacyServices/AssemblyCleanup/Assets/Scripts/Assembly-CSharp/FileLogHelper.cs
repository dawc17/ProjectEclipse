using System.IO;
using System.Text;

public static class FileLogHelper
{
	private static StringBuilder NGPACMILENE = new StringBuilder();

	public static void KDOOPDIPNLA(string DMKMNOINKFC)
	{
		NGPACMILENE.AppendLine(DMKMNOINKFC);
		Save();
	}

	private static void Save()
	{
		File.WriteAllText("/Users/otarius/Nekki/Temp/u.txt", NGPACMILENE.ToString());
	}
}
