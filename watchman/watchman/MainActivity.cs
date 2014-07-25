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
using Android.Telephony.Gsm;

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

		private Button _button;

		private void SetupUI()
		{
			_button = FindViewById<Button> (Resource.Id.myButton);
		}

		#endregion UI


		#region Azure stuff

		public static MobileServiceClient _azureClient;

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

		/// <summary>
		/// Setups the gcm.
		/// </summary>
		private void SetupGcm()
		{
			//http://www.dotnetthoughts.net/xamarin-android-push-notifications-using-google-cloud-messaging-gcm-and-asp-net/
			GcmClient.CheckDevice (this);
			GcmClient.CheckManifest (this);

			//The SENDER_ID is your Google API Console App Project Number
			//https://console.developers.google.com/project/571871212429

			if (!GcmClient.IsRegistered (this.ApplicationContext)) {
				GcmClient.Register (this.ApplicationContext, GcmBroadcastReceiver.SENDER_IDS);
				GcmClient.SetRegisteredOnServer (this.ApplicationContext, true);
			}

			var connection = new GcmServiceConnection (this);
			connection.ServiceConnected += onGcmConnected;
			BindService (new Intent ("com.xamarin.GcmService"), connection, Bind.AutoCreate);

		}

		private void onGcmConnected (object sender, EventArgs e)
		{
			_gcmService = this.GCMBinder.GetGcmService ();
			_gcmService.GcmMessageReceived += GcmMessageReceived;
		}

		private void GcmMessageReceived (object sender, GcmMessage message)
		{
			//TODO
			RunOnUiThread(()=>_button.Text = message.MessageText);
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
						foreach (string s in values.Split(";".ToCharArray())) {

							//6421505156
							SmsManager.Default.SendTextMessage ("+64210366688", null,
								"Your oldie is in trouble. spiked reading @ " + GetGeoAddresses().First().ToString(),null, null);

							/*
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

		#region FB stuff
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
			Button button = FindViewById<Button> (Resource.Id.myButton);
			
			button.Click += delegate {
				button.Text = string.Format ("{0} clicks!", count++);
			};

			setup ();
		}

		/// <summary>
		/// setup all the goodies
		/// </summary>
		private void setup()
		{
			SetupVibrator ();
			SetupLocationManager ();

		}


	}
}


