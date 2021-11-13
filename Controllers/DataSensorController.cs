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
        string Baseurl = "http://localhost:9080/"; 
        
        public DataSensorController()
        {
            
        }
        
        
        [HttpGet("{sensor:required}")]
        public async Task<IActionResult> GetData(string sensor, string sortOrder)  
        {  
            List<DataModel> dataModels = new List<DataModel>();  
              
            using (var client = new HttpClient())  
            {  
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);  
  
                client.DefaultRequestHeaders.Clear();  
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));  
                  
                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/Data/" + sensor);  
                
                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)  
                {  
                    //Storing the response details recieved from web api   
                    var dataResponse = response.Content.ReadAsStringAsync().Result;  

                    //Deserializing the response recieved from web api and storing into list  
                    var deserialize = JsonConvert.DeserializeObject<List<DataModel>>(dataResponse);
                    deserialize.ForEach(e => e.SensorType = sensor);

                    dataModels = deserialize;

                }

                var dataModelsQuery = dataModels.AsQueryable();
                ViewData["NameOrder"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                switch (sortOrder)
                {
                    case "name_desc":
                        dataModelsQuery = dataModelsQuery.OrderByDescending(a => a.HiveId);
                        break;
                    default:
                        dataModelsQuery = dataModelsQuery.OrderBy(a => a.HiveId);
                        break;
                }
                
                return View("DataSensor", dataModelsQuery);  
            }  
        }
    }
}