using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace MVC4GMAPS.Models
{
    public class Coordinate
    {
        public int ID { get; set; }

        [Required]
        public double Lat { get; set; }

        [Required]
        public double Lon { get; set; }

        [Required]
        public int Trip { get; set; }

        [Required]
        public string Country { get; set; }

        public string State { get; set; }       //LGMG_DB_MODIF_14_02_25

        [Required]
        public string City { get; set; }

        public string Number { get; set; }      //LGMG_DB_MODIF_14_02_25
        public string Street { get; set; }      //LGMG_DB_MODIF_14_02_25
        public string Village { get; set; }     //LGMG_DB_MODIF_14_02_25
        public string PostalCode { get; set; }  //LGMG_DB_MODIF_14_02_25
        public string PostUser { get; set; }    //LGMG 3/3/14

        public List<string> Weather;
        public List<string> Wind;
        public List<int> MaxTemp;
        public List<int> MinTemp;
        public List<int> Humidity;
        public List<int> Symbol;
    }

    public class CoordinateDBContext : DbContext
    {
        public DbSet<Coordinate> Coordinates { get; set; }
    }
}