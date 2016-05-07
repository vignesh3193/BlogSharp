using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class BlogContext : DbContext
    {
        public static BlogContext Create()
        {
            return new BlogContext();
        }
        public DbSet<Person> Persons { get; set; }
        public virtual DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
    }
}
