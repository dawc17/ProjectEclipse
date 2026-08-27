using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

public class TacticsArchiver
{
	public static void GOFALMFEDNF()
	{
		XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(SF2Paths.KKIDGPBOBNI(), "ComputerSettings.xml");
		XmlNode xmlNode = xmlDocument["Settings"]["OutcomeTables"]["Items"]["Weapons"];
		List<string> list = new List<string>();
		list.Add(string.Empty);
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			if (childNode.Name == "Weapon")
			{
				string item = childNode.Attributes["TacticWeapon"].CIPOICEEIBK(string.Empty);
				list.Add(item);
			}
		}
		foreach (string item2 in list)
		{
			GOFALMFEDNF(item2);
			foreach (string item3 in list)
			{
				GOFALMFEDNF(item2, item3);
			}
		}
	}

	private static void GOFALMFEDNF(string LGCMGHAFEDD)
	{
		byte[] LHJNAJKAFIK = new byte[0];
		AddTable(LGCMGHAFEDD, ref LHJNAJKAFIK);
		if (LHJNAJKAFIK.Length > 0)
		{
			string OEMALIFPGPO = string.Empty;
			GetFileName(LGCMGHAFEDD, ref OEMALIFPGPO);
			File.WriteAllBytes(OEMALIFPGPO, Compressor.Compress(LHJNAJKAFIK));
		}
	}

	private static void GOFALMFEDNF(string NDAJLDOMNLK, string AFKFIEAMFKG)
	{
		if (IsWeaponOrder(NDAJLDOMNLK, AFKFIEAMFKG))
		{
			byte[] LHJNAJKAFIK = new byte[0];
			AddTable(NDAJLDOMNLK, AFKFIEAMFKG, ref LHJNAJKAFIK);
			if (NDAJLDOMNLK != AFKFIEAMFKG)
			{
				AddTable(AFKFIEAMFKG, NDAJLDOMNLK, ref LHJNAJKAFIK);
			}
			if (LHJNAJKAFIK.Length > 0)
			{
				string OEMALIFPGPO = string.Empty;
				GetFileName(NDAJLDOMNLK, AFKFIEAMFKG, ref OEMALIFPGPO);
				File.WriteAllBytes(OEMALIFPGPO, Compressor.Compress(LHJNAJKAFIK));
			}
		}
	}

	private static void MFMGMPPALEG()
	{
		List<string> list = new List<string>();
		list.Add(string.Empty);
		list.Add("Fists");
		foreach (string item in list)
		{
			foreach (string item2 in list)
			{
				MFMGMPPALEG(item, item2);
			}
		}
	}

	public static void MFMGMPPALEG(string LGCMGHAFEDD)
	{
		string OEMALIFPGPO = string.Empty;
		if (LGCMGHAFEDD == string.Empty)
		{
			LGCMGHAFEDD = "default";
		}
		GetFileName(LGCMGHAFEDD, ref OEMALIFPGPO);
		byte[] array = ResourceManager.GetBinary(OEMALIFPGPO);
		if (array != null && array.Length > 0)
		{
			byte[] buffer = Compressor.EFJJNIMIBEO(array);
			using (MemoryStream input = new MemoryStream(buffer))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
					{
						AiData.HDHPLDFCDOF hDHPLDFCDOF = (AiData.HDHPLDFCDOF)binaryReader.ReadUInt32();
						string text = TacticalTableHolder.LNOMEMJCIAM(binaryReader);
						if (hDHPLDFCDOF != AiData.HDHPLDFCDOF.shiftTable)
						{
							uint num = binaryReader.ReadUInt32();
							if (0 < num)
							{
								byte[] mLFPOCMGFMB = binaryReader.ReadBytes((int)num);
								TacticalTableHolder jIHNHLAIKAN = new TacticalTableHolder();
								jIHNHLAIKAN.Load(mLFPOCMGFMB, (int)hDHPLDFCDOF, LGCMGHAFEDD);
								AiData.AddTableHolder(jIHNHLAIKAN, text, text, hDHPLDFCDOF);
							}
						}
						else
						{
							if (hDHPLDFCDOF != AiData.HDHPLDFCDOF.shiftTable)
							{
								continue;
							}
							uint num2 = binaryReader.ReadUInt32();
							if (0 >= num2)
							{
								continue;
							}
							byte[] buffer2 = binaryReader.ReadBytes((int)num2);
							using (MemoryStream input2 = new MemoryStream(buffer2))
							{
								using (BinaryReader binaryReader2 = new BinaryReader(input2))
								{
									uint num3 = binaryReader2.ReadUInt32();
									for (int i = 0; i < num3; i++)
									{
										string gOHIIMFFFJI = TacticalTableHolder.LNOMEMJCIAM(binaryReader2);
										InfoAnimation pJAHIOELGGD = AnimationData.BCIFKBJAFEC(gOHIIMFFFJI);
										if (pJAHIOELGGD != null)
										{
											pJAHIOELGGD.OBIBINIEJJE.LoadFromFile(pJAHIOELGGD, binaryReader2);
											continue;
										}
										ModelShiftTable pGDHGIJKPHN = new ModelShiftTable();
										pGDHGIJKPHN.LoadFromFile(null, binaryReader2);
									}
								}
							}
						}
					}
				}
			}
		}
		else
		{
			LLLOJBFMONN.Write("file {0} not unzip", OEMALIFPGPO);
		}
		GC.Collect();
	}

	public static void MFMGMPPALEG(string NDAJLDOMNLK, string AFKFIEAMFKG)
	{
		if (!IsWeaponOrder(NDAJLDOMNLK, AFKFIEAMFKG))
		{
			return;
		}
		string OEMALIFPGPO = string.Empty;
		GetFileName(NDAJLDOMNLK, AFKFIEAMFKG, ref OEMALIFPGPO);
		byte[] array = ResourceManager.GetBinary(OEMALIFPGPO);
		if (array != null && array.Length > 0)
		{
			byte[] buffer = Compressor.EFJJNIMIBEO(array);
			using (MemoryStream input = new MemoryStream(buffer))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
					{
						AiData.HDHPLDFCDOF hDHPLDFCDOF = (AiData.HDHPLDFCDOF)binaryReader.ReadUInt32();
						if (AiData.CheckIfTableExists(NDAJLDOMNLK, AFKFIEAMFKG, hDHPLDFCDOF))
						{
							LLLOJBFMONN.Write("Skipping - table {0}/{1} !", NDAJLDOMNLK, AFKFIEAMFKG);
							return;
						}
						LLLOJBFMONN.Write("Reading - table {0}/{1} !", NDAJLDOMNLK, AFKFIEAMFKG);
						string kEEMLGNLKPF = TacticalTableHolder.LNOMEMJCIAM(binaryReader);
						string text = TacticalTableHolder.LNOMEMJCIAM(binaryReader);
						byte[] mLFPOCMGFMB = new byte[0];
						uint num = binaryReader.ReadUInt32();
						if (0 < num)
						{
							mLFPOCMGFMB = binaryReader.ReadBytes((int)num);
						}
						TacticalTableHolder jIHNHLAIKAN = new TacticalTableHolder();
						jIHNHLAIKAN.Load(mLFPOCMGFMB, (int)hDHPLDFCDOF, text);
						AiData.AddTableHolder(jIHNHLAIKAN, kEEMLGNLKPF, text, hDHPLDFCDOF);
					}
				}
			}
		}
		else
		{
			LLLOJBFMONN.Write("file {0} not unzip", OEMALIFPGPO);
		}
		GC.Collect();
	}

	private static void AddTable(string LGCMGHAFEDD, ref byte[] LHJNAJKAFIK)
	{
		string dCOPLCIFCFL = SF2Paths.KKIDGPBOBNI() + "/tactics/dodge/" + LGCMGHAFEDD + ".tbs";
		string dCOPLCIFCFL2 = SF2Paths.KKIDGPBOBNI() + "/tactics/shiftTables/" + LGCMGHAFEDD + ".sts";
		byte[] array = ResourceManager.GetBinary(dCOPLCIFCFL);
		if (array != null && array.Length > 0)
		{
			LHJNAJKAFIK = Concat(LHJNAJKAFIK, BitConverter.GetBytes(2u));
			LHJNAJKAFIK = Concat(LHJNAJKAFIK, Encoding.ASCII.GetBytes(LGCMGHAFEDD));
			LHJNAJKAFIK = Concat(LHJNAJKAFIK, array);
		}
		byte[] array2 = ResourceManager.GetBinary(dCOPLCIFCFL2);
		if (array2 != null && array2.Length > 0)
		{
			LHJNAJKAFIK = Concat(LHJNAJKAFIK, BitConverter.GetBytes(7u));
			LHJNAJKAFIK = Concat(LHJNAJKAFIK, Encoding.ASCII.GetBytes(LGCMGHAFEDD));
			LHJNAJKAFIK = Concat(LHJNAJKAFIK, array2);
		}
	}

	private static void AddTable(string NDAJLDOMNLK, string AFKFIEAMFKG, ref byte[] LHJNAJKAFIK)
	{
		string text = NDAJLDOMNLK + "_" + AFKFIEAMFKG + ".tbs";
		string dCOPLCIFCFL = SF2Paths.KKIDGPBOBNI() + "/tactics/movements/" + text;
		string dCOPLCIFCFL2 = SF2Paths.KKIDGPBOBNI() + "/tactics/outcometablesforattack/" + text;
		byte[] array = ResourceManager.GetBinary(dCOPLCIFCFL);
		if (array != null && array.Length > 0)
		{
			LHJNAJKAFIK = Concat(LHJNAJKAFIK, BitConverter.GetBytes(1u));
			LHJNAJKAFIK = Concat(LHJNAJKAFIK, Encoding.ASCII.GetBytes(NDAJLDOMNLK));
			LHJNAJKAFIK = Concat(LHJNAJKAFIK, Encoding.ASCII.GetBytes(AFKFIEAMFKG));
			LHJNAJKAFIK = Concat(LHJNAJKAFIK, array);
		}
		byte[] array2 = ResourceManager.GetBinary(dCOPLCIFCFL2);
		if (array2 != null && array2.Length > 0)
		{
			LHJNAJKAFIK = Concat(LHJNAJKAFIK, BitConverter.GetBytes(0u));
			LHJNAJKAFIK = Concat(LHJNAJKAFIK, Encoding.ASCII.GetBytes(NDAJLDOMNLK));
			LHJNAJKAFIK = Concat(LHJNAJKAFIK, Encoding.ASCII.GetBytes(AFKFIEAMFKG));
			LHJNAJKAFIK = Concat(LHJNAJKAFIK, array2);
		}
	}

	private static bool IsWeaponOrder(string NDAJLDOMNLK, string AFKFIEAMFKG)
	{
		return NDAJLDOMNLK == AFKFIEAMFKG || NDAJLDOMNLK == string.Empty || (AFKFIEAMFKG != string.Empty && string.Compare(NDAJLDOMNLK, AFKFIEAMFKG) < 0);
	}

	private static int GetFileName(string LGCMGHAFEDD, ref string OEMALIFPGPO)
	{
		OEMALIFPGPO = SF2Paths.KKIDGPBOBNI() + "/tactics_compressed/" + LGCMGHAFEDD.ToLower() + ".atf";
		return OEMALIFPGPO.Length;
	}

	private static int GetFileName(string NDAJLDOMNLK, string AFKFIEAMFKG, ref string OEMALIFPGPO)
	{
		OEMALIFPGPO = SF2Paths.KKIDGPBOBNI() + "/tactics_compressed/" + NDAJLDOMNLK.ToLower() + "_" + AFKFIEAMFKG.ToLower() + ".atf";
		return OEMALIFPGPO.Length;
	}

	private static byte[] Concat(byte[] DHDMNHCIPEH, byte[] BGEEALIPKCC)
	{
		byte[] array = new byte[DHDMNHCIPEH.Length + BGEEALIPKCC.Length];
		DHDMNHCIPEH.CopyTo(array, 0);
		BGEEALIPKCC.CopyTo(array, DHDMNHCIPEH.Length);
		return array;
	}
}
