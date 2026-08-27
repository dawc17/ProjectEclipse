using System;
using System.Collections.Generic;
using YamlDotNet.Core;

public sealed class GenericCollectionNodeDeserializer : INodeDeserializer
{
	private readonly IObjectFactory IEBGHNHEOBB;

	private static readonly GenericStaticMethod ICACCOKCLGO = new GenericStaticMethod(() => LJAJEBCNABG<object>(null, null, null, null));

	public GenericCollectionNodeDeserializer(IObjectFactory EJPHFDCKCCE)
	{
		IEBGHNHEOBB = EJPHFDCKCCE;
	}

	bool INodeDeserializer.Deserialize(EventReader reader, Type MBLGNMBFHBI, Func<EventReader, Type, object> IJBAEAEDMCC, out object value)
	{
		Type type = ReflectionUtility.JIDNEGBGBGL(MBLGNMBFHBI, typeof(ICollection<>));
		if (type == null)
		{
			value = false;
			return false;
		}
		value = IEBGHNHEOBB.Create(MBLGNMBFHBI);
		ICACCOKCLGO.Invoke(type.GetGenericArguments(), reader, MBLGNMBFHBI, IJBAEAEDMCC, value);
		return true;
	}

	internal static void LJAJEBCNABG<TItem>(EventReader reader, Type MBLGNMBFHBI, Func<EventReader, Type, object> IJBAEAEDMCC, ICollection<TItem> DCJLKCFKCOM)
	{
		IList<TItem> GBAAEMCBDAM = DCJLKCFKCOM as IList<TItem>;
		reader.DODGGCGJJLL<JODGINIKFJF>();
		while (!reader.GPHIFFOGOGN<AKMKLAINLOL>())
		{
			ParsingEvent jMKLCDAKEOG = reader.OAPMECPBPKJ().AOJJOEHEPGM();
			object obj = IJBAEAEDMCC(reader, typeof(TItem));
			IValuePromise aGAMFLELGLG = obj as IValuePromise;
			if (aGAMFLELGLG == null)
			{
				DCJLKCFKCOM.Add(TypeConverterHelper.ChangeType<TItem>(obj));
				continue;
			}
			if (GBAAEMCBDAM != null)
			{
				int index = GBAAEMCBDAM.Count;
				DCJLKCFKCOM.Add(default(TItem));
				aGAMFLELGLG.add_ValueAvailable((object AFIEJABPAKA) =>
				{
					GBAAEMCBDAM[index] = TypeConverterHelper.ChangeType<TItem>(AFIEJABPAKA);
				});
				continue;
			}
			throw new ForwardAnchorNotSupportedException(jMKLCDAKEOG.OGPHJPFHBJL(), jMKLCDAKEOG.GDJHIJHFPHA(), "Forward alias references are not allowed because this type does not implement IList<>");
		}
		reader.DODGGCGJJLL<AKMKLAINLOL>();
	}
}
