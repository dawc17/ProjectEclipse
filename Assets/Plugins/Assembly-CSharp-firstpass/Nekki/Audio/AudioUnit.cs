using System.Diagnostics;
using UnityEngine;

namespace Nekki.Audio
{
	public class AudioUnit : MonoBehaviour
	{
		private PlayCommand PIEKHPPPIKO;

		private AudioSource _source;

		private Chanel _parent;

		private bool DIFDDHGKKDL;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool PMEFBNOLGEI;

		private bool IKELBJJFKJG;

		internal bool DJNNDGIICGA
		{
			get
			{
				return JOHIGCLLECD();
			}
			private set
			{
				CCIPGJMJJCA(value);
			}
		}

		public bool JNCMBHENOIM
		{
			get
			{
				return get_IsMute();
			}
			set
			{
				set_IsMute(value);
			}
		}

		internal bool EDEFCODFPGK
		{
			get
			{
				return CNMPNLDPPEL();
			}
		}

		internal bool JOHIGCLLECD()
		{
			return PMEFBNOLGEI;
		}

		private void CCIPGJMJJCA(bool value)
		{
			PMEFBNOLGEI = value;
		}

		public bool get_IsMute()
		{
			return _source.mute;
		}

		public void set_IsMute(bool value)
		{
			_source.mute = value;
		}

		internal void Init(Chanel PBJGAIDJFAG, PlayCommand NJOJDALGNKG, AudioClip PIKHEAGHOKB)
		{
			PIEKHPPPIKO = NJOJDALGNKG;
			if (!_source)
			{
				_source = base.gameObject.AddComponent<AudioSource>();
				_source.spatialBlend = 0f;
			}
			_source.clip = PIKHEAGHOKB;
			_source.loop = NJOJDALGNKG.ADCBILEEEEO();
			_source.volume = PBJGAIDJFAG.LFDFKPHKEGJ() * NJOJDALGNKG.AFKMLMCCJLI();
			_parent = PBJGAIDJFAG;
			CCIPGJMJJCA(false);
			IKELBJJFKJG = false;
			_source.Play();
		}

		public void Pause()
		{
			if (!(_source == null) && !JOHIGCLLECD())
			{
				CCIPGJMJJCA(true);
				IKELBJJFKJG = _source.isPlaying;
				_source.Pause();
			}
		}

		public void UnPause()
		{
			if (!(_source == null) && JOHIGCLLECD())
			{
				if (IKELBJJFKJG)
				{
					_source.Play();
				}
				_source.volume = _parent.LFDFKPHKEGJ() * PIEKHPPPIKO.AFKMLMCCJLI();
				CCIPGJMJJCA(false);
				IKELBJJFKJG = false;
			}
		}

		public void Stop(bool BJIOMMPCLEA = false)
		{
			if ((bool)_source)
			{
				if (!BJIOMMPCLEA)
				{
					_source.Stop();
					CCIPGJMJJCA(false);
					IKELBJJFKJG = false;
				}
				else
				{
					DIFDDHGKKDL = true;
				}
			}
		}

		internal bool CNMPNLDPPEL()
		{
			return !_source || ((bool)_source && !_source.isPlaying && !JOHIGCLLECD());
		}

		internal void Update()
		{
			if (JOHIGCLLECD() || CNMPNLDPPEL())
			{
				return;
			}
			// Scene teardown can destroy the owning channel before this component's
			// final Update.  The decompiled code kept dereferencing the stale owner
			// every frame, producing thousands of errors and severe Editor lag.
			if (_parent == null || PIEKHPPPIKO == null)
			{
				if (_source != null)
				{
					_source.Stop();
				}
				enabled = false;
				return;
			}
			if (DIFDDHGKKDL)
			{
				float num = _source.volume * 0.9f;
				if ((double)num < 0.05)
				{
					Stop();
					DIFDDHGKKDL = false;
				}
				else
				{
					_source.volume = num;
				}
			}
			else
			{
				_source.volume = _parent.LFDFKPHKEGJ() * PIEKHPPPIKO.AFKMLMCCJLI();
			}
		}

		internal void PJNFHNFLNNO()
		{
			if (_parent != null)
			{
				_parent.FreeUnit(this);
			}
		}
	}
}
