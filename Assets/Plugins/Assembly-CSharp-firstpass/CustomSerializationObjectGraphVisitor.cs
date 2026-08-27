using System.Collections.Generic;
using System.Linq;

public sealed class CustomSerializationObjectGraphVisitor : ChainedObjectGraphVisitor
{
	private readonly NEKGJNOFOFN NPIDIMCLNEM;

	private readonly IEnumerable<IYamlTypeConverter> DKICAFJEABL;

	public CustomSerializationObjectGraphVisitor(NEKGJNOFOFN NPIDIMCLNEM, IObjectGraphVisitor GDMFLLGPLNO, IEnumerable<IYamlTypeConverter> DKICAFJEABL)
		: base(GDMFLLGPLNO)
	{
		this.NPIDIMCLNEM = NPIDIMCLNEM;
		this.DKICAFJEABL = ((DKICAFJEABL == null) ? Enumerable.Empty<IYamlTypeConverter>() : DKICAFJEABL.ToList());
	}

	public override bool Enter(IObjectDescriptor value)
	{
		IYamlTypeConverter bLNPLLKJFLC = DKICAFJEABL.FirstOrDefault((IYamlTypeConverter GNAONAPDDLD) => GNAONAPDDLD.EFIEKANJAEC(value.get_Type()));
		if (bLNPLLKJFLC != null)
		{
			bLNPLLKJFLC.WriteYaml(NPIDIMCLNEM, value.OEAKCOHMIHH(), value.get_Type());
			return false;
		}
		IYamlSerializable mFKFJGLKJDL = value as IYamlSerializable;
		if (mFKFJGLKJDL != null)
		{
			mFKFJGLKJDL.WriteYaml(NPIDIMCLNEM);
			return false;
		}
		return base.Enter(value);
	}
}
