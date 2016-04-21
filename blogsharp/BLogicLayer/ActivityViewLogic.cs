using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
