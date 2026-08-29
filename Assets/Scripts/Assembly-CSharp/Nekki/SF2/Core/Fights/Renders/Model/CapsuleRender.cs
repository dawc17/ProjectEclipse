using SF2DE.Rendering.Interpolation;
using UnityEngine;
using UnityEngine.Rendering;

namespace Nekki.SF2.Core.Fights.Renders.Model
{
	public class CapsuleRender : MonoBehaviour
	{
		private float _Stroke = 1f;

		protected Capsule _Base;

		private static Material CBDOCDONEJE;

		private static Material OAKNHBMPNLH;

		private LineRenderer _LineRender;

		private readonly VectorSegmentInterpolation _Interpolation = new VectorSegmentInterpolation();

		public float NFOMECHPEOP
		{
			get
			{
				return get_Stroke();
			}
			set
			{
				set_Stroke(value);
			}
		}

		private static Material BHOOIDPODBM
		{
			get
			{
				return IJPDLBNIKEJ();
			}
		}

		private static Material FAOIEHIIFMP
		{
			get
			{
				return MJIHAKPIPMH();
			}
		}

		public float get_Stroke()
		{
			return _Stroke;
		}

		public void set_Stroke(float value)
		{
			_Stroke = value;
		}

		public Capsule get_Base()
		{
			return _Base;
		}

		public void set_Base(Capsule value)
		{
			_Base = value;
		}

		public static void set_color(Color value)
		{
			MJIHAKPIPMH().color = value;
			IJPDLBNIKEJ().color = value;
		}

		private static Material IJPDLBNIKEJ()
		{
			if (CBDOCDONEJE == null)
			{
				CBDOCDONEJE = new Material(Shader.Find("Sprites/Colored"));
			}
			return CBDOCDONEJE;
		}

		private static Material MJIHAKPIPMH()
		{
			if (OAKNHBMPNLH == null)
			{
				OAKNHBMPNLH = new Material(Shader.Find("Sprites/Default"));
			}
			return OAKNHBMPNLH;
		}

		private void Start()
		{
			_Stroke = _Base.IHEKOJKHPGP();
			_LineRender = base.gameObject.AddComponent<LineRenderer>();
			_LineRender.numCapVertices = 9;
			LineRenderer nLHJNOCKKGE = _LineRender;
			float pJMDIHLGNHB = _Stroke;
			_LineRender.endWidth = pJMDIHLGNHB;
			nLHJNOCKKGE.startWidth = pJMDIHLGNHB;
			_LineRender.useWorldSpace = false;
			_LineRender.sharedMaterial = MJIHAKPIPMH();
			_LineRender.shadowCastingMode = ShadowCastingMode.Off;
			_LineRender.receiveShadows = false;
			_LineRender.alignment = LineAlignment.TransformZ;
			Render();
		}

		private void Update()
		{
			Render();
		}

		public void Render()
		{
			if (_Base != null && !Vector2f.LFPMCJPCJBD(_Base.NDCACMDFLJN(), null) && !Vector2f.LFPMCJPCJBD(_Base.MINOGAHDDHA(), null) && !(_LineRender == null))
			{
				Vector3f eMAFACPEPDK = _Base.NDCACMDFLJN();
				Vector3f eMAFACPEPDK2 = _Base.MINOGAHDDHA();
				Vector3 rawStart = new Vector3(eMAFACPEPDK.GILCBJJPKBK(), eMAFACPEPDK.OBIMBNIBEFG(), eMAFACPEPDK.KMFEKANLCFO());
				Vector3 rawEnd = new Vector3(eMAFACPEPDK2.GILCBJJPKBK(), eMAFACPEPDK2.OBIMBNIBEFG(), eMAFACPEPDK2.KMFEKANLCFO());
				Vector3 start;
				Vector3 end;
				_Interpolation.Sample(rawStart, rawEnd, out start, out end);
				float num = end.x - start.x;
				float num2 = end.y - start.y;
				float x = start.x + num * _Base.JAEOCMCOEFE();
				float y = start.y + num2 * _Base.JAEOCMCOEFE();
				float x2 = start.x + num * (1f - _Base.PLFEEBJMGAK());
				float y2 = start.y + num2 * (1f - _Base.PLFEEBJMGAK());
				if (_Stroke != _Base.IHEKOJKHPGP())
				{
					_Stroke = _Base.IHEKOJKHPGP();
					LineRenderer nLHJNOCKKGE = _LineRender;
					float pJMDIHLGNHB = _Stroke;
					_LineRender.endWidth = pJMDIHLGNHB;
					nLHJNOCKKGE.startWidth = pJMDIHLGNHB;
				}
				_LineRender.SetPosition(0, new Vector3(x, y, 0f));
				_LineRender.SetPosition(1, new Vector3(x2, y2, 0f));
			}
		}
	}
}
