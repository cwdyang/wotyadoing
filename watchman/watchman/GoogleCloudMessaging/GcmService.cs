﻿using System;
using Android.App;
using Gcm.Client;
using Android.Content;

namespace watchman
{
	/// <summary>
	/// David's poogle stuff
	/// https://console.developers.google.com
	/// API key
	/// AIzaSyDQOysQzFL2oFtYCMnkLtH-DEkFT-GdGR4
	/// IPs
	/// Any IP allowed 
	/// 
	/// https://manage.windowsazure.com/@pcresourceshotmail.onmicrosoft.com#Workspaces/MobileServicesExtension/apps/cwdyangMobile/push
	/// </summary>
	[Service]
	[IntentFilter(new String[]{"com.xamarin.GcmService"})]
	public class GcmService:GcmServiceBase
	{
		public GcmService ():base(GcmBroadcastReceiver.SENDER_IDS) { }
		public string RegistrationId;

		private event GcmMessageReceivedHandler GcmMessageReceived;

		/// <summary>
		/// you got friggin mail!
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="intent">Intent.</param>
		protected override void OnMessage (Context context, Intent intent)
		{
			//check shiz is coming
			if (intent != null && intent.Extras != null) {

				var msg = intent.Extras.GetString ("message");

				GcmMessageReceived (this, new GcmMessage {
					From = "TheWatcher",
					To = "TheWatchee",
					MessageText = msg
				});

				CreateNotification ("The Watcher has seen..", msg);
			}


		}

		/// <summary>
		/// Initializes a new instance of the <see cref="watchman.GcmService"/> class.
		/// </summary>
		/// <param name="caption">Caption.</param>
		/// <param name="description">Description.</param>
		public void CreateNotification(string caption, string description)
		{
			var notificationManager = (NotificationManager)GetSystemService (Context.NotificationService);

			var uiIntent = new Intent(this, typeof(MainActivity));
			var notification = new Notification(Resource.Drawable.Icon, caption);
			notification.Flags = NotificationFlags.AutoCancel;
			notification.Defaults = NotificationDefaults.Sound;
			notification.SetLatestEventInfo(this, caption, description,
				PendingIntent.GetActivity(this, 0, uiIntent, 0));
			notificationManager.Notify(1, notification);
		}

		/// <summary>
		/// you no registered la
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="registrationId">Registration identifier.</param>
		protected override void OnUnRegistered (Context context, string registrationId)
		{
		}

		/// <summary>
		/// someting wong
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="errorId">Error identifier.</param>
		protected override void OnError (Context context, string errorId)
		{

		}

		/// <summary>
		/// Raises the registered event.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="registrationId">Registration identifier.</param>
		protected override void OnRegistered (Context context, string registrationId)
		{
			Console.WriteLine ("Device Id:" + registrationId);
		}
	}
}
