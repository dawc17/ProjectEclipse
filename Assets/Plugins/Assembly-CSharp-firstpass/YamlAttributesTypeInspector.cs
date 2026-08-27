using System;
using System.Collections.Generic;
using System.Linq;

public sealed class YamlAttributesTypeInspector : TypeInspectorSkeleton
{
	private readonly ITypeInspector CECGLIIIJJH;

	public YamlAttributesTypeInspector(ITypeInspector CECGLIIIJJH)
	{
		this.CECGLIIIJJH = CECGLIIIJJH;
	}

	public override IEnumerable<IPropertyDescriptor> GHIBHNJKIHN(Type LFLGCDNKNJI, object EGJHGBCEPHO)
	{
		return from PIIEECCHMAC in (from PIIEECCHMAC in CECGLIIIJJH.GHIBHNJKIHN(LFLGCDNKNJI, EGJHGBCEPHO)
				where PIIEECCHMAC.PJLLHGDNCIF<MOEOPMHGKCI>() == null
				select PIIEECCHMAC).Select((Func<IPropertyDescriptor, IPropertyDescriptor>)((IPropertyDescriptor PIIEECCHMAC) =>
			{
				PropertyDescriptor fLAHDIEMBAL = new PropertyDescriptor(PIIEECCHMAC);
				YamlAliasAttribute gAANEGEKJGH = PIIEECCHMAC.PJLLHGDNCIF<YamlAliasAttribute>();
				if (gAANEGEKJGH != null)
				{
					fLAHDIEMBAL.set_Name(gAANEGEKJGH.MIDPFGENBCF());
				}
				YamlMemberAttribute kGBEBCLPIIO = PIIEECCHMAC.PJLLHGDNCIF<YamlMemberAttribute>();
				if (kGBEBCLPIIO != null)
				{
					if (kGBEBCLPIIO.FDDGCEPMIJG() != null)
					{
						fLAHDIEMBAL.set_TypeOverride(kGBEBCLPIIO.FDDGCEPMIJG());
					}
					fLAHDIEMBAL.set_Order(kGBEBCLPIIO.BHDEMLGCNOJ());
					if (kGBEBCLPIIO.MIDPFGENBCF() != null)
					{
						if (gAANEGEKJGH != null)
						{
							throw new InvalidOperationException("Mixing YamlAlias(...) with YamlMember(Alias = ...) is an error. The YamlAlias attribute is obsolete and should be removed.");
						}
						fLAHDIEMBAL.set_Name(kGBEBCLPIIO.MIDPFGENBCF());
					}
				}
				return fLAHDIEMBAL;
			}))
			orderby PIIEECCHMAC.BHDEMLGCNOJ()
			select PIIEECCHMAC;
	}
}
