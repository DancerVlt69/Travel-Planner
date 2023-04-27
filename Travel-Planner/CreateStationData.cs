using System;

namespace TravelPlanner
{ 
	class CreateTrainStation
	{
		public CreateTrainStation(String stationId, String stationName)
		{
			this.stationName = stationName;
			this.stationId = stationId;
		}

		String stationId;
		public String StationId { get { return stationId; } }

		String stationName;
		public String StationName
		{
			get
			{
				return stationName;
			}
		}

		public override String ToString()
		{
			return StationName;
		}
	}

	class NewTimeString
	{
		public NewTimeString(String travelTime)
		{
			this.travelTime = travelTime;
		}

		String travelTime;
		String newTravelTime;
		String t1String; String t2String;

		public String TravelTime { get { return travelTime; } }
		public String NewTravelTime
		{
			get
			{
				for (int i = 6; i < travelTime.Length - 2; i++)
				{
					t1String += travelTime[i];
				}

				for (int i = 8; i < travelTime.Length; i++)
				{
					t2String += travelTime[i];
				}

				newTravelTime = String.Format("{0}:{1}", t1String, t2String);
				return newTravelTime;
			}
		}

		public override String ToString()
		{
			return NewTravelTime;
		}
	}

	class NewDateString
	{
		public NewDateString(String travelDate)
		{
			this.travelDate = travelDate;
		}

		String travelDate; String newTravelDate;
		String d1String; String d2String; String d3String;
		public String TravelDate { get { return travelDate; } }
		public String NewTravelDate
		{
			get
			{
				for (int i = travelDate.Length - 10; i <= 1; i++)
				{
					d3String += travelDate[i];
				}
				for (int i = travelDate.Length - 8; i <= 3; i++)
				{
					d2String += travelDate[i];
				}
				for (int i = travelDate.Length - 6; i <= 5; i++)
				{
					d1String += travelDate[i];
				}

				newTravelDate = String.Format("{0}.{1}.{2}", d1String, d2String, d3String);
				return newTravelDate;
			}
		}

		public override String ToString()
		{
			return NewTravelDate;
		}
	}

	class NewPlatformString
	{
		public NewPlatformString(String platformTrack)
		{
			this.platformTrack = platformTrack;
		}

		String platformTrack; String newPlatformTrack;

		public String PlaformTrack { get { return platformTrack; } }

		public String NewPlatformTrack
		{
			get
			{
				String nPT = String.Format("Track {0}", platformTrack);
				int c1 = platformTrack.Length;
				int c2 = nPT.Length;
				int c3;
				if (c1 >= 2) { c3 = 14; } else { c3 = 15; }
				while (c2 < c3)
				{
					nPT = nPT.PadLeft(c2 + 1, ' ').PadRight(c2 + 2, ' ');

					c2 = nPT.Length;
				}

				return nPT;
			}
		}
		public override String ToString()
		{
			return NewPlatformTrack;
		}
	}

}