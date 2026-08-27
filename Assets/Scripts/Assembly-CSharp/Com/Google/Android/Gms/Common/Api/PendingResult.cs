using System;
using Google.Developers;

namespace Com.Google.Android.Gms.Common.Api
{
	public class PendingResult<R> : JavaObjWrapper where R : Result
	{
		private const string CLASS_NAME = "com/google/android/gms/common/api/PendingResult";

		public PendingResult(IntPtr ptr)
			: base(ptr)
		{
		}

		public PendingResult()
			: base("com.google.android.gms.common.api.PendingResult")
		{
		}

		public R await(long arg_long_1, object arg_object_2)
		{
			return InvokeCall<R>("await", "(JLjava/util/concurrent/TimeUnit;)Lcom/google/android/gms/common/api/Result;", new object[2] { arg_long_1, arg_object_2 });
		}

		public R await()
		{
			return InvokeCall<R>("await", "()Lcom/google/android/gms/common/api/Result;", new object[0]);
		}

		public bool isCanceled()
		{
			return InvokeCall<bool>("isCanceled", "()Z", new object[0]);
		}

		public void cancel()
		{
			InvokeCallVoid("cancel", "()V");
		}

		public void setResultCallback(ResultCallback<R> arg_ResultCallback_1)
		{
			InvokeCallVoid("setResultCallback", "(Lcom/google/android/gms/common/api/ResultCallback;)V", arg_ResultCallback_1);
		}

		public void setResultCallback(ResultCallback<R> arg_ResultCallback_1, long arg_long_2, object arg_object_3)
		{
			InvokeCallVoid("setResultCallback", "(Lcom/google/android/gms/common/api/ResultCallback;JLjava/util/concurrent/TimeUnit;)V", arg_ResultCallback_1, arg_long_2, arg_object_3);
		}
	}
}
