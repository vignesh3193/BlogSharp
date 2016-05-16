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
        public static List<string> getTrends()
        {
            List<string> daily_trends = new List<string>();
            Dictionary<string, int> tags=new Dictionary<string, int>();
            
            using (var context = new BlogContext())
            {
                List<BlogPost> blogList = context.BlogPosts.OrderByDescending(blog => blog.dateCreated).ToList();
                foreach (BlogPost b in blogList)
                {
                    if(b.dateCreated.Date==DateTime.Today)
                    { 
                        ICollection<Tag> blog_tags = b.tags;

                        foreach (Tag tag in blog_tags)
                        {
                            string tagName = tag.tagName;
                            if (tags.ContainsKey(tagName))
                            {
                                tags[tagName] = tags[tagName] + 1;
                            }
                            else
                            {
                                tags.Add(tagName, 1);
                            }
                        }
                    }
                }                  
            }

            for(int i=0;i<15;i++)
            {
                if(tags.Count>0)
                {
                    daily_trends.Add(tags.FirstOrDefault(x => x.Value == tags.Values.Max()).Key);
                    tags.Remove(tags.FirstOrDefault(x => x.Value == tags.Values.Max()).Key);
                }
            }

            return daily_trends;
        } 

        public static List<Dictionary<string, double>> retrieveGeoPoints(List<String> addresses, List<String> blogNames, 
                                                                            List<String> ids, List<String> postDates)
        {

            // Create a list of geopoints, loop through and geocode the addresses,
            // and return the list of geocoded addresses

            List<Dictionary<string, double>> geoPoints = new List<Dictionary<string, double>>();

            int counter = 0;
            foreach (String address in addresses)
            {
               // the assumption here is that the length of addresses and blogNames is the same
               // it should be because both fields are required of a user when they register

               Dictionary<string, double> geoPoint = GeocodeAddress(address);
                
                // only add valid results
                if (geoPoint != null)
                {
                    geoPoints.Add(geoPoint);
                    counter++;
                }
                else
                {
                    // modify the blogNames list and remove the associated blogName, same with the ids
                    // this will ensure that there's a 1:1 mapping from addresses of a user to their blognames/ids

                    blogNames.RemoveAt(counter);
                    ids.RemoveAt(counter);
                    postDates.RemoveAt(counter);
                }
            }

            return geoPoints;
        }


        // The following two methods (MakeRequest and GeocodeAddress) come from Microsoft's developer network.
        // The second one, GeocodeAddress(), is adapted from some sample code that was also provided.
        // First method's link: https://msdn.microsoft.com/en-us/library/jj819168.aspx
        // Second method's link: https://msdn.microsoft.com/en-us/library/dd221354.aspx


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

            // If there was any error in making the request, return nothing; else, process the results
            if (geocodeRequest == null)
            {
                return null;
            }
            else if (geocodeResponse.StatusCode != 200)
            {
                return null;
            }
            else
            {
                try {
                    JSON.Location location = (JSON.Location)geocodeResponse.ResourceSets[0].Resources[0];

                    Dictionary<String, double> results = new Dictionary<string, double>();

                    results.Add("Latitude", location.Point.Coordinates[0]);
                    results.Add("Longitude", location.Point.Coordinates[1]);

                    return results;

                }
                catch (System.IndexOutOfRangeException indexError) {
                    // If the results were not complete, this error might occur
                    // return null since the results are only partial

                    return null;
                }

            }

        }

        public static Dictionary<String, List<String>> getTopBloggers()
        {
            // Create a list of lists where the first list is the list of blogger names
            // and the second list is their corresponding blog post counts
            IEnumerable<Person> topTenBloggers = null;
            Dictionary<String, List<String>> bloggerRankings = new Dictionary<String, List<String>>();
            bloggerRankings.Add("Labels", new List<string>());
            bloggerRankings.Add("postCount", new List<string>());


            using (var context = new BlogContext())
            {
                topTenBloggers = context.Persons.OrderByDescending(blogPosts => blogPosts.posts.Count).ToList();

                // If no users exist or no users have yet made any blog posts, return nothing
                // The site user will be sent a message to sign up since the site doesn't have any users

                if (topTenBloggers == null)
                {
                    return null;
                }

                topTenBloggers = topTenBloggers.Take(10);

                foreach (Person blogger in topTenBloggers)
                {
                    bloggerRankings["Labels"].Add(blogger.blogName);
                    bloggerRankings["postCount"].Add(blogger.posts.Count.ToString());
                }

            }

            return bloggerRankings;
        }

        public static Dictionary<String, List<String>> getTopTags()
        {
            // Create a list of lists where the first list is the list of blogger names
            // and the second list is their corresponding blog post counts

            Dictionary<String, List<String>> tagRankings = new Dictionary<String, List<String>>();
            DateTime endDate = DateTime.Today;
            DateTime middleEndDate = DateTime.Today.AddDays(-7);
            DateTime middleBeginDate = DateTime.Today.AddDays(-14);
            DateTime beginDate = DateTime.Today.AddDays(-21);

            using (var context = new BlogContext())
            {
                // Get lists of the posts from the last three weeks

                List<BlogPost> thisWeeksPosts = (from post in context.BlogPosts
                                                  where post.dateCreated <= endDate && post.dateCreated > middleEndDate
                                                  select post).ToList();

                // if there are no data yet for this week, we will not have a graph to populate
                if (thisWeeksPosts == null)
                {
                    return null;
                }
                

                List<BlogPost> lastWeeksPosts = (from post in context.BlogPosts
                                                  where post.dateCreated <= middleEndDate && post.dateCreated > middleBeginDate
                                                  select post).ToList();

                List<BlogPost> twoWeeksAgoPosts = (from post in context.BlogPosts
                                                    where post.dateCreated <= middleBeginDate && post.dateCreated > beginDate
                                                    select post).ToList();

                Dictionary<string, int> thisWeekTags = new Dictionary<string, int>();
                Dictionary<string, int> lastWeekTags = new Dictionary<string, int>();
                Dictionary<string, int> twoWeeksAgoTags = new Dictionary<string, int>();

                // First get the top 5 tags from the current week
                // these will be searched for in the two previous weeks
                foreach (BlogPost post in thisWeeksPosts)
                {
                    foreach (Tag tag in post.tags)
                    {
                        // if we haven't found the tag yet, add it
                        if (thisWeekTags.ContainsKey(tag.tagName) == false) {
                            thisWeekTags.Add(tag.tagName, 1);
                        }
                        // otherwise, just increment the count
                        else
                        {
                            thisWeekTags[tag.tagName] = thisWeekTags[tag.tagName] + 1;
                        }
                    }
                }

                // quickly check to see if 5 tags have been made yet; if not,
                // then similar to the case above where there were no blog posts yet,
                // return null since there's not enough data for the graph

                if (thisWeekTags.Keys.Count < 5)
                {
                    return null;
                }

                // Create a list that can be sorted according to the tag counts,
                // then reverse it so that the top 5 tags can be identified
                List<KeyValuePair<string, int>> thisWeekTags_List = thisWeekTags.ToList();
                thisWeekTags_List.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
                thisWeekTags_List.Reverse();

                List<String> topTags = new List<string>();

                for (int i = 0; i < 5; i++)
                {
                    topTags.Add(thisWeekTags_List.ElementAt(i).Key);
                    // we want to initialize the last week and two weeks ago dictionaries
                    // to have the tags -- in this way, even if the tag didn't come up
                    // in those weeks, there won't be an error when trying to access the tag key
                    // from each dictionary
                    lastWeekTags.Add(thisWeekTags_List.ElementAt(i).Key, 0);
                    twoWeeksAgoTags.Add(thisWeekTags_List.ElementAt(i).Key, 0);
                }

                // now we look for data on these tags from the other two weeks


                foreach (BlogPost post in lastWeeksPosts)
                {
                    foreach (Tag tag in post.tags)
                    {
                        // we're only interested in the current week's top 5 tags
                        if (topTags.Contains(tag.tagName))
                        {
                        // note: at this point we know we're dealing with one of the top tags,
                        // and we know that the top tags already have entries for the last week dictionary
                        // (same for the two weeks ago dictionary later on)

                            lastWeekTags[tag.tagName] = lastWeekTags[tag.tagName] + 1;

                        }
                    }
                }

                // same process, but this time for the very first week (the one two weeks ago)

                foreach (BlogPost post in twoWeeksAgoPosts)
                {
                    foreach (Tag tag in post.tags)
                    {
                        
                        if (topTags.Contains(tag.tagName))
                        {
                            twoWeeksAgoTags[tag.tagName] = twoWeeksAgoTags[tag.tagName] + 1;
                        }
                    }
                }

                // At this point we have info about the top 5 tags across the three weeks
                // Now we need to put all the info together coherently into a single dictionary
                // and return it


                tagRankings.Add("TagNames", new List<string>());
                tagRankings.Add("Tag1", new List<string>());
                tagRankings.Add("Tag2", new List<string>());
                tagRankings.Add("Tag3", new List<string>());
                tagRankings.Add("Tag4", new List<string>());
                tagRankings.Add("Tag5", new List<string>());

                // create entry for the tag names themselves --
                // they'll be used for the graph labels

                int tagCounter = 1;
                foreach (string tag in topTags)
                {
                    tagRankings["TagNames"].Add(tag);

                    // now add the three counts, in chronological order
                    // if a tag didn't come up at all in a given week, the value will be 0 since it was
                    // initialized as such earlier

                    tagRankings["Tag" + tagCounter.ToString()].Add(twoWeeksAgoTags[tag].ToString());
                    tagRankings["Tag" + tagCounter.ToString()].Add(lastWeekTags[tag].ToString());
                    tagRankings["Tag" + tagCounter.ToString()].Add(thisWeekTags[tag].ToString());
                    tagCounter++;

                }
            }

            return tagRankings;
        }
    }

}
