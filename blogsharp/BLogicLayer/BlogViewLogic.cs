using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;

namespace BLogicLayer
{

    public class BlogViewLogic
    {

        // This method assumes that the Person object is for a logged-in user
        // The blogID is the one that corresponds to the blog which a user is attempting to access
        public static int determineBlogViewCode(Person loggedUser, int blogID)
        {
            Person author = null;

            using (var context = new BlogContext())
            {
                author = context.BlogPosts.Find(blogID).person;
            }

            if (loggedUser.Id == author.Id)
            {
                // return 1, which will signify that the view should be from the author's perspective
                // (i.e., the author can edit the blog post in addition to the standard logged-in privileges)
                return 1;
            } else
            {
                // the logged-in user is not the author, so return 2, a code which will signify that
                // the view should only present standard logged-in privileges
                return 2;
            }
        }

        public static Person validateRouteID(string id, Person checkPerson, BlogContext db)
        {
            // This method will check to see if the user info should be
            // obtained through using a blogName or the int id for the blogger's database entry

            int parsedInt = 0;

            if (int.TryParse(id, out parsedInt) == true)
            {
                checkPerson = db.Persons.Find(parsedInt);
            }
            else
            {
                checkPerson = (from person in db.Persons
                               where person.blogName == id
                               select person).FirstOrDefault();
            }

            return checkPerson;
        }
    }
}
