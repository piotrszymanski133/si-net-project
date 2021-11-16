using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Application.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Controllers
{
    public class DataSensorController: Controller
    {
        string Baseurl = "http://api:9080/";
        public DataSensorController()
        {
            
        }

        [HttpGet("{sensor:required}")]
        public async Task<IActionResult> GetData(string sensor, string sortOrder,
            [FromQuery(Name = "startDate")] DateTime startDate, [FromQuery(Name = "endDate")] DateTime endDate,
            [FromQuery(Name = "hiveId")] int hiveId)
        {
            ViewData["hiveId"] = hiveId;
            ViewData["startDate"] = startDate;
            ViewData["endDate"] = endDate;

            List<DataModel> dataModels = new List<DataModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "?";

                if (hiveId != 0)
                {
                    url += "hive=" + hiveId + "&";

                }

                if (startDate != new DateTime())
                {
                    url += "sDate=" + startDate.ToString("yyyy-M-ddTHH:mm") + "&";
                }

                if (endDate != new DateTime())
                {
                    url += "eDate=" + endDate.ToString("yyyy-M-ddTHH:mm") + "&";
                }

                url = url[..^1];
                
                HttpResponseMessage response = await client.GetAsync("api/Data/" + sensor + url);
                
                if (response.IsSuccessStatusCode)
                {
                    var dataResponse = response.Content.ReadAsStringAsync().Result;
                    var deserialize = JsonConvert.DeserializeObject<List<DataModel>>(dataResponse);
                    dataModels = deserialize;

                }

                var dataModelsQuery = dataModels.AsQueryable();
                ViewData["HiveIdOrder"] = sortOrder == "hiveId_asc" ? "hiveId_desc" : "hiveId_asc";
                ViewData["DateTimeOrder"] = sortOrder == "dateTime_asc" ? "dateTime_desc" : "dateTime_asc";
                ViewData["ValueOrder"] = sortOrder == "value_asc" ? "value_desc" : "value_asc";
                switch (sortOrder)
                {
                    case "hiveId_desc":
                        dataModelsQuery = dataModelsQuery.OrderByDescending(a => a.HiveId);
                        break;
                    case "hiveId_asc":
                        dataModelsQuery = dataModelsQuery.OrderBy(a => a.HiveId);
                        break;
                    case "dateTime_desc":
                        dataModelsQuery = dataModelsQuery.OrderByDescending(a => a.DateTime);
                        break;
                    case "dateTime_asc":
                        dataModelsQuery = dataModelsQuery.OrderBy(a => a.DateTime);
                        break;
                    case "value_desc":
                        dataModelsQuery = dataModelsQuery.OrderByDescending(a => a.Value);
                        break;
                    case "value_asc":
                        dataModelsQuery = dataModelsQuery.OrderBy(a => a.Value);
                        break;
                    default:
                        dataModelsQuery = dataModelsQuery.OrderBy(a => a.DateTime);
                        break;

                }

                ViewData["Data"] = dataModelsQuery.ToList();

                return View("DataSensor", dataModelsQuery);
            }
        }
    }
}