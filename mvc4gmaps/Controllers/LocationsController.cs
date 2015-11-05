using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC4GMAPS.Models;
using System.Xml;
using System.Globalization;
using System.Web.Security;
using System.Threading;
using System.DirectoryServices.AccountManagement;

namespace MVC4GMAPS.Controllers
{
    public class LocationsController : Controller
    {
        private LocationDBContext db = new LocationDBContext();
        private Location MyRow = new Location();
        //private AspNetUserDBContext dbasp = new AspNetUserDBContext();
        
        // GET: /Locations/
        //public ActionResult Index()
        //{
        //    return View(db.Locations.ToList());
        //}

        // POST: /Locations/
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        
        public struct Coordinates
        {
            public string LAT { get; set; }
            public string LON { get; set; }
            public double Lat { get; set; }
            public double Lon { get; set; }
        }

        public bool LastPressedButtonWasSaveTrip = false;
        public string LastSavedTrip = "0";
        public int MyTrip = 0;

        public ActionResult Index(string Command, string trip, string origin, string waypoint1, string waypoint2, string waypoint3, string waypoint4, string waypoint5, string waypoint6, string waypoint7, string waypoint8, string waypoint9, string destination)
        {
            List<string> CoordinatesLink = new List<string>();
            List<Coordinates> CoordinatesStruct = new List<Coordinates>();
            List<int> KeyList = new List<int>(); //NOVO 23-12-13
            List<Location> MyLocations = new List<Location>();

            //bool LastPressedButtonWasSaveTrip = false;
            //string LastSavedTrip = "0";
            //int MyTrip = 0;

            //LGMG 3/3/14 DAQUI
            string UserLink;
            int MyCount;
            if (Command == "POST ALL")
            {
                MyLocations = db.Locations.Where(location => location.UserName == User.Identity.Name).ToList();
                MyCount = MyLocations.Count();

                if (MyCount >= 2)
                {
                    UserLink = "http://mapmytrips.apphb.com/?trip=0&UserId=" + MyLocations[0].UserId;
                    MyLocations.ElementAt(0).Village = UserLink;
                    return View(MyLocations);
                }
            }

            if (Command == "POST TRIP NUM:")
            {
                MyLocations = db.Locations.Where(location => location.UserName == User.Identity.Name).ToList();
                MyCount = MyLocations.Count();            

                if (MyCount >= 2)
                {
                    UserLink = "http://mapmytrips.apphb.com/?trip=" + MyLocations.ElementAt(0).Trip + "&UserId=" + MyLocations[0].UserId;
                    MyLocations.ElementAt(0).Village = UserLink;
                    return View(MyLocations);
                }
            }
            //LGMG 3/3/14 ATE AQUI
            if (Command=="SHOW TRIP NUM:")
            {
                int count = 0, i;

                db.Database.CreateIfNotExists(); //NOVO 24/1/2014

                if (String.IsNullOrEmpty(trip))
                {
                    return View(db.Locations.ToList());
                }
                else
                {
                    MyTrip = Convert.ToInt32(trip);
                    

                    if (MyTrip == 0) //print all routes
                    {
                        if (User.Identity.IsAuthenticated == false)
                            MyLocations = db.Locations.Where(location => location.UserName == null).ToList();
                        else
                            MyLocations = db.Locations.Where(location => location.UserName == User.Identity.Name).ToList();

                        MyLocations = db.Locations.ToList();

                        count = MyLocations.Count();

                        for (i = 0; i < count; i++)
                        {
                            KeyList.Add(MyLocations[i].ID);
                        }

                        if (count >= 2)
                        {
                            for (i = 0; i < count; i++)
                            {
                                CoordinatesLink.Add("http://maps.google.com/maps/api/geocode/xml?address=" + MyLocations[i].City.ToString() + "," + MyLocations[i].State.ToString() + "," + MyLocations[i].Country.ToString() + "&sensor=false"); //LGMG_DB_MODIF_14_02_25

                                XmlDocument MyXML = new XmlDocument();
                                MyXML.Load(CoordinatesLink[i]);
                                XmlElement root = MyXML.DocumentElement;
                                XmlNode node = root.SelectSingleNode("/GeocodeResponse/result/geometry/location"); //NOVO

                                string LAT = node["lat"].InnerText;
                                string LON = node["lng"].InnerText;
                                double Lat = double.Parse(LAT, CultureInfo.InvariantCulture);
                                double Lon = double.Parse(LON, CultureInfo.InvariantCulture);
                                var ROW = db.Locations.Find(KeyList[i]); //NOVO 23-12-13
                                ROW.Lat = Lat;
                                ROW.Lon = Lon;
                                db.SaveChanges();
                            }
                            return RedirectToAction("Index", "Home", new { trip });
                        }
                        else
                        {
                            if (User.Identity.IsAuthenticated == false)
                                return View(db.Locations.Where(location => location.UserName == null).ToList());
                            else
                                return View(db.Locations.Where(location => location.UserName == User.Identity.Name).ToList());
                        }
                    }
                    else
                    {
                        if (User.Identity.IsAuthenticated == false)
                            MyLocations = db.Locations.Where(location => (location.UserName == null) && (location.Trip == MyTrip)).ToList();
                        else
                            MyLocations = db.Locations.Where(location => (location.UserName == User.Identity.Name) && (location.Trip == MyTrip)).ToList();

                        count = MyLocations.Count();

                        for (i = 0; i < count; i++)
                        {
                            KeyList.Add(MyLocations[i].ID);
                        }

                        if (count >= 2)
                        {
                            for (i = 0; i < count; i++)
                            {
                                CoordinatesLink.Add("http://maps.google.com/maps/api/geocode/xml?address=" + MyLocations[i].City.ToString() + "," + MyLocations[i].City.ToString() + "," + MyLocations[i].Country.ToString() + "&sensor=false");

                                XmlDocument MyXML = new XmlDocument();
                                MyXML.Load(CoordinatesLink[i]);
                                XmlElement root = MyXML.DocumentElement;
                                XmlNode node = root.SelectSingleNode("/GeocodeResponse/result/geometry/location");

                                string LAT = node["lat"].InnerText;
                                string LON = node["lng"].InnerText;
                                double Lat = double.Parse(LAT, CultureInfo.InvariantCulture);
                                double Lon = double.Parse(LON, CultureInfo.InvariantCulture);

                                var ROW = db.Locations.Find(KeyList[i]); //NOVO 23-12-13
                                ROW.Lat = Lat;
                                ROW.Lon = Lon;
                                db.SaveChanges();
                            }
                            return RedirectToAction("Index", "Home", new { trip });
                        }
                        else
                        {
                            if (User.Identity.IsAuthenticated == false)
                                return View(db.Locations.Where(location => (location.UserName == null) && (location.Trip == MyTrip)).ToList());
                            else
                                return View(db.Locations.Where(location => (location.UserName == User.Identity.Name) && (location.Trip == MyTrip)).ToList());
                        }
                    }
                }
            }

            if ((Command == "SAVE TRIP") || (Command == "SAVE&SHOW TRIP") || (Command == "SHOW ALL"))
            {
                CoordinatesLink.Clear();
                int count=1, i=0;

                db.Database.CreateIfNotExists(); //NOVO 24/1/2014

                if ((String.IsNullOrEmpty(origin) || String.IsNullOrEmpty(destination)) && ((Command == "SAVE&SHOW TRIP") || ((Command == "SAVE TRIP"))))
                {
                    return View(db.Locations.ToList());
                }
                else if ((!String.IsNullOrEmpty(origin) && !String.IsNullOrEmpty(destination)))
                {                    
                    if ((Command=="SAVE TRIP") || (!LastPressedButtonWasSaveTrip))
                    {
                        //var abc = origin;
                        //var bca = waypoint1;
                        //var cab = destination;
                        if (Command == "SAVE TRIP")
                            LastPressedButtonWasSaveTrip = true;
                        else
                            LastPressedButtonWasSaveTrip = false;

                        //Antes de tudo, colocar as cidades na BD//
                        //Procurar por uma trip livre na BD
                        while (count != 0)
                        {
                            i++;

                            if (User.Identity.IsAuthenticated == false)
                                MyLocations = db.Locations.Where(location => (location.UserName == null) && (location.Trip == i)).ToList();
                            else
                                MyLocations = db.Locations.Where(location => (location.UserName == User.Identity.Name) && (location.Trip == i)).ToList();

                            //MyLocations=db.Locations.Where(location => location.Trip == i).ToList();
                            count = MyLocations.Count();
                        }
                        MyTrip = i;

                        List<AspNetUser> MyAspNetUser = new List<AspNetUser>();
                        List<AspNetUserLogin> MyAspNetUserLogin = new List<AspNetUserLogin>();
                        MyAspNetUser = db.AspNetUsers.ToList();
                        MyAspNetUserLogin = db.AspNetUserLogins.ToList();
                        MyLocations = db.Locations.ToList();
                        Location local = new Location();
                        
                        int n_links = 0;

                        i = 0;
                        string CityStateCountry = null;

                        while (i <= 10)
                        {
                            if (i == 0)
                                CityStateCountry = origin;
                            if (i == 1)
                                CityStateCountry = waypoint1;
                            if (i == 2)
                                CityStateCountry = waypoint2;
                            if (i == 3)
                                CityStateCountry = waypoint3;
                            if (i == 4)
                                CityStateCountry = waypoint4;
                            if (i == 5)
                                CityStateCountry = waypoint5;
                            if (i == 6)
                                CityStateCountry = waypoint6;
                            if (i == 7)
                                CityStateCountry = waypoint7;
                            if (i == 8)
                                CityStateCountry = waypoint8;
                            if (i == 9)
                                CityStateCountry = waypoint9;
                            if (i == 10)
                            {
                                CityStateCountry = destination;
                            }
                            i++;
                            if (CityStateCountry == null)
                                i = 10;
                            else
                            {
                                string str = "My Test String";
                                int index = str.IndexOf(' ');
                                index = str.IndexOf(' ', index + 1);
                                string result = str.Substring(0, index);
                                
                                int MyCommaCounter=0;
                                foreach(char c in CityStateCountry) {
                                    if(c==',')
                                        MyCommaCounter++;
                                }
                                
                                if (MyCommaCounter==1)
                                {
                                    local.Trip = MyTrip;
                                    local.City = CityStateCountry.Substring(0, CityStateCountry.IndexOf(","));
                                    CityStateCountry = CityStateCountry.Substring(CityStateCountry.IndexOf(",") + 1);
                                    local.Country = CityStateCountry;
                                }
                                else //2
                                {
                                    local.Trip = MyTrip;
                                    local.City = CityStateCountry.Substring(0, CityStateCountry.IndexOf(","));
                                    CityStateCountry = CityStateCountry.Substring(CityStateCountry.IndexOf(",") + 1);
                                    local.State = CityStateCountry.Substring(0, CityStateCountry.IndexOf(","));
                                    CityStateCountry = CityStateCountry.Substring(CityStateCountry.IndexOf(",") + 1);
                                    local.Country = CityStateCountry;
                                }
                                
                                //LGMG 3/3/14 DAQUI //WEATHERCAST
                                if (CityStateCountry != null)
                                {
                                    if (local.State != null)
                                        CoordinatesLink.Add("http://maps.google.com/maps/api/geocode/xml?address=" + local.City.ToString() + "," + local.State.ToString() + "," + local.Country.ToString() + "&sensor=false"); //LGMG_DB_MODIF_14_02_25
                                    else
                                        CoordinatesLink.Add("http://maps.google.com/maps/api/geocode/xml?address=" + local.City.ToString() + "," + local.City.ToString() + "," + local.Country.ToString() + "&sensor=false"); //LGMG_DB_MODIF_14_02_25

                                    XmlDocument MyXML = new XmlDocument();
                                    MyXML.Load(CoordinatesLink[n_links]);
                                    n_links++;
                                    XmlElement root = MyXML.DocumentElement;
                                    XmlNode node = root.SelectSingleNode("/GeocodeResponse/result/geometry/location");
                                    string LAT = node["lat"].InnerText;
                                    string LON = node["lng"].InnerText;
                                    double Lat = double.Parse(LAT, CultureInfo.InvariantCulture);
                                    double Lon = double.Parse(LON, CultureInfo.InvariantCulture);
                                    local.Lat = Lat;
                                    local.Lon = Lon;
                                }
                                //LGMG ATE AQUI

                                if (User.Identity.IsAuthenticated)
                                {
                                    local.UserName = User.Identity.Name;
                                    MyAspNetUser = MyAspNetUser.Where(AspNetUser => AspNetUser.UserName == local.UserName).ToList();
                                    local.UserId = MyAspNetUser[0].ID;
                                    local.UserEmail = MyAspNetUser[0].UserEmail;
                                    local.LoginProvider = MyAspNetUserLogin[0].LoginProvider;
                                }

                                if (ModelState.IsValid)
                                {
                                    db.Locations.Add(local);
                                    db.SaveChanges();
                                }
                                trip = local.Trip.ToString();
                            }
                        }
                    }
                }
                if (Command == "SHOW ALL") //print all routes
                {
                    if (User.Identity.IsAuthenticated == false)
                        MyLocations = db.Locations.Where(location => location.UserName == null).ToList();
                    else
                        MyLocations = db.Locations.Where(location => location.UserName == User.Identity.Name).ToList();
                    //MyLocations = db.Locations.ToList();

                    count = MyLocations.Count();

                    for (i = 0; i < count; i++)
                    {
                        KeyList.Add(MyLocations[i].ID);
                    }

                    if (count >= 2)
                    {
                        for (i = 0; i < count; i++)
                        {
                            if (MyLocations[i].Lat == 0) //NEW 14/2/2014
                            {//NEW 14/2/2014
                                CoordinatesLink.Add("http://maps.google.com/maps/api/geocode/xml?address=" + MyLocations[i].City.ToString() + "," + MyLocations[i].State.ToString() + "," + MyLocations[i].Country.ToString() + "&sensor=false"); //LGMG_DB_MODIF_14_02_25
                                
                                XmlDocument MyXML = new XmlDocument();
                                MyXML.Load(CoordinatesLink.Last());  //ALTERADO A 14/2/2014
                                XmlElement root = MyXML.DocumentElement;
                                XmlNode node = root.SelectSingleNode("/GeocodeResponse/result/geometry/location"); //NOVO

                                string LAT = node["lat"].InnerText;
                                string LON = node["lng"].InnerText;
                                double Lat = double.Parse(LAT, CultureInfo.InvariantCulture);
                                double Lon = double.Parse(LON, CultureInfo.InvariantCulture);
                                var ROW = db.Locations.Find(KeyList[i]); //NOVO 23-12-14
                                ROW.Lat = Lat;
                                ROW.Lon = Lon;
                            }//NEW 14/2/2014
                            else//NEW 14/2/2014
                            {
                                var ROW = db.Locations.Find(KeyList[i]); //NOVO 23-12-14
                                ROW.Lat = MyLocations[i].Lat;
                                ROW.Lon = MyLocations[i].Lon;
                            }
                            db.SaveChanges();
                        }
                        trip = "0";
                        return RedirectToAction("Index", "Home", new { trip });
                    }
                    else
                    {
                        if (User.Identity.IsAuthenticated == false)
                            return View(db.Locations.Where(location => location.UserName == null).ToList());
                        else
                            return View(db.Locations.Where(location => location.UserName == User.Identity.Name).ToList());
                    }
                }

                if (Command == "SAVE&SHOW TRIP") //PRINT ONLY CURRENT TRIP
                {
                    if (User.Identity.IsAuthenticated == false)
                        MyLocations = db.Locations.Where(location => (location.UserName == null) && (location.Trip == MyTrip)).ToList();
                    else
                        MyLocations = db.Locations.Where(location => (location.UserName == User.Identity.Name) && (location.Trip == MyTrip)).ToList();

                    count = MyLocations.Count();

                    for (i = 0; i < count; i++)
                    {
                        KeyList.Add(MyLocations[i].ID);
                    }

                    if (count >= 2)
                    {
                        for (i = 0; i < count; i++)
                        {
                            if (MyLocations[i].State!=null)
                                CoordinatesLink.Add("http://maps.google.com/maps/api/geocode/xml?address=" + MyLocations[i].City.ToString() + "," + MyLocations[i].State.ToString() + "," + MyLocations[i].Country.ToString() + "&sensor=false"); //LGMG_DB_MODIF_14_02_25
                            else
                                CoordinatesLink.Add("http://maps.google.com/maps/api/geocode/xml?address=" + MyLocations[i].City.ToString() + "," + MyLocations[i].City.ToString() + "," + MyLocations[i].Country.ToString() + "&sensor=false"); //LGMG_DB_MODIF_14_02_25
                           
                            XmlDocument MyXML = new XmlDocument();
                            MyXML.Load(CoordinatesLink[i]);
                            XmlElement root = MyXML.DocumentElement;
                            XmlNode node = root.SelectSingleNode("/GeocodeResponse/result/geometry/location");

                            string LAT = node["lat"].InnerText;
                            string LON = node["lng"].InnerText;
                            double Lat = double.Parse(LAT, CultureInfo.InvariantCulture);
                            double Lon = double.Parse(LON, CultureInfo.InvariantCulture);

                            var ROW = db.Locations.Find(KeyList[i]); //NOVO 23-12-13
                            ROW.Lat = Lat;
                            ROW.Lon = Lon;
                            db.SaveChanges();
                        }
                        return RedirectToAction("Index", "Home", new { trip });
                    }
                    else
                    {
                        if (User.Identity.IsAuthenticated == false)
                            return View(db.Locations.Where(location => (location.UserName == null) && (location.Trip == MyTrip)).ToList());
                        else
                            return View(db.Locations.Where(location => (location.UserName == User.Identity.Name) && (location.Trip == MyTrip)).ToList());
                    }
                }
            }
            //CASO NÃO EXISTA UM COMANDO (O SEPARADOR LOCATIONS ACABOU DE SER ABERTO):
            //db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Locations]"); //ESTA LINHA APAGA TODA A TABELA (INCLUINDO UTILIZADORES AUTENTICADOS)
            if (User.Identity.IsAuthenticated == false)
            {
                foreach (Location Row in db.Locations)
                {
                    if (Row.UserName == null)
                        db.Locations.Remove(Row);
                }
                db.SaveChanges();
            }
            return View(db.Locations.ToList());
            //ATE AQUI
        }

