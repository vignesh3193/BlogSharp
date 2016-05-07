using DataLayer;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections.Generic;

namespace BlogSharp.HelperFunctions
{
    public class Helper
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
    }
}