using Eclipse.Rendering.Interpolation;
using UnityEngine;

namespace Nekki.SF2.Core.Fights.Renders.Model
{
	public class EdgeRender : MonoBehaviour
	{
		private ModelEdge JJNIIAEBGIA;

		private LineRenderer _Line;

		private Eclipse.Rendering.ModelPresentation _Presentation;

		private bool _PresentationResolved;

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
			if (!_PresentationResolved)
			{
				_Presentation = GetComponentInParent<Eclipse.Rendering.ModelPresentation>();
				_PresentationResolved = true;
			}
			float alpha = Eclipse.Rendering.ModelPresentation.AlphaFor(_Presentation);
			float x;
			float y;
			float z;
			FightInterpolation.SamplePosition(JJNIIAEBGIA.GetStartNode(), alpha, out x, out y, out z);
			_Line.SetPosition(0, new Vector3(x, y, -1f));
			FightInterpolation.SamplePosition(JJNIIAEBGIA.GetEndNode(), alpha, out x, out y, out z);
			_Line.SetPosition(1, new Vector3(x, y, -1f));
		}
	}
}
