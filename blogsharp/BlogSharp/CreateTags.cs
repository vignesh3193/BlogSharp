using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataLayer;
using System.Collections.ObjectModel;

namespace BlogSharp
{
    public class CreateTags
    {
        public static void generate(BlogContext db)
        {
            string[] tags = new string[10];

            tags[0] = "dogs";
            tags[1] = "cats";
            tags[2] = "ferrari";
            tags[3] = "#coolstory";
            tags[4] = "aMAZing";
            tags[5] = "tooCool4U";
            tags[6] = "interesting";
            tags[7] = "studentlife";
            tags[8] = "yello!";
            tags[9] = "#thankful";

            string content = "generic blog post";

            Person[] users = new Person[2];

            users[0] = (from user in db.Persons
                       where user.Email == "jg@blogsharp.com"
                       select user).FirstOrDefault();

            users[1] = (from user in db.Persons
                        where user.Email == "vk@blogsharp.com"
                        select user).FirstOrDefault();

            DateTime[] dates = new DateTime[3];

            dates[2] = DateTime.Today.Date;
            dates[1] = dates[2].AddDays(-10.0);
            dates[0] = dates[1].AddDays(-20.0);
            Random r = new Random();

            for (int i = 100; i < 200; i++)
            {
                Person randomUser = users[r.Next(0, 2)];
                BlogPost testPost = new BlogPost();
                testPost.dateCreated = dates[r.Next(0, 3)];
                testPost.PersonId = randomUser.Id;
                testPost.tags = new Collection<Tag>();

                Tag testTag = new Tag();
                testTag.tagName = tags[r.Next(0, 10)];

                testPost.title = "makeTag" + i.ToString();
                testPost.content = content + i.ToString();
                testPost.tags.Add(testTag);
                if (randomUser.posts == null)
                    randomUser.posts = new Collection<BlogPost>();
                randomUser.posts.Add(testPost);
                db.BlogPosts.Add(testPost);

            }

            db.SaveChanges();

        }
    }
}