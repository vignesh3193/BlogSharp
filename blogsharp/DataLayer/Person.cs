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
    public class Person
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        public virtual ICollection<BlogPost> posts { get; set; }

        public virtual ICollection<Person> followers { get; set; }

        public virtual ICollection<Person> following { get; set; }

        [Required]
        public DateTime creation { get; set; }
        
        [Required]
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

        [Required]
        [DataType(DataType.MultilineText)]
        public string bio { get; set; }
    }
}
