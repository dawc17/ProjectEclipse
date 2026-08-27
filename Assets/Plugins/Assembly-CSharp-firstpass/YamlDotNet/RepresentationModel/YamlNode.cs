using System;
using System.Collections.Generic;
using YamlDotNet.Core;

namespace YamlDotNet.RepresentationModel
{
	[Serializable]
	public abstract class YamlNode
	{
		public string Anchor { get; set; }

		public string Tag { get; set; }

		public Mark Start { get; private set; }

		public Mark End { get; private set; }

		public abstract IEnumerable<YamlNode> AllNodes { get; }

		public YamlNode()
		{
		}

		internal void Load(NodeEvent CAJDINOLOJH, DocumentLoadingState state)
		{
			Tag = CAJDINOLOJH.LOIGCKFONHJ();
			if (CAJDINOLOJH.HCPOJDFJFMM() != null)
			{
				Anchor = CAJDINOLOJH.HCPOJDFJFMM();
				state.BOKLCAFFHOD(this);
			}
			Start = CAJDINOLOJH.OGPHJPFHBJL();
			End = CAJDINOLOJH.GDJHIJHFPHA();
		}

		internal static YamlNode GLNMJNFLLIN(EventReader DNBFFLFBDOB, DocumentLoadingState state)
		{
			if (DNBFFLFBDOB.GPHIFFOGOGN<Scalar>())
			{
				return new YamlScalarNode(DNBFFLFBDOB, state);
			}
			if (DNBFFLFBDOB.GPHIFFOGOGN<JODGINIKFJF>())
			{
				return new YamlSequenceNode(DNBFFLFBDOB, state);
			}
			if (DNBFFLFBDOB.GPHIFFOGOGN<MappingStart>())
			{
				return new YamlMappingNode(DNBFFLFBDOB, state);
			}
			if (DNBFFLFBDOB.GPHIFFOGOGN<AnchorAlias>())
			{
				AnchorAlias mBEGNNDMDKH = DNBFFLFBDOB.DODGGCGJJLL<AnchorAlias>();
				return state.GetNode(mBEGNNDMDKH.OEAKCOHMIHH(), false, mBEGNNDMDKH.OGPHJPFHBJL(), mBEGNNDMDKH.GDJHIJHFPHA()) ?? new YamlAliasNode(mBEGNNDMDKH.OEAKCOHMIHH());
			}
			throw new ArgumentException("The current event is of an unsupported type.", "events");
		}

		internal abstract void GPBMMFCHANP(DocumentLoadingState state);

		internal void Save(NEKGJNOFOFN NPIDIMCLNEM, EmitterState state)
		{
			if (!string.IsNullOrEmpty(Anchor) && !state.OPMMAOBDGLG().Add(Anchor))
			{
				NPIDIMCLNEM.Emit(new AnchorAlias(Anchor));
			}
			else
			{
				Emit(NPIDIMCLNEM, state);
			}
		}

		internal abstract void Emit(NEKGJNOFOFN NPIDIMCLNEM, EmitterState state);

		public abstract void GPHIFFOGOGN(IYamlVisitor NKECMANOOEM);

		protected bool Equals(YamlNode NOLFMPDGCOC)
		{
			return SafeEquals(Tag, NOLFMPDGCOC.Tag);
		}

		protected static bool SafeEquals(object NMBEADHHHFH, object OKCKNALOCCK)
		{
			if (NMBEADHHHFH != null)
			{
				return NMBEADHHHFH.Equals(OKCKNALOCCK);
			}
			if (OKCKNALOCCK != null)
			{
				return OKCKNALOCCK.Equals(NMBEADHHHFH);
			}
			return true;
		}

		public override int GetHashCode()
		{
			return AOJHKEDINCA(Tag);
		}

		protected static int AOJHKEDINCA(object value)
		{
			return (value != null) ? value.GetHashCode() : 0;
		}

		protected static int CombineHashCodes(int PKKJJLDNHAC, int AKJPBJPEDCF)
		{
			return ((PKKJJLDNHAC << 5) + PKKJJLDNHAC) ^ AKJPBJPEDCF;
		}

		public abstract YamlNode Clone();
	}
}
