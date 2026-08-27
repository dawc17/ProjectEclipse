using System;

public sealed class LambdaObjectFactory : IObjectFactory
{
	private readonly Func<Type, object> BPIELBMAJHK;

	public LambdaObjectFactory(Func<Type, object> DJFCIPIMOBC)
	{
		if (DJFCIPIMOBC == null)
		{
			throw new ArgumentNullException("factory");
		}
		BPIELBMAJHK = DJFCIPIMOBC;
	}

	public object Create(Type LFLGCDNKNJI)
	{
		return BPIELBMAJHK(LFLGCDNKNJI);
	}
}
