using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Google.Developers;
using UnityEngine;

namespace Com.Google.Android.Gms.Common.Api
{
	public abstract class ResultCallbackProxy<R> : JavaInterfaceProxy, ResultCallback<R> where R : Result
	{
		private const string CLASS_NAME = "com/google/android/gms/common/api/ResultCallback";

		public ResultCallbackProxy()
			: base("com/google/android/gms/common/api/ResultCallback")
		{
		}

		public abstract void OnResult(R arg_Result_1);

		public void onResult(R arg_Result_1)
		{
			OnResult(arg_Result_1);
		}

		public void onResult(AndroidJavaObject arg_Result_1)
		{
			IntPtr rawObject = arg_Result_1.GetRawObject();
			ConstructorInfo constructor = typeof(R).GetConstructor(new Type[1] { rawObject.GetType() });
			R val;
			if (constructor != null)
			{
				val = (R)constructor.Invoke(new object[1] { rawObject });
			}
			else
			{
				ConstructorInfo constructor2 = typeof(R).GetConstructor(new Type[0]);
				val = (R)constructor2.Invoke(new object[0]);
				Marshal.PtrToStructure(rawObject, val);
			}
			OnResult(val);
		}
	}
}
