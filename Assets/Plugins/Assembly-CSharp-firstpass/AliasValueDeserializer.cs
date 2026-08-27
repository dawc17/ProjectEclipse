using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using YamlDotNet.Core;

public sealed class AliasValueDeserializer : FFBEMOKFDNL
{
	private sealed class DMGFMLMIFGL : Dictionary<string, JFLHKBKDOFM>, KOOPFFDDANF
	{
		public void INOFEFDGNFL()
		{
			foreach (JFLHKBKDOFM value in base.Values)
			{
				if (!value.DHNFINJFGJM())
				{
					throw new AnchorNotFoundException(value.HBCNKNFPAIM.OGPHJPFHBJL(), value.HBCNKNFPAIM.GDJHIJHFPHA(), string.Format("Anchor '{0}' not found", value.HBCNKNFPAIM.OEAKCOHMIHH()));
				}
			}
		}
	}

	private sealed class JFLHKBKDOFM : IValuePromise
	{
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<object> ValueAvailable;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool NDKANKBCEOM;

		private object value;

		public readonly AnchorAlias HBCNKNFPAIM;

		public bool GCKFEGDHIGI
		{
			get
			{
				return DHNFINJFGJM();
			}
			private set
			{
				set_HasValue(value);
			}
		}

		public event Action<object> MJCKDPOOOMB
		{
			add
			{
				add_ValueAvailable(value);
			}
			remove
			{
				remove_ValueAvailable(value);
			}
		}

		public JFLHKBKDOFM(AnchorAlias LOKLDPLAPOL)
		{
			HBCNKNFPAIM = LOKLDPLAPOL;
		}

		public JFLHKBKDOFM(object value)
		{
			set_HasValue(true);
			this.value = value;
		}

		public void add_ValueAvailable(Action<object> value)
		{
			Action<object> action = ValueAvailable;
			Action<object> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref ValueAvailable, (Action<object>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}

		public void remove_ValueAvailable(Action<object> value)
		{
			Action<object> action = ValueAvailable;
			Action<object> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref ValueAvailable, (Action<object>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}

		public bool DHNFINJFGJM()
		{
			return NDKANKBCEOM;
		}

		private void set_HasValue(bool value)
		{
			NDKANKBCEOM = value;
		}

		public object OEAKCOHMIHH()
		{
			if (!DHNFINJFGJM())
			{
				throw new InvalidOperationException("Value not set");
			}
			return value;
		}

		public void set_Value(object value)
		{
			if (DHNFINJFGJM())
			{
				throw new InvalidOperationException("Value already set");
			}
			set_HasValue(true);
			this.value = value;
			if (ValueAvailable != null)
			{
				ValueAvailable(value);
			}
		}
	}

	private readonly FFBEMOKFDNL PMODPPCDACN;

	public AliasValueDeserializer(FFBEMOKFDNL PMODPPCDACN)
	{
		if (PMODPPCDACN == null)
		{
			throw new ArgumentNullException("innerDeserializer");
		}
		this.PMODPPCDACN = PMODPPCDACN;
	}

	public object BBNMBCMJOFM(EventReader reader, Type MBLGNMBFHBI, SerializerState state, FFBEMOKFDNL IJBAEAEDMCC)
	{
		AnchorAlias mBEGNNDMDKH = reader.GNNPKHDPGLN<AnchorAlias>();
		if (mBEGNNDMDKH != null)
		{
			DMGFMLMIFGL dMGFMLMIFGL = state.Get<DMGFMLMIFGL>();
			JFLHKBKDOFM value;
			if (!dMGFMLMIFGL.TryGetValue(mBEGNNDMDKH.OEAKCOHMIHH(), out value))
			{
				value = new JFLHKBKDOFM(mBEGNNDMDKH);
				dMGFMLMIFGL.Add(mBEGNNDMDKH.OEAKCOHMIHH(), value);
			}
			return (!value.DHNFINJFGJM()) ? value : value.OEAKCOHMIHH();
		}
		string text = null;
		NodeEvent dGMPGIHHKCN = reader.Peek<NodeEvent>();
		if (dGMPGIHHKCN != null && !string.IsNullOrEmpty(dGMPGIHHKCN.HCPOJDFJFMM()))
		{
			text = dGMPGIHHKCN.HCPOJDFJFMM();
		}
		object obj = PMODPPCDACN.BBNMBCMJOFM(reader, MBLGNMBFHBI, state, IJBAEAEDMCC);
		if (text != null)
		{
			DMGFMLMIFGL dMGFMLMIFGL2 = state.Get<DMGFMLMIFGL>();
			JFLHKBKDOFM value2;
			if (!dMGFMLMIFGL2.TryGetValue(text, out value2))
			{
				dMGFMLMIFGL2.Add(text, new JFLHKBKDOFM(obj));
			}
			else
			{
				if (value2.DHNFINJFGJM())
				{
					throw new DuplicateAnchorException(dGMPGIHHKCN.OGPHJPFHBJL(), dGMPGIHHKCN.GDJHIJHFPHA(), string.Format("Anchor '{0}' already defined", text));
				}
				value2.set_Value(obj);
			}
		}
		return obj;
	}
}
