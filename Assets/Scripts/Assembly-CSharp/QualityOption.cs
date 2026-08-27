using System.Collections.Generic;
using System.Xml;

public class QualityOption
{
	public enum PJGLFCCEFIL
	{
		OPTION_NONE = 0,
		OPTION_REDUCE_FPS = 1,
		OPTION_PARTICLES_OFF = 2,
		OPTION_SRQUENCES_OFF = 3
	}

	public enum HPNJCDGIHLI
	{
		QUALITY_LOW = 0,
		QUALITY_MEDIUM = 1,
		QUALITY_HIGH = 2,
		QUALITY_NONE = 3
	}

	private PJGLFCCEFIL _type;

	private List<string> _conditions = new List<string>();

	public QualityOption(XmlNode node)
	{
		_type = MHIPNMFFBJK(node.Attributes["Name"].CIPOICEEIBK(string.Empty));
		foreach (XmlNode childNode in node.ChildNodes)
		{
			_conditions.Add(childNode.Attributes["Name"].CIPOICEEIBK(string.Empty));
		}
	}

	public static PJGLFCCEFIL MHIPNMFFBJK(string name)
	{
		switch (name)
		{
		case "ReduceFPS":
			return PJGLFCCEFIL.OPTION_REDUCE_FPS;
		case "ParticlesOff":
			return PJGLFCCEFIL.OPTION_PARTICLES_OFF;
		case "SequencesOff":
			return PJGLFCCEFIL.OPTION_SRQUENCES_OFF;
		default:
			LLLOJBFMONN.Error("QualityOption::getOptionFromString - unknown type: %s", name);
			return PJGLFCCEFIL.OPTION_NONE;
		}
	}

	public static HPNJCDGIHLI ONPFEBDGLFO(string name)
	{
		switch (name)
		{
		case "LOW":
			return HPNJCDGIHLI.QUALITY_LOW;
		case "MEDIUM":
			return HPNJCDGIHLI.QUALITY_MEDIUM;
		case "HIGH":
			return HPNJCDGIHLI.QUALITY_HIGH;
		default:
			return HPNJCDGIHLI.QUALITY_NONE;
		}
	}

	public static string GetNextQualityCondition(string HEPNIDFNHBA, string FPIDIHLACAM)
	{
		string text = "LOW";
		switch (ONPFEBDGLFO(FPIDIHLACAM))
		{
		case HPNJCDGIHLI.QUALITY_HIGH:
		case HPNJCDGIHLI.QUALITY_NONE:
			return KNBGOLLPFFI(HEPNIDFNHBA);
		case HPNJCDGIHLI.QUALITY_MEDIUM:
			return LNHKDBPLPAN(HEPNIDFNHBA);
		default:
			return "LOW";
		}
	}

	public static bool CompareQualityCondition(string MKICABFAHFA, string JMLKHIPBCLI)
	{
		return ONPFEBDGLFO(MKICABFAHFA) > ONPFEBDGLFO(JMLKHIPBCLI);
	}

	public void IOHJMJKLIOD()
	{
		string text = GraphicsController.PMAODLMLDLK();
		foreach (string item in _conditions)
		{
			if (item == text)
			{
				ACKHHGECBAE();
				break;
			}
		}
	}

	private static string KNBGOLLPFFI(string HEPNIDFNHBA)
	{
		switch (HEPNIDFNHBA)
		{
		case "LOW":
			return "MEDIUM";
		case "MEDIUM":
			return "HIGH";
		case "HIGH":
			return "LOW";
		default:
			return "HIGH";
		}
	}

	private static string LNHKDBPLPAN(string HEPNIDFNHBA)
	{
		if (HEPNIDFNHBA == "LOW")
		{
			return "MEDIUM";
		}
		if (HEPNIDFNHBA == "MEDIUM")
		{
			return "LOW";
		}
		return "MEDIUM";
	}

	private void ACKHHGECBAE()
	{
		switch (_type)
		{
		case PJGLFCCEFIL.OPTION_REDUCE_FPS:
			break;
		case PJGLFCCEFIL.OPTION_PARTICLES_OFF:
			GameUtils.LEEIGNICAMN = true;
			break;
		case PJGLFCCEFIL.OPTION_SRQUENCES_OFF:
			GameUtils.GBCMHICHIOI = true;
			break;
		default:
			LLLOJBFMONN.Error("QualityOption::turnOption - unknown type: %s", _type);
			break;
		}
	}
}
