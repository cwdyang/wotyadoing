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
			insertEvenLog ("1234|A|F");
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
		private const string _btDeviceName = "HC-06";
		private BluetoothSocket _btSocket;
		private BluetoothServerSocket _btsSocket;
		private BluetoothDevice _btDevice;
		private BluetoothAdapter _btAdapter = BluetoothAdapter.DefaultAdapter;
		private List<BluetoothDevice> _btDevices = new List<BluetoothDevice>();
		private string _btMacAddress = "00001101-0000-1000-8000-00805f9b34fb";
		private byte[] _btInputBuffer;

		private void SetupBt()
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

				_btSocket = _btDevices.First ().CreateRfcommSocketToServiceRecord(UUID.FromString(_btMacAddress));

			}
			catch(Exception ex) {
			}
		}

		private void buttonStateSet()
		{
			_btnState.Text = "";
			//_btnState.Background = ?


		}

		/// <summary>
		/// Inserts the even log.
		/// </summary>
		/// <param name="rawData">Raw data.</param>
		private async void insertEvenLog(string rawData)
		{
			//1234|O|K|SentriCare Client - Ver 0.1 BETA -> Hello World!;1234|A|F;1234|A|P;1234|A|P;1234|A|P;1234|A|P;1234|A|P;1234|C|P;1234|A|G;1234|C|G;
			string[] rawCode = rawData.Split ('|').ToArray ();
			if(rawCode.Length == 3)//make sure it's in this format 1234|A|F
			{
				EventLog eventLog = new EventLog {
					device_id = rawCode [0], //123
					event_type = rawCode [1],//A
					event_data = rawCode [2],//F
					created_at = DateTime.Now,
					longitute = _location.Longitude.ToString(),
					latitude = _location.Latitude.ToString(),
					altitude = _location.Altitude.ToString(),
				};
				await _azureClient.GetTable<EventLog> ().InsertAsync (eventLog);
			}
			else
			{
				//log it & discard it, may be corrupted
			}
		}

		//MAQSOOD
		//1234|O|K|SentriCare Client - Ver 0.1 BETA -> Hello World!;1234|A|F;1234|A|P;1234|A|P;1234|A|P;1234|A|P;1234|A|P;1234|C|P;1234|A|G;1234|C|G;
		private async void ReadBtStream()
		{

			if (_btSocket == null)
				return;
			try
			{
				_btSocket.Connect();

				_btInputBuffer = new byte[2048];

				int read;
				CurrentPlatform.Init ();

					for (; (read = _btSocket.InputStream.Read (_btInputBuffer, 0, _btInputBuffer.Length)) > -1;) {

					string values = System.Text.Encoding.Default.GetString (_btInputBuffer);
					//Console.WriteLine (values);
					try {
						//http://social.msdn.microsoft.com/Forums/windowsazure/en-US/a47c9b11-81d0-4f19-8090-133250d3fb4b/problem-insertasync-xamarin-azure-mobile-service?forum=azuremobile
						//Data coming in:
						//deviceid,eventtype,reasoncode,reading/data;
						//1|A|G|100;
						foreach (string rawCode in values.Split(";".ToCharArray())) {

							//6421505156
							SmsManager.Default.SendTextMessage ("+64210366688", null,
								"Mrs Agnes Brown is in trouble. @ " + GetGeoAddresses().First().ToString(),null, null);

							//insert this event log
							insertEvenLog(rawCode);
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

							GC.Collect (0);
						}

					} catch (Exception e) {
						var e1 = e;
					}
				}
				
			}
			 catch (Exception e) {
				var e1 = e;
			}
		}
		#endregion BT stuff

		#region AzureNotificationHub

		//http://azure.microsoft.com/en-us/documentation/articles/notification-hubs-android-get-started/#register


		#endregion 

		#region FB stuff

		private string _fbAccessToken;
		private string _fbAPIAccessToken = "CAACEdEose0cBAHwUXwTtKZAzXjKvwoPTIDddICeeM9NIQy0bADMkQfZA9osZBfgsCzmncOYv1lpvc4zuhhSLqixMeGFa42VPFU8f3bQue7SH54XuIQIbE8woeQF3sxrR8JxDDKZCigawPy53qbShQ0i9ILkArdPIHsS84H4Ib2AQMWRhwa9yODjF2dtR0EyK1Tug6ZA0zelLTtONNPVre";
		/// <summary>
		/// this is da shit
		/// http://facebooksdk.net/docs/faq/
		/// </summary>
		private void SetupFb()
		{
			var fb = new FacebookClient();
			var result = fb.Get("oauth/access_token", new { 
				client_id     = "1443593652568359",//"app_id", 
				client_secret = "c186fdf4592956d6ae73715b1410de75",//"app_secret", 
				grant_type    = "client_credentials" 
			});

			_fbAccessToken = Convert.ToString(result);

			_fbAccessToken = _fbAccessToken.Replace (@"{""access_token"":""",string.Empty).Replace (@"""}",string.Empty);//  @"{""access_token"":""1443593652568359|glxYP4d6TA9FpE0MUTqbMm7uW_Q""}";

			PostToFb ("grandma wana post to fb" + DateTime.Now.ToString());
		}

		/// <summary>
		/// fix the mother fucker here
		/// https://developers.facebook.com/tools/explorer/145634995501895/
		/// https://developers.facebook.com/tools/explorer/1443593652568359/?method=GET&path=me%3Ffields%3Did%2Cname&version=v2.0#_=_
		/// </summary>
		private void PostToFb(string message)
		{
			var fb = new FacebookClient("CAACEdEose0cBAFFM3OyG9lISm35ZAnyxDGU5C3ppPIDBxX1lGTdfGIKKHiBovi7OKpCDOuouxFZCj60U3FoY92uObyH4maq3VlFdOImwa7EG8WgQjvkHnNlTSolf75U0Y30PAIrdjwn219STXvYUI2iMfnT2TEXLh78QXIni9y9avV9tTZBOwns1lPTm9lyRRFFQU97lZBPqxZAqAILtZB");

			fb.AppId = "1443593652568359";
			fb.AppSecret = "c186fdf4592956d6ae73715b1410de75";
			fb.AccessToken = _fbAPIAccessToken;//_fbAccessToken;

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
		}

		/// <summary>
		/// setup all the goodies
		/// </summary>
		private void setup()
		{
			SetupUI ();
			SetupVibrator ();
			SetupLocationManager ();
			SetupBt ();
			SetupAzure ();
			SetupGcm ();
			SetupFb ();
		}

		private void SetButtonState()
		{
			_btnState.SetText (Resource.String.status_alarm);
			_btnState.SetBackgroundResource (Resource.Drawable.Icon);
		}
	}
}


