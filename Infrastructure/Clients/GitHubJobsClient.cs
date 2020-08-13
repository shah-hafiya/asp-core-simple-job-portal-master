using JobPortal.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace JobPortal.Infrastructure.Clients
{
    public class GitHubJobsClient
    {
        public async Task<GitHubJobsResponseModel> GetGeolocation(GitHubJobsRequestModel requestModel)
        {
            using (var httpCllient = new HttpClient())
            {
                httpCllient.BaseAddress = new Uri("https://jobs.github.com/positions.json?page=1");

                StringBuilder uriBuilder = new StringBuilder("/json");
                requestModel.page = "1";
                if (!string.IsNullOrEmpty(requestModel.page))
                {
                    uriBuilder.Append("/");
                    uriBuilder.Append($"{requestModel.page}");
                }

                var queryStringDict = new Dictionary<string, string>();             

                httpCllient.DefaultRequestHeaders.Accept.Clear();
                httpCllient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpCllient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

                var httpResponse = await httpCllient.GetAsync(new Uri(uriBuilder.ToString(), UriKind.Relative));

                if (httpResponse.IsSuccessStatusCode)
                {
                    var data = httpResponse.Content.ReadAsStringAsync().Result;
                    var geolocationResponse = JsonConvert.DeserializeObject<GitHubJobsResponseModel>(data);

                    return geolocationResponse;
                }
            }

            return null;
        }
    }
}
