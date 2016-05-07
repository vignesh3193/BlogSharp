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

  //  public class CommentContext : DbContext
   // {
   //     public virtual DbSet<Comment> Comments { get; set; }
    
       // public System.Data.Entity.DbSet<DataLayer.Person> Users { get; set; }

       // public System.Data.Entity.DbSet<DataLayer.BlogPost> Blogs { get; set; }


    //}
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int BlogPostId { get; set; }

        [ForeignKey("BlogPostId")]
        public virtual BlogPost blogPost { get; set; }
        
        [Required]
        public string Author { get; set; }

        [Required]
        public string contents { get; set; }

        [Required]
        public DateTime dateCreated { get; set; }
    }
}
