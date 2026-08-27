using System;
using System.Globalization;
using System.Linq;

public class RoundtripObjectGraphTraversalStrategy : FullObjectGraphTraversalStrategy
{
	public RoundtripObjectGraphTraversalStrategy(Serializer MGFPEIHBMLD, ITypeInspector GIJPGEHPILC, ITypeResolver CBMKGNIHPFO, int maxRecursion)
		: base(MGFPEIHBMLD, GIJPGEHPILC, CBMKGNIHPFO, maxRecursion)
	{
	}

	protected override void FGAICKPNBIH(IObjectDescriptor value, IObjectGraphVisitor NKECMANOOEM, int KDONPJHEEBI)
	{
		if (!value.get_Type().HNJLMKINHHM() && !MGFPEIHBMLD.NGJMKPBNGPP().Any((IYamlTypeConverter ILHDJDNPFKH) => ILHDJDNPFKH.EFIEKANJAEC(value.get_Type())))
		{
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Type '{0}' cannot be deserialized because it does not have a default constructor or a type converter.", value.get_Type()));
		}
		base.FGAICKPNBIH(value, NKECMANOOEM, KDONPJHEEBI);
	}
}
