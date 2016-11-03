using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

// API KEY : AIzaSyAnQ3IBfEGJ7G7hAhhBlXUZDA48ikMHk7o

namespace Server_API
{
    class Program
    {
        const string apiKey = "AIzaSyAnQ3IBfEGJ7G7hAhhBlXUZDA48ikMHk7o";

        public static string ParseData(string data)
        {

            try
            {
                JObject origin = JObject.Parse(data);

                // Access the correct element by going through the arrays of data within the Json
                // The index will always be 0
                MapData map = new MapData
                {
                    latitude = new List<double>
                    {
                        Double.Parse(origin["routes"][0]["legs"][0]["start_location"]["lat"].ToString()),
                        Double.Parse(origin["routes"][0]["legs"][0]["end_location"]["lat"].ToString())
                    },

                    longitude = new List<double>
                    {
                        Double.Parse(origin["routes"][0]["legs"][0]["start_location"]["lng"].ToString()),
                        Double.Parse(origin["routes"][0]["legs"][0]["end_location"]["lng"].ToString())
                    },

                    polyline = origin["routes"][0]["overview_polyline"]["points"].ToString(),

                    totalDistance = Int32.Parse(origin["routes"][0]["legs"][0]["distance"]["value"].ToString())

                };


                return JsonConvert.SerializeObject(map, Formatting.Indented);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return "error";
        }

        static void Main(string[] args)
        {

            Server serverO = new Server(HttpResponse, "http://127.0.0.1:8080/");

            serverO.StartListening();
   

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }

        public static string HttpResponse(HttpListenerRequest request)
        {

            // Return the word error if an error occurs 
            if (request.HttpMethod != "GET")
                return "error";
          
            // Return the word error if an error occurs 
            if (request.QueryString["origin"] == null || request.QueryString["origin"].Length == 0
                || request.QueryString["destination"] == null || request.QueryString["destination"].Length == 0)
                return "error";

            // Construct a url that we can use to request data from google maps
            string requestUrl = "https://maps.googleapis.com/maps/api/directions/json?";
            requestUrl += "origin=" + request.QueryString["origin"] + "&destination=" + request.QueryString["destination"]
                + "&key=" + apiKey;
           
            WebRequest googleMaps = WebRequest.Create(requestUrl);

            googleMaps.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = googleMaps.GetResponse();
            
            // Read the response
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);

            string mapsResponse = reader.ReadToEnd();
            string parsedJson = ParseData(mapsResponse);
            
            return parsedJson;
        }

        
    }
}
