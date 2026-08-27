using System;
using System.Collections.Generic;

public sealed class ArrayNodeDeserializer : INodeDeserializer
{
	private static readonly GenericStaticMethod ICACCOKCLGO = new GenericStaticMethod(() => LJAJEBCNABG<object>(null, null, null));

	bool INodeDeserializer.Deserialize(EventReader reader, Type MBLGNMBFHBI, Func<EventReader, Type, object> IJBAEAEDMCC, out object value)
	{
		if (!MBLGNMBFHBI.IsArray)
		{
			value = false;
			return false;
		}
		value = ICACCOKCLGO.Invoke(new Type[1] { MBLGNMBFHBI.GetElementType() }, reader, MBLGNMBFHBI, IJBAEAEDMCC);
		return true;
	}

	private static TItem[] LJAJEBCNABG<TItem>(EventReader reader, Type MBLGNMBFHBI, Func<EventReader, Type, object> IJBAEAEDMCC)
	{
		List<TItem> list = new List<TItem>();
		GenericCollectionNodeDeserializer.LJAJEBCNABG(reader, MBLGNMBFHBI, IJBAEAEDMCC, list);
		return list.ToArray();
	}
}
