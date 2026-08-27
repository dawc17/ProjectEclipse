using System;
using System.Collections;

public sealed class CNJMKCEKKNN : INodeDeserializer
{
	private readonly IObjectFactory IEBGHNHEOBB;

	public CNJMKCEKKNN(IObjectFactory EJPHFDCKCCE)
	{
		IEBGHNHEOBB = EJPHFDCKCCE;
	}

	bool INodeDeserializer.Deserialize(EventReader reader, Type MBLGNMBFHBI, Func<EventReader, Type, object> IJBAEAEDMCC, out object value)
	{
		if (!typeof(IList).IsAssignableFrom(MBLGNMBFHBI))
		{
			value = false;
			return false;
		}
		reader.DODGGCGJJLL<JODGINIKFJF>();
		IList GBAAEMCBDAM = (IList)IEBGHNHEOBB.Create(MBLGNMBFHBI);
		while (!reader.GPHIFFOGOGN<AKMKLAINLOL>())
		{
			object obj = IJBAEAEDMCC(reader, typeof(object));
			IValuePromise aGAMFLELGLG = obj as IValuePromise;
			if (aGAMFLELGLG == null)
			{
				GBAAEMCBDAM.Add(obj);
				continue;
			}
			int index = GBAAEMCBDAM.Count;
			GBAAEMCBDAM.Add(null);
			aGAMFLELGLG.add_ValueAvailable((object AFIEJABPAKA) =>
			{
				GBAAEMCBDAM[index] = AFIEJABPAKA;
			});
		}
		value = GBAAEMCBDAM;
		reader.DODGGCGJJLL<AKMKLAINLOL>();
		return true;
	}
}
