using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

public abstract class TypeInspectorSkeleton : ITypeInspector
{
	public abstract IEnumerable<IPropertyDescriptor> GHIBHNJKIHN(Type LFLGCDNKNJI, object EGJHGBCEPHO);

	public IPropertyDescriptor DBLHKMEGOEK(Type LFLGCDNKNJI, object EGJHGBCEPHO, string name, bool GNFDAJLHBCN)
	{
		IEnumerable<IPropertyDescriptor> enumerable = from PIIEECCHMAC in GHIBHNJKIHN(LFLGCDNKNJI, EGJHGBCEPHO)
			where PIIEECCHMAC.get_Name() == name
			select PIIEECCHMAC;
		using (IEnumerator<IPropertyDescriptor> enumerator = enumerable.GetEnumerator())
		{
			if (!enumerator.MoveNext())
			{
				if (GNFDAJLHBCN)
				{
					return null;
				}
				throw new SerializationException(string.Format(CultureInfo.InvariantCulture, "Property '{0}' not found on type '{1}'.", name, LFLGCDNKNJI.FullName));
			}
			IPropertyDescriptor current = enumerator.Current;
			if (enumerator.MoveNext())
			{
				throw new SerializationException(string.Format(CultureInfo.InvariantCulture, "Multiple properties with the name/alias '{0}' already exists on type '{1}', maybe you're misusing YamlAlias or maybe you are using the wrong naming convention? The matching properties are: {2}", name, LFLGCDNKNJI.FullName, string.Join(", ", enumerable.Select((IPropertyDescriptor PIIEECCHMAC) => PIIEECCHMAC.get_Name()).ToArray())));
			}
			return current;
		}
	}
}
