using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Xml;
using System.Text.Json;
using RestSharp;
using System.Collections.Generic;

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

            var clientdata = ReadJsonFile.getClientData();

            var baseUrl = "https://apis.deutschebahn.com/db-api-marketplace/apis/timetables/v1/";
            var urlEnc = WebUtility.UrlEncode(inputTrainStation.Text);

            var client = new RestClient(baseUrl + "station/" + urlEnc);
            var request = new RestRequest("", Method.Get);
            request.AddHeader("DB-Client-Id", clientdata[0]);
            request.AddHeader("DB-Api-Key", clientdata[1]);
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

            private static String ClientUrlString(String baseUrl, String urlEncodeString)
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
                var clientdata = ReadJsonFile.getClientData();
                var request = new RestRequest("", Method.Get);
                request.AddHeader("DB-Client-Id", clientdata[0]);
                request.AddHeader("DB-Api-Key", clientdata[1]);
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
            var clientdata = ReadJsonFile.getClientData();
            request.AddHeader("DB-Client-Id", clientdata[0]);
            request.AddHeader("DB-Api-Key", clientdata[1]);
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
                    NewPlatformString newPlatformString = new NewPlatformString(platformTrack);
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
        }

        public class ClientData
        {
            public String ClientID { get; set; }
            public String ClientAPI { get; set; }
        }
        
        public static class ReadJsonFile
        {
            public static List<String> getClientData()
            {
                string text = File.ReadAllText(@"../../../.env/clientdata.json");
                var clientData = JsonSerializer.Deserialize<ClientData>(text);

                var id = clientData.ClientID;
                var api = clientData.ClientAPI;

                // Create a list of strings.
                var textValue = new List<String>();
                textValue.Add(id);
                textValue.Add(api);

                return textValue;
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
    }
}
