using MissionsApi.Dtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MissionsApi.Services
{
    public class AgentRepository : IAgentRepository
    {
        //  instead of using traditional db - for simple task we can use flat file
        const string FILE_PATH = @"c:\Agents.txt";
        public AgentRepository()
        {
            if (!File.Exists(FILE_PATH))
            {
                using (var sw = new StreamWriter(FILE_PATH))
                {
                    sw.Write(string.Empty); //  init the "db"
                }
            }
        }

        public async Task<Mission> FindClosest(MissionSearch search)
        {
            Mission closest = null;
            var allMissionsWithLatLong = await FillMissionsLatLong();
            LatLong toCompare = null;

            if (!string.IsNullOrEmpty(search.Address))
                toCompare = await GetGeoAddressAsync(search.Address);
            else if (!string.IsNullOrEmpty(search.Latlng))
            {
                var latlngArr = search.Latlng.Split(",");

                if (latlngArr.Length == 2)
                {
                    toCompare = new LatLong()
                    {
                        Latitude = double.Parse(latlngArr[0]),
                        Longitude = double.Parse(latlngArr[1])
                    };
                }
            }
            if (allMissionsWithLatLong.Count > 0 && toCompare != null)
            {
                var firstElement = allMissionsWithLatLong.FirstOrDefault();
                double minLatitude = Math.Abs(firstElement.LatLong.Latitude - toCompare.Latitude), minLongitude = Math.Abs(firstElement.LatLong.Longitude - toCompare.Longitude);
                closest = firstElement;

                foreach (var mission in allMissionsWithLatLong)
                {
                    if (mission.LatLong != null)
                    {
                        if (Math.Abs(mission.LatLong.Latitude - toCompare.Latitude) < minLatitude && Math.Abs(mission.LatLong.Longitude - toCompare.Longitude) < minLongitude)
                        {
                            minLatitude = Math.Abs(mission.LatLong.Latitude - toCompare.Latitude);
                            minLatitude = Math.Abs(mission.LatLong.Longitude - toCompare.Longitude);
                            closest = mission;
                        }
                    }
                }
            }

            return closest;
        }

        public async Task<List<Mission>> GetAllAsync()
        {
            using (StreamReader r = new StreamReader(FILE_PATH))
            {
                string json = await r.ReadToEndAsync();
                return JsonConvert.DeserializeObject<List<Mission>>(json);
            }
        }

        private async Task<LatLong> GetGeoAddressAsync(string address)
        {
            LatLong latLong = null;

            using (var httpClient = new HttpClient())
            {
                string base_url = "http://api.positionstack.com/v1/forward?access_key=81937c53a85d16c34934f438f2e0cb91&query=";

                await httpClient.GetAsync($"{base_url}{address}")
                    .ContinueWith(async (taskwithresponse) =>
                    {
                        var response = taskwithresponse.Result;

                        if (response.IsSuccessStatusCode)
                        {
                            string result = await response.Content.ReadAsStringAsync();
                            JObject jObject = JObject.Parse(result);
                            ApiResult res = JsonConvert.DeserializeObject<ApiResult>(result);

                            if (res != null && res.data != null)
                            {
                                var closest = res.data.FirstOrDefault();

                                if (closest != null)
                                    latLong = new LatLong() { Latitude = closest.latitude, Longitude = closest.longitude };
                            }
                        }
                    });
            }
            return latLong;
        }

        private async Task<List<Mission>> FillMissionsLatLong()
        {
            var allMissions = await GetAllAsync();

            foreach (var mission in allMissions)
            {
                mission.LatLong = await GetGeoAddressAsync(mission.Address);
            }

            return allMissions;
        }

        public async Task<string> GetMostIsolatedCountry()
        {
            var allMissions = await GetAllAsync();

            if (allMissions == null)
                return "none";

            var query = allMissions.GroupBy(m => m.Agent)   //  group by agent
                .Where(r => r.Count() == 1).SelectMany(x => x)  //  only isolated agents counts
                .GroupBy(r => r.Country)    //  group by country
                .OrderByDescending(o => o.Count()).FirstOrDefault();    //  take the top of the list

            return query?.Key;
        }

        public async Task<Guid> SaveMissionAsync(Mission mission)
        {
            var allMissions = await GetAllAsync();
           
            if (allMissions == null)
                allMissions = new List<Mission>();
            else
            {
                var exist = allMissions.FirstOrDefault(m => m.Address == mission.Address && m.Agent == mission.Agent && m.Country == mission.Country && m.Date == mission.Date);

                if (exist != null)
                    return exist.Id;

            }
            
            Guid id = mission.Id = Guid.NewGuid();

            allMissions.Add(mission);

            string json = JsonConvert.SerializeObject(allMissions);

            using (var sw = new StreamWriter(FILE_PATH))
            {
                await sw.WriteAsync(json);
            }

            return id;
        }

    }
}
