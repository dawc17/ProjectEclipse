using System;
using System.Collections.Generic;

public sealed class DefaultObjectFactory : IObjectFactory
{
	private static readonly Dictionary<Type, Type> HLNILELMELH = new Dictionary<Type, Type>
	{
		{
			typeof(IEnumerable<>),
			typeof(List<>)
		},
		{
			typeof(ICollection<>),
			typeof(List<>)
		},
		{
			typeof(IList<>),
			typeof(List<>)
		},
		{
			typeof(IDictionary<, >),
			typeof(Dictionary<, >)
		}
	};

	public object Create(Type LFLGCDNKNJI)
	{
		Type value;
		if (LFLGCDNKNJI.EDALBNGKHAD() && HLNILELMELH.TryGetValue(LFLGCDNKNJI.GetGenericTypeDefinition(), out value))
		{
			LFLGCDNKNJI = value.MakeGenericType(LFLGCDNKNJI.GetGenericArguments());
		}
		try
		{
			return Activator.CreateInstance(LFLGCDNKNJI);
		}
		catch (Exception innerException)
		{
			string message = string.Format("Failed to create an instance of type '{0}'.", LFLGCDNKNJI);
			throw new InvalidOperationException(message, innerException);
		}
	}
}
