using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

internal static class ReflectionUtility
{
	public static Type JIDNEGBGBGL(Type LFLGCDNKNJI, Type GLBGHAEDBDC)
	{
		foreach (Type item in GetImplementedInterfaces(LFLGCDNKNJI))
		{
			if (item.DOGPNFBHJAC() && item.GetGenericTypeDefinition() == GLBGHAEDBDC)
			{
				return item;
			}
		}
		return null;
	}

	public static IEnumerable<Type> GetImplementedInterfaces(Type LFLGCDNKNJI)
	{
		if (LFLGCDNKNJI.EDALBNGKHAD())
		{
			yield return LFLGCDNKNJI;
		}
		Type[] interfaces = LFLGCDNKNJI.GetInterfaces();
		for (int i = 0; i < interfaces.Length; i++)
		{
			yield return interfaces[i];
		}
	}

	public static MethodInfo GetMethod(Expression<Action> DIMMKEJBJKN)
	{
		MethodInfo methodInfo = ((MethodCallExpression)DIMMKEJBJKN.Body).Method;
		if (methodInfo.IsGenericMethod)
		{
			methodInfo = methodInfo.GetGenericMethodDefinition();
		}
		return methodInfo;
	}

	public static MethodInfo GetMethod<T>(Expression<Action<T>> DIMMKEJBJKN)
	{
		MethodInfo methodInfo = ((MethodCallExpression)DIMMKEJBJKN.Body).Method;
		if (methodInfo.IsGenericMethod)
		{
			methodInfo = methodInfo.GetGenericMethodDefinition();
		}
		return methodInfo;
	}
}
