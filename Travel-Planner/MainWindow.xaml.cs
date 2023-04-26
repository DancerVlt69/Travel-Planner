using System;
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
        
        private void search_IsInFocus(object sender, RoutedEventArgs e)
        {
            showStartTime.IsEnabled = false;
        }
        private void search_Click(object sender, RoutedEventArgs eventArgs)
        {
            showStartTime.IsEnabled = false;
            var baseUrl = "https://apis.deutschebahn.com/db-api-marketplace/apis/timetables/v1/";
            var urlEnc =  WebUtility.UrlEncode(inputTrainStation.Text);
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

        private void showTime_Click(object sender, RoutedEventArgs eventArgs)
        {
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

                    NewTimeString newTimeString = new NewTimeString(travelTime);
                    NewDateString newDateString = new NewDateString(travelTime);

                    String newListEntry = String.Format("{0} | {1} | {2}", newDateString, newTimeString, destStation);

                    listStartTime.Items.Add(newListEntry);

                    // string[] newStationArray = stationName.Split('|');
                    // int count = newStationArray.Length;
                }
            }    
        }
           
        private void listTrainStations_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            showStartTime.IsEnabled = true;
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
