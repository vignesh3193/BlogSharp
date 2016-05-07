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
    //public class BlogPostContext : DbContext
    //{
      //  public virtual DbSet<BlogPost> BlogPosts { get; set; }

       // public DbSet<DataLayer.Person> Users { get; set; }
       
    //}

    public class BlogPost
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PersonId { get; set; }

        [ForeignKey("PersonId")]
        public virtual Person person { get; set; }

        public ICollection<Rating> ratings { get; set; }

        public ICollection<Comment> comments { get; set; }

        public ICollection<string> tags { get; set; }

        public DateTime dateCreated { get; set; }

        public string title { get; set; }

        public string content { get; set; }
    
        public ICollection<Person> userRatings { get; set; }

    }
}
