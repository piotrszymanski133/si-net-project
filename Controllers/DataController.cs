using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Application.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Controllers
{
    public class DataController :Controller
    {
        string Baseurl = "http://localhost:9080/"; 
        
        public DataController()
        {
            
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllData(string sortOrder, [FromQuery(Name = "sensorType")] string sensorType, [FromQuery(Name = "startDate")] DateTime startDate, [FromQuery(Name = "endDate")] DateTime endDate, [FromQuery(Name = "hiveId")] int hiveId = 0)

        {
            ViewData["sensorType"] = sensorType;
            ViewData["hiveId"] = hiveId;
            ViewData["startDate"] = startDate;
            ViewData["endDate"] = endDate;
            
            List<DataModel> dataModels = new List<DataModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage allData;

                string url = "?";

                if (hiveId != 0)
                {
                    url += "hive=" + hiveId + "&";

                }
                
                if (sensorType != null)
                {
                    url += "sensor=" + sensorType + "&";
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
                
                allData = await client.GetAsync("api/Data/all" + url);
                
                if (allData.IsSuccessStatusCode)
                {
  
                    var dataAllResponse = allData.Content.ReadAsStringAsync().Result;
                    var deserializeAllData = JsonConvert.DeserializeObject<List<DataModel>>(dataAllResponse);
                    dataModels = deserializeAllData;
                }
                
                var dataModelsQuery = dataModels.AsQueryable();
                ViewData["SensorTypeOrder"] = sortOrder == "sensorType_asc" ? "sensorType_desc" : "sensorType_asc";
                ViewData["HiveIdOrder"] = sortOrder == "hiveId_asc" ? "hiveId_desc" : "hiveId_asc";
                ViewData["DateTimeOrder"] = sortOrder == "dateTime_asc" ? "dateTime_desc" : "dateTime_asc";
                ViewData["ValueOrder"] = sortOrder == "value_asc" ? "value_desc" : "value_asc";
                switch (sortOrder)
                {
                    case "sensorType_desc":
                        dataModelsQuery = dataModelsQuery.OrderByDescending(a => a.Type);
                        break;
                    case "sensorType_asc":
                        dataModelsQuery = dataModelsQuery.OrderBy(a => a.Type);
                        break;
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

                return View("Data", dataModelsQuery);
            }
        }
    }  
}
