using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace MVC4GMAPS.Models
{
    public class Location
    {
        public int ID { get; set; }
        
        [Required]
        public string Country { get; set; }

        public string State { get; set; }       //LGMG_DB_MODIF_14_02_25

        [Required]
        public string City { get; set; }

        public string Number { get; set; }      //LGMG_DB_MODIF_14_02_25
        public string Street { get; set; }      //LGMG_DB_MODIF_14_02_25
        public string Village { get; set; }     //LGMG_DB_MODIF_14_02_25
        public string PostalCode { get; set; }  //LGMG_DB_MODIF_14_02_25
                
        public double Lat { get; set; }

        public double Lon { get; set; }

        [Required]
        public int Trip { get; set; }

        public string UserName {get; set; }     //LGMG 30/1/2014

        public string UserId { get; set; }      //LGMG 30/1/2014

        public string UserEmail { get; set; }   //LGMG 30/1/2014

        public string LoginProvider { get; set; } //LGMG 3/3/14
    }

    public class AspNetUser
    {
        public string ID { get; set; }

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }

        public string UserEmail { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Discriminator { get; set; }
    }

    public class AspNetUserLogin
    {
        [Key] public string UserId { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
    }

    /*public class AspNetUserDBContext : DbContext
    {
        public DbSet<AspNetUser> AspNetUsers { get; set; }
    }*/

    public class LocationDBContext : DbContext
    {
        public DbSet<Location> Locations { get; set; }
        public DbSet<AspNetUser> AspNetUsers { get; set; }
        public DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
    }
}