using System;
using System.Collections;
using System.Collections.Generic;

public sealed class EnumerableNodeDeserializer : INodeDeserializer
{
	bool INodeDeserializer.Deserialize(EventReader reader, Type MBLGNMBFHBI, Func<EventReader, Type, object> IJBAEAEDMCC, out object value)
	{
		Type type;
		if (MBLGNMBFHBI == typeof(IEnumerable))
		{
			type = typeof(object);
		}
		else
		{
			Type type2 = ReflectionUtility.JIDNEGBGBGL(MBLGNMBFHBI, typeof(IEnumerable<>));
			if (type2 != MBLGNMBFHBI)
			{
				value = null;
				return false;
			}
			type = type2.GetGenericArguments()[0];
		}
		Type arg = typeof(List<>).MakeGenericType(type);
		value = IJBAEAEDMCC(reader, arg);
		return true;
	}
}
