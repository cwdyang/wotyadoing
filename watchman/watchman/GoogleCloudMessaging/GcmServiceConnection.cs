using System;
using Android.Content;
using Android.OS;

namespace watchman
{
	public delegate void GCMServiceConnectHandler(object sender,EventArgs e);

	public class GcmServiceConnection: Java.Lang.Object, IServiceConnection
	{
		public event GCMServiceConnectHandler ServiceConnected;

		public MainActivity Activity;

		public GcmServiceConnection (MainActivity activity)
		{
			this.Activity = activity;
		}

		public void OnServiceConnected (ComponentName name, IBinder service)
		{
			var binder = service as GcmServiceBinder;

			if (binder != null) {
				Activity.GCMBinder = binder;
				Activity.IsGcmBound = true;
				ServiceConnected (this, null);
			}
		}

		public void OnServiceDisconnected (ComponentName name)
		{
			Activity.IsGcmBound = false;
		}
	}
}

