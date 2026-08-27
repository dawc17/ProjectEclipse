using System;
using System.Linq.Expressions;
using System.Reflection;

public sealed class GenericStaticMethod
{
	private readonly MethodInfo DLMNFPDKIEI;

	public GenericStaticMethod(Expression<Action> BOPGDKGIGHM)
	{
		MethodCallExpression methodCallExpression = (MethodCallExpression)BOPGDKGIGHM.Body;
		DLMNFPDKIEI = methodCallExpression.Method.GetGenericMethodDefinition();
	}

	public object Invoke(Type[] GIAFINCFDLC, params object[] arguments)
	{
		try
		{
			return DLMNFPDKIEI.MakeGenericMethod(GIAFINCFDLC).Invoke(null, arguments);
		}
		catch (TargetInvocationException mPFFFAOGBJE)
		{
			throw mPFFFAOGBJE.Unwrap();
		}
	}
}
