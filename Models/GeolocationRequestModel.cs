using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobPortal.Models
{
    public class GeolocationRequestModel
    {
        public string Fields { get; set; }
        public string Lang { get; set; }
        public string Query { get; set; }
    }
}
