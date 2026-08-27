using System;

public class GuidConverter : IYamlTypeConverter
{
	public bool EFIEKANJAEC(Type LFLGCDNKNJI)
	{
		return LFLGCDNKNJI == typeof(Guid);
	}

	public object ReadYaml(IParser BPGMNGAJMKK, Type LFLGCDNKNJI)
	{
		string g = ((Scalar)BPGMNGAJMKK.AOJJOEHEPGM()).OEAKCOHMIHH();
		BPGMNGAJMKK.PCCMLADDNDG();
		return new Guid(g);
	}

	public void WriteYaml(NEKGJNOFOFN NPIDIMCLNEM, object value, Type LFLGCDNKNJI)
	{
		NPIDIMCLNEM.Emit(new Scalar(((Guid)value).ToString("D")));
	}
}
