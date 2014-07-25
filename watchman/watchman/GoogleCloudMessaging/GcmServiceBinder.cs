using System;

namespace watchman
{

	/// <summary>
	/// Gcm service binder.
	/// </summary>
	public class GcmServiceBinder
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

	public delegate void GcmMessageReceivedHandler(object sender,GcmMessage message);

}

