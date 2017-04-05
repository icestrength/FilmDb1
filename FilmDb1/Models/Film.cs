using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace FilmDb1.Models
{
    public class Film
    {
        public int Id { get; set; }
      
        public string Name { get; set; }
        
        public string Description { get; set; }

        public byte[] Image { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
        public Film()
        {
            Categories = new List<Category>();
        }
    }
}