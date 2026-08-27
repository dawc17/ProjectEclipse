using System;
using System.Collections.Generic;
using System.Linq;

public sealed class NamingConventionTypeInspector : TypeInspectorSkeleton
{
	private readonly ITypeInspector CECGLIIIJJH;

	private readonly INamingConvention LELOAKPLJEH;

	public NamingConventionTypeInspector(ITypeInspector CECGLIIIJJH, INamingConvention LELOAKPLJEH)
	{
		if (CECGLIIIJJH == null)
		{
			throw new ArgumentNullException("innerTypeDescriptor");
		}
		this.CECGLIIIJJH = CECGLIIIJJH;
		if (LELOAKPLJEH == null)
		{
			throw new ArgumentNullException("namingConvention");
		}
		this.LELOAKPLJEH = LELOAKPLJEH;
	}

	public override IEnumerable<IPropertyDescriptor> GHIBHNJKIHN(Type LFLGCDNKNJI, object EGJHGBCEPHO)
	{
		return CECGLIIIJJH.GHIBHNJKIHN(LFLGCDNKNJI, EGJHGBCEPHO).Select((Func<IPropertyDescriptor, IPropertyDescriptor>)((IPropertyDescriptor PIIEECCHMAC) =>
		{
			PropertyDescriptor fLAHDIEMBAL = new PropertyDescriptor(PIIEECCHMAC);
			fLAHDIEMBAL.set_Name(LELOAKPLJEH.CBNOIMMJDGO(PIIEECCHMAC.get_Name()));
			return fLAHDIEMBAL;
		}));
	}
}
