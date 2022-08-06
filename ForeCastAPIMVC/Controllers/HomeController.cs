using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ForeCastAPIMVC.Models;
using System.Net;
using Newtonsoft.Json;
using System;

namespace ForeCastAPIMVC.Controllers
{
    public class HomeController : Controller
    {
        public static List<City> ObjList;
        // GET: Home
        [HttpGet]
        public ActionResult Index()
        {
            //List of city can be downloaded 'http://bulk.openweathermap.org/sample/city.list.json.gz'
            var path = Server.MapPath(@"~/App_Data/city_list.json");
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                ObjList = JsonConvert.DeserializeObject<List<City>>(json);
            }
            return View();
        }
        [HttpPost]
        public JsonResult Index(string Prefix)
        {
            var Name = (from N in ObjList where N.Name.ToUpper().Contains(Prefix.ToUpper()) select new { N.Name });
            return Json(Name, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DetailsView(string txtCity)
        {
            string appId = "58e697934e84d0c710cebff2ff04d696";

            //API path with CITY parameter and other parameters.  
            string url = string.Format("http://api.openweathermap.org/data/2.5/weather?q={0}&units=metric&cnt=1&APPID={1}", txtCity, appId);
            ResultViewModel rslt = new ResultViewModel();
            using (WebClient client = new WebClient())
            {
                string json = client.DownloadString(url);

                //Converting to OBJECT from JSON string.  
                RootObject weatherInfo = JsonConvert.DeserializeObject<RootObject>(json);
                rslt.Country = weatherInfo.sys.country;
                rslt.City = weatherInfo.name;
                rslt.Lat = Convert.ToString(weatherInfo.coord.lat);
                rslt.Lon = Convert.ToString(weatherInfo.coord.lon);
                rslt.Description = weatherInfo.weather[0].description;
                rslt.Humidity = Convert.ToString(weatherInfo.main.humidity);
                rslt.Temp = Convert.ToString(weatherInfo.main.temp);
                rslt.TempFeelsLike = Convert.ToString(weatherInfo.main.feels_like);
                rslt.TempMax = Convert.ToString(weatherInfo.main.temp_max);
                rslt.TempMin = Convert.ToString(weatherInfo.main.temp_min);
                rslt.WeatherIcon = weatherInfo.weather[0].icon;

                rslt.speed = Convert.ToString(weatherInfo.wind.speed);
                rslt.deg = Convert.ToString(weatherInfo.wind.deg);
                rslt.gust = Convert.ToString(weatherInfo.wind.gust);
            }
            return View("DetailsView", rslt);
        }
    }
}