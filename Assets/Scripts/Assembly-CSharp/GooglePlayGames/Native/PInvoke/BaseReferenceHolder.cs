using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.PInvoke
{
	internal abstract class BaseReferenceHolder : IDisposable
	{
		private static Dictionary<HandleRef, BaseReferenceHolder> _refs = new Dictionary<HandleRef, BaseReferenceHolder>();

		private HandleRef mSelfPointer;

		public BaseReferenceHolder(IntPtr pointer)
		{
			mSelfPointer = PInvokeUtilities.CheckNonNull(new HandleRef(this, pointer));
		}

		protected bool IsDisposed()
		{
			return PInvokeUtilities.IsNull(mSelfPointer);
		}

		protected HandleRef SelfPtr()
		{
			if (IsDisposed())
			{
				throw new InvalidOperationException("Attempted to use object after it was cleaned up");
			}
			return mSelfPointer;
		}

		protected abstract void CallDispose(HandleRef selfPointer);

		~BaseReferenceHolder()
		{
			Dispose(true);
		}

		public void Dispose()
		{
			Dispose(false);
			GC.SuppressFinalize(this);
		}

		internal IntPtr AsPointer()
		{
			return SelfPtr().Handle;
		}

		private void Dispose(bool fromFinalizer)
		{
			if ((fromFinalizer || !_refs.ContainsKey(mSelfPointer)) && !PInvokeUtilities.IsNull(mSelfPointer))
			{
				CallDispose(mSelfPointer);
				mSelfPointer = new HandleRef(this, IntPtr.Zero);
			}
		}

		internal void ReferToMe()
		{
			_refs[SelfPtr()] = this;
		}

		internal void ForgetMe()
		{
			if (_refs.ContainsKey(SelfPtr()))
			{
				_refs.Remove(SelfPtr());
				Dispose(false);
			}
		}
	}
}
