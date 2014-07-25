using System;
using Android.Content;
using Android.App;
using Gcm.Client;

namespace watchman
{
	
	[BroadcastReceiver(Permission=Constants.PERMISSION_GCM_INTENTS)]
	[IntentFilter(new string[] { Constants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "@PACKAGE_NAME@" })]
	[IntentFilter(new string[] { Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "@PACKAGE_NAME@" })]	
	[IntentFilter(new string[] { Constants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "@PACKAGE_NAME@" })]
	public class GcmBroadcastReceiver : GcmBroadcastReceiverBase<GcmService>
	{
		//SENDER_ID is Google API Console App Project Number (david's account)
		public static string[] SENDER_IDS = new string[] {"571871212429"};
		public static string[] API_KEYS = new string[] {"AIzaSyDQOysQzFL2oFtYCMnkLtH-DEkFT-GdGR4"};
	}
}

