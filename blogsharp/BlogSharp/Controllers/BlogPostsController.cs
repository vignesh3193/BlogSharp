using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataLayer;
using System.Collections.ObjectModel;
using BlogSharp.HelperFunctions;

namespace BlogSharp.Controllers
{
    public class BlogPostsController : Controller
    {
        private BlogContext db = new BlogContext();

        // GET: BlogPosts
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated) // If not logged in
                RedirectToAction("Account", "Login", "BlogPosts/Index");

            Person thisPerson = Helper.getLoggedInUser();

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

        // GET: BlogPosts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Create
        public ActionResult Create()
        {
            ViewBag.PersonId = new SelectList(db.Persons, "Id", "Email");
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                return RedirectToAction("Index");
            }

            ViewBag.PersonId = new SelectList(db.Persons, "Id", "Email", blogPost.PersonId);
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person thisPerson = Helper.getLoggedInUser();
            BlogPost blogPost = Helper.getBlogPosts(thisPerson).Find(post => post.Id == id);
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
        public ActionResult Edit([Bind(Include = "Id,PersonId,dateCreated,title,content")] BlogPost blogPost)
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
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
