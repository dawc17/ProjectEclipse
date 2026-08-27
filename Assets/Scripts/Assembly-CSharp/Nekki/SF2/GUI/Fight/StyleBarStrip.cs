using UnityEngine.UI;

namespace Nekki.SF2.GUI.Fight
{
	public class StyleBarStrip : ResolutionImageSkew
	{
		private float BMEMLDIKHBB;

		private float PIPOJNKLHKF;

		private int _framesToEnd;

		public void Init(float value)
		{
			base.type = Type.Filled;
			base.fillMethod = FillMethod.Horizontal;
			base.fillAmount = value;
			BMEMLDIKHBB = value;
			PIPOJNKLHKF = value;
		}

		public void SetValue(float value, int frames)
		{
			PIPOJNKLHKF = value;
			_framesToEnd = frames;
			if (frames <= 0)
			{
				BMEMLDIKHBB = PIPOJNKLHKF;
				base.fillAmount = BMEMLDIKHBB;
			}
		}

		public void Render()
		{
			if (BMEMLDIKHBB != PIPOJNKLHKF)
			{
				if (_framesToEnd <= 0)
				{
					BMEMLDIKHBB = PIPOJNKLHKF;
					base.fillAmount = BMEMLDIKHBB;
				}
				else
				{
					float num = BMEMLDIKHBB - PIPOJNKLHKF;
					float num2 = num / (float)_framesToEnd;
					BMEMLDIKHBB -= num2;
					base.fillAmount = BMEMLDIKHBB;
				}
				_framesToEnd--;
			}
		}
	}
}
