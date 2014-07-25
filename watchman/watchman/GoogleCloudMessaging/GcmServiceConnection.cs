using System;
using Android.Content;

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

		public void OnServiceConnected (ComponentName name, Android.OS.IBinder service)
		{
			var binder = (GcmServiceBinder)service;

			if (binder != null) {

			}
		}

		public void OnServiceDisconnected (ComponentName name)
		{
			throw new NotImplementedException ();
		}
	}
}

