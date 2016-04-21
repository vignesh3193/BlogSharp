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
        public long totalRaters { get; set; }

        [Required]
        public long totalRatings { get; set; }

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
        public ICollection<User> ratings { get; set; }

    }
}
