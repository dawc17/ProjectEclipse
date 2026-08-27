using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace GooglePlayGames.Native.PInvoke
{
	internal static class PInvokeUtilities
	{
		internal delegate UIntPtr OutStringMethod([In][Out] byte[] out_bytes, UIntPtr out_size);

		internal delegate UIntPtr OutMethod<T>([In][Out] T[] out_bytes, UIntPtr out_size);

		private static readonly DateTime UnixEpoch = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);

		internal static HandleRef CheckNonNull(HandleRef reference)
		{
			if (IsNull(reference))
			{
				throw new InvalidOperationException();
			}
			return reference;
		}

		internal static bool IsNull(HandleRef reference)
		{
			return IsNull(HandleRef.ToIntPtr(reference));
		}

		internal static bool IsNull(IntPtr pointer)
		{
			return pointer.Equals(IntPtr.Zero);
		}

		internal static DateTime FromMillisSinceUnixEpoch(long millisSinceEpoch)
		{
			return UnixEpoch.Add(TimeSpan.FromMilliseconds(millisSinceEpoch));
		}

		internal static string OutParamsToString(OutStringMethod outStringMethod)
		{
			UIntPtr out_size = outStringMethod(null, UIntPtr.Zero);
			if (out_size.Equals(UIntPtr.Zero))
			{
				return null;
			}
			string text = null;
			try
			{
				byte[] array = new byte[out_size.ToUInt32()];
				outStringMethod(array, out_size);
				return Encoding.UTF8.GetString(array, 0, (int)(out_size.ToUInt32() - 1));
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception creating string from char array: " + ex);
				return string.Empty;
			}
		}

		internal static T[] OutParamsToArray<T>(OutMethod<T> outMethod)
		{
			UIntPtr out_size = outMethod(null, UIntPtr.Zero);
			if (out_size.Equals(UIntPtr.Zero))
			{
				return new T[0];
			}
			T[] array = new T[out_size.ToUInt64()];
			outMethod(array, out_size);
			return array;
		}

		internal static IEnumerable<T> ToEnumerable<T>(UIntPtr size, Func<UIntPtr, T> getElement)
		{
			for (ulong i = 0uL; i < size.ToUInt64(); i++)
			{
				yield return getElement(new UIntPtr(i));
			}
		}

		internal static IEnumerator<T> ToEnumerator<T>(UIntPtr size, Func<UIntPtr, T> getElement)
		{
			return ToEnumerable(size, getElement).GetEnumerator();
		}

		internal static UIntPtr ArrayToSizeT<T>(T[] array)
		{
			if (array == null)
			{
				return UIntPtr.Zero;
			}
			return new UIntPtr((ulong)array.Length);
		}

		internal static long ToMilliseconds(TimeSpan span)
		{
			double totalMilliseconds = span.TotalMilliseconds;
			if (totalMilliseconds > 9.223372036854776E+18)
			{
				return long.MaxValue;
			}
			if (totalMilliseconds < -9.223372036854776E+18)
			{
				return long.MinValue;
			}
			return Convert.ToInt64(totalMilliseconds);
		}
	}
}
