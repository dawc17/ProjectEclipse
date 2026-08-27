using System;
using System.Collections.Generic;
using System.Linq;

public sealed class TypeConverterNodeDeserializer : INodeDeserializer
{
	private readonly IEnumerable<IYamlTypeConverter> JNONHBMNKDK;

	public TypeConverterNodeDeserializer(IEnumerable<IYamlTypeConverter> JNONHBMNKDK)
	{
		if (JNONHBMNKDK == null)
		{
			throw new ArgumentNullException("converters");
		}
		this.JNONHBMNKDK = JNONHBMNKDK;
	}

	bool INodeDeserializer.Deserialize(EventReader reader, Type MBLGNMBFHBI, Func<EventReader, Type, object> IJBAEAEDMCC, out object value)
	{
		IYamlTypeConverter bLNPLLKJFLC = JNONHBMNKDK.FirstOrDefault((IYamlTypeConverter ILHDJDNPFKH) => ILHDJDNPFKH.EFIEKANJAEC(MBLGNMBFHBI));
		if (bLNPLLKJFLC == null)
		{
			value = null;
			return false;
		}
		value = bLNPLLKJFLC.ReadYaml(reader.OAPMECPBPKJ(), MBLGNMBFHBI);
		return true;
	}
}
