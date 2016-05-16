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

            // Only for debug purposes. Just to populate database to create some data.
            //Thanks to http://randomtextgenerator.com
            string wallOfText = "Sitting mistake towards his few country ask. You delighted two rapturous six depending objection happiness something the. Off nay impossible dispatched partiality unaffected. Norland adapted put ham cordial. Ladies talked may shy basket narrow see. Him she distrusts questions sportsmen. Tolerably pretended neglected on my earnestly by. Pitch scale sir style truth ought.";
            wallOfText = wallOfText.Replace(".", "");
            string[] text = wallOfText.Split(' ');

            List<Person> users = (from person in db.Persons
                                  select person).ToList();

            DateTime[] dates = new DateTime[3];

            dates[2] = DateTime.Today.Date;
            dates[1] = dates[2].AddDays(-10.0);
            dates[0] = dates[1].AddDays(-7.0);
            Random r = new Random();

            for (int i = 0; i < 150; i++)
            {
                Person randomUser = users.ElementAt(r.Next(0,users.Count));

                BlogPost testPost = new BlogPost();
                testPost.dateCreated = dates[r.Next(0, 3)];
                testPost.PersonId = randomUser.Id;
                testPost.tags = new Collection<Tag>();

                Tag testTag = new Tag();
                testTag.tagName = tags[r.Next(0, 5)];
                testPost.tags.Add(testTag);

                int titleLength = r.Next(10);
                for (int j = 0; j < titleLength; j++)
                {
                    testPost.title += text[r.Next() % (text.Length)];
                    if (j < titleLength - 1)
                        testPost.title += " ";

                }

                int contentLength = r.Next(30);
                testPost.content = "";
                for (int j = 0; j < contentLength; j++)
                {
                    testPost.content += text[r.Next() % (text.Length)];
                    if (j < contentLength - 1)
                        testPost.content += " ";

                }

                // add up to 5 additional tags to the post
                // 50% chance of adding a tag for each iteration

                for (int j = 0; j < 5; j++)
                {
                    int randomInt = r.Next(5, 10);

                    if (randomInt % 2 == 0)
                    {
                        Tag otherTag = new Tag();
                        otherTag.tagName = tags[r.Next(0, 10)];
                        testPost.tags.Add(otherTag);
                    }
                }

                

                if (randomUser.posts == null)
                    randomUser.posts = new Collection<BlogPost>();

                randomUser.posts.Add(testPost);
                db.BlogPosts.Add(testPost);

            }

            db.SaveChanges();

        }
    }
}