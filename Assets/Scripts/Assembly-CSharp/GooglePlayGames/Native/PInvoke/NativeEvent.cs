using System;
using System.Runtime.InteropServices;
using GooglePlayGames.BasicApi.Events;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class NativeEvent : BaseReferenceHolder, IEvent
	{
		public string Id
		{
			get
			{
				return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr out_size) => Event.Event_Id(SelfPtr(), out_string, out_size));
			}
		}

		public string Name
		{
			get
			{
				return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr out_size) => Event.Event_Name(SelfPtr(), out_string, out_size));
			}
		}

		public string Description
		{
			get
			{
				return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr out_size) => Event.Event_Description(SelfPtr(), out_string, out_size));
			}
		}

		public string ImageUrl
		{
			get
			{
				return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr out_size) => Event.Event_ImageUrl(SelfPtr(), out_string, out_size));
			}
		}

		public ulong CurrentCount
		{
			get
			{
				return Event.Event_Count(SelfPtr());
			}
		}

		public EventVisibility Visibility
		{
			get
			{
				Types.EventVisibility eventVisibility = Event.Event_Visibility(SelfPtr());
				switch (eventVisibility)
				{
				case Types.EventVisibility.HIDDEN:
					return EventVisibility.Hidden;
				case Types.EventVisibility.REVEALED:
					return EventVisibility.Revealed;
				default:
					throw new InvalidOperationException("Unknown visibility: " + eventVisibility);
				}
			}
		}

		internal NativeEvent(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			Event.Event_Dispose(selfPointer);
		}

		public override string ToString()
		{
			if (IsDisposed())
			{
				return "[NativeEvent: DELETED]";
			}
			return string.Format("[NativeEvent: Id={0}, Name={1}, Description={2}, ImageUrl={3}, CurrentCount={4}, Visibility={5}]", Id, Name, Description, ImageUrl, CurrentCount, Visibility);
		}
	}
}
