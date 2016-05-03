using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLogicLayer.GeocodeService;
using DataLayer;
using System.Net;
using System.Runtime.Serialization.Json;
using BLogicLayer.JSON;

namespace BLogicLayer
{
    public class ActivityViewLogic
    {
        public static ICollection<String> getTrends()
        {
            ICollection<String> daily_trends = new List<String>();
            Dictionary<String, int> tags=new Dictionary<string, int>();
            
            
            using (var context = new BlogPostContext())
            {
                foreach (BlogPost b in context.BlogPosts)
                {
                    if(b.dateCreated==DateTime.Today)
                    { 
                        ICollection<String> blog_tags = b.tags;

                        foreach (String tag in blog_tags)
                        {
                            if (tags.ContainsKey(tag))
                            {
                                tags[tag] = tags[tag] + 1;
                            }
                            else
                            {
                                tags.Add(tag, 1);
                            }
                        }
                    }
                }                  
            }

            for(int i=0;i<15;i++)
            {
                while(tags.Count>0)
                {
                    daily_trends.Add(tags.Max().Key);
                    tags.Remove(tags.Max().Key);
                }
            }

            return daily_trends;
        } 

        public static List<Dictionary<string, double>> retrieveGeoPoints(List<String> addresses)
        {

            // Now create a list of geopoints, loop through and geocode the addresses,
            // and return the list of geocoded addresses

            List<Dictionary<string, double>> geoPoints = new List<Dictionary<string, double>>();

            foreach (String address in addresses)
            {
               Dictionary<string, double> geoPoint = GeocodeAddress(address);
                
                // only add valid results
                if (geoPoint != null)
                {
                    geoPoints.Add(geoPoint);
                }
            }

            return geoPoints;
        }


        // The following two methods come from Microsoft's developer network.
        // The second one, GeocodeAddress(), is adapted from some sample code that was also provided.
        // First method's link: https://msdn.microsoft.com/en-us/library/dd221354.aspx
        // Second method's link: https://msdn.microsoft.com/en-us/library/jj819168.aspx

        public static Response MakeRequest(string requestUrl)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Response));
                    object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    Response jsonResponse
                    = objResponse as Response;
                    return jsonResponse;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }


        private static Dictionary<string, double> GeocodeAddress(String query)
        {
            string key = Environment.GetEnvironmentVariable("BING_MAPS_KEY");
            String geocodeRequest = string.Format("http://dev.virtualearth.net/REST/v1/Locations?q={0}&key={1}", query, key);
            Response geocodeResponse = MakeRequest(geocodeRequest);

            if (geocodeResponse == null)
            {
                return null;
            }
            else
            {
                JSON.Location location = (JSON.Location)geocodeResponse.ResourceSets[0].Resources[0];

                Dictionary<String, double> results = new Dictionary<string, double>();

                results.Add("Latitude", location.Point.Coordinates[0]);
                results.Add("Longitude", location.Point.Coordinates[1]);

                return results;

            }

        }
    }

}
