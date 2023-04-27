using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Windows;
using System.Xml;
using RestSharp;

namespace TravelPlanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // https://data.deutschebahn.com/dataset/api-fahrplan
        // https://apis.deutschebahn.com/db-api-marketplace/apis/timetables/v1
        // Datenlizenz: Creative Common Attribution 4.0 International ( CC BY 4.0 )
        // by Deutsche Bahn AG

        private void search_Click(object sender, RoutedEventArgs eventArgs)
        {
            showStartTime.IsEnabled = false;
            var baseUrl = "https://apis.deutschebahn.com/db-api-marketplace/apis/timetables/v1/";
            var urlEnc = WebUtility.UrlEncode(inputTrainStation.Text);

            var client = new RestClient(baseUrl + "station/" + urlEnc);
            var request = new RestRequest("", Method.Get);
            request.AddHeader("DB-Client-Id", clientId);
            request.AddHeader("DB-Api-Key", clientAPI);
            request.AddHeader("accept", "application/xml");
            RestResponse response = client.Execute(request);

            String s = response.Content.ToString();
            var doc = new XmlDocument();
            doc.LoadXml(s);
            var nodes = doc.GetElementsByTagName("station");

            foreach (XmlNode node in nodes)
            {
                listTrainStations.Items.Clear();
                listStartTime.Items.Clear();
                String stationId = node.Attributes["eva"].Value;
                String stationName = node.Attributes["name"].Value;
                if (stationName == null || stationName == "")
                    listTrainStations.Items.Add("no train station found");
                else listTrainStations.Items.Add(new CreateTrainStation(stationId, stationName));
            }
        }

         static class GetData
        {
            
             public static String getDBData()
            {
                var baseUrl = "https://apis.deutschebahn.com/db-api-marketplace/apis/timetables/v1/";
                var startDate = DateTime.Now.ToString("yyMMdd");
                var startTime = DateTime.Now.ToString("HH");
                //var urlEnc = WebUtility.UrlEncode(inputTrainStation.Text);
                //var stationId = ((CreateTrainStation)listTrainStations.SelectedItem).StationId;

                return "";
            }

            private static String ClientUrlString (String baseUrl, String urlEncodeString)
            {
                var pathString = "station";
                var clientUrlString = String.Format("{0}{1}/{2}", baseUrl, pathString, urlEncodeString);

                return clientUrlString;
            }

            private static String ClientUrlString(String baseUrl, String stationId, String startDate, String startTime)
            {
                var pathString = "plan";
                var clientUrlString = String.Format("{0}{1}/{2}/[3}/{4}", baseUrl, pathString, stationId, startDate, startTime);

                return clientUrlString;
            }

            public static void requestData(RestClient client)
            {
                var request = new RestRequest("", Method.Get);
                request.AddHeader("DB-Client-Id", clientId);
                request.AddHeader("DB-Api-Key", clientAPI);
                request.AddHeader("accept", "application/xml");
                RestResponse response = client.Execute(request);
                var doc = new XmlDocument();
                
                doc.LoadXml(response.Content.ToString());
            }
        }

        private void showTime_Click(Object sender, RoutedEventArgs eventArgs)
        {
            listStartTime.Items.Clear();
            if (listTrainStations.SelectedItem == null) return;

            var baseUrl = "https://apis.deutschebahn.com/db-api-marketplace/apis/timetables/v1/";
            var startDate = DateTime.Now.ToString("yyMMdd");
            var startTime = DateTime.Now.ToString("HH");
            var stationId = ((CreateTrainStation)listTrainStations.SelectedItem).StationId;
            
            var client = new RestClient(baseUrl + "plan/" + stationId + "/" + startDate + "/" + startTime);

            var request = new RestRequest("", Method.Get);
            request.AddHeader("DB-Client-Id", clientId);
            request.AddHeader("DB-Api-Key", clientAPI);
            request.AddHeader("accept", "application/xml");
            RestResponse response = client.Execute(request);
            var doc = new XmlDocument();
            doc.LoadXml(response.Content.ToString());


            var nodes = doc.GetElementsByTagName("s");
            var nodes2 = doc.GetElementsByTagName("dp");

            foreach (XmlNode node in nodes)
            {
                listStartTime.Items.Clear();
                
                foreach (XmlNode node2 in nodes2)
                {
                    String travelTime = node2.Attributes["pt"].Value;
                    String destStation = node2.Attributes["ppth"].Value;
                    String platformTrack = node2.Attributes["pp"].Value;

                    // TODO - String newViaStation = getNewViaString(destStation); 
                    
                    NewDateString newDateString = new NewDateString(travelTime);
                    NewTimeString newTimeString = new NewTimeString(travelTime);
                    NewPlatform newPlatformString = new NewPlatform(platformTrack);
                    String newDestStation = getNewStationString(destStation);

                    String newListEntry = String.Format(" {0}  |   {1}   |{2}|  {3}", newDateString, newTimeString, newPlatformString, newDestStation);

                    listStartTime.Items.Add(newListEntry);

                }
            }    
        }
           
        private void listTrainStations_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            showStartTime.IsEnabled = true;
        }

        public static String getNewStationString(String stationString)
        {
            string[] stationStringArray = stationString.Split('|');

            var newViaString = "";

            if (!(stationStringArray.Length <= 1))
            {
                for (int i = 0; i < stationStringArray.Length - 1; i++)
                {
                    if (i < stationStringArray.Length - 2)
                    {
                        newViaString += stationStringArray[i] + ", ";
                    }
                    else
                    {
                        newViaString += stationStringArray[i];
                    }
                }
            }

            var newDestString = stationStringArray[stationStringArray.Length - 1];

            if (newViaString.Length != 0)
            {
                var newStationString = String.Format("{0} via {1}", newDestString, newViaString);
                return newStationString;
            }

            else
            {
                var newStationString = String.Format("{0}", newDestString);
                return newStationString;
            }

            //Console.WriteLine(); // only for Debug
            // Console.WriteLine(newStationString); // only for Debug
            
        }

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
            { get
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

        class NewPlatform
        {
            public NewPlatform(String platformTrack)
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
       








        private void about(object sender, RoutedEventArgs eventArgs)
        {
            var msgString = "Version: v0.0.1. beta\n" + "Author:  DancerVLT69\n" + "\n" + "Contact: Discord, Twitter";
            MessageBox.Show("About this Program:\n\n" + msgString , "About" , MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void exit(object sender, RoutedEventArgs eventArgs)
        {
            MessageBoxResult result = MessageBox.Show("Programm wirklich beenden?\n\n", " Achtung! ", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes) { Close(); }
                    
            return;
        }














        const string clientId = "c4f0cca9062888a04823e3b5ea338707";
        const string clientAPI = "b7d64752022ff03de15659ccf3c9d8bf";

   
    }
}
