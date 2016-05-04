using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogSharp.Controllers
{
    public class BlogController : Controller
    {
        private DataLayer.BlogPostContext db = new DataLayer.BlogPostContext();
        private DataLayer.PersonContext peopleDb = new DataLayer.PersonContext();
        // GET: Blog
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search([Bind(Include = "KeyWord")] String s)
        {
            if (User.Identity.IsAuthenticated)
            {
                //Syd- come back and add to query to make sure it's either public or someone current user is following
                ICollection<DataLayer.BlogPost> posts = (ICollection<DataLayer.BlogPost>) (from p in db.BlogPosts where p.tags.Contains(s) select p);
                return View(posts.ToList());
            } else
            {
           
                ICollection<DataLayer.BlogPost> results = null;
                ICollection <DataLayer.BlogPost> posts = (ICollection<DataLayer.BlogPost>)(from p in db.BlogPosts where p.tags.Contains(s) select p);
                foreach(DataLayer.BlogPost item in posts){
                    DataLayer.Person person = (from u in peopleDb.People where u.Id == item.UserId select u).First();
         
                    if (!person.isPrivate)
                    {
                        results.Add(item);
                    }
                  
                }
                return View(results.ToList());
                
            }
           // return View();
        }
    }
}