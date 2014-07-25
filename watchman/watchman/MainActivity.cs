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

namespace watchman
{
	[Activity (Label = "watchman", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity,ILocationListener
	{
		int count = 1;

		#region Azure stuff
		#endregion Azure stuff

		#region GCM stuff



		#endregion GCM stuff

		#region BT stuff
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


