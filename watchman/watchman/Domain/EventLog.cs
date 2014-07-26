using System;

namespace watchman
{
	//	//1234|O|K|SentriCare Client - Ver 0.1 BETA -> Hello World!;1234|A|F;1234|A|P;1234|A|P;1234|A|P;1234|A|P;1234|A|P;1234|C|P;1234|A|G;1234|C|G;

	public class EventLog
	{
		public string id;
		public string device_id;
		public string severity;
		public string event_type;
		public string longitute;
		public string latitude;
		public string altitude;
		public DateTime created_at;
		public DateTime updated_at;
		public string event_data;
	}
}

