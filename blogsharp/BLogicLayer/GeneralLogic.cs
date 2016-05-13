using DataLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BLogicLayer
{
    public class GeneralLogic
    {
        public static Person getLoggedInUser(BlogContext blogCtx)
        {
            Person thisPerson = (from user in blogCtx.Persons
                                 where user.Email == HttpContext.Current.User.Identity.Name
                                 select user).FirstOrDefault();
            return thisPerson;

        }
        public static List<BlogPost> getBlogPosts(BlogContext blogCtx, Person thisPerson)
        {
            var blogPosts = (from posts in blogCtx.BlogPosts
                             where posts.PersonId == thisPerson.Id
                             select posts).ToList<BlogPost>();
            return blogPosts;

        }

        public static double getRatingOPost(BlogPost post)
        {
            double res = 0.0;
            foreach(Rating r in post.ratings){
                res += r.ratingNumber;
            }
            if (post.ratings != null && post.ratings.Count != 0)
                res /= post.ratings.Count;

            return res;
        }

        public static void createRandomBlogPosts(BlogContext blogCtx, int numPosts)
        {
            //Thanks to http://randomtextgenerator.com
            string wallOfText = "Sitting mistake towards his few country ask. You delighted two rapturous six depending objection happiness something the. Off nay impossible dispatched partiality unaffected. Norland adapted put ham cordial. Ladies talked may shy basket narrow see. Him she distrusts questions sportsmen. Tolerably pretended neglected on my earnestly by. Sex scale sir style truth ought.";
            wallOfText = wallOfText.Replace(".","");
            string[] text = wallOfText.Split(' ');
            Random rnd = new Random();
            List<int> userIDs = (from user in blogCtx.Persons
                                 select user.Id).ToList();
            for (int i = 0; i < numPosts; i++)
            {
                BlogPost post = new BlogPost();
                post.dateCreated = DateTime.Now.AddMinutes(-1.0 * rnd.NextDouble() % 1000000);
                post.PersonId = userIDs.ElementAt(rnd.Next(userIDs.Count));
                post.title = "";
                int titleLength = rnd.Next(10);
                for(int j = 0; j < titleLength; j++)
                {
                    post.title += text[rnd.Next()%(text.Length)] + " ";
                }
                post.title.Remove(post.title.Length - 1, 1);

                int contentLength = rnd.Next(30);
                post.content = "";
                for (int j = 0; j < contentLength; j++)
                {
                    post.content += text[rnd.Next() % (text.Length)] + " ";
                }
                post.content.Remove(post.title.Length - 1, 1);

                int tagLength = rnd.Next(5);
                if (post.tags == null)
                    post.tags = new Collection<Tag>();
                
                for(int j = 0; j < tagLength; j++)
                {
                    string s = text[rnd.Next() % (text.Length)];
                    //Tag thisTag = blogCtx.Tags.Find(tag => tag.tagName == "");
                    //if (thisTag == null)
                    //{
                    //    thisTag = new Tag();
                    //    thisTag.tagName = s;
                    //    blogCtx.Tags.Add(thisTag);
                    //}
                }
                

            }
        }
    }
}