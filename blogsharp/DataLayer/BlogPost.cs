using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class BlogPostContext : DbContext
    {
        public virtual DbSet<BlogPost> BlogPosts { get; set; }

        public System.Data.Entity.DbSet<DataLayer.Person> Users { get; set; }

        public System.Data.Entity.DbSet<DataLayer.Comment> Comments { get; set; }

        public System.Data.Entity.DbSet<DataLayer.Rating> Ratings { get; set; }


    }

    public class BlogPost
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual Person user { get; set; }

        [Required]
        public ICollection<Rating> ratings { get; set; }

        [Required]
        public ICollection<Comment> comments { get; set; }

        [Required]
        public ICollection<string> tags { get; set; }

        [Required]
        public DateTime dateCreated { get; set; }

        [Required]
        public string title { get; set; }

        [Required]
        public string content { get; set; }

        [Required]
        public ICollection<Person> userRatings { get; set; }

    }
}
