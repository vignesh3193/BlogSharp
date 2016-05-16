using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLogicLayer;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Web.Script.Serialization;

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

        [HttpPost]
        public ActionResult Details(BlogPostDetailsViewModel model)
        {
            Person currUser = GeneralLogic.getLoggedInUser(db);
            BlogPost blogPost = (from b in db.BlogPosts
                                 where (model.blogID == b.Id)
                                 select b).FirstOrDefault();

            if (model.newRating != null)
            {
                using (db)
                {
                    Rating r = new Rating();
                    r.blogPost = blogPost;
                    r.BlogPostId = blogPost.Id;
                    r.ratingNumber = (int)model.newRating;
                    r.username = User.Identity.Name;
                    blogPost.ratings.Add(r);
                    db.SaveChanges();
                }
            }

            if (model.newComment != null)
            {
                Comment c = new Comment();
                c.theAuthorID = currUser.Id;
                c.Author = currUser.FirstName + " " + currUser.LastName;
                c.blogPost = blogPost;
                c.contents = model.newComment;
                c.dateCreated = DateTime.Now;

                if (blogPost.comments == null)
                {
                    blogPost.comments = new Collection<Comment>();
                    blogPost.comments.Add(c);
                }
                else
                {
                    blogPost.comments.Add(c);
                }

                db.SaveChanges();
            }
            return RedirectToAction("Details", "Blog", model.blogID);
        }

        // GET: BlogPosts/Details/5
        public ActionResult Details(int? id)
        {
            Person thisPerson = GeneralLogic.getLoggedInUser(db);
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            BlogPostDetailsViewModel details = new BlogPostDetailsViewModel();
            details.author = blogPost.person;
            details.blogID = id.Value;
            details.comments = blogPost.comments;
            details.content = blogPost.content;
            details.date = blogPost.dateCreated;
            details.tags = blogPost.tags;
            details.title = blogPost.title;
            details.ratings = blogPost.ratings;
            if (details.ratings == null)
            {
                details.ratings = new Collection<Rating>();
            }

            if (details.comments == null)
            {
                details.comments = new Collection<Comment>();
                blogPost.comments = new Collection<Comment>();
            }


            if (blogPost == null)
            {
                return HttpNotFound();
            }
            double avgrating = 0.0; ;
            foreach (Rating r in blogPost.ratings)
            {
                avgrating += r.ratingNumber;
            }
            avgrating = avgrating / blogPost.ratings.Count;
            ViewBag.avgRating = avgrating;

            //return View(blogPost);
            if (thisPerson != null)
            {
                ViewBag.userID = thisPerson.Id;
            }
            else
            {
                ViewBag.userID = null;
            }
            //ViewBag.userID = thisPerson.Id;
            return View(details);
        }

        //Blog/CreateSearch is a form that takes in keywords that will correspond to tags, titles, or users
        public ActionResult CreateSearch()
        {
            return View();
        }

        //pass the words from the form to the search method and return results
        [HttpPost]
        public ActionResult CreateSearch([Bind(Include = "title,date,newRating")] BlogPostDetailsViewModel blogPost)
        {

            return RedirectToAction("Search", "Blog", new { s = blogPost.title.ToString(), date = blogPost.date, rating = blogPost.newRating });
        }

        public ActionResult Search(string s, DateTime? date, int? rating)
        {
            //if no search words redirect to create search form
            if (s == null)
            {
                return RedirectToAction("CreateSearch", "Blog");
            }
            //if user is logged in, show posts that have either tags or titles containing the search param
            //and only of authors that are not private, or that the current user is following.
            if (User.Identity.IsAuthenticated)
            {
               

                var thisPerson = GeneralLogic.getLoggedInUser(db);
                //get posts by tag, title or users
                List<BlogPost> posts = (from p in db.BlogPosts
                                        where (p.tags.Any(tag => tag.tagName.Equals(s)) || (p.title.Contains(s))) && (thisPerson.following.Contains(p.person) || !p.person.isPrivate)
                                        select p).ToList();
                List<BlogPost> results = new List<BlogPost>();
                foreach (BlogPost p in posts)
                {
                    results.Add(p);
                }
                if (date != DateTime.Now)
                {

                    //remove posts that were created earlier than the specified date from the collection of results
                    foreach (BlogPost p in posts)
                    {
                        if (p.dateCreated.CompareTo(date) < 0)
                        {
                            results.Remove(p);
                        }
                    }
                    if (rating != null && posts.Count > 0)
                    {
                        foreach (BlogPost p in posts)
                        {
                            //get the average rating and compare to the parameter rating
                            double avg = GeneralLogic.getRatingOPost(p);
                            if ((avg <= (double)rating) && results.Contains(p))
                            {
                                results.Remove(p);
                            }
                        }
                    }
                }
                List<Person> people = (from u in db.Persons
                                       where u.FirstName.Contains(s) || u.LastName.Contains(s)
                                       select u).ToList();

                List<Person> blogs = (from b in db.Persons
                                      where b.blogName.Contains(s)
                                      select b).ToList();
                ViewBag.posts = posts;
                ViewBag.people = people;
                ViewBag.blogs = blogs;
                return View(posts.ToList());
            }
            else //else, show only public posts
            {

                List<BlogPost> posts = (from p in db.BlogPosts
                                        where (p.tags.Any(tag => tag.tagName.Equals(s) || p.title.Contains(s)) &&
                                        !p.person.isPrivate)
                                        select p).ToList();
                List<BlogPost> results = new List<BlogPost>();
                foreach(BlogPost p in posts)
                {
                    results.Add(p);
                }
                if (date != DateTime.Now)
                {
                   
                    
                    foreach(BlogPost p in posts)
                    {
                        if(p.dateCreated.CompareTo(date) < 0)
                        {
                            results.Remove(p);
                        }
                    }
                    if(rating != null && posts.Count > 0)
                    {
                        foreach(BlogPost p in posts)
                        {
                            double avg = GeneralLogic.getRatingOPost(p);
                            if((avg <= (double)rating) && results.Contains(p))
                            {
                                results.Remove(p);
                            }
                        }
                    }
                }
                List<Person> people = (from u in db.Persons
                                       where (u.FirstName.Contains(s) || u.LastName.Contains(s)) && (!u.isPrivate)
                                       select u).ToList();
                List<Person> blogs = (from b in db.Persons
                                      where (b.blogName.Contains(s)) && !b.isPrivate
                                      select b).ToList();
                ViewBag.posts = results;
                ViewBag.people = people;
                ViewBag.blogs = blogs;
                return View(posts.ToList());

            }
        }


        // GET: BlogPosts/Create
        public ActionResult Create()
        {
            ViewBag.PersonId = new SelectList(db.Persons, "Id", "Email");
            //ViewBag.currentTags = new List<string>();
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
                foreach (var s in blogPost.tags.Split(','))
                {
                    string trimmed = s.Trim();
                    Tag thisTag = allTags.Find(tag => tag.tagName.Equals(trimmed));
                    if (thisTag == null)
                    {
                        thisTag = new Tag();
                        thisTag.tagName = trimmed;
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
                return RedirectToAction("Details", "Blog", new { id = newPost.Id });
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Person thisPerson = GeneralLogic.getLoggedInUser(db);
            BlogPost blogPost = GeneralLogic.getBlogPosts(db, thisPerson).Find(post => post.Id == id);
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

        [HttpGet]
        public ActionResult EditBio(string id)
        {
            Person checkPerson = null;
            checkPerson = BlogViewLogic.validateRouteID(id, checkPerson, db);

            if (checkPerson == null)
            {
                return RedirectToAction("Error", "Blog");
            }

            if (checkPerson.Email == User.Identity.Name)
            {
                ViewBag.editID = id;
                BioViewModel userBio = new BioViewModel();
                userBio.bio = checkPerson.bio;
                return View(userBio);
            }
            else
            {
                return RedirectToAction("Error", "Blog");
            }
        }

        [HttpPost]
        public ActionResult EditBio(BioViewModel editedBio)
        {
            if (ModelState.IsValid)
            {
                using (db)
                {
                    Person userToEdit = (from person in db.Persons
                                         where person.Email == User.Identity.Name
                                         select person).FirstOrDefault();
                    userToEdit.bio = editedBio.bio;
                    db.SaveChanges();
                }

                return View(editedBio);
            }
            else
            {
                ViewBag.errorMsg = "An error occurred when trying to update your bio.  Please try again in a few moments.";
                return View(editedBio);
            }

        }


        // GET: BlogPosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                RedirectToAction("Index", "Home");
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


        public ActionResult Profile(string id)
        {

            Person checkPerson = null;
            checkPerson = BlogViewLogic.validateRouteID(id, checkPerson, db);

            if (checkPerson == null)
            {
                return RedirectToAction("Error", "Blog");
            }

        var curruser = GeneralLogic.getLoggedInUser(db);
        //adding current user to viewbag so we can check in the View if the current user is already following this blog
        ViewBag.CurrUser = curruser;

            double AvgRating = UserViewLogic.getUserRating(checkPerson.Id);
            ICollection<String> CommonTags = UserViewLogic.getCommonTags(checkPerson);
            ViewBag.AvgRating = AvgRating;
            ViewBag.cTags = CommonTags;
            return View(checkPerson);
    }


        //next 2 functions take in id of the profile being viewed and updates their followers and following tables
        public ActionResult Follow(int Id)
        {
            using (db)
            {
                var curruser = GeneralLogic.getLoggedInUser(db);
                var toFollow = db.Persons.Find(Id);
                if (toFollow.isPrivate) // If trying to follow a private person
                {
                    if (toFollow.notifications == null)
                        toFollow.notifications = new List<Person>();

                    if (!toFollow.notifications.Contains(curruser)) // If not already requested
                    {
                        toFollow.notifications.Add(curruser);
                        db.SaveChanges();
                    }
                }
                else // If not private, just grant automatically.
                {
                    curruser.following.Add(toFollow);
                    toFollow.followers.Add(curruser);
                    db.SaveChanges();
                }
            };
            return RedirectToAction("Profile", new { id = Id });
        }

        public ActionResult UnFollow(int Id)
        {
            using (db)
            {
                var curruser = GeneralLogic.getLoggedInUser(db);
                var toUnfollow = db.Persons.Find(Id);

                curruser.following.Remove(toUnfollow);
                toUnfollow.followers.Remove(curruser);
                db.SaveChanges();
                return RedirectToAction("Profile", new { id = Id });
            };
        }

        public ActionResult FollowRequests ()
        {
            Person currUser = GeneralLogic.getLoggedInUser(db);
            return View(currUser.notifications);
        }

        public ActionResult Approve(int Id)
        {
            Person currUser = GeneralLogic.getLoggedInUser(db);
            using (db)
            {
                Person follower = db.Persons.ToList().Find(p => p.Id == Id);
                currUser.notifications.Remove(follower);
                currUser.followers.Add(follower);
                follower.following.Add(currUser);

                db.SaveChanges();
            }
            return RedirectToAction("FollowRequests");
        }

        public ActionResult Reject(int Id)
        {
            Person currUser = GeneralLogic.getLoggedInUser(db);
            using (db)
            {
                Person follower = db.Persons.ToList().Find(p => p.Id == Id);
                currUser.notifications.Remove(follower);
                db.SaveChanges();
            }
            return RedirectToAction("FollowRequests");
        }

        public ActionResult Error()
        {
            return View();
        }

        // methods for AJAX
        public string GetTopBloggers()
        {
            var jsonMaker = new JavaScriptSerializer();

            return jsonMaker.Serialize(ActivityViewLogic.getTopBloggers());
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