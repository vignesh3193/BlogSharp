using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using System.Collections;

namespace BLogicLayer
{
    class UserViewLogic
    {
        public static void follow(int curruser, int author)
        {
            using (var context = new PersonContext())
            {
                Person Curruser = context.People.Find(curruser);
                Person Author = context.People.Find(author);
                Curruser.following.Add(Author);
                Author.followers.Add(Curruser);
                context.SaveChanges();
            }
        }

        public static double getUserRating(int userid)
        {
            double user_rating = 0.0;
            int count = 0;
            
            using (var context = new PersonContext())
            {
                Person user = context.People.Find(userid);
                ICollection < BlogPost > user_posts= user.posts;

                foreach (BlogPost temp in user_posts)
                {
                    count += temp.ratings.Count;
                    ICollection<Rating> post_ratings = temp.ratings;

                    foreach (Rating temp1 in post_ratings)
                    {
                        user_rating += temp1.ratingNumber;
                    }                
                }
            }
            user_rating = user_rating / count;
            return user_rating;
        }
    }
}
