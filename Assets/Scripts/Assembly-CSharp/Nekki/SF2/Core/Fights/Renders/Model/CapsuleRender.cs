using Eclipse.Rendering.Interpolation;
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

		private Eclipse.Rendering.ModelPresentation _Presentation;

		private int _TintVersion = -1;

		private Color _OriginalStartColor;

		private Color _OriginalEndColor;

		private LineRenderer _RimLine;

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

		// Experimental rim light: an offset, rim-coloured twin line behind this one.
		private void UpdateRim(Vector3 start, Vector3 end)
		{
			bool active = Eclipse.Rendering.RimLight.Active;
			if (!active)
			{
				if (_RimLine != null && _RimLine.gameObject.activeSelf) _RimLine.gameObject.SetActive(false);
				return;
			}
			if (_RimLine == null)
			{
				GameObject rim = new GameObject("Rim");
				rim.transform.SetParent(base.transform, false);
				_RimLine = rim.AddComponent<LineRenderer>();
				_RimLine.numCapVertices = _LineRender.numCapVertices;
				_RimLine.useWorldSpace = false;
				_RimLine.sharedMaterial = _LineRender.sharedMaterial;
				_RimLine.shadowCastingMode = ShadowCastingMode.Off;
				_RimLine.receiveShadows = false;
				_RimLine.alignment = LineAlignment.TransformZ;
			}
			if (!_RimLine.gameObject.activeSelf) _RimLine.gameObject.SetActive(true);
			_RimLine.transform.localPosition = Eclipse.Rendering.RimLight.LocalOffset(base.transform);
			_RimLine.startWidth = _RimLine.endWidth = _Stroke;
			Color color = Eclipse.Rendering.RimLight.ColorFor(base.transform);
			_RimLine.startColor = _RimLine.endColor = color;
			_RimLine.SetPosition(0, start);
			_RimLine.SetPosition(1, end);
		}

		// Per-capsule tint from the owning fighter; the shared materials stay untouched.
		private void ApplyTint()
		{
			if (_Presentation == null || _TintVersion == _Presentation.TintVersion) return;
			_TintVersion = _Presentation.TintVersion;
			Color? tint = _Presentation.Tint;
			_LineRender.startColor = tint ?? _OriginalStartColor;
			_LineRender.endColor = tint ?? _OriginalEndColor;
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
			_OriginalStartColor = _LineRender.startColor;
			_OriginalEndColor = _LineRender.endColor;
			_Presentation = GetComponentInParent<Eclipse.Rendering.ModelPresentation>();
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
				Vector3 rawStart = new Vector3(eMAFACPEPDK.GetX(), eMAFACPEPDK.GetY(), eMAFACPEPDK.GetZ());
				Vector3 rawEnd = new Vector3(eMAFACPEPDK2.GetX(), eMAFACPEPDK2.GetY(), eMAFACPEPDK2.GetZ());
				Vector3 start;
				Vector3 end;
				_Interpolation.Sample(rawStart, rawEnd, Eclipse.Rendering.ModelPresentation.AlphaFor(_Presentation), out start, out end);
				ApplyTint();
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
				UpdateRim(new Vector3(x, y, 0f), new Vector3(x2, y2, 0f));
			}
		}
	}
}
