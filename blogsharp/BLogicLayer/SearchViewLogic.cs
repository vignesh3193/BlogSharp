using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLogicLayer
{
    public class SearchViewLogic
    {
        public static ICollection<BlogPost> retrieveMostPopularPosts()
        {
            // First retrieve the 10 most recent blog posts

            IEnumerable<BlogPost> recentPosts = new List<BlogPost>();

            using (var context = new BlogContext())
            {
                recentPosts = (from post in context.BlogPosts
                               orderby post.dateCreated descending
                               select post).ToList();
            };

            recentPosts = recentPosts.Take(10);

            // Now calculate the popularity metric

            // This is calculated in realtime rather than in the database,
            // in order to ensure up to date popularity 

            // Popularity = 0.60 * number of userRatings + 0.40 * comments
            // Note: this formula is a simplified one of the one we initially provided in the proposal

            ICollection<Tuple<double, BlogPost>> popularityTuples = new List<Tuple<double, BlogPost>>();

            foreach (BlogPost post in recentPosts) {
                double popularityMetric = (0.60 * post.userRatings.Count) + (0.40 * post.comments.Count);
                popularityTuples.Add(new Tuple<double, BlogPost>(popularityMetric, post));
            }

            popularityTuples.OrderByDescending(x => x.Item1).ToList();


            // Create the final collection of popular posts, return it

            ICollection<BlogPost> mostPopularPosts = new List<BlogPost>();

            foreach (Tuple<double,BlogPost> postPair in popularityTuples)
            {
                mostPopularPosts.Add(postPair.Item2);
            }

            return mostPopularPosts;
        }
    }
}
