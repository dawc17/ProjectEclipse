using System;
using Google.Developers;

namespace Com.Google.Android.Gms.Common.Api
{
	public class Status : JavaObjWrapper, Result
	{
		private const string CLASS_NAME = "com/google/android/gms/common/api/Status";

		public static object CREATOR
		{
			get
			{
				return JavaObjWrapper.GetStaticObjectField<object>("com/google/android/gms/common/api/Status", "CREATOR", "Landroid/os/Parcelable$Creator;");
			}
		}

		public static string NULL
		{
			get
			{
				return JavaObjWrapper.GetStaticStringField("com/google/android/gms/common/api/Status", "NULL");
			}
		}

		public static int CONTENTS_FILE_DESCRIPTOR
		{
			get
			{
				return JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/api/Status", "CONTENTS_FILE_DESCRIPTOR");
			}
		}

		public static int PARCELABLE_WRITE_RETURN_VALUE
		{
			get
			{
				return JavaObjWrapper.GetStaticIntField("com/google/android/gms/common/api/Status", "PARCELABLE_WRITE_RETURN_VALUE");
			}
		}

		public Status(IntPtr ptr)
			: base(ptr)
		{
		}

		public Status(int arg_int_1, string arg_string_2, object arg_object_3)
		{
			CreateInstance("com/google/android/gms/common/api/Status", arg_int_1, arg_string_2, arg_object_3);
		}

		public Status(int arg_int_1, string arg_string_2)
		{
			CreateInstance("com/google/android/gms/common/api/Status", arg_int_1, arg_string_2);
		}

		public Status(int arg_int_1)
		{
			CreateInstance("com/google/android/gms/common/api/Status", arg_int_1);
		}

		public bool equals(object arg_object_1)
		{
			return InvokeCall<bool>("equals", "(Ljava/lang/Object;)Z", new object[1] { arg_object_1 });
		}

		public string toString()
		{
			return InvokeCall<string>("toString", "()Ljava/lang/String;", new object[0]);
		}

		public int hashCode()
		{
			return InvokeCall<int>("hashCode", "()I", new object[0]);
		}

		public bool isInterrupted()
		{
			return InvokeCall<bool>("isInterrupted", "()Z", new object[0]);
		}

		public Status getStatus()
		{
			return InvokeCall<Status>("getStatus", "()Lcom/google/android/gms/common/api/Status;", new object[0]);
		}

		public bool isCanceled()
		{
			return InvokeCall<bool>("isCanceled", "()Z", new object[0]);
		}

		public int describeContents()
		{
			return InvokeCall<int>("describeContents", "()I", new object[0]);
		}

		public object getResolution()
		{
			return InvokeCall<object>("getResolution", "()Landroid/app/PendingIntent;", new object[0]);
		}

		public int getStatusCode()
		{
			return InvokeCall<int>("getStatusCode", "()I", new object[0]);
		}

		public string getStatusMessage()
		{
			return InvokeCall<string>("getStatusMessage", "()Ljava/lang/String;", new object[0]);
		}

		public bool hasResolution()
		{
			return InvokeCall<bool>("hasResolution", "()Z", new object[0]);
		}

		public void startResolutionForResult(object arg_object_1, int arg_int_2)
		{
			InvokeCallVoid("startResolutionForResult", "(Landroid/app/Activity;I)V", arg_object_1, arg_int_2);
		}

		public void writeToParcel(object arg_object_1, int arg_int_2)
		{
			InvokeCallVoid("writeToParcel", "(Landroid/os/Parcel;I)V", arg_object_1, arg_int_2);
		}

		public bool isSuccess()
		{
			return InvokeCall<bool>("isSuccess", "()Z", new object[0]);
		}
	}
}
