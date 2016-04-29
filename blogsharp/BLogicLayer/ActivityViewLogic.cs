using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLogicLayer.GeocodeService;
using DataLayer;

namespace BLogicLayer
{
    class ActivityViewLogic
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

        public List<String> retrieveGeoPoints()
        {
            // Create list of addresses to geocode

            List<String> addresses = new List<String>();

            using (var context = new PersonContext())
            {
                addresses = (from blogger in context.People
                             select blogger.location).ToList();
            }

            // Now create a list of geopoints, loop through and geocode the addresses,
            // and return the list of geocoded addresses

            List<String> geoPoints = new List<String>();

            foreach (String address in addresses)
            {
                string geoPoint = GeocodeAddress(address);
                
                // only add valid results
                if (geoPoint != "No Results Found")
                {
                    geoPoints.Add(geoPoint);
                }
            }

            return geoPoints;
        }


        // Note: This method comes from Microsft's developer network.  We are using it as a helper method for
        // quickly and conveniently getting latitude/longitude information.  The Activity page
        // will use this information for plotting bloggers' locations on a map.
        // Source: https://msdn.microsoft.com/en-us/library/dd221354.aspx

        // We will soon acquire an API key to be used below

        private String GeocodeAddress(string address)
        {
            string results = "";
            string key = "insert your Bing Maps key here";
            GeocodeRequest geocodeRequest = new GeocodeRequest();

            // Set the credentials using a valid Bing Maps key
            geocodeRequest.Credentials = new GeocodeService.Credentials();
            geocodeRequest.Credentials.ApplicationId = key;

            // Set the full address query
            geocodeRequest.Query = address;

            // Set the options to only return high confidence results 
            ConfidenceFilter[] filters = new ConfidenceFilter[1];
            filters[0] = new ConfidenceFilter();
            filters[0].MinimumConfidence = GeocodeService.Confidence.High;

            // Add the filters to the options
            GeocodeOptions geocodeOptions = new GeocodeOptions();
            geocodeOptions.Filters = filters;
            geocodeRequest.Options = geocodeOptions;

            // Make the geocode request
            GeocodeServiceClient geocodeService = new GeocodeServiceClient();
            GeocodeResponse geocodeResponse = geocodeService.Geocode(geocodeRequest);

            if (geocodeResponse.Results.Length > 0)
                results = String.Format("Latitude: {0}\nLongitude: {1}",
                  geocodeResponse.Results[0].Locations[0].Latitude,
                  geocodeResponse.Results[0].Locations[0].Longitude);
            else
                results = "No Results Found";

            return results;
        }
    }

}
