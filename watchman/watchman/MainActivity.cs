using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Gcm.Client;
using Microsoft.WindowsAzure.MobileServices;
using Android.Bluetooth;
using Java.Util;
using Android.Telephony;
using Facebook;
using WindowsAzure.Messaging;
using Java.IO;
using System.Text.RegularExpressions;


namespace watchman
{
	[Activity (Label = "sentricare", MainLauncher = true, Icon = "@drawable/icon")]
	[assembly:Permission (Name = Android.Manifest.Permission.Internet)]
	[assembly:Permission (Name = Android.Manifest.Permission.WriteExternalStorage)]
	//TODO 
	[assembly:MetaData ("com.facebook.sdk.ApplicationId", Value ="@string/app_id")]
	public class MainActivity : Activity,ILocationListener
	{
		int count = 1;

		#region UI

		private Button _btnState;
		private Button _btnStatus;
		private Button _btnDial111;

		private void SetupUI()
		{
			_btnState = FindViewById<Button> (Resource.Id.btnState);
			_btnStatus = FindViewById<Button> (Resource.Id.btnStatus);
			_btnDial111 = FindViewById<Button> (Resource.Id.btnDial);

			_btnState.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.discon));

			_btnState.Click+= delegate {
				//_btnState.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.alarm));

				SetupBtTest();
				if(!_btConnected)
					Task.Run(()=> ReadBtStream ());
				//SetButtonState("","");
				//SetButtonState("A","G");
				//SetButtonState("A","F");
				//SetButtonState("A","P");
				//SetButtonState("W","G");
				//SetButtonState("C","");
				//SetButtonState("W","F");
				//SetButtonState("W","P");
				//SetButtonState("O","");
				//SetButtonState("C","");

			};

			_btnDial111.Click += delegate {
				_btnDial111.Text = string.Format ("Dialed 111...");

				var uri = Android.Net.Uri.Parse ("tel:+64226512690");
				var intent = new Intent (Intent.ActionCall, uri); 

				StartActivity (intent);     
			};

			//TODO try O and K 
			Task.Run(()=> SetButtonState("O","K"));

