using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace MVC4GMAPS.Models
{
    public class AspNetUser
    {
        public int ID { get; set; }

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }

        public string HomeTown { get; set; }

        public DateTime BirthDate { get; set; }

        public string Discriminator { get; set; } 

        public string UserId { get; set; } 

        public string UserEmail { get; set; }
    }

    public class AspNetUserDBContext : DbContext
    {
        public DbSet<AspNetUser> AspNetUsers { get; set; }
    }
}