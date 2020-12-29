using System.Collections.Generic;

namespace MissionsApi.Dtos
{
    public class ApiResult
    {
        public List<Datum> data { get; set; }
    }

    public class Datum
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string type { get; set; }
        public double distance { get; set; }
        public string name { get; set; }
        public string number { get; set; }
        public string postal_code { get; set; }
        public string street { get; set; }
        public double confidence { get; set; }
        public string region { get; set; }
        public string region_code { get; set; }
        public object county { get; set; }
        public string locality { get; set; }
        public object administrative_area { get; set; }
        public string neighbourhood { get; set; }
        public string country { get; set; }
        public string country_code { get; set; }
        public string continent { get; set; }
        public string label { get; set; }
    }
}
