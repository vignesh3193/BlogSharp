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

        [Display(Name = "Ratings")]
        public virtual ICollection<Rating> ratings { get; set; }

        [Display(Name ="Comments")]
        public virtual ICollection<Comment> comments { get; set; }

        [Display(Name = "Tags")]
        public virtual ICollection<Tag> tags { get; set; }

        [Display(Name ="Date Created")]
        public DateTime dateCreated { get; set; }

        [Display(Name ="Title")]
        public string title { get; set; }

        [Display(Name ="Content")]
        public string content { get; set; }

        public virtual ICollection<Person> userRatings { get; set; }

    }

    public class BlogPostCreateViewModel
    {
        [Display(AutoGenerateField = false)]
        public int PersonId { get; set; }

        [Display(Name = "Tags")]
        [DataType(DataType.MultilineText)]
        public string tags { get; set; }

        [Display(Name = "Title")]
        [DataType(DataType.Text)]
        public string title { get; set; }

        [Display(Name = "Content")]
        [DataType(DataType.MultilineText)]
        public string content { get; set; }



    }
}
