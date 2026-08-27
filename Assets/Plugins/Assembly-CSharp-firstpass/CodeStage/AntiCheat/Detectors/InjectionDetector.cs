using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace CodeStage.AntiCheat.Detectors
{
	[AddComponentMenu("Code Stage/Anti-Cheat Toolkit/Injection Detector")]
	public class InjectionDetector : ActDetectorBase
	{
		private class FCBAKCJACOA
		{
			public readonly string name;

			public readonly int[] DEACEHPECDK;

			public FCBAKCJACOA(string name, int[] DEACEHPECDK)
			{
				this.name = name;
				this.DEACEHPECDK = DEACEHPECDK;
			}
		}

		internal const string JCAOMBMKNDE = "Injection Detector";

		internal const string MGAMICFMIJK = "[ACTk] Injection Detector: ";

		private static int instancesInScene;

		private bool EGAAPPNDOML;

		private FCBAKCJACOA[] FPJFAKAJEPE;

		private string[] EMJOPBIEHFC;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static InjectionDetector OGKMDFDNIEN;

		public static InjectionDetector BPCBBHAKFDM
		{
			get
			{
				return get_Instance();
			}
			private set
			{
				set_Instance(value);
			}
		}

		private static InjectionDetector MCEPJKHJPIJ
		{
			get
			{
				return NNMHGMJELIL();
			}
		}

		private InjectionDetector()
		{
		}

		public static void StartDetection()
		{
			if (get_Instance() != null)
			{
				get_Instance().FCJDKBEGPEF(null);
			}
			else
			{
				UnityEngine.Debug.LogError("[ACTk] Injection Detector: can't be started since it doesn't exists in scene or not yet initialized!");
			}
		}

		public static void StartDetection(UnityAction callback)
		{
			NNMHGMJELIL().FCJDKBEGPEF(callback);
		}

		public static void StopDetection()
		{
			if (get_Instance() != null)
			{
				get_Instance().DJEBEEIELBB();
			}
		}

		public static void Dispose()
		{
			if (get_Instance() != null)
			{
				get_Instance().HIEIKJFAIJE();
			}
		}

		public static InjectionDetector get_Instance()
		{
			return OGKMDFDNIEN;
		}

		private static void set_Instance(InjectionDetector value)
		{
			OGKMDFDNIEN = value;
		}

		private static InjectionDetector NNMHGMJELIL()
		{
			if (get_Instance() != null)
			{
				return get_Instance();
			}
			if (ActDetectorBase.detectorsContainer == null)
			{
				ActDetectorBase.detectorsContainer = new GameObject("Anti-Cheat Toolkit Detectors");
			}
			set_Instance(ActDetectorBase.detectorsContainer.AddComponent<InjectionDetector>());
			return get_Instance();
		}

		private void Awake()
		{
			instancesInScene++;
			if (Init(get_Instance(), "Injection Detector"))
			{
				set_Instance(this);
			}
			SceneManager.sceneLoaded += FOFIOMHDCOM;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			instancesInScene--;
		}

		private void FOFIOMHDCOM(Scene MHOCFOODLLL, LoadSceneMode NMMPBADCFHK)
		{
			KJCKJOKLPLL();
		}

		private void KJCKJOKLPLL()
		{
			if (instancesInScene < 2)
			{
				if (!keepAlive)
				{
					HIEIKJFAIJE();
				}
			}
			else if (!keepAlive && get_Instance() != this)
			{
				HIEIKJFAIJE();
			}
		}

		private void FCJDKBEGPEF(UnityAction callback)
		{
			if (EKDNCONELMD)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Injection Detector: already running!", this);
				return;
			}
			if (!base.enabled)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Injection Detector: disabled but StartDetection still called from somewhere (see stack trace for this message)!", this);
				return;
			}
			if (callback != null && detectionEventHasListener)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Injection Detector: has properly configured Detection Event in the inspector, but still get started with Action callback. Both Action and Detection Event will be called on detection. Are you sure you wish to do this?", this);
			}
			if (callback == null && !detectionEventHasListener)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Injection Detector: was started without any callbacks. Please configure Detection Event in the inspector, or pass the callback Action to the StartDetection method.", this);
				base.enabled = false;
				return;
			}
			detectionAction = callback;
			AKFEAJDLIKF = true;
			EKDNCONELMD = true;
			if (FPJFAKAJEPE == null)
			{
				BCPPMOGBHGO();
			}
			if (EGAAPPNDOML)
			{
				MCDANNDOEIK();
			}
			else if (!DDHLLHCAAIN())
			{
				AppDomain.CurrentDomain.AssemblyLoad += JLPHDNIMOJE;
			}
			else
			{
				MCDANNDOEIK();
			}
		}

		protected override void LICPBNOFNOB()
		{
			FCJDKBEGPEF(null);
		}

		protected override void HEGJDFPFMII()
		{
			EKDNCONELMD = false;
			AppDomain.CurrentDomain.AssemblyLoad -= JLPHDNIMOJE;
		}

		protected override void KLJNEJIEMCN()
		{
			if (detectionAction != null || detectionEventHasListener)
			{
				EKDNCONELMD = true;
				AppDomain.CurrentDomain.AssemblyLoad += JLPHDNIMOJE;
			}
		}

		protected override void DJEBEEIELBB()
		{
			if (AKFEAJDLIKF)
			{
				AppDomain.CurrentDomain.AssemblyLoad -= JLPHDNIMOJE;
				detectionAction = null;
				AKFEAJDLIKF = false;
				EKDNCONELMD = false;
			}
		}

		protected override void HIEIKJFAIJE()
		{
			base.HIEIKJFAIJE();
			if (get_Instance() == this)
			{
				set_Instance(null);
			}
		}

		private void JLPHDNIMOJE(object ABONPDBPJBA, AssemblyLoadEventArgs LKIOKGCNKHE)
		{
			if (!BAPALPIJPBP(LKIOKGCNKHE.LoadedAssembly))
			{
				MCDANNDOEIK();
			}
		}

		private bool DDHLLHCAAIN()
		{
			bool result = false;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			if (assemblies.Length == 0)
			{
				result = true;
			}
			else
			{
				Assembly[] array = assemblies;
				foreach (Assembly eHKCIGHDNMI in array)
				{
					if (!BAPALPIJPBP(eHKCIGHDNMI))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		private bool BAPALPIJPBP(Assembly EHKCIGHDNMI)
		{
			string text = EHKCIGHDNMI.GetName().Name;
			int value = DNBGCOIKKIC(EHKCIGHDNMI);
			bool result = false;
			for (int i = 0; i < FPJFAKAJEPE.Length; i++)
			{
				FCBAKCJACOA fCBAKCJACOA = FPJFAKAJEPE[i];
				if (fCBAKCJACOA.name == text && Array.IndexOf(fCBAKCJACOA.DEACEHPECDK, value) != -1)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private void BCPPMOGBHGO()
		{
			TextAsset textAsset = (TextAsset)Resources.Load("fndid", typeof(TextAsset));
			if (textAsset == null)
			{
				EGAAPPNDOML = true;
				return;
			}
			string[] separator = new string[1] { ":" };
			MemoryStream memoryStream = new MemoryStream(textAsset.bytes);
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			int num = binaryReader.ReadInt32();
			FPJFAKAJEPE = new FCBAKCJACOA[num];
			for (int i = 0; i < num; i++)
			{
				string bAINMLLIKOL = binaryReader.ReadString();
				bAINMLLIKOL = ObscuredString.EncryptDecrypt(bAINMLLIKOL, "Elina");
				string[] array = bAINMLLIKOL.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				int num2 = array.Length;
				if (num2 > 1)
				{
					string gOHIIMFFFJI = array[0];
					int[] array2 = new int[num2 - 1];
					for (int j = 1; j < num2; j++)
					{
						array2[j - 1] = int.Parse(array[j]);
					}
					FPJFAKAJEPE[i] = new FCBAKCJACOA(gOHIIMFFFJI, array2);
					continue;
				}
				EGAAPPNDOML = true;
				binaryReader.Close();
				memoryStream.Close();
				return;
			}
			binaryReader.Close();
			memoryStream.Close();
			Resources.UnloadAsset(textAsset);
			EMJOPBIEHFC = new string[256];
			for (int k = 0; k < 256; k++)
			{
				EMJOPBIEHFC[k] = k.ToString("x2");
			}
		}

		private int DNBGCOIKKIC(Assembly EHKCIGHDNMI)
		{
			AssemblyName assemblyName = EHKCIGHDNMI.GetName();
			byte[] publicKeyToken = assemblyName.GetPublicKeyToken();
			string text = ((publicKeyToken.Length < 8) ? assemblyName.Name : (assemblyName.Name + DMCNCEOHHFI(publicKeyToken)));
			int num = 0;
			int length = text.Length;
			for (int i = 0; i < length; i++)
			{
				num += text[i];
				num += num << 10;
				num ^= num >> 6;
			}
			num += num << 3;
			num ^= num >> 11;
			return num + (num << 15);
		}

		private string DMCNCEOHHFI(byte[] KPAMPCLHCEN)
		{
			string text = string.Empty;
			for (int i = 0; i < 8; i++)
			{
				text += EMJOPBIEHFC[KPAMPCLHCEN[i]];
			}
			return text;
		}
	}
}
