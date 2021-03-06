﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int BlogPostId { get; set; }

        [ForeignKey("BlogPostId")]
        public virtual BlogPost blogPost { get; set; }

        public int theAuthorID { get; set; }
        
        [Required]
        public string Author { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string contents { get; set; }

        [Required]
        public DateTime dateCreated { get; set; }
    }
}
