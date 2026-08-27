using System;
using System.Collections.Generic;
using System.Linq;

public sealed class ReadableAndWritablePropertiesTypeInspector : TypeInspectorSkeleton
{
	private readonly ITypeInspector DIGADLIMNPD;

	public ReadableAndWritablePropertiesTypeInspector(ITypeInspector CECGLIIIJJH)
	{
		DIGADLIMNPD = CECGLIIIJJH;
	}

	public override IEnumerable<IPropertyDescriptor> GHIBHNJKIHN(Type LFLGCDNKNJI, object EGJHGBCEPHO)
	{
		return from PIIEECCHMAC in DIGADLIMNPD.GHIBHNJKIHN(LFLGCDNKNJI, EGJHGBCEPHO)
			where PIIEECCHMAC.HHHGHBBDMHC()
			select PIIEECCHMAC;
	}
}
