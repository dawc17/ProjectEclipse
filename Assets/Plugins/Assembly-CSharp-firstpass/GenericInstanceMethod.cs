using System;
using System.Linq.Expressions;
using System.Reflection;

public sealed class GenericInstanceMethod<TInstance>
{
	private readonly MethodInfo DLMNFPDKIEI;

	public GenericInstanceMethod(Expression<Action<TInstance>> BOPGDKGIGHM)
	{
		MethodCallExpression methodCallExpression = (MethodCallExpression)BOPGDKGIGHM.Body;
		DLMNFPDKIEI = methodCallExpression.Method.GetGenericMethodDefinition();
	}

	public object Invoke(Type[] GIAFINCFDLC, TInstance instance, params object[] arguments)
	{
		try
		{
			return DLMNFPDKIEI.MakeGenericMethod(GIAFINCFDLC).Invoke(instance, arguments);
		}
		catch (TargetInvocationException mPFFFAOGBJE)
		{
			throw mPFFFAOGBJE.Unwrap();
		}
	}
}
