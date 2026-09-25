using Eclipse.Modding;
using UnityEngine;
using UnityEngine.SceneManagement;

// Mod-configured full-screen effects for fights (sf2.visuals.bloom and
// sf2.visuals.impact): bloom on bright pixels and a short radial impact on heavy
// hits. Presentation only; frames pass through untouched unless a mod enables them.
[RequireComponent(typeof(Camera))]
public sealed class EclipseScreenEffects : MonoBehaviour
{
	private const int Levels = 5;

	private readonly RenderTexture[] _levels = new RenderTexture[Levels];
	private Material _material;
	private bool _unsupported;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void RegisterSceneHook()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name != "Fight" && scene.name != "Dojo") return;
		Camera camera = Camera.main;
		if (camera != null && camera.GetComponent<EclipseScreenEffects>() == null)
			camera.gameObject.AddComponent<EclipseScreenEffects>();
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		ModVisualDefinition bloomSettings = ModVisuals.Active(ModVisualEffect.Bloom);
		bool bloom = bloomSettings != null && bloomSettings.Number("intensity") > 0f;
		float impact = ModVisuals.CurrentImpact;
		if ((!bloom && impact <= 0f) || !EnsureMaterial())
		{
			Graphics.Blit(source, destination);
			return;
		}

		RenderTexture bloomTexture = null;
		int count = 0;
		if (bloom)
		{
			_material.SetFloat("_Threshold", bloomSettings.Number("threshold"));
			_material.SetFloat("_Knee", Mathf.Max(bloomSettings.Number("knee"), 0.001f));
			int width = source.width / 2, height = source.height / 2;
			RenderTexture previous = source;
			for (; count < Levels && width >= 4 && height >= 4; count++)
			{
				_levels[count] = RenderTexture.GetTemporary(width, height, 0, source.format);
				_levels[count].filterMode = FilterMode.Bilinear;
				Graphics.Blit(previous, _levels[count], _material, count == 0 ? 0 : 1);
				previous = _levels[count];
				width /= 2; height /= 2;
			}
			for (int i = count - 2; i >= 0; i--)
				Graphics.Blit(_levels[i + 1], _levels[i], _material, 2);
			bloomTexture = count > 0 ? _levels[0] : null;
		}

		_material.SetTexture("_BloomTex", bloomTexture != null ? (Texture)bloomTexture : Texture2D.blackTexture);
		_material.SetFloat("_BloomIntensity", bloomTexture != null ? bloomSettings.Number("intensity") : 0f);
		_material.SetFloat("_Impact", impact);
		Graphics.Blit(source, destination, _material, 3);

		for (int i = 0; i < count; i++)
		{
			RenderTexture.ReleaseTemporary(_levels[i]);
			_levels[i] = null;
		}
	}

	private bool EnsureMaterial()
	{
		if (_material != null) return true;
		if (_unsupported) return false;
		Shader shader = Resources.Load<Shader>("shaders/EclipseScreenEffects");
		if (shader == null || !shader.isSupported)
		{
			_unsupported = true;
			Debug.LogWarning("[Eclipse] Screen effects shader is unavailable.");
			return false;
		}
		_material = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
		return true;
	}

	private void OnDestroy()
	{
		if (_material != null) Destroy(_material);
	}
}
