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
        public async Task<IActionResult> GetAllData([FromQuery(Name = "startDate")] DateTime startDate, [FromQuery(Name = "endDate")] DateTime endDate, [FromQuery(Name = "hiveId")] int hiveId = -1)

        {
            ViewData["startDate"] = startDate;
            ViewData["endDate"] = endDate;
            
            List<DataModel> dataModels = new List<DataModel>();

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage temperatures;
                HttpResponseMessage winds;
                HttpResponseMessage pressures;
                HttpResponseMessage humidities;

                string url = "?";

                if (hiveId != -1)
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
                
                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                temperatures = await client.GetAsync("api/Data/temperatures" + url);
                winds = await client.GetAsync("api/Data/winds" + url);
                pressures = await client.GetAsync("api/Data/pressures" + url);
                humidities = await client.GetAsync("api/Data/humidities" + url);
                
                //Checking the response is successful or not which is sent using HttpClient  
                if (temperatures.IsSuccessStatusCode && winds.IsSuccessStatusCode && pressures.IsSuccessStatusCode &&
                    humidities.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var dataResponseTemperatures = temperatures.Content.ReadAsStringAsync().Result;
                    var dataResponseWinds = winds.Content.ReadAsStringAsync().Result;
                    var dataResponsePressures = pressures.Content.ReadAsStringAsync().Result;
                    var dataResponseHumidities = humidities.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into list  
                    var deserializeTemperatures =
                        JsonConvert.DeserializeObject<List<DataModel>>(dataResponseTemperatures);
                    deserializeTemperatures.ForEach(e => e.SensorType = "temperatures");
                    var deserializeWinds = JsonConvert.DeserializeObject<List<DataModel>>(dataResponseWinds);
                    deserializeWinds.ForEach(e => e.SensorType = "winds");
                    var deserializePressures = JsonConvert.DeserializeObject<List<DataModel>>(dataResponsePressures);
                    deserializePressures.ForEach(e => e.SensorType = "pressures");
                    var deserializeHumidities = JsonConvert.DeserializeObject<List<DataModel>>(dataResponseHumidities);
                    deserializeHumidities.ForEach(e => e.SensorType = "humidities");

                    dataModels.AddRange(deserializeTemperatures);
                    dataModels.AddRange(deserializeWinds);
                    dataModels.AddRange(deserializePressures);
                    dataModels.AddRange(deserializeHumidities);

                }

                return View("Data", dataModels);
            }
        }
    }  
}
