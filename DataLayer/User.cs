using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public ICollection<BlogPost> posts { get; set; }

        [Required]
        public ICollection<User> followers { get; set; }

        [Required]
        public ICollection<User> following { get; set; }

        [Required]
        public DateTime creation { get; set; }
        
        public DateTime birthday { get; set; }

        [Required]
        public string blogName { get; set; }

        [Required]
        public string location { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public Boolean isPrivate { get; set; }
    }
}
