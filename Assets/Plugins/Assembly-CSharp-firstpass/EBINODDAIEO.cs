using System;
using System.Collections;

public sealed class EBINODDAIEO : INodeDeserializer
{
	private readonly IObjectFactory IEBGHNHEOBB;

	public EBINODDAIEO(IObjectFactory EJPHFDCKCCE)
	{
		IEBGHNHEOBB = EJPHFDCKCCE;
	}

	bool INodeDeserializer.Deserialize(EventReader reader, Type MBLGNMBFHBI, Func<EventReader, Type, object> IJBAEAEDMCC, out object value)
	{
		if (!typeof(IDictionary).IsAssignableFrom(MBLGNMBFHBI))
		{
			value = false;
			return false;
		}
		reader.DODGGCGJJLL<MappingStart>();
		IDictionary dictionary = (IDictionary)IEBGHNHEOBB.Create(MBLGNMBFHBI);
		while (!reader.GPHIFFOGOGN<BLFPJCPALDH>())
		{
			object KGBGENDIMBC = IJBAEAEDMCC(reader, typeof(object));
			IValuePromise aGAMFLELGLG = KGBGENDIMBC as IValuePromise;
			object EJMKBJGNOOB = IJBAEAEDMCC(reader, typeof(object));
			IValuePromise aGAMFLELGLG2 = EJMKBJGNOOB as IValuePromise;
			if (aGAMFLELGLG == null)
			{
				if (aGAMFLELGLG2 == null)
				{
					dictionary.Add(KGBGENDIMBC, EJMKBJGNOOB);
					continue;
				}
				aGAMFLELGLG2.add_ValueAvailable((object AFIEJABPAKA) =>
				{
					dictionary.Add(KGBGENDIMBC, AFIEJABPAKA);
				});
				continue;
			}
			if (aGAMFLELGLG2 == null)
			{
				aGAMFLELGLG.add_ValueAvailable((object AFIEJABPAKA) =>
				{
					dictionary.Add(AFIEJABPAKA, EJMKBJGNOOB);
				});
				continue;
			}
			bool hasFirstPart = false;
			aGAMFLELGLG.add_ValueAvailable((object AFIEJABPAKA) =>
			{
				if (hasFirstPart)
				{
					dictionary.Add(AFIEJABPAKA, EJMKBJGNOOB);
				}
				else
				{
					KGBGENDIMBC = AFIEJABPAKA;
					hasFirstPart = true;
				}
			});
			aGAMFLELGLG2.add_ValueAvailable((object AFIEJABPAKA) =>
			{
				if (hasFirstPart)
				{
					dictionary.Add(KGBGENDIMBC, AFIEJABPAKA);
				}
				else
				{
					EJMKBJGNOOB = AFIEJABPAKA;
					hasFirstPart = true;
				}
			});
		}
		value = dictionary;
		reader.DODGGCGJJLL<BLFPJCPALDH>();
		return true;
	}
}
