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
			Vector3f eMAFACPEPDK = JJNIIAEBGIA.OGLAOHGLBHI().ICLEOFDKDIF();
			Vector3f eMAFACPEPDK2 = JJNIIAEBGIA.KMHHBEKNHCJ().ICLEOFDKDIF();
			_Line.SetPosition(0, new Vector3(eMAFACPEPDK.GILCBJJPKBK(), eMAFACPEPDK.OBIMBNIBEFG(), -1f));
			_Line.SetPosition(1, new Vector3(eMAFACPEPDK2.GILCBJJPKBK(), eMAFACPEPDK2.OBIMBNIBEFG(), -1f));
		}
	}
}
