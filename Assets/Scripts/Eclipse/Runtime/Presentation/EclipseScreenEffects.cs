using Eclipse.Modding;
using UnityEngine;
using UnityEngine.SceneManagement;

// Mod-configured full-screen effects for fights (sf2.visuals.bloom,
// sf2.visuals.impact and sf2.fx.screen): bloom on bright pixels, a short radial
// impact on heavy hits, and colour grading with a vignette, grain, halation and
// an accent colour kept through desaturation. Presentation only; frames pass
// through untouched unless a mod enables one of them.
[RequireComponent(typeof(Camera))]
public sealed class EclipseScreenEffects : MonoBehaviour
{
	private const int Levels = 5;

	private readonly RenderTexture[] _levels = new RenderTexture[Levels];
	private readonly RenderTexture[] _halationLevels = new RenderTexture[Levels];
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

	// Slow motion from triggered sf2.fx.screen grades follows unscaled time.
	private void Update()
	{
		ModVisuals.UpdateTimeScale();
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		ModVisualDefinition bloomSettings = ModVisuals.Active(ModVisualEffect.Bloom);
		bool bloom = bloomSettings != null && bloomSettings.Number("intensity") > 0f;
		float impact = ModVisuals.CurrentImpact;
		ModVisuals.ScreenGrade grade = ModVisuals.CurrentGrade();
		if ((!bloom && impact <= 0f && !grade.Active) || !EnsureMaterial())
		{
			Graphics.Blit(source, destination);
			return;
		}

		RenderTexture bloomTexture = null;
		int count = 0;
		if (bloom)
			bloomTexture = BuildGlow(source, _levels, bloomSettings.Number("threshold"), bloomSettings.Number("knee"), out count);
		RenderTexture halationTexture = null;
		int halationCount = 0;
		if (grade.Active && grade.Halation > 0f)
			halationTexture = BuildGlow(source, _halationLevels, grade.HalationThreshold, 0.25f, out halationCount);

		_material.SetTexture("_BloomTex", bloomTexture != null ? (Texture)bloomTexture : Texture2D.blackTexture);
		_material.SetFloat("_BloomIntensity", bloomTexture != null ? bloomSettings.Number("intensity") : 0f);
		_material.SetFloat("_Impact", impact);
		_material.SetFloat("_Saturation", grade.Active ? grade.Saturation : 1f);
		_material.SetFloat("_Contrast", grade.Active ? grade.Contrast : 1f);
		_material.SetFloat("_Brightness", grade.Active ? grade.Brightness : 0f);
		_material.SetColor("_Tint", grade.Tint);
		_material.SetFloat("_TintStrength", grade.Active ? grade.TintStrength : 0f);
		_material.SetFloat("_Vignette", grade.Active ? grade.Vignette : 0f);
		_material.SetVector("_VignetteCenter", grade.Active ? (Vector4)grade.VignetteCenter : Vector4.zero);
		_material.SetFloat("_Grain", grade.Active ? grade.Grain : 0f);
		_material.SetFloat("_GrainTime", Time.unscaledTime % 97f);
		_material.SetTexture("_HalationTex", halationTexture != null ? (Texture)halationTexture : Texture2D.blackTexture);
		_material.SetFloat("_Halation", halationTexture != null ? grade.Halation : 0f);
		_material.SetColor("_HalationColor", grade.HalationColor);
		_material.SetFloat("_AccentStrength", grade.Active ? grade.AccentStrength : 0f);
		_material.SetFloat("_AccentWidth", grade.AccentWidth);
		float h, s, v;
		Color.RGBToHSV(grade.Accent, out h, out s, out v);
		_material.SetFloat("_AccentHue", h);
		Graphics.Blit(source, destination, _material, 3);

		Release(_levels, count);
		Release(_halationLevels, halationCount);
	}

	// Threshold, downsample and upsample into a soft glow of the bright pixels.
	private RenderTexture BuildGlow(RenderTexture source, RenderTexture[] levels, float threshold, float knee, out int count)
	{
		_material.SetFloat("_Threshold", threshold);
		_material.SetFloat("_Knee", Mathf.Max(knee, 0.001f));
		int width = source.width / 2, height = source.height / 2;
		RenderTexture previous = source;
		count = 0;
		for (; count < Levels && width >= 4 && height >= 4; count++)
		{
			levels[count] = RenderTexture.GetTemporary(width, height, 0, source.format);
			levels[count].filterMode = FilterMode.Bilinear;
			Graphics.Blit(previous, levels[count], _material, count == 0 ? 0 : 1);
			previous = levels[count];
			width /= 2; height /= 2;
		}
		for (int i = count - 2; i >= 0; i--)
			Graphics.Blit(levels[i + 1], levels[i], _material, 2);
		return count > 0 ? levels[0] : null;
	}

	private static void Release(RenderTexture[] levels, int count)
	{
		for (int i = 0; i < count; i++)
		{
			RenderTexture.ReleaseTemporary(levels[i]);
			levels[i] = null;
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
		ModVisuals.ReleaseTimeScale();
		if (_material != null) Destroy(_material);
	}
}
