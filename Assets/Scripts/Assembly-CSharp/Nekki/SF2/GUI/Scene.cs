using System.Diagnostics;
using UnityEngine;

namespace Nekki.SF2.GUI
{
	public abstract class Scene<T> : ModuleHolder where T : Scene<T>
	{
		private static T EADAACFGGGM__BackingField;
		public enum EEDLPLHMEEE
		{
			ON_HINT_CREATED = 200
		}

		[SerializeField]
		private WideScreenController _WideScreenController;

		[SerializeField]
		private DebugUI _DebugCanvasPrefab;

		public const int MODAL_LAYER_TOUCH_PRIORITY = -999999;

		protected ScreenInfo LDHNLPEOMFC;

		protected Sprite GKOFJFKBGOH;

		public static bool IsPause;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static T EADAACFGGGM;

		protected bool NDHHFHHBFEC;

		protected bool CPOMIKGDIEK;

		public static T BLOOLFFMKFI
		{
			get
			{
				return get_Current();
			}
			protected set
			{
				GAKMJOBBBAD(value);
			}
		}

		public abstract ScreenType PNAJHDBDDLP { get; }

		public static T get_Current()
		{
			return Scene<T>.EADAACFGGGM__BackingField;
		}

		protected static void GAKMJOBBBAD(T value)
		{
			Scene<T>.EADAACFGGGM__BackingField = value;
		}

		public abstract ScreenType get_SceneId();

		protected virtual void Init(object data)
		{
			NDHHFHHBFEC = true;
			GKOFJFKBGOH = null;
			LDHNLPEOMFC = new ScreenInfo();
			IsPause = false;
			if (!AssemblyController.KMEOEAGGPBI())
			{
			}
		}

		protected virtual void PJNFHNFLNNO()
		{
			CPOMIKGDIEK = true;
			if (!AssemblyController.KMEOEAGGPBI())
			{
			}
		}

		protected override void Awake()
		{
			T val = this as T;
			if (SceneManagerSF.Init(val.get_SceneId()))
			{
				GAKMJOBBBAD(this as T);
				SceneManagerSF.DJKMOGJMHLO(get_SceneId());
				GIHJGHJJJGK();
				base.Awake();
				Init(Module.ELEBLBJKDBI().DMCJGOMOJEF.Data);
				if (get_SceneId() != ScreenType.Loader)
				{
					Module.ELEBLBJKDBI().NFEBHLDPHHI(this);
				}
				if (_WideScreenController != null)
				{
					_WideScreenController.Run();
				}
			}
		}

		private void GIHJGHJJJGK()
		{
			if (_DebugCanvasPrefab != null && SystemProperties.DBBOCENKMGD())
			{
				DebugUI debugUI = Object.Instantiate(_DebugCanvasPrefab);
				debugUI.name = "[DebugCanvas]";
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (NDHHFHHBFEC && !CPOMIKGDIEK)
			{
				PJNFHNFLNNO();
			}
			GAKMJOBBBAD((T)null);
			Module.ELEBLBJKDBI().JOCFBBAAPBE(this);
		}

		public virtual void UpdateScene(object data)
		{
		}

		public virtual void Reload(object data)
		{
			ScreenType iPKNDMINFMJ = Module.ELEBLBJKDBI().NMCNDOPKFJD();
		}

		public virtual Sprite GetVisualObject(VisualObjectType NBLGANHBAEH)
		{
			return null;
		}

		public void ToggleModalLayer(bool value)
		{
		}
	}
}
