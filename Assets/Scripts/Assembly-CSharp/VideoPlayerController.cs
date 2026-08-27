using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Nekki.SF2.GUI;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
	public delegate void PPCCKEGFAHH();

	private VideoPlayer FGLGMBPGLHP;

	private AudioSource EPAIJBJBACG;

	private bool GHCOIBINJBP;

	private bool completionRaised;

	private float prepareStartedAt;

	private const float PrepareTimeoutSeconds = 8f;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private PPCCKEGFAHH ShowCompleted;

	public event PPCCKEGFAHH FCMPLHIDMJK
	{
		add
		{
			add_ShowCompleted(value);
		}
		remove
		{
			remove_ShowCompleted(value);
		}
	}

	public void add_ShowCompleted(PPCCKEGFAHH value)
	{
		PPCCKEGFAHH pPCCKEGFAHH = ShowCompleted;
		PPCCKEGFAHH pPCCKEGFAHH2;
		do
		{
			pPCCKEGFAHH2 = pPCCKEGFAHH;
			pPCCKEGFAHH = Interlocked.CompareExchange(ref ShowCompleted, (PPCCKEGFAHH)Delegate.Combine(pPCCKEGFAHH2, value), pPCCKEGFAHH);
		}
		while ((object)pPCCKEGFAHH != pPCCKEGFAHH2);
	}

	public void remove_ShowCompleted(PPCCKEGFAHH value)
	{
		PPCCKEGFAHH pPCCKEGFAHH = ShowCompleted;
		PPCCKEGFAHH pPCCKEGFAHH2;
		do
		{
			pPCCKEGFAHH2 = pPCCKEGFAHH;
			pPCCKEGFAHH = Interlocked.CompareExchange(ref ShowCompleted, (PPCCKEGFAHH)Delegate.Remove(pPCCKEGFAHH2, value), pPCCKEGFAHH);
		}
		while ((object)pPCCKEGFAHH != pPCCKEGFAHH2);
	}

	public void Init()
	{
		FGLGMBPGLHP = GetComponent<VideoPlayer>();
		EPAIJBJBACG = GetComponent<AudioSource>();
		FGLGMBPGLHP.audioOutputMode = VideoAudioOutputMode.AudioSource;
		FGLGMBPGLHP.controlledAudioTrackCount = 1;
		FGLGMBPGLHP.EnableAudioTrack(0, true);
		FGLGMBPGLHP.SetTargetAudioSource(0, EPAIJBJBACG);
		FGLGMBPGLHP.loopPointReached += PDIMNFGFIOF;
		FGLGMBPGLHP.prepareCompleted += KMKFIJPBBPC;
		FGLGMBPGLHP.errorReceived += OnVideoError;
		FGLGMBPGLHP.targetCamera = UnityEngine.Camera.main;
		UnityEngine.Camera.main.backgroundColor = Color.black;
		completionRaised = false;
	}

	private void Update()
	{
		if (!GHCOIBINJBP)
		{
			return;
		}
		if (Input.touchCount > 0 || Input.anyKeyDown || Input.GetMouseButtonDown(0))
		{
			PNANBCJNMAL();
			return;
		}
		if (FGLGMBPGLHP != null && !FGLGMBPGLHP.isPrepared && Time.realtimeSinceStartup - prepareStartedAt >= PrepareTimeoutSeconds)
		{
			UnityEngine.Debug.LogWarning("[Video] Preparation timed out; continuing without video: " + FGLGMBPGLHP.url);
			PNANBCJNMAL();
		}
	}

	public void Play(string BEPKJNKCKPH)
	{
		Screen.sleepTimeout = -1;
		FGLGMBPGLHP.source = VideoSource.Url;
		if (FGLGMBPGLHP != null)
		{
			FGLGMBPGLHP.url = BEPKJNKCKPH;
			FGLGMBPGLHP.audioOutputMode = VideoAudioOutputMode.AudioSource;
			FGLGMBPGLHP.controlledAudioTrackCount = 1;
			FGLGMBPGLHP.EnableAudioTrack(0, true);
			FGLGMBPGLHP.SetTargetAudioSource(0, EPAIJBJBACG);
			prepareStartedAt = Time.realtimeSinceStartup;
			FGLGMBPGLHP.Prepare();
			GHCOIBINJBP = true;
		}
	}

	public void Play(VideoClip PIKHEAGHOKB)
	{
		Screen.sleepTimeout = -1;
		FGLGMBPGLHP.source = VideoSource.VideoClip;
		if (FGLGMBPGLHP != null)
		{
			FGLGMBPGLHP.clip = PIKHEAGHOKB;
			prepareStartedAt = Time.realtimeSinceStartup;
			FGLGMBPGLHP.Prepare();
			GHCOIBINJBP = true;
		}
	}

	private void KMKFIJPBBPC(VideoPlayer EJPOJJKKICO)
	{
		Sound.CKIHDLJBGAE();
		AJKJNJDGBAM(false);
		if (FGLGMBPGLHP != null)
		{
			FGLGMBPGLHP.prepareCompleted -= KMKFIJPBBPC;
			FGLGMBPGLHP.Play();
		}
		if (EPAIJBJBACG != null)
		{
			EPAIJBJBACG.Play();
		}
	}

	private void PDIMNFGFIOF(VideoPlayer EJPOJJKKICO = null)
	{
		if (completionRaised)
		{
			return;
		}
		completionRaised = true;
		GHCOIBINJBP = false;
		AJKJNJDGBAM(true);
		if (FGLGMBPGLHP != null)
		{
			FGLGMBPGLHP.loopPointReached -= PDIMNFGFIOF;
			FGLGMBPGLHP.prepareCompleted -= KMKFIJPBBPC;
			FGLGMBPGLHP.errorReceived -= OnVideoError;
		}
		Screen.sleepTimeout = -2;
		Sound.MPAHNMFMHHK();
		ShowCompleted();
		FGLGMBPGLHP = null;
		EPAIJBJBACG = null;
	}

	private void OnVideoError(VideoPlayer source, string message)
	{
		UnityEngine.Debug.LogWarning("[Video] Playback failed; continuing without video: " + message);
		PDIMNFGFIOF(source);
	}

	private void PNANBCJNMAL()
	{
		if (FGLGMBPGLHP != null)
		{
			FGLGMBPGLHP.Stop();
		}
		if (EPAIJBJBACG != null)
		{
			EPAIJBJBACG.Stop();
		}
		PDIMNFGFIOF();
	}

	private void AJKJNJDGBAM(bool value)
	{
		ModuleHolder moduleHolder = Module.ELEBLBJKDBI().BOHBCFMJPCA();
		if (moduleHolder != null)
		{
			moduleHolder.GetCanvas().enabled = value;
		}
	}
}
