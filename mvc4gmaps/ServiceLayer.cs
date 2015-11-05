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

namespace MVC4GMAPS.Models
{
    public class ServiceLayer
    {
        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("ARGH!");
            return input.First().ToString().ToUpper() + String.Join("", input.Skip(1));
        }

        public List<Coordinate> GetDataBaseSlice(string trip, string UserId, string code)
        {
            List<Location> MyLocations = new List<Location>();
            List<Coordinate> MyCoordinates = new List<Coordinate>();
            LocationDBContext dblocations = new LocationDBContext();
            CoordinateDBContext dbcoordinates = new CoordinateDBContext();
            List<string> WeatherLink = new List<string>();
            int i, j=0, count, AuxInt;
            int MyTrip = Convert.ToInt32(trip);
            string aux, AuxStr;

            if (String.IsNullOrEmpty(UserId))//LGMG 3/3/14
            {//LGMG 3/3/14
                if (MyTrip == 0)
                {
                    if (HttpContext.Current.User.Identity.IsAuthenticated == false)
                        MyLocations = dblocations.Locations.Where(location => location.UserName == null).ToList();
                    else
                        MyLocations = dblocations.Locations.Where(location => location.UserName == HttpContext.Current.User.Identity.Name).ToList();
                }
                else
                {
                    if (HttpContext.Current.User.Identity.IsAuthenticated == false)
                        MyLocations = dblocations.Locations.Where(location => (location.UserName == null) && (location.Trip == MyTrip)).ToList();
                    else
                        MyLocations = dblocations.Locations.Where(location => (location.UserName == HttpContext.Current.User.Identity.Name) && (location.Trip == MyTrip)).ToList();
                }
            }//LGMG 3/3/14
            else //LGMG 3/3/14 DAQUI 
            {
                if (MyTrip == 0)
                    MyLocations = dblocations.Locations.Where(location => location.UserId == UserId).ToList();
                else
                {
                    MyLocations = dblocations.Locations.Where(location => (location.UserId == UserId) && (location.Trip == MyTrip)).ToList();
                }
            }
            //LGMG 3/3/14 ATE AQUI 

            count = MyLocations.Count();

            bool exception=false;

            for (i = 0; i < count; i++) //Weather in each city
            {
                WeatherLink.Add("http://api.openweathermap.org/data/2.5/forecast/daily?q=" + MyLocations[i].City.ToString() + "," + MyLocations[i].Country.ToString() + "&mode=xml&units=metric&cnt=7&APPID=cf5ddc486fa7da9b22055da77747966d");
                XmlDocument MyXML = new XmlDocument();
                    
                try//
                {//
                    MyXML.Load(WeatherLink[i]);
                }//
                catch (Exception e)//
                {//
                    if (e.Message == "Data at the root level is invalid. Line 1, position 1.")//
                    {
                        try
                        {
                            WeatherLink.RemoveAt(i);
                            WeatherLink.Add("http://api.openweathermap.org/data/2.5/forecast/daily?q=" + MyLocations[i].City.ToString() + "&mode=xml&units=metric&cnt=7&APPID=cf5ddc486fa7da9b22055da77747966d");
                            MyXML.Load(WeatherLink[i]);
                        }
                        catch (Exception ex)
                        {
                            exception = true;
                        }
                    }
                        //exception=true;//
                }

                if (!exception)//
                {//
                    XmlElement root = MyXML.DocumentElement;
                    while (root==null)
                    {
                        MyXML.Load(WeatherLink[i]);
                        root = MyXML.DocumentElement;
                    }
                    XmlNodeList nodes;

                    DateTime today = DateTime.Now;
                    Coordinate MyCoordinate = new Coordinate();
                    MyCoordinate.Weather = new List<string>();
                    MyCoordinate.Wind = new List<string>();
                    MyCoordinate.MaxTemp = new List<int>();
                    MyCoordinate.MinTemp = new List<int>();
                    MyCoordinate.Humidity = new List<int>();
                    MyCoordinate.Symbol = new List<int>();

                    while (j < 7) //Weather in the next 7 days 
                    {
                        aux = "/weatherdata/forecast/time[@day = '" + today.Year.ToString() + "-" + today.Month.ToString("00") + "-" + today.Day.ToString("00") + "']";
                            
                        nodes = root.SelectNodes(aux);

                        MyCoordinate.Weather.Add(FirstCharToUpper(nodes[0]["clouds"].GetAttribute("value").ToString()));
                        MyCoordinate.Wind.Add(nodes[0]["windSpeed"].GetAttribute("name").ToString().ToLower());
                        AuxStr = nodes[0]["temperature"].GetAttribute("max");
                        AuxInt = Convert.ToInt32(double.Parse(AuxStr, CultureInfo.InvariantCulture));
                        MyCoordinate.MaxTemp.Add(AuxInt);
                        AuxStr = nodes[0]["temperature"].GetAttribute("min");
                        AuxInt = Convert.ToInt32(double.Parse(AuxStr, CultureInfo.InvariantCulture));
                        MyCoordinate.MinTemp.Add(AuxInt);
                        AuxStr = nodes[0]["humidity"].GetAttribute("value");
                        AuxInt = Convert.ToInt32(double.Parse(AuxStr, CultureInfo.InvariantCulture));
                        MyCoordinate.Humidity.Add(AuxInt);
                        AuxStr = nodes[0]["symbol"].GetAttribute("number");
                        AuxInt = Convert.ToInt32(double.Parse(AuxStr, CultureInfo.InvariantCulture));
                        MyCoordinate.Symbol.Add(AuxInt);

                        today = today.AddDays(1);
                        j++;
                    }
                    j = 0;

                    MyCoordinate.ID = MyLocations[i].ID;
                    MyCoordinate.Lat = MyLocations[i].Lat;
                    MyCoordinate.Lon = MyLocations[i].Lon;
                    MyCoordinate.Country = MyLocations[i].Country;
                    MyCoordinate.City = MyLocations[i].City;
                    MyCoordinate.State = MyLocations[i].State; //LGMG_DB_MODIF_14_02_25
                    MyCoordinate.Trip = MyLocations[i].Trip;

                    if (UserId != null) //LGMG 3/3/14
                        MyCoordinate.PostUser = MyLocations[i].UserName; //LGMG 3/3/14

                    MyCoordinates.Add(MyCoordinate);
                }//
                else//
                {//
                    exception = false;
                    //XmlElement root = MyXML.DocumentElement;
                    //XmlNodeList nodes;

                    DateTime today = DateTime.Now;
                    Coordinate MyCoordinate = new Coordinate();
                    MyCoordinate.Weather = new List<string>();
                    MyCoordinate.Wind = new List<string>();
                    MyCoordinate.MaxTemp = new List<int>();
                    MyCoordinate.MinTemp = new List<int>();
                    MyCoordinate.Humidity = new List<int>();
                    MyCoordinate.Symbol = new List<int>();

                    while (j < 7) //Weather in the next 7 days 
                    {
                        MyCoordinate.Weather.Add(".");
                        MyCoordinate.Wind.Add(".");
                        MyCoordinate.MaxTemp.Add(0);
                        MyCoordinate.MinTemp.Add(0);
                        MyCoordinate.Humidity.Add(0);
                        MyCoordinate.Symbol.Add(0);
                        today = today.AddDays(1);
                        j++;
                    }
                    j = 0;

                    MyCoordinate.ID = MyLocations[i].ID;
                    MyCoordinate.Lat = MyLocations[i].Lat;
                    MyCoordinate.Lon = MyLocations[i].Lon;
                    MyCoordinate.Country = MyLocations[i].Country;
                    MyCoordinate.State = MyLocations[i].State; //LGMG_DB_MODIF_14_02_25
                    MyCoordinate.City = MyLocations[i].City;
                    MyCoordinate.Trip = MyLocations[i].Trip;
                    if (UserId != null) //LGMG 3/3/14
                        MyCoordinate.PostUser = MyLocations[i].UserId; //LGMG 3/3/14

                    MyCoordinates.Add(MyCoordinate);
                }//
            }
            return MyCoordinates;
        }
    }
}