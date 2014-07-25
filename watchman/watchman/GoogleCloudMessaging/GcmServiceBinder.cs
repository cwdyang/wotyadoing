using System;
using Android.App;
using Android.OS;
using Android.Content;
using Gcm.Client;
using Android.Widget;

namespace watchman
{

	/// <summary>
	/// Gcm service binder.
	/// </summary>
	public class GcmServiceBinder: Binder
	{
		GcmService service;

		public GcmServiceBinder (GcmService service)
		{
			this.service = service;
		}

		public GcmService GetGcmService ()
		{
			return service;
		}
	}

	public class GcmMessage
	{
		public string From;
		public string To;
		public string MessageText;
	}

	public class GcmRegistration
	{
		public string RegistrationId;
	}

	public delegate void GcmMessageReceivedHandler(object sender,GcmMessage message);
	public delegate void GcmRegisteredHandler(object sender,GcmRegistration message);

}

