using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using YamlDotNet.RepresentationModel;

namespace Nekki.Yaml
{
	[Serializable]
	public class Scalar : Node
	{
		public delegate void GALGMABOBDE();

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private static GALGMABOBDE TextUpdate;

		private YamlScalarNode _scalar;

		public string text
		{
			get
			{
				return _scalar.Value;
			}
		}

		public static event GALGMABOBDE NIFBLBEKKBE
		{
			add
			{
				HPDAACEPGKP(value);
			}
			remove
			{
				KLFNNKDLIKA(value);
			}
		}

		public Scalar(string HODKINDOEGD, YamlScalarNode DJMPIGLOHBC)
		{
			base.typeNode = "Scalar";
			base.key = HODKINDOEGD;
			base.value = DJMPIGLOHBC;
			_scalar = (YamlScalarNode)base.value;
		}

		public Scalar(string HODKINDOEGD, string NFICJMLCGEO)
		{
			base.typeNode = "Scalar";
			base.key = HODKINDOEGD;
			base.value = new YamlScalarNode(NFICJMLCGEO);
			_scalar = (YamlScalarNode)base.value;
		}

		public static void HPDAACEPGKP(GALGMABOBDE value)
		{
			GALGMABOBDE gALGMABOBDE = TextUpdate;
			GALGMABOBDE gALGMABOBDE2;
			do
			{
				gALGMABOBDE2 = gALGMABOBDE;
				gALGMABOBDE = Interlocked.CompareExchange(ref TextUpdate, (GALGMABOBDE)Delegate.Combine(gALGMABOBDE2, value), gALGMABOBDE);
			}
			while ((object)gALGMABOBDE != gALGMABOBDE2);
		}

		public static void KLFNNKDLIKA(GALGMABOBDE value)
		{
			GALGMABOBDE gALGMABOBDE = TextUpdate;
			GALGMABOBDE gALGMABOBDE2;
			do
			{
				gALGMABOBDE2 = gALGMABOBDE;
				gALGMABOBDE = Interlocked.CompareExchange(ref TextUpdate, (GALGMABOBDE)Delegate.Remove(gALGMABOBDE2, value), gALGMABOBDE);
			}
			while ((object)gALGMABOBDE != gALGMABOBDE2);
		}

		private static void FIAKCFNHJFC()
		{
			GALGMABOBDE textUpdate = TextUpdate;
			if (textUpdate != null)
			{
				textUpdate();
			}
		}

		public void NBGHLFJPOGM(string NFICJMLCGEO)
		{
			_scalar.Value = NFICJMLCGEO;
			FIAKCFNHJFC();
		}

		public string GetText()
		{
			return _scalar.Value;
		}
	}
}
