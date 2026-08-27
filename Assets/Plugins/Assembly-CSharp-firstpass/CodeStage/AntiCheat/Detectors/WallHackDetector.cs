using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace CodeStage.AntiCheat.Detectors
{
	[AddComponentMenu("Code Stage/Anti-Cheat Toolkit/WallHack Detector")]
	public class WallHackDetector : ActDetectorBase
	{
		internal const string JCAOMBMKNDE = "WallHack Detector";

		internal const string MGAMICFMIJK = "[ACTk] WallHack Detector: ";

		private const string BJBMCGMOLEK = "[WH Detector Service]";

		private const string BMACHGPMCME = "Hidden/ACTk/WallHackTexture";

		private const int NJBEJKDFPDI = 4;

		private const int DOKJCFOCGNH = 4;

		private readonly Vector3 PKIHJKCDMHD = new Vector3(0f, 0f, 1f);

		private static int instancesInScene;

		private readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

		[Tooltip("Check for the \"walk through the walls\" kind of cheats made via Rigidbody hacks?")]
		[SerializeField]
		private bool checkRigidbody = true;

		[Tooltip("Check for the \"walk through the walls\" kind of cheats made via Character Controller hacks?")]
		[SerializeField]
		private bool checkController = true;

		[SerializeField]
		[Tooltip("Check for the \"see through the walls\" kind of cheats made via shader or driver hacks (wireframe, color alpha, etc.)?")]
		private bool checkWireframe = true;

		[Tooltip("Check for the \"shoot through the walls\" kind of cheats made via Raycast hacks?")]
		[SerializeField]
		private bool checkRaycast = true;

		[Range(1f, 60f)]
		[Tooltip("Delay between Wireframe module checks, from 1 up to 60 secs.")]
		public int wireframeDelay = 10;

		[Range(1f, 60f)]
		[Tooltip("Delay between Raycast module checks, from 1 up to 60 secs.")]
		public int raycastDelay = 10;

		[Tooltip("World position of the container for service objects within 3x3x3 cube (drawn as red wire cube in scene).")]
		public Vector3 spawnPosition;

		[Tooltip("Maximum false positives in a row for each detection module before registering a wall hack.")]
		public byte maxFalsePositives = 3;

		private GameObject CKOKDJEKEMF;

		private GameObject MCONGCAGIMB;

		private GameObject FBDCPDEOFGI;

		private Camera wfCamera;

		private MeshRenderer NMFDCIBDLKI;

		private MeshRenderer PIGHOMIKPEH;

		private Color CEHKBLKIMMH = Color.black;

		private Color HOOHEPNGMLK = Color.black;

		private Shader wfShader;

		private Material wfMaterial;

		private Texture2D HJHIKCOBBKA;

		private Texture2D KDCBKALFPNI;

		private RenderTexture renderTexture;

		private int GKGKFDDEFJE = -1;

		private int PNOHJNCLGJI = -1;

		private Rigidbody rigidPlayer;

		private CharacterController charControllerPlayer;

		private float charControllerVelocity;

		private byte AOEMKDEFLNP;

		private byte KOBEPCAABCE;

		private byte BBFALJPOABG;

		private byte CNLPNKHBPAI;

		private bool JAOPPCJPDAH;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static WallHackDetector OGKMDFDNIEN;

		public bool MEEGFIIGOCO
		{
			get
			{
				return get_CheckRigidbody();
			}
			set
			{
				set_CheckRigidbody(value);
			}
		}

		public bool LKDCHIHPBKJ
		{
			get
			{
				return get_CheckController();
			}
			set
			{
				set_CheckController(value);
			}
		}

		public bool EECFNBDKOEB
		{
			get
			{
				return get_CheckWireframe();
			}
			set
			{
				set_CheckWireframe(value);
			}
		}

		public bool EONBNOEEILC
		{
			get
			{
				return get_CheckRaycast();
			}
			set
			{
				set_CheckRaycast(value);
			}
		}

		public static WallHackDetector BPCBBHAKFDM
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

		private static WallHackDetector MCEPJKHJPIJ
		{
			get
			{
				return NNMHGMJELIL();
			}
		}

		private WallHackDetector()
		{
		}

		public bool get_CheckRigidbody()
		{
			return checkRigidbody;
		}

		public void set_CheckRigidbody(bool value)
		{
			if (checkRigidbody == value || !Application.isPlaying || !base.enabled || !base.gameObject.activeSelf)
			{
				return;
			}
			checkRigidbody = value;
			if (AKFEAJDLIKF)
			{
				JMJDMMILGMB();
				if (checkRigidbody)
				{
					StartRigidModule();
				}
				else
				{
					OAIBAJBLCPF();
				}
			}
		}

		public bool get_CheckController()
		{
			return checkController;
		}

		public void set_CheckController(bool value)
		{
			if (checkController == value || !Application.isPlaying || !base.enabled || !base.gameObject.activeSelf)
			{
				return;
			}
			checkController = value;
			if (AKFEAJDLIKF)
			{
				JMJDMMILGMB();
				if (checkController)
				{
					StartControllerModule();
				}
				else
				{
					LJHNKNGKNJP();
				}
			}
		}

		public bool get_CheckWireframe()
		{
			return checkWireframe;
		}

		public void set_CheckWireframe(bool value)
		{
			if (checkWireframe == value || !Application.isPlaying || !base.enabled || !base.gameObject.activeSelf)
			{
				return;
			}
			checkWireframe = value;
			if (AKFEAJDLIKF)
			{
				JMJDMMILGMB();
				if (checkWireframe)
				{
					IDEIAKJHKMK();
				}
				else
				{
					OLOAFNAAFNF();
				}
			}
		}

		public bool get_CheckRaycast()
		{
			return checkRaycast;
		}

		public void set_CheckRaycast(bool value)
		{
			if (checkRaycast == value || !Application.isPlaying || !base.enabled || !base.gameObject.activeSelf)
			{
				return;
			}
			checkRaycast = value;
			if (AKFEAJDLIKF)
			{
				JMJDMMILGMB();
				if (checkRaycast)
				{
					JBEFELONIIP();
				}
				else
				{
					PMLJHEJNGLM();
				}
			}
		}

		public static void StartDetection()
		{
			if (get_Instance() != null)
			{
				get_Instance().FCJDKBEGPEF(null, get_Instance().spawnPosition, get_Instance().maxFalsePositives);
			}
			else
			{
				UnityEngine.Debug.LogError("[ACTk] WallHack Detector: can't be started since it doesn't exists in scene or not yet initialized!");
			}
		}

		public static void StartDetection(UnityAction callback)
		{
			StartDetection(callback, NNMHGMJELIL().spawnPosition);
		}

		public static void StartDetection(UnityAction callback, Vector3 PBPJOBANACG)
		{
			StartDetection(callback, PBPJOBANACG, NNMHGMJELIL().maxFalsePositives);
		}

		public static void StartDetection(UnityAction callback, Vector3 PBPJOBANACG, byte JKBEIPOFGCI)
		{
			NNMHGMJELIL().FCJDKBEGPEF(callback, PBPJOBANACG, JKBEIPOFGCI);
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

		public static WallHackDetector get_Instance()
		{
			return OGKMDFDNIEN;
		}

		private static void set_Instance(WallHackDetector value)
		{
			OGKMDFDNIEN = value;
		}

		private static WallHackDetector NNMHGMJELIL()
		{
			if (get_Instance() != null)
			{
				return get_Instance();
			}
			if (ActDetectorBase.detectorsContainer == null)
			{
				ActDetectorBase.detectorsContainer = new GameObject("Anti-Cheat Toolkit Detectors");
			}
			set_Instance(ActDetectorBase.detectorsContainer.AddComponent<WallHackDetector>());
			return get_Instance();
		}

		private void Awake()
		{
			instancesInScene++;
			if (Init(get_Instance(), "WallHack Detector"))
			{
				set_Instance(this);
			}
			SceneManager.sceneLoaded += FOFIOMHDCOM;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			StopAllCoroutines();
			if (CKOKDJEKEMF != null)
			{
				UnityEngine.Object.Destroy(CKOKDJEKEMF);
			}
			if (wfMaterial != null)
			{
				wfMaterial.mainTexture = null;
				wfMaterial.shader = null;
				wfMaterial = null;
				wfShader = null;
				HJHIKCOBBKA = null;
				KDCBKALFPNI = null;
				renderTexture.DiscardContents();
				renderTexture.Release();
				renderTexture = null;
			}
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

		private void FixedUpdate()
		{
			if (EKDNCONELMD && checkRigidbody && !(rigidPlayer == null) && rigidPlayer.transform.localPosition.z > 1f)
			{
				AOEMKDEFLNP++;
				if (!CEGHFCJKFAL())
				{
					OAIBAJBLCPF();
					StartRigidModule();
				}
			}
		}

		private void Update()
		{
			if (!EKDNCONELMD || !checkController || charControllerPlayer == null || !(charControllerVelocity > 0f))
			{
				return;
			}
			charControllerPlayer.Move(new Vector3(UnityEngine.Random.Range(-0.002f, 0.002f), 0f, charControllerVelocity));
			if (charControllerPlayer.transform.localPosition.z > 1f)
			{
				KOBEPCAABCE++;
				if (!CEGHFCJKFAL())
				{
					LJHNKNGKNJP();
					StartControllerModule();
				}
			}
		}

		private void FCJDKBEGPEF(UnityAction callback, Vector3 MDCJBPDNAOG, byte OPBFDLIKAKP)
		{
			if (EKDNCONELMD)
			{
				UnityEngine.Debug.LogWarning("[ACTk] WallHack Detector: already running!", this);
				return;
			}
			if (!base.enabled)
			{
				UnityEngine.Debug.LogWarning("[ACTk] WallHack Detector: disabled but StartDetection still called from somewhere (see stack trace for this message)!", this);
				return;
			}
			if (callback != null && detectionEventHasListener)
			{
				UnityEngine.Debug.LogWarning("[ACTk] WallHack Detector: has properly configured Detection Event in the inspector, but still get started with Action callback. Both Action and Detection Event will be called on detection. Are you sure you wish to do this?", this);
			}
			if (callback == null && !detectionEventHasListener)
			{
				UnityEngine.Debug.LogWarning("[ACTk] WallHack Detector: was started without any callbacks. Please configure Detection Event in the inspector, or pass the callback Action to the StartDetection method.", this);
				base.enabled = false;
				return;
			}
			detectionAction = callback;
			spawnPosition = MDCJBPDNAOG;
			maxFalsePositives = OPBFDLIKAKP;
			AOEMKDEFLNP = 0;
			KOBEPCAABCE = 0;
			BBFALJPOABG = 0;
			CNLPNKHBPAI = 0;
			StartCoroutine(JLNJONBBDPM());
			AKFEAJDLIKF = true;
			EKDNCONELMD = true;
		}

		protected override void LICPBNOFNOB()
		{
			FCJDKBEGPEF(null, spawnPosition, maxFalsePositives);
		}

		protected override void HEGJDFPFMII()
		{
			if (EKDNCONELMD)
			{
				EKDNCONELMD = false;
				OAIBAJBLCPF();
				LJHNKNGKNJP();
				OLOAFNAAFNF();
				PMLJHEJNGLM();
			}
		}

		protected override void KLJNEJIEMCN()
		{
			if (detectionAction != null || detectionEventHasListener)
			{
				EKDNCONELMD = true;
				if (checkRigidbody)
				{
					StartRigidModule();
				}
				if (checkController)
				{
					StartControllerModule();
				}
				if (checkWireframe)
				{
					IDEIAKJHKMK();
				}
				if (checkRaycast)
				{
					JBEFELONIIP();
				}
			}
		}

		protected override void DJEBEEIELBB()
		{
			if (AKFEAJDLIKF)
			{
				HEGJDFPFMII();
				detectionAction = null;
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

		private void JMJDMMILGMB()
		{
			if (base.enabled && base.gameObject.activeSelf)
			{
				if (GKGKFDDEFJE == -1)
				{
					GKGKFDDEFJE = LayerMask.NameToLayer("Ignore Raycast");
				}
				if (PNOHJNCLGJI == -1)
				{
					PNOHJNCLGJI = LayerMask.GetMask("Ignore Raycast");
				}
				if (CKOKDJEKEMF == null)
				{
					CKOKDJEKEMF = new GameObject("[WH Detector Service]");
					CKOKDJEKEMF.layer = GKGKFDDEFJE;
					CKOKDJEKEMF.transform.position = spawnPosition;
					UnityEngine.Object.DontDestroyOnLoad(CKOKDJEKEMF);
				}
				if ((checkRigidbody || checkController) && MCONGCAGIMB == null)
				{
					MCONGCAGIMB = new GameObject("SolidWall");
					MCONGCAGIMB.AddComponent<BoxCollider>();
					MCONGCAGIMB.layer = GKGKFDDEFJE;
					MCONGCAGIMB.transform.parent = CKOKDJEKEMF.transform;
					MCONGCAGIMB.transform.localScale = new Vector3(3f, 3f, 0.5f);
					MCONGCAGIMB.transform.localPosition = Vector3.zero;
				}
				else if (!checkRigidbody && !checkController && MCONGCAGIMB != null)
				{
					UnityEngine.Object.Destroy(MCONGCAGIMB);
				}
				if (checkWireframe && wfCamera == null)
				{
					if (wfShader == null)
					{
						wfShader = Shader.Find("Hidden/ACTk/WallHackTexture");
					}
					if (wfShader == null)
					{
						UnityEngine.Debug.LogError("[ACTk] WallHack Detector: can't find 'Hidden/ACTk/WallHackTexture' shader!\nPlease make sure you have it included at the Editor > Project Settings > Graphics.", this);
						checkWireframe = false;
					}
					else if (!wfShader.isSupported)
					{
						UnityEngine.Debug.LogError("[ACTk] WallHack Detector: can't detect wireframe cheats on this platform!", this);
						checkWireframe = false;
					}
					else
					{
						if (CEHKBLKIMMH == Color.black)
						{
							CEHKBLKIMMH = HMNELLILHHO();
							do
							{
								HOOHEPNGMLK = HMNELLILHHO();
							}
							while (ColorsSimilar(CEHKBLKIMMH, HOOHEPNGMLK, 10));
						}
						if (HJHIKCOBBKA == null)
						{
							HJHIKCOBBKA = new Texture2D(4, 4, TextureFormat.RGB24, false);
							HJHIKCOBBKA.filterMode = FilterMode.Point;
							Color[] array = new Color[16];
							for (int i = 0; i < 16; i++)
							{
								if (i < 8)
								{
									array[i] = CEHKBLKIMMH;
								}
								else
								{
									array[i] = HOOHEPNGMLK;
								}
							}
							HJHIKCOBBKA.SetPixels(array, 0);
							HJHIKCOBBKA.Apply();
						}
						if (renderTexture == null)
						{
							renderTexture = new RenderTexture(4, 4, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
							renderTexture.autoGenerateMips = false;
							renderTexture.filterMode = FilterMode.Point;
							renderTexture.Create();
						}
						if (KDCBKALFPNI == null)
						{
							KDCBKALFPNI = new Texture2D(4, 4, TextureFormat.RGB24, false);
							KDCBKALFPNI.filterMode = FilterMode.Point;
						}
						if (wfMaterial == null)
						{
							wfMaterial = new Material(wfShader);
							wfMaterial.mainTexture = HJHIKCOBBKA;
						}
						if (NMFDCIBDLKI == null)
						{
							GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
							UnityEngine.Object.Destroy(gameObject.GetComponent<BoxCollider>());
							gameObject.name = "WireframeFore";
							gameObject.layer = GKGKFDDEFJE;
							gameObject.transform.parent = CKOKDJEKEMF.transform;
							gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
							NMFDCIBDLKI = gameObject.GetComponent<MeshRenderer>();
							NMFDCIBDLKI.sharedMaterial = wfMaterial;
							NMFDCIBDLKI.shadowCastingMode = ShadowCastingMode.Off;
							NMFDCIBDLKI.receiveShadows = false;
							NMFDCIBDLKI.enabled = false;
						}
						if (PIGHOMIKPEH == null)
						{
							GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Quad);
							UnityEngine.Object.Destroy(gameObject2.GetComponent<MeshCollider>());
							gameObject2.name = "WireframeBack";
							gameObject2.layer = GKGKFDDEFJE;
							gameObject2.transform.parent = CKOKDJEKEMF.transform;
							gameObject2.transform.localPosition = new Vector3(0f, 0f, 1f);
							gameObject2.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
							PIGHOMIKPEH = gameObject2.GetComponent<MeshRenderer>();
							PIGHOMIKPEH.sharedMaterial = wfMaterial;
							PIGHOMIKPEH.shadowCastingMode = ShadowCastingMode.Off;
							PIGHOMIKPEH.receiveShadows = false;
							PIGHOMIKPEH.enabled = false;
						}
						if (wfCamera == null)
						{
							wfCamera = new GameObject("WireframeCamera").AddComponent<Camera>();
							wfCamera.gameObject.layer = GKGKFDDEFJE;
							wfCamera.transform.parent = CKOKDJEKEMF.transform;
							wfCamera.transform.localPosition = new Vector3(0f, 0f, -1f);
							wfCamera.clearFlags = CameraClearFlags.Color;
							wfCamera.backgroundColor = Color.black;
							wfCamera.orthographic = true;
							wfCamera.orthographicSize = 0.5f;
							wfCamera.nearClipPlane = 0.01f;
							wfCamera.farClipPlane = 2.1f;
							wfCamera.depth = 0f;
							wfCamera.renderingPath = RenderingPath.Forward;
							wfCamera.useOcclusionCulling = false;
							wfCamera.allowHDR = false;
							wfCamera.targetTexture = renderTexture;
							wfCamera.enabled = false;
						}
					}
				}
				else if (!checkWireframe && wfCamera != null)
				{
					UnityEngine.Object.Destroy(NMFDCIBDLKI.gameObject);
					UnityEngine.Object.Destroy(PIGHOMIKPEH.gameObject);
					wfCamera.targetTexture = null;
					UnityEngine.Object.Destroy(wfCamera.gameObject);
				}
				if (checkRaycast && FBDCPDEOFGI == null)
				{
					FBDCPDEOFGI = GameObject.CreatePrimitive(PrimitiveType.Plane);
					FBDCPDEOFGI.name = "ThinWall";
					FBDCPDEOFGI.layer = GKGKFDDEFJE;
					FBDCPDEOFGI.transform.parent = CKOKDJEKEMF.transform;
					FBDCPDEOFGI.transform.localScale = new Vector3(0.2f, 1f, 0.2f);
					FBDCPDEOFGI.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
					FBDCPDEOFGI.transform.localPosition = new Vector3(0f, 0f, 1.4f);
					UnityEngine.Object.Destroy(FBDCPDEOFGI.GetComponent<Renderer>());
					UnityEngine.Object.Destroy(FBDCPDEOFGI.GetComponent<MeshFilter>());
				}
				else if (!checkRaycast && FBDCPDEOFGI != null)
				{
					UnityEngine.Object.Destroy(FBDCPDEOFGI);
				}
			}
			else if (CKOKDJEKEMF != null)
			{
				UnityEngine.Object.Destroy(CKOKDJEKEMF);
			}
		}

		private IEnumerator JLNJONBBDPM()
		{
			yield return waitForEndOfFrame;
			JMJDMMILGMB();
			if (checkRigidbody)
			{
				StartRigidModule();
			}
			if (checkController)
			{
				StartControllerModule();
			}
			if (checkWireframe)
			{
				IDEIAKJHKMK();
			}
			if (checkRaycast)
			{
				JBEFELONIIP();
			}
		}

		private void StartRigidModule()
		{
			if (!checkRigidbody)
			{
				OAIBAJBLCPF();
				GBDCOPBKFBA();
				JMJDMMILGMB();
				return;
			}
			if (!rigidPlayer)
			{
				FNIEFGIGGPA();
			}
			if (rigidPlayer.transform.localPosition.z <= 1f && AOEMKDEFLNP > 0)
			{
				AOEMKDEFLNP = 0;
			}
			rigidPlayer.rotation = Quaternion.identity;
			rigidPlayer.angularVelocity = Vector3.zero;
			rigidPlayer.transform.localPosition = new Vector3(0.75f, 0f, -1f);
			rigidPlayer.velocity = PKIHJKCDMHD;
			Invoke("StartRigidModule", 4f);
		}

		private void StartControllerModule()
		{
			if (!checkController)
			{
				LJHNKNGKNJP();
				IPNJNBGAJLM();
				JMJDMMILGMB();
				return;
			}
			if (!charControllerPlayer)
			{
				MKFDEMOFGIB();
			}
			if (charControllerPlayer.transform.localPosition.z <= 1f && KOBEPCAABCE > 0)
			{
				KOBEPCAABCE = 0;
			}
			charControllerPlayer.transform.localPosition = new Vector3(-0.75f, 0f, -1f);
			charControllerVelocity = 0.01f;
			Invoke("StartControllerModule", 4f);
		}

		private void IDEIAKJHKMK()
		{
			if (!checkWireframe)
			{
				OLOAFNAAFNF();
				JMJDMMILGMB();
			}
			else if (!JAOPPCJPDAH)
			{
				Invoke("ShootWireframeModule", wireframeDelay);
			}
		}

		private void ShootWireframeModule()
		{
			StartCoroutine(KDCLKLLNDHK());
			Invoke("ShootWireframeModule", wireframeDelay);
		}

		private IEnumerator KDCLKLLNDHK()
		{
			wfCamera.enabled = true;
			yield return waitForEndOfFrame;
			NMFDCIBDLKI.enabled = true;
			PIGHOMIKPEH.enabled = true;
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = renderTexture;
			wfCamera.Render();
			NMFDCIBDLKI.enabled = false;
			PIGHOMIKPEH.enabled = false;
			while (!renderTexture.IsCreated())
			{
				yield return waitForEndOfFrame;
			}
			KDCBKALFPNI.ReadPixels(new Rect(0f, 0f, 4f, 4f), 0, 0, false);
			KDCBKALFPNI.Apply();
			RenderTexture.active = active;
			if (wfCamera == null)
			{
				yield return null;
			}
			wfCamera.enabled = false;
			if (!(KDCBKALFPNI.GetPixel(0, 3) != CEHKBLKIMMH) && !(KDCBKALFPNI.GetPixel(0, 1) != HOOHEPNGMLK) && !(KDCBKALFPNI.GetPixel(3, 3) != CEHKBLKIMMH) && !(KDCBKALFPNI.GetPixel(3, 1) != HOOHEPNGMLK) && !(KDCBKALFPNI.GetPixel(1, 3) != CEHKBLKIMMH) && !(KDCBKALFPNI.GetPixel(2, 3) != CEHKBLKIMMH) && !(KDCBKALFPNI.GetPixel(1, 1) != HOOHEPNGMLK) && !(KDCBKALFPNI.GetPixel(2, 1) != HOOHEPNGMLK))
			{
				if (BBFALJPOABG > 0)
				{
					BBFALJPOABG = 0;
				}
			}
			else
			{
				BBFALJPOABG++;
				JAOPPCJPDAH = CEGHFCJKFAL();
			}
			yield return null;
		}

		private void JBEFELONIIP()
		{
			if (!checkRaycast)
			{
				PMLJHEJNGLM();
				JMJDMMILGMB();
			}
			else
			{
				Invoke("ShootRaycastModule", raycastDelay);
			}
		}

		private void ShootRaycastModule()
		{
			if (Physics.Raycast(CKOKDJEKEMF.transform.position, CKOKDJEKEMF.transform.TransformDirection(Vector3.forward), 1.5f, PNOHJNCLGJI))
			{
				if (CNLPNKHBPAI > 0)
				{
					CNLPNKHBPAI = 0;
				}
			}
			else
			{
				CNLPNKHBPAI++;
				if (CEGHFCJKFAL())
				{
					return;
				}
			}
			Invoke("ShootRaycastModule", raycastDelay);
		}

		private void OAIBAJBLCPF()
		{
			if ((bool)rigidPlayer)
			{
				rigidPlayer.velocity = Vector3.zero;
			}
			CancelInvoke("StartRigidModule");
		}

		private void LJHNKNGKNJP()
		{
			if ((bool)charControllerPlayer)
			{
				charControllerVelocity = 0f;
			}
			CancelInvoke("StartControllerModule");
		}

		private void OLOAFNAAFNF()
		{
			CancelInvoke("ShootWireframeModule");
		}

		private void PMLJHEJNGLM()
		{
			CancelInvoke("ShootRaycastModule");
		}

		private void FNIEFGIGGPA()
		{
			GameObject gameObject = new GameObject("RigidPlayer");
			gameObject.AddComponent<CapsuleCollider>().height = 2f;
			gameObject.layer = GKGKFDDEFJE;
			gameObject.transform.parent = CKOKDJEKEMF.transform;
			gameObject.transform.localPosition = new Vector3(0.75f, 0f, -1f);
			rigidPlayer = gameObject.AddComponent<Rigidbody>();
			rigidPlayer.useGravity = false;
		}

		private void MKFDEMOFGIB()
		{
			GameObject gameObject = new GameObject("ControlledPlayer");
			gameObject.AddComponent<CapsuleCollider>().height = 2f;
			gameObject.layer = GKGKFDDEFJE;
			gameObject.transform.parent = CKOKDJEKEMF.transform;
			gameObject.transform.localPosition = new Vector3(-0.75f, 0f, -1f);
			charControllerPlayer = gameObject.AddComponent<CharacterController>();
		}

		private void GBDCOPBKFBA()
		{
			if ((bool)rigidPlayer)
			{
				UnityEngine.Object.Destroy(rigidPlayer.gameObject);
				rigidPlayer = null;
			}
		}

		private void IPNJNBGAJLM()
		{
			if ((bool)charControllerPlayer)
			{
				UnityEngine.Object.Destroy(charControllerPlayer.gameObject);
				charControllerPlayer = null;
			}
		}

		private bool CEGHFCJKFAL()
		{
			bool result = false;
			if (KOBEPCAABCE > maxFalsePositives || AOEMKDEFLNP > maxFalsePositives || BBFALJPOABG > maxFalsePositives || CNLPNKHBPAI > maxFalsePositives)
			{
				MCDANNDOEIK();
				result = true;
			}
			return result;
		}

		private static Color32 HMNELLILHHO()
		{
			return new Color32((byte)UnityEngine.Random.Range(0, 256), (byte)UnityEngine.Random.Range(0, 256), (byte)UnityEngine.Random.Range(0, 256), byte.MaxValue);
		}

		private static bool ColorsSimilar(Color32 OHLMPFPIFMB, Color32 GJOGACNLCDC, int NKABGNCLCJP)
		{
			return Math.Abs(OHLMPFPIFMB.r - GJOGACNLCDC.r) < NKABGNCLCJP && Math.Abs(OHLMPFPIFMB.g - GJOGACNLCDC.g) < NKABGNCLCJP && Math.Abs(OHLMPFPIFMB.b - GJOGACNLCDC.b) < NKABGNCLCJP;
		}
	}
}
