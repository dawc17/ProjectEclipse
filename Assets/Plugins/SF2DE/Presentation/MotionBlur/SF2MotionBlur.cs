using UnityEngine;
using UnityEngine.SceneManagement;

// Lightweight temporal motion blur for the recovered built-in render pipeline.
// This is presentation-only: it blends completed camera frames and never feeds
// data back into the fixed-step fight simulation.
[RequireComponent(typeof(Camera))]
public sealed class SF2MotionBlur : MonoBehaviour
{
	private const string ShaderName = "Hidden/SF2/FrameBlendMotionBlur";

	private Material _material;

	private RenderTexture _history;

	private bool _historyValid;

	private int _historyWidth;

	private int _historyHeight;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void RegisterSceneHook()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		EnsureOnMainCamera();
	}

	public static void EnsureOnMainCamera()
	{
		string sceneName = SceneManager.GetActiveScene().name;
		if (sceneName != "Fight" && sceneName != "Dojo")
		{
			return;
		}
		Camera camera = Camera.main;
		if (camera != null && camera.GetComponent<SF2MotionBlur>() == null)
		{
			camera.gameObject.AddComponent<SF2MotionBlur>();
		}
	}

	private void OnEnable()
	{
		_historyValid = false;
	}

	private void OnDisable()
	{
		ReleaseResources();
	}

	private void OnDestroy()
	{
		ReleaseResources();
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!SF2DisplayFrameRate.MotionBlurEnabled || SF2DisplayFrameRate.MotionBlurStrength <= 0.001f)
		{
			Graphics.Blit(source, destination);
			_historyValid = false;
			return;
		}

		if (!EnsureResources(source))
		{
			Graphics.Blit(source, destination);
			return;
		}

		float deltaTime = Time.unscaledDeltaTime;
		if (!_historyValid || deltaTime <= 0f || deltaTime > 0.1f)
		{
			Graphics.Blit(source, destination);
			Graphics.Blit(source, _history);
			_historyValid = true;
			return;
		}

		// Convert the user-facing strength to an exposure/decay time. Using time
		// rather than a fixed per-frame blend keeps the appearance stable as the
		// display rate changes.
		float shutterSeconds = Mathf.Lerp(1f / 480f, 1f / 60f, SF2DisplayFrameRate.MotionBlurStrength);
		float historyWeight = Mathf.Exp(-deltaTime / shutterSeconds);
			_material.SetTexture("_HistoryTex", _history);
			_material.SetFloat("_HistoryWeight", Mathf.Clamp01(historyWeight));
			Graphics.Blit(source, destination, _material);

			// Keep an unblurred previous camera frame. Besides avoiding recursive
			// trails, using the image-effect source keeps history orientation
			// consistent across render-target/back-buffer transitions.
			Graphics.Blit(source, _history);
		}

	private bool EnsureResources(RenderTexture source)
	{
		if (_material == null)
		{
			Shader shader = Resources.Load<Shader>("shaders/SF2FrameBlendMotionBlur");
			if (shader == null)
			{
				shader = Shader.Find(ShaderName);
			}
			if (shader == null || !shader.isSupported)
			{
				return false;
			}
			_material = new Material(shader);
			_material.hideFlags = HideFlags.HideAndDontSave;
		}

		if (_history == null || _historyWidth != source.width || _historyHeight != source.height)
		{
			ReleaseHistory();
			_history = new RenderTexture(source.width, source.height, 0, source.format);
			_history.name = "SF2 Motion Blur History";
			_history.hideFlags = HideFlags.HideAndDontSave;
			_history.filterMode = FilterMode.Bilinear;
			_history.wrapMode = TextureWrapMode.Clamp;
			_history.Create();
			_historyWidth = source.width;
			_historyHeight = source.height;
			_historyValid = false;
		}

		return _history != null && _history.IsCreated();
	}

	private void ReleaseResources()
	{
		ReleaseHistory();
		if (_material != null)
		{
			DestroyRuntimeObject(_material);
			_material = null;
		}
	}

	private void ReleaseHistory()
	{
		if (_history != null)
		{
			_history.Release();
			DestroyRuntimeObject(_history);
			_history = null;
		}
		_historyWidth = 0;
		_historyHeight = 0;
		_historyValid = false;
	}

	private static void DestroyRuntimeObject(UnityEngine.Object value)
	{
		if (value == null)
		{
			return;
		}
		if (Application.isPlaying)
		{
			UnityEngine.Object.Destroy(value);
		}
		else
		{
			UnityEngine.Object.DestroyImmediate(value);
		}
	}
}
