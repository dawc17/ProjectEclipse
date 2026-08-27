using System.Xml;

public class DeviceComparison : ComparisonExpression
{
	public enum DPBGDMLNHGA
	{
		PARAMETER_NONE = 0,
		MEMORY_TOTAL = 1,
		MEMORY_FREE = 2,
		CORES_COUNT = 3
	}

	private DPBGDMLNHGA OCDIMEJGIGP;

	public DeviceComparison(XmlNode node)
		: base(node)
	{
		if (node.Attributes != null)
		{
			OCDIMEJGIGP = GMEPNMHBPDN((node.Attributes["Value"] != null) ? node.Attributes["Value"].Value : null);
			DIKPCBMONEH = float.Parse(node.Attributes["Than"].Value);
		}
		JBMIOBKHDHK();
	}

	public static DPBGDMLNHGA GMEPNMHBPDN(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			AdvLog.CCOFFJPPAKC("DeviceComparison::GetParameterFromString - empty parameter name");
			return DPBGDMLNHGA.PARAMETER_NONE;
		}
		switch (name)
		{
		case "_DeviceTotalMem":
			return DPBGDMLNHGA.MEMORY_TOTAL;
		case "_DeviceFreeMem":
			return DPBGDMLNHGA.MEMORY_FREE;
		case "_DeviceCoresNum":
			return DPBGDMLNHGA.CORES_COUNT;
		default:
			AdvLog.CCOFFJPPAKC(string.Format("DeviceComparison::GetParameterFromString - unknown type: {0}", name));
			return DPBGDMLNHGA.PARAMETER_NONE;
		}
	}

	public void JBMIOBKHDHK()
	{
		switch (OCDIMEJGIGP)
		{
		case DPBGDMLNHGA.MEMORY_TOTAL:
			MAFCNMOAIDA = SystemProperties.NICPICAMAOH().AOJLHDILEBJ / 1024;
			break;
		case DPBGDMLNHGA.MEMORY_FREE:
			MAFCNMOAIDA = SystemProperties.NICPICAMAOH().LGEEAANABHH / 1024;
			break;
		case DPBGDMLNHGA.CORES_COUNT:
			MAFCNMOAIDA = SystemProperties.NICPICAMAOH().MOMPODBNJNE;
			break;
		default:
			AdvLog.CCOFFJPPAKC(string.Format("DeviceComparison::UpdateParameter - unknown type: {0}", OCDIMEJGIGP));
			break;
		}
	}
}
