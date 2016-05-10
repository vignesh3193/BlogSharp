using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Mvc;
using BLogicLayer;
using BlogSharp.Models;
using DataLayer;
using System.Collections.ObjectModel;

namespace BlogSharp.Controllers
{
    public class HomeController : Controller
    {
        //private ApplicationDbContext person_db = new ApplicationDbContext();
        private BlogContext personContext = new BlogContext();
        public ActionResult Index()
        {
            ICollection<String> trends = ActivityViewLogic.getTrends();
            ViewBag.trends = trends;
            if(Request.IsAuthenticated)
            {
                Person currsuer = GeneralLogic.getLoggedInUser(personContext);
                List<BlogPost> blogPosts=new List<BlogPost>();
                foreach(BlogPost p in personContext.BlogPosts)
                {
                    if(p.person.followers.Contains(currsuer))
                    {
                        blogPosts.Add(p);
                    }
                }
                if (blogPosts.Count > 0)
                {
                    return View(blogPosts);
                } 
            }
            return View(personContext.BlogPosts);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Activity()
        {
            List<String> addresses = new List<String>();
            List<String> blogNames = new List<String>();

            using (personContext)
            {
                addresses = (from blogger in personContext.Persons
                             select blogger.location).ToList();

                blogNames = (from blogger in personContext.Persons
                             select blogger.blogName).ToList();
            }

            var jsonMaker = new JavaScriptSerializer();
            ViewBag.geocodes = jsonMaker.Serialize(ActivityViewLogic.retrieveGeoPoints(addresses, blogNames));
            ViewBag.blogNames = jsonMaker.Serialize(blogNames);
            return View();
        }
    }
}