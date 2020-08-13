using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JobPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JobPortal.Controllers
{
    public class GitHubJobsController : Controller
    {
        public IActionResult Index()
        {
            string url = "https://jobs.github.com/positions.json?page=1";
            var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            request.Method = "GET";

            var content = string.Empty;
            using (var response = (System.Net.HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }

            IList<GitHubJobsResponseModel> gitjobs= JsonConvert.DeserializeObject<List<GitHubJobsResponseModel>>(content);

            return View(gitjobs);
        }

        
    }
    }

