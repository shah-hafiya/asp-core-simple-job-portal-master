using JobPortal.Models;
using JobPortal.WebAPI.Infrastructure.Clients;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace JobPortal.Controllers
{
    [ApiController]
    public class IPGeolocationController : ControllerBase
    {
        private readonly IGeolocationClient _client;

        public IPGeolocationController(IGeolocationClient client)
        {
            _client = client;
        }

        [Route("ip-geolocation")]
        public async Task<IActionResult> Get()
        {
            string url = "http://edns.ip-api.com/json";
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

            var dns = JsonConvert.DeserializeObject<DnsResponse>(content);

            return Ok(await _client.GetGeolocation(new GeolocationRequestModel
            {
                Lang = "en",
                Query = dns.Dns.Ip
            }));
        }
    }
}