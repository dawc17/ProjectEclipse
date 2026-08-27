using UnityEngine;

namespace Nekki.SF2.GUI.Scripts
{
	public class LoadingCircle : MonoBehaviour
	{
		[SerializeField]
		private int _SegmentsCount = 12;

		[SerializeField]
		private float _Timeout = 0.1f;

		private float PFLLICEIKCM;

		private Vector3 _Step;

		public bool BDJPLHOKIPF
		{
			get
			{
				return get_IsPlaying();
			}
		}

		public void Play()
		{
			PFLLICEIKCM = _Timeout;
			_Step = new Vector3(0f, 0f, -360f / (float)_SegmentsCount);
			base.transform.localEulerAngles = Vector3.zero;
			base.gameObject.SetActive(true);
		}

		public void Stop()
		{
			base.gameObject.SetActive(false);
		}

		public bool get_IsPlaying()
		{
			return base.gameObject.activeSelf;
		}

		private void Awake()
		{
			Stop();
		}

		private void Update()
		{
			if (PFLLICEIKCM > 1E-06f)
			{
				PFLLICEIKCM -= Time.deltaTime;
				return;
			}
			PFLLICEIKCM = _Timeout;
			base.transform.localEulerAngles += _Step;
		}
	}
}
