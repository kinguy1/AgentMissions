using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissionsApi.Dtos
{
    public class Mission
    {
        public Guid Id { get; set; }
        public string Agent { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public DateTime Date { get; set; }
        public LatLong LatLong { get; set; }
    }
}
