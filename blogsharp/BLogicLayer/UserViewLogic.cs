﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using System.Collections;

namespace BLogicLayer
{
    public class UserViewLogic
    {
        public static void follow(int curruser, int author)
        {
            using (var context = new BlogContext())
            {
                Person Curruser = context.Persons.Find(curruser);
                Person Author = context.Persons.Find(author);
                Curruser.following.Add(Author);
                Author.followers.Add(Curruser);
                context.SaveChanges();
            }
        }

        public static double getUserRating(int userid)
        {
            double user_rating = 0.0;
            int count = 0;
            
            using (var context = new BlogContext())
            {
                Person user = context.Persons.Find(userid);
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
