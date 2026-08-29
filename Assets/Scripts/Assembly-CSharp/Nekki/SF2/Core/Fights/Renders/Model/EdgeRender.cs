using Eclipse.Rendering.Interpolation;
using UnityEngine;

namespace Nekki.SF2.Core.Fights.Renders.Model
{
	public class EdgeRender : MonoBehaviour
	{
		private ModelEdge JJNIIAEBGIA;

		private LineRenderer _Line;

		public ModelEdge EDPCJALFPLE
		{
			set
			{
				set_Edge(value);
			}
		}

		public void set_Edge(ModelEdge value)
		{
			JJNIIAEBGIA = value;
			if (_Line == null)
			{
				FBJIIKIODKL();
			}
		}

		public void set_Color(Color value)
		{
			_Line.startColor = value;
			_Line.endColor = value;
		}

		private void FBJIIKIODKL()
		{
			_Line = base.gameObject.AddComponent<LineRenderer>();
			_Line.material = new Material(Shader.Find("Sprites/Default"));
			_Line.useWorldSpace = false;
		}

		private void Update()
		{
			float alpha = FightInterpolation.CurrentAlpha;
			float x;
			float y;
			float z;
			FightInterpolation.SamplePosition(JJNIIAEBGIA.OGLAOHGLBHI(), alpha, out x, out y, out z);
			_Line.SetPosition(0, new Vector3(x, y, -1f));
			FightInterpolation.SamplePosition(JJNIIAEBGIA.KMHHBEKNHCJ(), alpha, out x, out y, out z);
			_Line.SetPosition(1, new Vector3(x, y, -1f));
		}
	}
}
