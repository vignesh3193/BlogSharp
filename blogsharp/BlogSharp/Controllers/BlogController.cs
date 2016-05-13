using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLogicLayer;
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

        [HttpPost]
        public ActionResult Details([Bind(Include ="author,date,tags,title,blogID,content,comments,newComment,ratings")] BlogPostDetailsViewModel model, string ratings)
        {
            //   int rating = Int32.Parse(ratings);
            int rating = 0;
            if(ratings != null)
            {
                using (db)
                {
                    BlogPost b = db.BlogPosts.Find();
                    Rating r = new Rating();
                    r.blogPost = b;
                    r.BlogPostId = b.Id;
                    r.ratingNumber = rating;
                    r.username = User.Identity.Name;
                    b.ratings.Add(r);
                    db.SaveChanges();

                    return Redirect("/");
                }
            }

            Person currUser = GeneralLogic.getLoggedInUser(db);
            BlogPost blogPost = (from b in db.BlogPosts
                                 where (model.blogID == b.Id)
                                 select b).FirstOrDefault();
            Comment c = new Comment();
            c.Author = currUser.FirstName + " "+currUser.LastName;
            c.blogPost = blogPost;
            c.contents = model.newComment;
            c.dateCreated = DateTime.Now;
            
            if (blogPost.comments == null)
            {
                blogPost.comments = new Collection<Comment>();
                blogPost.comments.Add(c);
            } else
            {
                blogPost.comments.Add(c);
            }
           
            db.SaveChanges();
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
            if(details.ratings == null)
            {
                details.ratings = new Collection<Rating>();
            }

            if(details.comments == null)
            {
                details.comments = new Collection<Comment>();
                blogPost.comments = new Collection<Comment>();
            }
           

            if (blogPost == null)
            {
                return HttpNotFound();
            }
            double avgrating = 0.0; ;
            foreach(Rating r in blogPost.ratings)
            {
                avgrating += r.ratingNumber;
            }
            avgrating = avgrating / blogPost.ratings.Count;
            ViewBag.avgRating = avgrating;

            //return View(blogPost);
            if (thisPerson != null)
            {
                ViewBag.userID = thisPerson.Id;
            }else
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
        public ActionResult CreateSearch([Bind(Include = "title,dateCreated")] BlogPostCreateViewModel blogPost)
        {

            return RedirectToAction("Search", "Blog", new { s = blogPost.title.ToString(), date = DateTime.Now, rating = 0 });
        }

        public ActionResult Search(string s, DateTime? date, int rating = 0)
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
                //Person user = User.Identity;
                
                var thisPerson = GeneralLogic.getLoggedInUser(db);

                List<BlogPost> posts = (from p in db.BlogPosts
                                        where (p.tags.Any(tag => tag.tagName.Equals(s)) || (p.title.Contains(s))) && (thisPerson.following.Contains(p.person) || !p.person.isPrivate)
                                        select p).ToList();
                List<Person> people = (from u in db.Persons
                                       where u.FirstName.Equals(s)
                                       select u).ToList();
                ViewBag.posts = posts;
                ViewBag.people = people;
                return View(posts.ToList());
            }
            else //else, show only public posts
            {

                List<BlogPost> posts = (from p in db.BlogPosts
                                        where (p.tags.Any(tag => tag.tagName.Equals(s) || p.title.Contains(s)) &&
                                        !p.person.isPrivate)
                                        select p).ToList();
                List<Person> people = (from u in db.Persons
                                       where u.FirstName.Equals(s) && (!u.isPrivate)
                                       select u).ToList();
                ViewBag.posts = posts;
                ViewBag.people = people;
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
        public ActionResult EditBio(int? id)
        {
            Person checkPerson = db.Persons.Find(id);

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

    //displays profile based on user id, still need to add default redirection
    public ActionResult Profile(int id)
    {
        var user = db.Persons.Find(id);
        var curruser = GeneralLogic.getLoggedInUser(db);
        //adding current user to viewbag so we can check in the View if the current user is already following this blog
        ViewBag.CurrUser = curruser;
        return View(user);
    }


    //next 2 functions take in id of the profile being viewed and updates their followers and following tables
    public ActionResult Follow(int Id)
    {
        using (db)
        {
            var curruser = GeneralLogic.getLoggedInUser(db);
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
            var curruser = GeneralLogic.getLoggedInUser(db);
            var toUnfollow = db.Persons.Find(Id);

            curruser.following.Remove(toUnfollow);
            toUnfollow.followers.Remove(curruser);
            db.SaveChanges();
            return RedirectToAction("Profile", new { id = Id });
        };
    }

    public ActionResult Error()
        {
            return View();
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