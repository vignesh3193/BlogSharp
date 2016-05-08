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

       // public ActionResult Search()
        //{
          //  return View();
 //       }

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

        //[HttpPost]
        public ActionResult Search(string s)
        {
            if(s.Length == 0)
            {
                RedirectToAction("Index", "Blog",null);
            }
            if (User.Identity.IsAuthenticated)
            {
                //Syd- come back and add to query to make sure it's either public or someone current user is following
                //Need to double check this query. I'm not sure if it works -Omer
                Person thisPerson = Helper.getLoggedInUser(db);
                List<BlogPost> posts =(from p in db.BlogPosts
                                       where p.tags.Any(tag => tag.tagName.Equals(s)
                                       && (!p.person.isPrivate || p.person.followers.Contains(thisPerson)))
                                       select p).ToList();
                return View(posts.ToList());
            } else
            {
                Person person = Helper.getLoggedInUser(db);
                List<BlogPost> posts = (from p in db.BlogPosts
                                        where p.tags.Any(tag => tag.tagName.Equals(s) &&
                                        !p.person.isPrivate)
                                        select p).ToList();
                return View(posts);
                
            }
        }


        // GET: BlogPosts/Create
        public ActionResult Create()
        {
            ViewBag.PersonId = new SelectList(db.Persons, "Id", "Email");
            ViewBag.currentTags = new List<string>();
            return View();
        }

        // POST: BlogPosts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "title,content,tags")] BlogPostCreateViewModel blogPost)
        {
            if (ModelState.IsValid)
            {
                Person thisPerson = (from user in db.Persons
                                     where user.Email == User.Identity.Name
                                     select user).FirstOrDefault();
                BlogPost newPost = new BlogPost();
                newPost.dateCreated = DateTime.Now;
                newPost.PersonId = thisPerson.Id;
                newPost.tags = new Collection<Tag>();
                var allTags = db.Tags.ToList();
                foreach(var s in blogPost.tags.Split(',')){
                    Tag thisTag = allTags.Find(tag => tag.tagName.Equals(s));
                    if(thisTag == null)
                    {
                        thisTag = new Tag();
                        thisTag.tagName = s;
                        db.Tags.Add(thisTag);
                    }
                    newPost.tags.Add(thisTag);
                }
                newPost.title = blogPost.title;
                newPost.content = blogPost.content;
                if (thisPerson.posts == null)
                    thisPerson.posts = new Collection<BlogPost>();
                thisPerson.posts.Add(newPost);
                db.BlogPosts.Add(newPost);
                db.SaveChanges();
                return RedirectToAction("ViewPosts",thisPerson.Id);
            }
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

        //displays profile based on user id, still need to add default redirection
        public ActionResult Profile(int id)
        {   
            var user = db.Persons.Find(id);
            var curruser = Helper.getLoggedInUser(db);
            //adding current user to viewbag so we can check in the View if the current user is already following this blog
            ViewBag.CurrUser = curruser;
            return View(user);
        }


        //next 2 functions take in id of the profile being viewed and updates their followers and following tables
        public ActionResult Follow(int Id)
        {
            using (db)
            {
                var curruser = Helper.getLoggedInUser(db);
                var toFollow = db.Persons.Find(Id);

                curruser.following.Add(toFollow);
                toFollow.followers.Add(curruser);
                db.SaveChanges();
                return RedirectToAction("Profile", new { id = Id });
            };
            
        }

        public ActionResult UnFollow(int Id)
        {
            using (db)
            {
                var curruser = Helper.getLoggedInUser(db);
                var toUnfollow = db.Persons.Find(Id);

                curruser.following.Remove(toUnfollow);
                toUnfollow.followers.Remove(curruser);
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