using System.Linq;
using Application.Models;

namespace Application.Functions
{
    public class Functions
    {
        public static IQueryable<DataModel> switchOrderFunction(string sortOrder, IQueryable<DataModel> dataModelsQuery)
        {
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

            return dataModelsQuery;
        }
    }
}