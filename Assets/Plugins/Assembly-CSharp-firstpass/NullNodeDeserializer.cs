using System;

public sealed class NullNodeDeserializer : INodeDeserializer
{
	bool INodeDeserializer.Deserialize(EventReader reader, Type MBLGNMBFHBI, Func<EventReader, Type, object> IJBAEAEDMCC, out object value)
	{
		value = null;
		NodeEvent dGMPGIHHKCN = reader.Peek<NodeEvent>();
		bool flag = dGMPGIHHKCN != null && FMNBNDEEDKK(dGMPGIHHKCN);
		if (flag)
		{
			reader.FHCPPKNIOKB();
		}
		return flag;
	}

	private bool FMNBNDEEDKK(NodeEvent ABOEBNGCALL)
	{
		if (ABOEBNGCALL.LOIGCKFONHJ() == "tag:yaml.org,2002:null")
		{
			return true;
		}
		Scalar lEACOCDHICF = ABOEBNGCALL as Scalar;
		if (lEACOCDHICF == null || lEACOCDHICF.HALCJLMJDII() != IBEOFCPMMJJ.Plain)
		{
			return false;
		}
		string text = lEACOCDHICF.OEAKCOHMIHH();
		if (text == string.Empty)
		{
			goto IL_0086;
		}
		switch (text)
		{
		case "~":
		case "null":
		case "Null":
			goto IL_0086;
		}
		int result = ((text == "NULL") ? 1 : 0);
		goto IL_0087;
		IL_0087:
		return (byte)result != 0;
		IL_0086:
		result = 1;
		goto IL_0087;
	}
}
