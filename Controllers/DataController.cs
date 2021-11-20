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
using Application.Functions;

namespace Application.Controllers
{
    public class DataController :Controller
    {
        string Baseurl = "http://api:80/"; 
        
        public DataController()
        {
            
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllData(string sortOrder, [FromQuery(Name = "sensorType")] string sensorType, [FromQuery(Name = "startDate")] DateTime startDate, [FromQuery(Name = "endDate")] DateTime endDate, [FromQuery(Name = "hiveId")] int hiveId = 0)

        {
            ViewData["sensorType"] = sensorType;
            ViewData["hiveId"] = hiveId;
            ViewData["startDate"] = startDate.ToString("yyyy-M-ddTHH:mm");
            ViewData["endDate"] = endDate.ToString("yyyy-M-ddTHH:mm");
            
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

                dataModelsQuery = Functions.Functions.switchOrderFunction(sortOrder, dataModelsQuery);
                
                List<DataModel> dataModelsQueryList = dataModelsQuery.ToList();
                dataModelsQueryList.ForEach(dataModel => dataModel.DateTime = dataModel.DateTime.ToLocalTime());
                ViewData["Data"] = dataModelsQueryList;

                return View("Data", dataModelsQuery);
            }
        }
    }  
}
