using UnityEngine;

namespace Nekki.SF2.GUI.Map
{
	public class IndicatorFight : SFMonoBehaviour<object>
	{
		public enum ILPCJIPBONE
		{
			IsOn = 0,
			IsOff = 1,
			IsLocked = 2
		}

		public const string FILE_INDICATOR_ATLAS = "MiscSprites";

		public const string FILE_INDICATOR_ON = "MiscSprites.indicatorOn";

		public const string FILE_INDICATOR_OFF = "MiscSprites.indicatorOff";

		public const string FILE_INDICATOR_LOCKED = "MiscSprites.indicatorLocked";

		private ILPCJIPBONE LDOJANLOFHI = ILPCJIPBONE.IsOff;

		private float _scale = 1f;

		public ILPCJIPBONE GCDHNODCJAA
		{
			get
			{
				return get_CurrentState();
			}
			set
			{
				set_CurrentState(value);
			}
		}

		public float FOAHMAOBFEA
		{
			get
			{
				return get_Scale();
			}
			set
			{
				set_Scale(value);
			}
		}

		public ILPCJIPBONE get_CurrentState()
		{
			return LDOJANLOFHI;
		}

		public void set_CurrentState(ILPCJIPBONE value)
		{
			LDOJANLOFHI = value;
			ResolutionImage component = GetComponent<ResolutionImage>();
			component.set_TexturePath("MiscSprites");
			switch (LDOJANLOFHI)
			{
			case ILPCJIPBONE.IsOn:
				component.set_SpriteName("MiscSprites.indicatorOn");
				break;
			case ILPCJIPBONE.IsOff:
				component.set_SpriteName("MiscSprites.indicatorOff");
				break;
			case ILPCJIPBONE.IsLocked:
				component.set_SpriteName("MiscSprites.indicatorLocked");
				break;
			}
		}

		public float get_Scale()
		{
			return _scale;
		}

		public void set_Scale(float value)
		{
			_scale = value;
			ResolutionImage component = GetComponent<ResolutionImage>();
			component.transform.localScale = new Vector3(_scale, _scale);
		}
	}
}
