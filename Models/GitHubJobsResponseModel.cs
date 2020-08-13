using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobPortal.Models
{
    public class GitHubJobsResponseModel
    {
        public string id { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string company { get; set; }
        public string company_url { get; set; }
        public string location { get; set; }
        public string title { get; set; }
        

    }
}
