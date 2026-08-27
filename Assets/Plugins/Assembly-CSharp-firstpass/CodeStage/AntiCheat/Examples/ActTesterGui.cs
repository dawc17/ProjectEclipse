using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace CodeStage.AntiCheat.Examples
{
	[AddComponentMenu("")]
	public class ActTesterGui : MonoBehaviour
	{
		private const string KHAMBPGILCG = "#FF4040";

		private const string OLJKKKGHPBI = "#02C85F";

		private const string AGHDKHFHEPL = "name";

		private const string HMABOENBKHP = "money";

		private const string MIAHDAJCHOM = "lifeBar";

		private const string CEKEKDEPJNG = "gameComplete";

		private const string HJDHCAHHCDH = "demoUint";

		private const string IALKMKNGCAM = "demoLong";

		private const string MLOODEINEDD = "demoDouble";

		private const string PMDLJFEEAMA = "demoVector2";

		private const string NGEKALCENHG = "demoVector3";

		private const string PIBHBOAHCHG = "demoQuaternion";

		private const string KPANFHDDGFA = "demoRect";

		private const string DIDANHNOBGI = "demoColor";

		private const string AHBIMCEOIKN = "demoByteArray";

		private const string PLNHFGEKFCK = "http://j.mp/1gxg1tf";

		private const string AHJJPLILHNM = "http://j.mp/1iBK5pz";

		private const string DCJCMMAMFFI = "http://j.mp/1FRAL5L";

		private const string FNCHKBIIBEA = "http://j.mp/1LCdpDa";

		private const string MEBEDDGJHNP = "http://j.mp/1KVrpxi";

		private const string GEPEPFPLCNL = "http://docs.unity3d.com/ScriptReference/PlayerPrefs.html";

		[Header("Regular variables")]
		public string regularString = "I'm regular string";

		public int regularInt = 1987;

		public float regularFloat = 2013.0524f;

		public Vector3 regularVector3 = new Vector3(10.5f, 11.5f, 12.5f);

		[Header("Obscured (secure) variables")]
		public ObscuredString obscuredString = (ObscuredString)("I'm obscured string");

		public ObscuredInt obscuredInt = (ObscuredInt)(1987);

		public ObscuredFloat obscuredFloat = (ObscuredFloat)(2013.0524f);

		public ObscuredVector3 obscuredVector3 = (ObscuredVector3)(new Vector3(10.5f, 11.5f, 12.5f));

		public ObscuredBool obscuredBool = (ObscuredBool)(true);

		public ObscuredLong obscuredLong = (ObscuredLong)(945678987654123345L);

		public ObscuredDouble obscuredDouble = (ObscuredDouble)(9.45678987654);

		public ObscuredVector2 obscuredVector2 = (ObscuredVector2)(new Vector2(8.5f, 9.5f));

		[Header("Other")]
		public string prefsEncryptionKey = "change me!";

		private readonly string[] tabs = new string[3] { "Variables protection", "Saves protection", "Cheating detectors" };

		private int JEMIENMLNKL;

		private string DGMEPCBKPEA;

		private string JGENFAEFIGJ;

		private string GMBAPAPCHCJ;

		private int DNHIHOBELMC;

		private bool LOLIHLHKEMG;

		private bool HMJKPKPCDEH;

		private bool OCEFNFANBGG;

		private bool FPJPGKBDMBF;

		private bool ALLDLDNMKHD;

		private bool MBPMDGDAFIB;

		private readonly StringBuilder logBuilder = new StringBuilder();

		public void OnSpeedHackDetected()
		{
			FPJPGKBDMBF = true;
			Debug.Log("Speed hack Detected!");
		}

		public void OnInjectionDetected()
		{
			OCEFNFANBGG = true;
			Debug.Log("Injection Detected!");
		}

		public void OnObscuredTypeCheatingDetected()
		{
			ALLDLDNMKHD = true;
			Debug.Log("Obscured Vars Cheating Detected!");
		}

		public void OnWallHackDetected()
		{
			MBPMDGDAFIB = true;
			Debug.Log("Wall hack Detected!");
		}

		private void OnValidate()
		{
			if (Application.isPlaying)
			{
				ObscuredPrefs.PPNGALKEMIO(prefsEncryptionKey);
			}
		}

		private void Awake()
		{
			ObscuredPrefs.PPNGALKEMIO(prefsEncryptionKey);
			ObscuredPrefs.BOFHLEDGKGJ = ECHDGOJOJEB;
			ObscuredPrefs.HHFALGBHMFH = MFICIHICJAL;
		}

		private void Start()
		{
			LELCPGOLMME();
			EGMDGPNLLEH();
			BEPHKDNOCLJ();
			CLODPJMGPBI();
			Invoke("RandomizeObscuredVars", UnityEngine.Random.Range(1f, 10f));
		}

		private void RandomizeObscuredVars()
		{
			obscuredInt.GMCADPGOCHM();
			obscuredFloat.GMCADPGOCHM();
			obscuredString.GMCADPGOCHM();
			obscuredVector3.GMCADPGOCHM();
			Invoke("RandomizeObscuredVars", UnityEngine.Random.Range(1f, 10f));
		}

		private void LELCPGOLMME()
		{
			logBuilder.Length = 0;
			logBuilder.AppendLine("[ACTk] <b>[ ObscuredString test ]</b>");
			ObscuredString.SetNewCryptoKey("I LOVE MY GIRLz");
			string text = "the Goscurry is not a lie ;)";
			logBuilder.AppendLine("Original string:\n" + text);
			ObscuredString obscuredString = (ObscuredString)(text);
			logBuilder.AppendLine("How your string is stored in memory when obscured:\n" + obscuredString.ECEBFGCJIDA());
			Debug.Log(logBuilder);
		}

		private void EGMDGPNLLEH()
		{
			logBuilder.Length = 0;
			logBuilder.AppendLine("[ACTk] <b>[ ObscuredInt test ]</b>");
			ObscuredInt.SetNewCryptoKey(434523);
			int num = 5;
			logBuilder.AppendLine("Original lives count: " + num);
			ObscuredInt bAINMLLIKOL = (ObscuredInt)(num);
			logBuilder.AppendLine("How your lives count is stored in memory when obscured: " + bAINMLLIKOL.ECEBFGCJIDA());
			ObscuredInt.SetNewCryptoKey(666);
			num = (int)(bAINMLLIKOL);
			bAINMLLIKOL = (ObscuredInt)((int)(bAINMLLIKOL) - 2);
			bAINMLLIKOL = (ObscuredInt)((int)(bAINMLLIKOL) + num + 10);
			bAINMLLIKOL = (ObscuredInt)((int)(bAINMLLIKOL) / 2);
			bAINMLLIKOL = ObscuredInt.ALEAHDHGCJL(bAINMLLIKOL);
			ObscuredInt.SetNewCryptoKey(999);
			bAINMLLIKOL = ObscuredInt.ALEAHDHGCJL(bAINMLLIKOL);
			bAINMLLIKOL = ObscuredInt.DDKOKLNFNPB(bAINMLLIKOL);
			logBuilder.AppendLine(string.Concat("Lives count after few usual operations: ", bAINMLLIKOL, " (", bAINMLLIKOL.ToString("X"), "h)"));
			Debug.Log(logBuilder);
		}

		private void BEPHKDNOCLJ()
		{
			logBuilder.Length = 0;
			logBuilder.AppendLine("[ACTk] <b>[ ObscuredFloat test ]</b>");
			ObscuredFloat.SetNewCryptoKey(404);
			float num = 99.9f;
			logBuilder.AppendLine("Original health bar: " + num);
			ObscuredFloat bAINMLLIKOL = (ObscuredFloat)(num);
			logBuilder.AppendLine("How your health bar is stored in memory when obscured: " + bAINMLLIKOL.ECEBFGCJIDA());
			ObscuredFloat.SetNewCryptoKey(666);
			bAINMLLIKOL = (ObscuredFloat)((float)(bAINMLLIKOL) + 6f);
			bAINMLLIKOL = (ObscuredFloat)((float)(bAINMLLIKOL) - 1.5f);
			bAINMLLIKOL = ObscuredFloat.ALEAHDHGCJL(bAINMLLIKOL);
			bAINMLLIKOL = ObscuredFloat.DDKOKLNFNPB(bAINMLLIKOL);
			bAINMLLIKOL = ObscuredFloat.DDKOKLNFNPB(bAINMLLIKOL);
			bAINMLLIKOL = (ObscuredFloat)(num - (float)(bAINMLLIKOL) + 10.5f);
			logBuilder.AppendLine("Health bar after few usual operations: " + bAINMLLIKOL);
			Debug.Log(logBuilder);
		}

		private void CLODPJMGPBI()
		{
			logBuilder.Length = 0;
			logBuilder.AppendLine("[ACTk] <b>[ ObscuredVector3 test ]</b>");
			ObscuredVector3.SetNewCryptoKey(404);
			Vector3 vector = new Vector3(54.1f, 64.3f, 63.2f);
			logBuilder.AppendLine("Original position: " + vector);
			ObscuredVector3 rawObfuscatedVector = (ObscuredVector3)(vector);
			ObscuredVector3.RawEncryptedVector3 rawEncryptedVector = rawObfuscatedVector.ECEBFGCJIDA();
			logBuilder.AppendLine("How your position is stored in memory when obscured: (" + rawEncryptedVector.x + ", " + rawEncryptedVector.y + ", " + rawEncryptedVector.z + ")");
			Debug.Log(logBuilder);
		}

		private void ECHDGOJOJEB()
		{
			LOLIHLHKEMG = true;
		}

		private void MFICIHICJAL()
		{
			HMJKPKPCDEH = true;
		}

		private void OnGUI()
		{
			GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
			gUIStyle.alignment = TextAnchor.UpperCenter;
			GUILayout.BeginArea(new Rect(10f, 5f, Screen.width - 20, Screen.height - 10));
			GUILayout.Label("<color=\"#0287C8\"><b>Anti-Cheat Toolkit Sandbox</b></color>", gUIStyle);
			GUILayout.Label("Here you can overview common ACTk features and try to cheat something yourself.", gUIStyle);
			GUILayout.Space(5f);
			JEMIENMLNKL = GUILayout.Toolbar(JEMIENMLNKL, tabs);
			if (JEMIENMLNKL == 0)
			{
				GUILayout.Label("ACTk offers own collection of the secure types to let you protect your variables from <b>ANY</b> memory hacking tools (Cheat Engine, ArtMoney, GameCIH, Game Guardian, etc.).");
				GUILayout.Space(5f);
				using (new HorizontalLayout())
				{
					GUILayout.Label("<b>Obscured types:</b>\n<color=\"#75C4EB\">" + MBNHEDGLCLO() + "</color>", GUILayout.MinWidth(130f));
					GUILayout.Space(10f);
					using (new VerticalLayout(GUI.skin.box))
					{
						GUILayout.Label("Below you can try to cheat few variables of the regular types and their obscured (secure) analogues (you may change initial values from Tester object inspector):");
						GUILayout.Space(10f);
						using (new HorizontalLayout())
						{
							GUILayout.Label("<b>string:</b> " + regularString, GUILayout.Width(250f));
							if (GUILayout.Button("Add random value"))
							{
								regularString += (char)UnityEngine.Random.Range(97, 122);
							}
							if (GUILayout.Button("Reset"))
							{
								regularString = string.Empty;
							}
						}
						using (new HorizontalLayout())
						{
							GUILayout.Label("<b>ObscuredString:</b> " + (string)(obscuredString), GUILayout.Width(250f));
							if (GUILayout.Button("Add random value"))
							{
								obscuredString = (ObscuredString)((string)(obscuredString) + (char)UnityEngine.Random.Range(97, 122));
							}
							if (GUILayout.Button("Reset"))
							{
								obscuredString = (ObscuredString)(string.Empty);
							}
						}
						GUILayout.Space(10f);
						using (new HorizontalLayout())
						{
							GUILayout.Label("<b>int:</b> " + regularInt, GUILayout.Width(250f));
							if (GUILayout.Button("Add random value"))
							{
								regularInt += UnityEngine.Random.Range(1, 100);
							}
							if (GUILayout.Button("Reset"))
							{
								regularInt = 0;
							}
						}
						using (new HorizontalLayout())
						{
							GUILayout.Label("<b>ObscuredInt:</b> " + obscuredInt, GUILayout.Width(250f));
							if (GUILayout.Button("Add random value"))
							{
								obscuredInt = (ObscuredInt)((int)(obscuredInt) + UnityEngine.Random.Range(1, 100));
							}
							if (GUILayout.Button("Reset"))
							{
								obscuredInt = (ObscuredInt)(0);
							}
						}
						GUILayout.Space(10f);
						using (new HorizontalLayout())
						{
							GUILayout.Label("<b>float:</b> " + regularFloat, GUILayout.Width(250f));
							if (GUILayout.Button("Add random value"))
							{
								regularFloat += UnityEngine.Random.Range(1f, 100f);
							}
							if (GUILayout.Button("Reset"))
							{
								regularFloat = 0f;
							}
						}
						using (new HorizontalLayout())
						{
							GUILayout.Label("<b>ObscuredFloat:</b> " + obscuredFloat, GUILayout.Width(250f));
							if (GUILayout.Button("Add random value"))
							{
								obscuredFloat = (ObscuredFloat)((float)(obscuredFloat) + UnityEngine.Random.Range(1f, 100f));
							}
							if (GUILayout.Button("Reset"))
							{
								obscuredFloat = (ObscuredFloat)(0f);
							}
						}
						GUILayout.Space(10f);
						using (new HorizontalLayout())
						{
							GUILayout.Label("<b>Vector3:</b> " + regularVector3, GUILayout.Width(250f));
							if (GUILayout.Button("Add random value"))
							{
								regularVector3 += UnityEngine.Random.insideUnitSphere;
							}
							if (GUILayout.Button("Reset"))
							{
								regularVector3 = Vector3.zero;
							}
						}
						using (new HorizontalLayout())
						{
							GUILayout.Label("<b>ObscuredVector3:</b> " + obscuredVector3, GUILayout.Width(250f));
							if (GUILayout.Button("Add random value"))
							{
								obscuredVector3 = ObscuredVector3.PHEFFKMOOCM(obscuredVector3, UnityEngine.Random.insideUnitSphere);
							}
							if (GUILayout.Button("Reset"))
							{
								obscuredVector3 = (ObscuredVector3)(Vector3.zero);
							}
						}
					}
				}
			}
			else if (JEMIENMLNKL == 1)
			{
				GUILayout.Label("ACTk has secure layer for the PlayerPrefs: <color=\"#75C4EB\">ObscuredPrefs</color>. It protects data from view, detects any cheating attempts, optionally locks data to the current device and supports additional data types.");
				GUILayout.Space(5f);
				using (new HorizontalLayout())
				{
					GUILayout.Label("<b>Supported types:</b>\n" + PLJAKCEMHOA(), GUILayout.MinWidth(130f));
					using (new VerticalLayout(GUI.skin.box))
					{
						GUILayout.Label("Below you can try to cheat both regular PlayerPrefs and secure ObscuredPrefs:");
						using (new VerticalLayout())
						{
							GUILayout.Label("<color=\"#FF4040\"><b>PlayerPrefs:</b></color>\neasy to cheat, only 3 supported types", gUIStyle);
							GUILayout.Space(5f);
							if (string.IsNullOrEmpty(JGENFAEFIGJ))
							{
								KKPNIEOLNGP();
							}
							using (new HorizontalLayout())
							{
								GUILayout.Label(JGENFAEFIGJ, GUILayout.Width(270f));
								using (new VerticalLayout())
								{
									using (new HorizontalLayout())
									{
										if (GUILayout.Button("Save"))
										{
											COPMPOFEGJO();
										}
										if (GUILayout.Button("Load"))
										{
											KKPNIEOLNGP();
										}
									}
									if (GUILayout.Button("Delete"))
									{
										FOPIDOGDNJB();
									}
								}
							}
						}
						GUILayout.Space(5f);
						using (new VerticalLayout())
						{
							GUILayout.Label("<color=\"#02C85F\"><b>ObscuredPrefs:</b></color>\nsecure, lot of additional types and extra options", gUIStyle);
							GUILayout.Space(5f);
							if (string.IsNullOrEmpty(GMBAPAPCHCJ))
							{
								OOBHLEHEKIK();
							}
							using (new HorizontalLayout())
							{
								GUILayout.Label(GMBAPAPCHCJ, GUILayout.Width(270f));
								using (new VerticalLayout())
								{
									using (new HorizontalLayout())
									{
										if (GUILayout.Button("Save"))
										{
											NFBNFAKEJFI();
										}
										if (GUILayout.Button("Load"))
										{
											OOBHLEHEKIK();
										}
									}
									if (GUILayout.Button("Delete"))
									{
										OCDBIDGFMKJ();
									}
									using (new HorizontalLayout())
									{
										GUILayout.Label("LockToDevice level");
										OOMDDMEDAOH("http://j.mp/1gxg1tf");
									}
									DNHIHOBELMC = GUILayout.SelectionGrid(DNHIHOBELMC, new string[3]
									{
										ObscuredPrefs.EAONKJOAGJI.None.ToString(),
										ObscuredPrefs.EAONKJOAGJI.Soft.ToString(),
										ObscuredPrefs.EAONKJOAGJI.Strict.ToString()
									}, 3);
									ObscuredPrefs.PJAJBMBNKJN = (ObscuredPrefs.EAONKJOAGJI)DNHIHOBELMC;
									GUILayout.Space(5f);
									using (new HorizontalLayout())
									{
										ObscuredPrefs.AOFOAEDPLCO = GUILayout.Toggle(ObscuredPrefs.AOFOAEDPLCO, "preservePlayerPrefs");
										OOMDDMEDAOH("http://j.mp/1iBK5pz");
									}
									using (new HorizontalLayout())
									{
										ObscuredPrefs.PIGNHFAAJDM = GUILayout.Toggle(ObscuredPrefs.PIGNHFAAJDM, "emergencyMode");
										OOMDDMEDAOH("http://j.mp/1FRAL5L");
									}
									using (new HorizontalLayout())
									{
										ObscuredPrefs.GOKCHJKPDCN = GUILayout.Toggle(ObscuredPrefs.GOKCHJKPDCN, "readForeignSaves");
										OOMDDMEDAOH("http://j.mp/1LCdpDa");
									}
									GUILayout.Space(5f);
									GUILayout.Label("<color=\"" + ((!LOLIHLHKEMG) ? "#02C85F" : "#FF4040") + "\">Saves modification detected: " + LOLIHLHKEMG + "</color>");
									GUILayout.Label("<color=\"" + ((!HMJKPKPCDEH) ? "#02C85F" : "#FF4040") + "\">Foreign saves detected: " + HMJKPKPCDEH + "</color>");
								}
							}
						}
						GUILayout.Space(5f);
						OOMDDMEDAOH("http://docs.unity3d.com/ScriptReference/PlayerPrefs.html", "Visit docs to see where PlayerPrefs are stored", -1);
					}
				}
			}
			else
			{
				GUILayout.Label("ACTk is able to detect some types of cheating to let you take action on the cheating players. This example scene has all possible detectors and all of them are automatically start on scene start.");
				GUILayout.Space(5f);
				using (new VerticalLayout(GUI.skin.box))
				{
					GUILayout.Label("<b>Speed Hack Detector</b>");
					GUILayout.Label("Allows to detect Cheat Engine's speed hack (and maybe some other speed hack tools) usage.");
					GUILayout.Label("<color=\"" + ((!FPJPGKBDMBF) ? "#02C85F" : "#FF4040") + "\">Detected: " + FPJPGKBDMBF.ToString().ToLower() + "</color>");
					GUILayout.Space(10f);
					GUILayout.Label("<b>Obscured Cheating Detector</b>");
					GUILayout.Label("Detects cheating of any Obscured type (except ObscuredPrefs, it has own detection features) used in project.");
					GUILayout.Label("<color=\"" + ((!ALLDLDNMKHD) ? "#02C85F" : "#FF4040") + "\">Detected: " + ALLDLDNMKHD.ToString().ToLower() + "</color>");
					GUILayout.Space(10f);
					GUILayout.Label("<b>WallHack Detector</b>");
					GUILayout.Label("Detects common types of wall hack cheating: walking through the walls (Rigidbody and CharacterController modules), shooting through the walls (Raycast module), looking through the walls (Wireframe module).");
					GUILayout.Label("<color=\"" + ((!MBPMDGDAFIB) ? "#02C85F" : "#FF4040") + "\">Detected: " + MBPMDGDAFIB.ToString().ToLower() + "</color>");
					GUILayout.Space(10f);
					GUILayout.Label("<b>Injection Detector</b>");
					GUILayout.Label("Allows to detect foreign managed assemblies in your application.");
					GUILayout.Label("<color=\"" + ((!OCEFNFANBGG) ? "#02C85F" : "#FF4040") + "\">Detected: " + OCEFNFANBGG.ToString().ToLower() + "</color>");
				}
			}
			GUILayout.EndArea();
		}

		private string MBNHEDGLCLO()
		{
			string result = "Can't get the list, sorry :(";
			string PKLOIFLHINB = string.Empty;
			if (string.IsNullOrEmpty(DGMEPCBKPEA))
			{
				IEnumerable<Type> source = from GNAONAPDDLD in Assembly.GetExecutingAssembly().GetTypes()
					where GNAONAPDDLD.IsPublic && GNAONAPDDLD.Namespace == "CodeStage.AntiCheat.ObscuredTypes" && GNAONAPDDLD.Name != "ObscuredPrefs"
					select GNAONAPDDLD;
				source.ToList().ForEach((Type GNAONAPDDLD) =>
				{
					if (PKLOIFLHINB.Length > 0)
					{
						PKLOIFLHINB = PKLOIFLHINB + "\n" + GNAONAPDDLD.Name;
					}
					else
					{
						PKLOIFLHINB += GNAONAPDDLD.Name;
					}
				});
				if (!string.IsNullOrEmpty(PKLOIFLHINB))
				{
					result = PKLOIFLHINB;
					DGMEPCBKPEA = PKLOIFLHINB;
				}
			}
			else
			{
				result = DGMEPCBKPEA;
			}
			return result;
		}

		private string PLJAKCEMHOA()
		{
			return "int\nfloat\nstring\n<color=\"#75C4EB\">uint\ndouble\nlong\nbool\nbyte[]\nVector2\nVector3\nQuaternion\nColor\nRect</color>";
		}

		private void KKPNIEOLNGP()
		{
			JGENFAEFIGJ = "int: " + PlayerPrefs.GetInt("money", -1) + "\n";
			string jGENFAEFIGJ = JGENFAEFIGJ;
			JGENFAEFIGJ = jGENFAEFIGJ + "float: " + PlayerPrefs.GetFloat("lifeBar", -1f) + "\n";
			JGENFAEFIGJ = JGENFAEFIGJ + "string: " + PlayerPrefs.GetString("name", "No saved PlayerPrefs!");
		}

		private void COPMPOFEGJO()
		{
			PlayerPrefs.SetInt("money", 456);
			PlayerPrefs.SetFloat("lifeBar", 456.789f);
			PlayerPrefs.SetString("name", "Hey, there!");
			PlayerPrefs.Save();
		}

		private void FOPIDOGDNJB()
		{
			PlayerPrefs.DeleteKey("money");
			PlayerPrefs.DeleteKey("lifeBar");
			PlayerPrefs.DeleteKey("name");
			PlayerPrefs.Save();
		}

		private void OOBHLEHEKIK()
		{
			byte[] array = ObscuredPrefs.GetByteArray("demoByteArray", 0, 4);
			GMBAPAPCHCJ = "int: " + ObscuredPrefs.GetInt("money", -1) + "\n";
			string gMBAPAPCHCJ = GMBAPAPCHCJ;
			GMBAPAPCHCJ = gMBAPAPCHCJ + "float: " + ObscuredPrefs.GetFloat("lifeBar", -1f) + "\n";
			GMBAPAPCHCJ = GMBAPAPCHCJ + "string: " + ObscuredPrefs.GetString("name", "No saved ObscuredPrefs!") + "\n";
			gMBAPAPCHCJ = GMBAPAPCHCJ;
			GMBAPAPCHCJ = gMBAPAPCHCJ + "bool: " + ObscuredPrefs.GetBool("gameComplete", false) + "\n";
			gMBAPAPCHCJ = GMBAPAPCHCJ;
			GMBAPAPCHCJ = gMBAPAPCHCJ + "uint: " + ObscuredPrefs.GetUInt("demoUint", 0u) + "\n";
			gMBAPAPCHCJ = GMBAPAPCHCJ;
			GMBAPAPCHCJ = gMBAPAPCHCJ + "long: " + ObscuredPrefs.GetLong("demoLong", -1L) + "\n";
			gMBAPAPCHCJ = GMBAPAPCHCJ;
			GMBAPAPCHCJ = gMBAPAPCHCJ + "double: " + ObscuredPrefs.GetDouble("demoDouble", -1.0) + "\n";
			gMBAPAPCHCJ = GMBAPAPCHCJ;
			GMBAPAPCHCJ = string.Concat(gMBAPAPCHCJ, "Vector2: ", ObscuredPrefs.GetVector2("demoVector2", Vector2.zero), "\n");
			gMBAPAPCHCJ = GMBAPAPCHCJ;
			GMBAPAPCHCJ = string.Concat(gMBAPAPCHCJ, "Vector3: ", ObscuredPrefs.GetVector3("demoVector3", Vector3.zero), "\n");
			gMBAPAPCHCJ = GMBAPAPCHCJ;
			GMBAPAPCHCJ = string.Concat(gMBAPAPCHCJ, "Quaternion: ", ObscuredPrefs.GetQuaternion("demoQuaternion", Quaternion.identity), "\n");
			gMBAPAPCHCJ = GMBAPAPCHCJ;
			GMBAPAPCHCJ = string.Concat(gMBAPAPCHCJ, "Rect: ", ObscuredPrefs.GetRect("demoRect", new Rect(0f, 0f, 0f, 0f)), "\n");
			gMBAPAPCHCJ = GMBAPAPCHCJ;
			GMBAPAPCHCJ = string.Concat(gMBAPAPCHCJ, "Color: ", ObscuredPrefs.GetColor("demoColor", Color.black), "\n");
			gMBAPAPCHCJ = GMBAPAPCHCJ;
			GMBAPAPCHCJ = gMBAPAPCHCJ + "byte[]: {" + array[0] + "," + array[1] + "," + array[2] + "," + array[3] + "}";
		}

		private void NFBNFAKEJFI()
		{
			ObscuredPrefs.SetInt("money", 123);
			ObscuredPrefs.SetFloat("lifeBar", 123.456f);
			ObscuredPrefs.SetString("name", "Goscurry is not a lie ;)");
			ObscuredPrefs.SetBool("gameComplete", true);
			ObscuredPrefs.SetUInt("demoUint", 1234567891u);
			ObscuredPrefs.SetLong("demoLong", 1234567891234567890L);
			ObscuredPrefs.SetDouble("demoDouble", 1.234567890123456);
			ObscuredPrefs.SetVector2("demoVector2", Vector2.one);
			ObscuredPrefs.SetVector3("demoVector3", Vector3.one);
			ObscuredPrefs.SetQuaternion("demoQuaternion", Quaternion.Euler(new Vector3(10f, 20f, 30f)));
			ObscuredPrefs.SetRect("demoRect", new Rect(1.5f, 2.6f, 3.7f, 4.8f));
			ObscuredPrefs.SetColor("demoColor", Color.red);
			ObscuredPrefs.SetByteArray("demoByteArray", new byte[4] { 44, 104, 43, 32 });
			ObscuredPrefs.Save();
		}

		private void OCDBIDGFMKJ()
		{
			ObscuredPrefs.LPJJAFDEKIB("money");
			ObscuredPrefs.LPJJAFDEKIB("lifeBar");
			ObscuredPrefs.LPJJAFDEKIB("name");
			ObscuredPrefs.LPJJAFDEKIB("gameComplete");
			ObscuredPrefs.LPJJAFDEKIB("demoUint");
			ObscuredPrefs.LPJJAFDEKIB("demoLong");
			ObscuredPrefs.LPJJAFDEKIB("demoDouble");
			ObscuredPrefs.LPJJAFDEKIB("demoVector2");
			ObscuredPrefs.LPJJAFDEKIB("demoVector3");
			ObscuredPrefs.LPJJAFDEKIB("demoQuaternion");
			ObscuredPrefs.LPJJAFDEKIB("demoRect");
			ObscuredPrefs.LPJJAFDEKIB("demoColor");
			ObscuredPrefs.LPJJAFDEKIB("demoByteArray");
			ObscuredPrefs.Save();
		}

		private void OOMDDMEDAOH(string BEPKJNKCKPH)
		{
			OOMDDMEDAOH(BEPKJNKCKPH, 30);
		}

		private void OOMDDMEDAOH(string BEPKJNKCKPH, int JMLAKAKDBBL)
		{
			OOMDDMEDAOH(BEPKJNKCKPH, "?", JMLAKAKDBBL);
		}

		private void OOMDDMEDAOH(string BEPKJNKCKPH, string GCKANEECDHE, int JMLAKAKDBBL)
		{
			GUILayoutOption[] array = new GUILayoutOption[1];
			if (JMLAKAKDBBL != -1)
			{
				array[0] = GUILayout.Width(JMLAKAKDBBL);
			}
			else
			{
				array = null;
			}
			if (GUILayout.Button(GCKANEECDHE, array))
			{
				Application.OpenURL(BEPKJNKCKPH);
			}
		}

		private void OnApplicationQuit()
		{
			FOPIDOGDNJB();
			OCDBIDGFMKJ();
		}
	}
}
