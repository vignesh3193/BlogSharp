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
            List<String> trends = ActivityViewLogic.getTrends();
            ViewBag.trends = trends;
            //if user is logged in see public and followings posts
            if (Request.IsAuthenticated)
            {
                Person currUser = GeneralLogic.getLoggedInUser(personContext);
                List<KeyValuePair<int, string>> recommendations = new List<KeyValuePair<int, string>>();
                GeneralLogic.getRecommendedPeopleFor(personContext, currUser).ForEach(person => recommendations.Add(new KeyValuePair<int, string>(person.Id, person.FirstName + " " + person.LastName)));
                ViewBag.recommendations = recommendations;
                List<BlogPost> blogPosts=new List<BlogPost>();
                List<BlogPost> blogList = personContext.BlogPosts.OrderByDescending(blog => blog.dateCreated).ToList();
                foreach (BlogPost p in blogList)
                {
                    if (p.person.followers.Contains(currUser) || !p.person.isPrivate)
                    {
                        blogPosts.Add(p);
                    }
                }
                return View(blogPosts);
            
            } else
            {
                //if user is not logged in, only see public posts
                List<BlogPost> blogPosts = new List<BlogPost>();
                foreach (BlogPost p in personContext.BlogPosts)
                {
                    if(!p.person.isPrivate)
                    {
                        blogPosts.Add(p);
                    }
                }
                return View(blogPosts);
            }
           
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