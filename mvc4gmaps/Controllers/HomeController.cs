using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script;
using Newtonsoft.Json;
using MVC4GMAPS.Models;
using System.Xml;
using System.Globalization;

namespace MVC4GMAPS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string trip, string UserId, string code) //LGMG 3/3/14
        {
            List<Coordinate> MyCoordinates = new List<Coordinate>();
            
            if (String.IsNullOrEmpty(trip))
            {
                MyCoordinates.Add(new Coordinate() { ID = 1, Lat = 10, Lon = 10, Trip = 1 });
                MyCoordinates.Add(new Coordinate() { ID = 1, Lat = 20, Lon = 20, Trip = 1 });
                return View(MyCoordinates);       
            }
            else
            {
                ServiceLayer MyServiceObject = new ServiceLayer();
                MyCoordinates = MyServiceObject.GetDataBaseSlice(trip, UserId, code);
                return View(MyCoordinates);
            }
            
        }

        public ActionResult About()
        {
            ViewBag.Message1 = "MapMyTrips aims to become (in a near future) a lighter alternative to services like ViaMichelin."; 
            ViewBag.Message2 = "It allows multiple tracing, which might be interesting if you like car-travelling. You can insert your route record and check which roads are yet to be discovered...";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message1 = "Please contact MapMyTrips if your profile matches one of these:";
            ViewBag.Message2 = ">> You are a student and you have a particular doubt about Google Maps API / ASP.Net MVC Development";
            ViewBag.Message3 = ">> You have some free time and wouldn't mind spending 5 minutes telling us how you found MapMyTrips";
            ViewBag.Message4 = ">> You're extremely kind and wouldn't mind to express how helpful MapMyTrips was for you";
            ViewBag.Message5 = ">> You found a bug on MapMyTrips and you know how to replicate it over and over";
            //ViewBag.Message6 = ">> Your name is Mark Zuckerberg and you're considering purchasing this application";
            return View();
        }
    }
}