        // GET: /Locations/Details/5
        public ActionResult Details(int? id)
        {
            db.Database.CreateIfNotExists(); //NOVO 24/1/2014
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // GET: /Locations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Locations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Country,State,City,Lat,Lon,Trip,UserName,UserId")] Location location)
        {
            var valueToClean1 = ModelState["Lat"];
            var valueToClean2 = ModelState["Lon"];
            valueToClean1.Errors.Clear();
            valueToClean2.Errors.Clear();

            List<AspNetUser> MyAspNetUser = new List<AspNetUser>();
            MyAspNetUser = db.AspNetUsers.ToList();
            List<AspNetUserLogin> MyAspNetUserLogin = new List<AspNetUserLogin>();
            MyAspNetUserLogin = db.AspNetUserLogins.ToList();
            List<Location> MyLocations = new List<Location>();
            MyLocations = db.Locations.ToList();
            

            if (User.Identity.IsAuthenticated)
            {
                location.UserName = User.Identity.Name;
                MyAspNetUser = MyAspNetUser.Where(AspNetUser => AspNetUser.UserName == location.UserName).ToList();
                location.UserId = MyAspNetUser[0].ID;
                location.UserEmail = MyAspNetUser[0].UserEmail;
                location.LoginProvider = MyAspNetUserLogin[0].LoginProvider;
            }
            
            if (ModelState.IsValid)
            {
                db.Locations.Add(location);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(location);
        }

        // GET: /Locations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // POST: /Locations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Country,City,Lat,Lon,Trip,UserName,UserId")] Location location) //LGMG_DB_MODIF_14_02_25
        {
            var valueToClean1 = ModelState["Lat"];
            var valueToClean2 = ModelState["Lon"];
            valueToClean1.Errors.Clear();
            valueToClean2.Errors.Clear();

            var v = db.Locations.Find(location.ID); //NOVO 11/2/2014 para resolver bug sempre que faço edit

            List<AspNetUser> MyAspNetUser = new List<AspNetUser>();
            MyAspNetUser = db.AspNetUsers.ToList();
            List<AspNetUserLogin> MyAspNetUserLogin = new List<AspNetUserLogin>();
            MyAspNetUserLogin = db.AspNetUserLogins.ToList();
            List<Location> MyLocations = new List<Location>();
            MyLocations = db.Locations.ToList();

            if (User.Identity.IsAuthenticated)
            {
                location.UserName = User.Identity.Name;
                MyAspNetUser = MyAspNetUser.Where(AspNetUser => AspNetUser.UserName == location.UserName).ToList();
                location.UserId = MyAspNetUser[0].ID;
                location.UserEmail = MyAspNetUser[0].UserEmail;
                location.LoginProvider = MyAspNetUserLogin[0].LoginProvider;
            }
            
            if (ModelState.IsValid)
            {
                db.Entry(v).CurrentValues.SetValues(location); //NOVO 11/2/2014
                //db.Entry(location).State = EntityState.Modified; //comentado a 11/2/2014
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(location);
        }

        // GET: /Locations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);

            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // POST: /Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Location location = db.Locations.Find(id);

            //new: erase all locations with the same trip from here
            int TripToRemove = location.Trip;
            
            foreach (Location MyLocation in db.Locations)
            {
                if (MyLocation.Trip == TripToRemove)
                    db.Locations.Remove(MyLocation);
            }
            //to here
            //instead of:
            //db.Locations.Remove(location);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
