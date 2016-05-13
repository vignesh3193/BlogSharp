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

        [DataType(DataType.Date)]
        [Display(Name ="Date Created")]
        public DateTime dateCreated { get; set; }

        [Display(Name ="Blog Title")]
        public string title { get; set; }

        [Display(Name ="Blog Content")]
        [DataType(DataType.MultilineText)]
        public string content { get; set; }

        public virtual ICollection<Person> userRatings { get; set; }

    }

    public class BlogPostCreateViewModel
    {
        [Display(AutoGenerateField = false)]
        public int PersonId { get; set; }

        [Display(Name = "Tags")]
        public string tags { get; set; }

        [Display(Name = "Blog Title")]
        [DataType(DataType.Text)]
        public string title { get; set; }

        [Display(Name = "Blog Content")]
        [DataType(DataType.MultilineText)]
        public string content { get; set; }



    }

    public class BlogPostDetailsViewModel
    {
        [Display(Name = "Author")]
        public Person author { get; set; }

        [Display(Name = "Date Created")]
        public DateTime date { get; set; }

        [Display(Name = "Tags")]
        public ICollection<Tag> tags { get; set; }

        [Display(Name = "Blog Title")]
        [DataType(DataType.Text)]
        public string title { get; set; }

        public int blogID { get; set; }

        [Display(Name = "Blog Content")]
        [DataType(DataType.MultilineText)]
        public string content { get; set; }

        [Display(Name ="Comments")]
        [DataType(DataType.MultilineText)]
        public ICollection<Comment> comments { get; set; }

        [DataType(DataType.MultilineText)]
        public string newComment { get; set; }

        public ICollection<Rating> ratings { get; set; }

    }
}