			//_btnDial111.Click += onDial111Click;
		}

		#endregion UI


		#region Azure stuff

		private static MobileServiceClient _azureClient;

		//daves shiz
		private void SetupAzure()
		{
			_azureClient = new MobileServiceClient( "https://cwdyangmobile.azure-mobile.net/", "LwceRhiGkbclAKUwJgJckVXtPkYczn96" );
		}

		#endregion Azure stuff

		#region GCM stuff

		public GcmServiceBinder GCMBinder;
		public bool IsGcmBound;
		private GcmService _gcmService;
		private string _gcmRegistrationId;

		/// <summary>
		/// Setups the gcm.
		/// </summary>
		private void SetupGcm()
		{

			//Temp
			//http://www.dotnetthoughts.net/xamarin-android-push-notifications-using-google-cloud-messaging-gcm-and-asp-net/
			GcmClient.CheckDevice (this);
			GcmClient.CheckManifest (this);



			//The SENDER_ID is your Google API Console App Project Number
			//https://console.developers.google.com/project/571871212429

			if (!GcmClient.IsRegistered (this.ApplicationContext) || true) {
				GcmClient.Register (this.ApplicationContext, GcmBroadcastReceiver.SENDER_IDS);
				GcmClient.SetRegisteredOnServer (this.ApplicationContext, true);
			}

			var connection = new GcmServiceConnection (this);
			connection.ServiceConnected += onGcmConnected;
			BindService (new Intent ("com.xamarin.GcmServiceWatchman"), connection, Bind.AutoCreate);

		}

		void onGcmRegistered (object sender, GcmRegistration message)
		{

			RunOnUiThread(async()=>{

				//http://azure.microsoft.com/en-us/documentation/articles/partner-xamarin-notification-hubs-android-get-started/
				try{
					_gcmRegistrationId = message.RegistrationId;
					_btnStatus.Text = "Registered with google cloud messaging + azure notification hub";
					await _azureClient.GetTable<GcmRegistrations>().InsertAsync(new GcmRegistrations{registrationid=message.RegistrationId});
				}
				catch(Exception ex)
				{

				}
			});
		}

		private void onGcmConnected (object sender, EventArgs e)
		{
			_gcmService = this.GCMBinder.GetGcmService ();

			_gcmService.GcmRegistered += onGcmRegistered;
			_gcmService.GcmMessageReceived += onGcmMessageReceived;
		}

		private void onGcmMessageReceived (object sender, GcmMessage message)
		{
			//TODO
			RunOnUiThread(()=>_btnState.Text = message.MessageText);
		}
		#endregion GCM stuff

		#region BT stuff

		private static int  REQUEST_ENABLE_BLUETOOTH      = 0x1111;
		private static int  REQUEST_DISCOVERABLE_BLUETOOTH  = 0x222;
		private const string _btDeviceName = "SFX";//"SFX"; //PROD"HC-06"; //TEST"SFX";
		private BluetoothSocket _btSocket;
		private BluetoothServerSocket _btsSocket;
		private BluetoothDevice _btDevice;
		private BluetoothAdapter _btAdapter = BluetoothAdapter.DefaultAdapter;
		private List<BluetoothDevice> _btDevices = new List<BluetoothDevice>();
		private string _btMacAddress = "00001101-0000-1000-8000-00805f9b34fb";
		private byte[] _btInputBuffer;

		private bool _btConnected;
	


		private void SetupBtTest()
		{
			try
			{
				if (_btAdapter != null &&
					!_btAdapter.IsEnabled) {
					Intent enabler = new Intent(BluetoothAdapter.ActionRequestEnable);
					StartActivityForResult(enabler, REQUEST_ENABLE_BLUETOOTH);
				}

				if (_btAdapter.IsDiscovering)
					_btAdapter.CancelDiscovery ();

				_btAdapter.StartDiscovery();

				_btDevices = _btAdapter.BondedDevices.Where(x=>(new string[]{_btDeviceName}.ToList().Contains(x.Name))).ToList();

				if (_btDevices.Count == 0) {
					//TODO log something
					return;
				}

				//_btSocket = _btDevices.First ().CreateRfcommSocketToServiceRecord(UUID.FromString(_btMacAddress));

				 var ids = _btDevices[0].GetUuids();

				_btSocket = _btDevices.First ().CreateRfcommSocketToServiceRecord(ids[0].Uuid); //UUID.FromString());

			}
			catch(Exception ex) {
			}
		}

		private void buttonStateSet()
		{
			_btnState.Text = "";
			//_btnState.Background = ?


		}

		private async Task sendSMS()
		{
			try{
				SmsManager.Default.SendTextMessage ("+64210366688", null,
					"Mrs Nana has reported an incident:" + _btnState.Text,null, null);
				/*
				SmsManager.Default.SendTextMessage ("+6421505156", null,
					"Mrs Nana has reported an incident:" + _btnState.Text,null, null);
				SmsManager.Default.SendTextMessage ("+61410549248", null,
					"Mrs Nana has reported an incident:" + _btnState.Text,null, null);
				SmsManager.Default.SendTextMessage ("+61211700097", null,
					"Mrs Nana has reported an incident:" + _btnState.Text,null, null);
				*/
			}
			catch(Exception ex) {
			}
		}

		/// <summary>
		/// Inserts the even log.
		/// </summary>
		/// <param name="rawData">Raw data.</param>
		private async Task insertEvenLog(string rawData)
		{
			try
			{
			//1234|O|K|SentriCare Client - Ver 0.1 BETA -> Hello World!;1234|A|F;1234|A|P;1234|A|P;1234|A|P;1234|A|P;1234|A|P;1234|C|P;1234|A|G;1234|C|G;
			string[] rawCode = rawData.Split ('|').ToArray ();
			if(rawCode.Length == 3)//make sure it's in this format 1234|A|F
			{
				var address = GetGeoAddresses ().First ();

				var et = rawCode [1];
				var er = rawCode [2];

				string eventReason = "";
				string eventType = "";

					RunOnUiThread(()=>
						{
				eventReason = _codeDescPairs.Where (x => x.Key == er).Select (y => y.Value).FirstOrDefault();
				eventType = _codeDescPairs.Where (x => x.Key == et).Select (y => y.Value).FirstOrDefault();
						});

				event_log eventLog = new event_log {
					device_id = "1234", //123
					event_type = eventType,//A
					event_reason = eventReason,//F
					longitude = _location.Longitude.ToString(),
					latitude = _location.Latitude.ToString(),
					altitude = address.SubThoroughfare + " " + address.Thoroughfare + ", " + address.Locality,
					care_receiver = "Bob White",
					comments = "incident reported",
					operator_NAME = "Elena M",
					severity = (et=="G")?"Critical":"High",
					status = "Unattended"
				};

				if (new string[]{Resources.GetString(Resource.String.event_type_alarm_code),Resources.GetString(Resource.String.event_type_warn_code)}.Contains(et) ) {
					await sendSMS ();
					//PostToFb ("Sentricare: incident reported [ID=1234] @" + DateTime.Now.ToString ());
				}

				await UnlockPhone ();

				await SetButtonState(rawCode [1],rawCode [2]);

				await _azureClient.GetTable<event_log> ().InsertAsync (eventLog);
			}
			else
			{
				//log it & discard it, may be corrupted
			}
			}
			catch(Exception ex) {
			}
		}



		private void ReadBtStream_COLD()
		{
			byte[] buffer = new byte[4096];  // buffer store for the stream
			int bytes; // bytes returned from read()
			_btSocket.Connect();

			_btConnected=true;
			// Keep listening to the InputStream until an exception occurs
			while (true) {
				try {

					bytes = _btSocket.InputStream.Read (buffer, 0, buffer.Length);
					byte[] readBuf = (byte[])buffer;
					String strIncom =  System.Text.Encoding.Default.GetString (readBuf); 

					System.Console.WriteLine(strIncom);

					//string[] values = strIncom.Split("\r\n".ToCharArray());

					//Console.WriteLine(values);
				} catch (Exception ex) {
				}
			}
		}

		//MAQSOOD
		//1234|O|K|SentriCare Client - Ver 0.1 BETA -> Hello World!;1234|A|F;1234|A|P;1234|A|P;1234|A|P;1234|A|P;1234|A|P;1234|C|P;1234|A|G;1234|C|G;
		private void ReadBtStream()
		{

			if (_btSocket== null)
				return;
			try
			{
				_btSocket.Connect();

				_btConnected=true;


				_btInputBuffer = new byte[1024];

				int read;
				CurrentPlatform.Init ();

				//BufferedInputStream a = new BufferedInputStream( _btSocket.InputStream);

				Regex r = new Regex(@"\w+\|\w\|\w");

				for (; (read  =  _btSocket.InputStream.Read(_btInputBuffer, 0, _btInputBuffer.Length)) > -1;) {

					//_btSocket.InputStream.Flush();

					string values = System.Text.Encoding.Default.GetString (_btInputBuffer);

					/*
					System.Console.WriteLine("---");
					System.Console.WriteLine (values);
					RunOnUiThread(()=>
						_btnStatus.Text = values);
						*/
					try {
						//http://social.msdn.microsoft.com/Forums/windowsazure/en-US/a47c9b11-81d0-4f19-8090-133250d3fb4b/problem-insertasync-xamarin-azure-mobile-service?forum=azuremobile
						//Data coming in:
						//deviceid,eventtype,reasoncode,reading/data;
						//1|A|G|100;
						//foreach (string rawCode in values.Split(";".ToCharArray())) {

						foreach(Match ex in r.Matches(values))
						{
							//6421505156
							//SmsManager.Default.SendTextMessage ("+64210366688", null,
							//	"Mrs Agnes Brown is in trouble. @ " + GetGeoAddresses().First().ToString(),null, null);

							//insert this event log
							Task.Run(()=> insertEvenLog (ex.Value).Wait());
							//_location;
							/*
							 * 
							 * example insert
							Reading reading = new Reading {
								sensor = s.Split ("|".ToArray ()) [0],
								unit = s.Split ("|".ToArray ()) [1],
								inputvalue = float.Parse (s.Split ("|".ToArray ()) [2])
							};

							await _azureClient.GetTable<Reading> ().InsertAsync (reading);
							*/

							//GC.Collect (0);
						break;
						}

					} catch (Exception e) {
						var e1 = e;
					}
				}
				
			}
			 catch (Exception e) {
				var e1 = e;
				_btConnected = false;
			}

			_btConnected = false;
		}
		#endregion BT stuff

		#region AzureNotificationHub

		//http://azure.microsoft.com/en-us/documentation/articles/notification-hubs-android-get-started/#register


		#endregion 

		#region FB stuff

		private string _fbAccessToken;

		private string _fbAPIAccessToken = "CAACEdEose0cBAEj9PbzBEfXC7QcZAlzMSOQSYdQsPkgDGnz5WVMAXOjJfL9EBXWnfXu7LfH6Lk94itFDlCZAmKZC5IPc4RPSFYU2gSN1vmL7ZBAcFfyikVis4SdnKwoyqJkt5W4pTPNuhZAvMI9WZCryKlEyh5VzScmBtRn0Y1hVFxEUCpGd5CGTNWTSoDLlv030OrkTLQZCm9wkep0uL32";
		/// <summary>
		/// this is da shit
		/// http://facebooksdk.net/docs/faq/
		/// </summary>
		private void SetupFb()
		{
			/*
			var fb = new FacebookClient();
			var result = fb.Get("oauth/access_token", new { 
				client_id     = "1443593652568359",//"app_id", 
				client_secret = "c186fdf4592956d6ae73715b1410de75",//"app_secret", 
				grant_type    = "client_credentials" 
			});

			_fbAccessToken = Convert.ToString(result);

			_fbAccessToken = _fbAccessToken.Replace (@"{""access_token"":""",string.Empty).Replace (@"""}",string.Empty);//  @"{""access_token"":""1443593652568359|glxYP4d6TA9FpE0MUTqbMm7uW_Q""}";
			*/

			//
		}

		/// <summary>
		/// fix the mother fucker here
		/// https://developers.facebook.com/tools/explorer/145634995501895/
		/// https://developers.facebook.com/tools/explorer/1443593652568359/?method=GET&path=me%3Ffields%3Did%2Cname&version=v2.0#_=_
		/// </summary>
		private void PostToFb(string message)
		{
			var fb = new FacebookClient ();

			fb.AppId = "1392784214294156";
			fb.AppSecret = "1f87756e9fe953ceab2c3da8f959fa9c";
			fb.AccessToken = "CAACEdEose0cBAOP2OBgGcb78HXtgVrbkn9Jy7Y3K9nvNn7iWx51jsBeY90yZBz1ia0Rf3IoEg1\nKvKJ0kY9Q16jdFxRnxrwAcD1ofO2CN398T8LdHjeo1jd3uCRWTopY5zs1fRPbxOw1uB0nuDBbSS\nBDBFzfoZA1J78OWvgIcuSGIvjJkesON6qZAIrC8wwZD";//_fbAccessToken;

			try
			{
				fb.Post ("me/feed", new { message = message });
			}
			catch (Exception ex) {
				var s = ex.ToString ();
			}
			/*
			fb.PostTaskAsync ("me/feed", new { message = myMessage }).ContinueWith (t => {
				if (!t.IsFaulted) {
					string message = "Great, your message has been posted to you wall!";
					Console.WriteLine (message);
				}
			});
			*/

		}

		#endregion FB stuff

		#region Location stuff

		private LocationManager _locationManager;
		private Location _location;
		private string _locationProvider;

		/// <summary>
		/// Location shitz
		/// </summary>
		private void SetupLocationManager()
		{
			try
			{
				_locationManager = (LocationManager)GetSystemService (LocationService);

				IList<string> acceptableLocationProviders = _locationManager.GetProviders(new Criteria
						{
							Accuracy = Accuracy.Fine
					}, true);

				if (acceptableLocationProviders.Any())
				{
					_locationProvider = acceptableLocationProviders.First();
				}
				else
				{
					_locationProvider = String.Empty;
				}
			
				if(_locationProvider!=string.Empty)
					_locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);

				_location = _locationManager.GetLastKnownLocation(_locationProvider);
			}
			catch(Exception ex) {

			}
		}

		public IEnumerable<Address> GetGeoAddresses()
		{
			try{
				if(_location!=null)
				{
					Geocoder geoCoder = new Geocoder (this.ApplicationContext);
					IList<Address> addresses = geoCoder.GetFromLocation(_location.Latitude,_location.Longitude,10);
					return addresses;
				}
				else
					return Enumerable.Empty<Address>();
			}
			catch(Exception e) {
				return Enumerable.Empty<Address>();
			}
		}

		public void OnLocationChanged (Location location)
		{
			_location = location;
		}

		public void OnProviderDisabled (string provider)
		{
			_locationProvider = "";
		}

		public void OnProviderEnabled (string provider)
		{
			_locationProvider = provider;
		}

		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
			_locationProvider = provider;
		}

		#endregion Location stuff

		#region Vibrate stuff

		private Vibrator _vibrator;

		private void SetupVibrator()
		{
			_vibrator = (Vibrator)GetSystemService (VibratorService);
		}

		#endregion Vibrate stuff

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.btnState);
			
			button.Click += delegate {
				//button.Text = string.Format ("{0} clicks!", count++);
			};

			setup ();

			Task.Run(()=> ReadBtStream ());
		}


		Dictionary<string,string> _codeDescPairs = new Dictionary<string, string> ();

		/// <summary>
		/// setup all the goodies
		/// </summary>
		private void setup()
		{
			SetupStrings ();

			Task.Run(()=> UnlockPhone ().Wait());

			SetupUI ();
			SetupVibrator ();
			SetupLocationManager ();
			SetupBtTest ();
			SetupAzure ();
			SetupGcm ();
			SetupFb ();

			//Task.Run(()=> insertEvenLog ("1234|A|F").Wait());


		}

		private void SetupStrings()
		{


			_codeDescPairs.Add (Resources.GetString (Resource.String.reason_gas_code), Resources.GetString (Resource.String.reason_gas_desc));
			_codeDescPairs.Add (Resources.GetString (Resource.String.reason_fall_code), Resources.GetString (Resource.String.reason_fall_desc));
			_codeDescPairs.Add (Resources.GetString (Resource.String.reason_panic_code), Resources.GetString (Resource.String.reason_panic_desc));

			_codeDescPairs.Add (Resources.GetString (Resource.String.event_type_cancel_code), Resources.GetString (Resource.String.event_type_cancel_desc));
			_codeDescPairs.Add (Resources.GetString (Resource.String.event_type_alarm_code), Resources.GetString (Resource.String.event_type_alarm_desc));
			_codeDescPairs.Add (Resources.GetString (Resource.String.event_type_warn_code), Resources.GetString (Resource.String.event_type_warning_desc));
			_codeDescPairs.Add (Resources.GetString (Resource.String.event_type_ok_code), Resources.GetString (Resource.String.event_type_ok_desc));


		}

		private DateTime? _lastAlert = null;

		private async Task UnlockPhone()
		{
			/*
			Window window = this.Window;
			window.AddFlags(WindowManagerFlags.DismissKeyguard);
			window.AddFlags(WindowManagerFlags.ShowWhenLocked);
			window.AddFlags(WindowManagerFlags.TurnScreenOn);
			*/

			PowerManager pm = (PowerManager) this.GetSystemService(Context.PowerService);
			PowerManager.WakeLock w1 = pm.NewWakeLock (
				WakeLockFlags.Full|
				WakeLockFlags.AcquireCausesWakeup|
				WakeLockFlags.ScreenBright|
				WakeLockFlags.OnAfterRelease, "NotificationReceiver");

			w1.Acquire ();
			//w1.Release ();
		}

		private async Task SetButtonState(string eventType,string reason)
		{

			var buttonDesc = "";

			bool etA = Resources.GetString (Resource.String.event_type_alarm_code)==(eventType);
			bool etO = Resources.GetString (Resource.String.event_type_ok_code)==(eventType);
			bool etC = Resources.GetString (Resource.String.event_type_cancel_code)==(eventType);
			bool etW = Resources.GetString (Resource.String.event_type_warn_code)==(eventType);

			buttonDesc += (etA)?Resources.GetString (Resource.String.event_type_alarm_desc):string.Empty;
			buttonDesc += (etW)?Resources.GetString (Resource.String.event_type_warning_desc):string.Empty;
			buttonDesc += (etC)?Resources.GetString (Resource.String.event_type_cancel_desc):string.Empty;
			buttonDesc += (etO)?Resources.GetString (Resource.String.event_type_ok_desc):string.Empty;



			if(buttonDesc=="")
				buttonDesc = "Disconnected";

			bool rG = Resources.GetString (Resource.String.reason_gas_code)==(reason);
			bool rF = Resources.GetString (Resource.String.reason_fall_code)==(reason);
			bool rP = Resources.GetString (Resource.String.reason_panic_code)==(reason);

			if (rF) {
			}

			buttonDesc += (rG)?" " + Resources.GetString (Resource.String.reason_gas_desc):string.Empty;
			buttonDesc += (rF)?" " + Resources.GetString (Resource.String.reason_fall_desc):string.Empty;
			buttonDesc += (rP)?" " + Resources.GetString (Resource.String.reason_panic_desc):string.Empty;


			RunOnUiThread (() => {
				_btnState.SetBackgroundResource ((etA)? 
					Resource.Drawable.alarm:
					(etW)?
					Resource.Drawable.warn:
					(etO||etC)?Resource.Drawable.ok:Resource.Drawable.discon);

				if (etA || etW) {
					_lastAlert = DateTime.Now;
					_btnStatus.Text = "Emergency personnel on their way to you, please stay where you are.";
				} else if (etC && _lastAlert.HasValue) {
					_btnStatus.Text = "Last alert: " + _lastAlert.Value.ToString ("ddd dd/mm/yyyy hh:mm tt");
				}

				_btnState.Text = buttonDesc;
			}
			);
		}
	}
}


