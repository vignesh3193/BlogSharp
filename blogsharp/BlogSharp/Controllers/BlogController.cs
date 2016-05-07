using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogSharp.HelperFunctions;
using System.Collections.ObjectModel;
using System.Data.Entity;

namespace BlogSharp.Controllers
{
    public class BlogController : Controller
    {
        private BlogContext db = new BlogContext();   
        // GET: Blog
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search()
        {
            return View();
        }

        // GET: BlogPosts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                RedirectToAction("Home" , "Index");
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
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
                    Person person = Helper.getLoggedInUser(db);
         
                    if (!person.isPrivate)
                    {
                        results.Add(item);
                    }
                  
                }
                return View(results.ToList());
                
            }
           // return View();
        }


        // GET: BlogPosts/Create
        public ActionResult Create()
        {
            ViewBag.PersonId = new SelectList(db.Persons, "Id", "Email");
            return View();
        }

        // POST: BlogPosts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "title,content")] BlogPost blogPost)
        {
            if (ModelState.IsValid)
            {
                Person thisPerson = (from user in db.Persons
                                     where user.Email == User.Identity.Name
                                     select user).FirstOrDefault();
                blogPost.dateCreated = DateTime.Now;
                blogPost.PersonId = thisPerson.Id;
                if (thisPerson.posts == null)
                    thisPerson.posts = new Collection<BlogPost>();
                thisPerson.posts.Add(blogPost);
                db.BlogPosts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Profile",thisPerson.Id);
            }

            ViewBag.PersonId = new SelectList(db.Persons, "Id", "Email", blogPost.PersonId);
            return View(blogPost);
        }

        // GET: BlogPosts
        public ActionResult ViewPosts()
        {
            if (!User.Identity.IsAuthenticated) // If not logged in
                RedirectToAction("Account", "Login", "BlogPosts/Index");

            Person thisPerson = Helper.getLoggedInUser(db);

            if (thisPerson != null) // 
            {
                var blogPosts = (from posts in db.BlogPosts
                                 where posts.PersonId == thisPerson.Id
                                 select posts).ToList<BlogPost>();
                if (blogPosts == null)
                    blogPosts = new List<BlogPost>();
                return View(blogPosts.ToList());
            }
            return View();

        }

        // GET: BlogPosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                RedirectToAction("Home", "Index");
            }
            Person thisPerson = Helper.getLoggedInUser(db);
            BlogPost blogPost = Helper.getBlogPosts(db, thisPerson).Find(post => post.Id == id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonId = new SelectList(db.Persons, "Id", "Email", blogPost.PersonId);
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PersonId,comments,dateCreated,person,ratings,tags,userRatings,title,content")] BlogPost blogPost)
        {
            if (ModelState.IsValid)
            {
                db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PersonId = new SelectList(db.Persons, "Id", "Email", blogPost.PersonId);
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                RedirectToAction("Home", "Index");
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.BlogPosts.Find(id);
            db.BlogPosts.Remove(blogPost);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Profile(int id)
        {   
            var user = db.Persons.Find(id);
            //ViewBag.posts = Helper.getBlogPosts(db, user);
           var curruser = from a in db.Persons where a.Email.Equals(User.Identity.Name) select a;
            ViewBag.CurrUser = curruser.First();
            return View(user);
        }
        
        public ActionResult Follow(int Id)
        {
            using (db)
            {
                var curruser = from a in db.Persons where a.Email.Equals(User.Identity.Name) select a;
                var toFollow = db.Persons.Find(Id);

                curruser.First().following.Add(toFollow);
                toFollow.followers.Add(curruser.First());
                db.SaveChanges();
                return RedirectToAction("Profile", new { id = Id });
            };
            
        }
    
        public ActionResult UnFollow(int Id)
        {
            using (db)
            {
                var curruser = from a in db.Persons where a.Email.Equals(User.Identity.Name) select a;
                var toUnfollow = db.Persons.Find(Id);

                curruser.First().following.Remove(toUnfollow);
                toUnfollow.followers.Remove(curruser.First());
                db.SaveChanges();
                return RedirectToAction("Profile", new { id = Id });
            };
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}