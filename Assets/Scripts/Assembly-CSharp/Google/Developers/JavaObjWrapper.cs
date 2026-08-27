using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Google.Developers
{
	public class JavaObjWrapper
	{
		private IntPtr raw;

		private IntPtr cachedRawClass = IntPtr.Zero;

		public IntPtr RawObject
		{
			get
			{
				return raw;
			}
		}

		public virtual IntPtr RawClass
		{
			get
			{
				if (cachedRawClass == IntPtr.Zero && raw != IntPtr.Zero)
				{
					cachedRawClass = AndroidJNI.GetObjectClass(raw);
				}
				return cachedRawClass;
			}
		}

		protected JavaObjWrapper()
		{
		}

		public JavaObjWrapper(string clazzName)
		{
			raw = AndroidJNI.AllocObject(AndroidJNI.FindClass(clazzName));
		}

		public JavaObjWrapper(IntPtr rawObject)
		{
			raw = rawObject;
		}

		public void CreateInstance(string clazzName, params object[] args)
		{
			if (raw != IntPtr.Zero)
			{
				throw new Exception("Java object already set");
			}
			IntPtr constructorID = AndroidJNIHelper.GetConstructorID(RawClass, args);
			jvalue[] args2 = ConstructArgArray(args);
			raw = AndroidJNI.NewObject(RawClass, constructorID, args2);
		}

		protected static jvalue[] ConstructArgArray(object[] theArgs)
		{
			object[] array = new object[theArgs.Length];
			for (int i = 0; i < theArgs.Length; i++)
			{
				if (theArgs[i] is JavaObjWrapper)
				{
					array[i] = ((JavaObjWrapper)theArgs[i]).raw;
				}
				else
				{
					array[i] = theArgs[i];
				}
			}
			jvalue[] array2 = AndroidJNIHelper.CreateJNIArgArray(array);
			for (int j = 0; j < theArgs.Length; j++)
			{
				if (theArgs[j] is JavaObjWrapper)
				{
					array2[j].l = ((JavaObjWrapper)theArgs[j]).raw;
				}
				else if (theArgs[j] is JavaInterfaceProxy)
				{
					IntPtr l = AndroidJNIHelper.CreateJavaProxy((AndroidJavaProxy)theArgs[j]);
					array2[j].l = l;
				}
			}
			if (array2.Length == 1)
			{
				for (int k = 0; k < array2.Length; k++)
				{
					Debug.Log("---- [" + k + "] -- " + array2[k].l);
				}
			}
			return array2;
		}

		public static T StaticInvokeObjectCall<T>(string type, string name, string sig, params object[] args)
		{
			IntPtr clazz = AndroidJNI.FindClass(type);
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, name, sig);
			jvalue[] args2 = ConstructArgArray(args);
			IntPtr intPtr = AndroidJNI.CallStaticObjectMethod(clazz, staticMethodID, args2);
			ConstructorInfo constructor = typeof(T).GetConstructor(new Type[1] { intPtr.GetType() });
			if (constructor != null)
			{
				return (T)constructor.Invoke(new object[1] { intPtr });
			}
			if (typeof(T).IsArray)
			{
				return AndroidJNIHelper.ConvertFromJNIArray<T>(intPtr);
			}
			Debug.Log("Trying cast....");
			Type typeFromHandle = typeof(T);
			return (T)Marshal.PtrToStructure(intPtr, typeFromHandle);
		}

		public static void StaticInvokeCallVoid(string type, string name, string sig, params object[] args)
		{
			IntPtr clazz = AndroidJNI.FindClass(type);
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, name, sig);
			jvalue[] args2 = ConstructArgArray(args);
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, args2);
		}

		public static T GetStaticObjectField<T>(string clsName, string name, string sig)
		{
			IntPtr clazz = AndroidJNI.FindClass(clsName);
			IntPtr staticFieldID = AndroidJNI.GetStaticFieldID(clazz, name, sig);
			IntPtr staticObjectField = AndroidJNI.GetStaticObjectField(clazz, staticFieldID);
			ConstructorInfo constructor = typeof(T).GetConstructor(new Type[1] { staticObjectField.GetType() });
			if (constructor != null)
			{
				return (T)constructor.Invoke(new object[1] { staticObjectField });
			}
			Type typeFromHandle = typeof(T);
			return (T)Marshal.PtrToStructure(staticObjectField, typeFromHandle);
		}

		public static int GetStaticIntField(string clsName, string name)
		{
			IntPtr clazz = AndroidJNI.FindClass(clsName);
			IntPtr staticFieldID = AndroidJNI.GetStaticFieldID(clazz, name, "I");
			return AndroidJNI.GetStaticIntField(clazz, staticFieldID);
		}

		public static string GetStaticStringField(string clsName, string name)
		{
			IntPtr clazz = AndroidJNI.FindClass(clsName);
			IntPtr staticFieldID = AndroidJNI.GetStaticFieldID(clazz, name, "Ljava/lang/String;");
			return AndroidJNI.GetStaticStringField(clazz, staticFieldID);
		}

		public static float GetStaticFloatField(string clsName, string name)
		{
			IntPtr clazz = AndroidJNI.FindClass(clsName);
			IntPtr staticFieldID = AndroidJNI.GetStaticFieldID(clazz, name, "F");
			return AndroidJNI.GetStaticFloatField(clazz, staticFieldID);
		}

		public void InvokeCallVoid(string name, string sig, params object[] args)
		{
			IntPtr methodID = AndroidJNI.GetMethodID(RawClass, name, sig);
			jvalue[] args2 = ConstructArgArray(args);
			AndroidJNI.CallVoidMethod(raw, methodID, args2);
		}

		public T InvokeCall<T>(string name, string sig, params object[] args)
		{
			Type typeFromHandle = typeof(T);
			IntPtr methodID = AndroidJNI.GetMethodID(RawClass, name, sig);
			jvalue[] args2 = ConstructArgArray(args);
			if (methodID == IntPtr.Zero)
			{
				Debug.LogError("Cannot get method for " + name);
				throw new Exception("Cannot get method for " + name);
			}
			if (typeFromHandle == typeof(bool))
			{
				return (T)(object)AndroidJNI.CallBooleanMethod(raw, methodID, args2);
			}
			if (typeFromHandle == typeof(string))
			{
				return (T)(object)AndroidJNI.CallStringMethod(raw, methodID, args2);
			}
			if (typeFromHandle == typeof(int))
			{
				return (T)(object)AndroidJNI.CallIntMethod(raw, methodID, args2);
			}
			if (typeFromHandle == typeof(float))
			{
				return (T)(object)AndroidJNI.CallFloatMethod(raw, methodID, args2);
			}
			if (typeFromHandle == typeof(double))
			{
				return (T)(object)AndroidJNI.CallDoubleMethod(raw, methodID, args2);
			}
			if (typeFromHandle == typeof(byte))
			{
				return (T)(object)AndroidJNI.CallByteMethod(raw, methodID, args2);
			}
			if (typeFromHandle == typeof(char))
			{
				return (T)(object)AndroidJNI.CallCharMethod(raw, methodID, args2);
			}
			if (typeFromHandle == typeof(long))
			{
				return (T)(object)AndroidJNI.CallLongMethod(raw, methodID, args2);
			}
			if (typeFromHandle == typeof(short))
			{
				return (T)(object)AndroidJNI.CallShortMethod(raw, methodID, args2);
			}
			return InvokeObjectCall<T>(name, sig, args);
		}

		public static T StaticInvokeCall<T>(string type, string name, string sig, params object[] args)
		{
			Type typeFromHandle = typeof(T);
			IntPtr clazz = AndroidJNI.FindClass(type);
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, name, sig);
			jvalue[] args2 = ConstructArgArray(args);
			if (typeFromHandle == typeof(bool))
			{
				return (T)(object)AndroidJNI.CallStaticBooleanMethod(clazz, staticMethodID, args2);
			}
			if (typeFromHandle == typeof(string))
			{
				return (T)(object)AndroidJNI.CallStaticStringMethod(clazz, staticMethodID, args2);
			}
			if (typeFromHandle == typeof(int))
			{
				return (T)(object)AndroidJNI.CallStaticIntMethod(clazz, staticMethodID, args2);
			}
			if (typeFromHandle == typeof(float))
			{
				return (T)(object)AndroidJNI.CallStaticFloatMethod(clazz, staticMethodID, args2);
			}
			if (typeFromHandle == typeof(double))
			{
				return (T)(object)AndroidJNI.CallStaticDoubleMethod(clazz, staticMethodID, args2);
			}
			if (typeFromHandle == typeof(byte))
			{
				return (T)(object)AndroidJNI.CallStaticByteMethod(clazz, staticMethodID, args2);
			}
			if (typeFromHandle == typeof(char))
			{
				return (T)(object)AndroidJNI.CallStaticCharMethod(clazz, staticMethodID, args2);
			}
			if (typeFromHandle == typeof(long))
			{
				return (T)(object)AndroidJNI.CallStaticLongMethod(clazz, staticMethodID, args2);
			}
			if (typeFromHandle == typeof(short))
			{
				return (T)(object)AndroidJNI.CallStaticShortMethod(clazz, staticMethodID, args2);
			}
			return StaticInvokeObjectCall<T>(type, name, sig, args);
		}

		public T InvokeObjectCall<T>(string name, string sig, params object[] theArgs)
		{
			IntPtr methodID = AndroidJNI.GetMethodID(RawClass, name, sig);
			jvalue[] args = ConstructArgArray(theArgs);
			IntPtr intPtr = AndroidJNI.CallObjectMethod(raw, methodID, args);
			if (intPtr.Equals(IntPtr.Zero))
			{
				return default(T);
			}
			ConstructorInfo constructor = typeof(T).GetConstructor(new Type[1] { intPtr.GetType() });
			if (constructor != null)
			{
				return (T)constructor.Invoke(new object[1] { intPtr });
			}
			Type typeFromHandle = typeof(T);
			return (T)Marshal.PtrToStructure(intPtr, typeFromHandle);
		}
	}
}
