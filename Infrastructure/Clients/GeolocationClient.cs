using JobPortal.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace JobPortal.WebAPI.Infrastructure.Clients
{
    public class GeolocationClient : IGeolocationClient
    {
        public async Task<GeolocationResponse> GetGeolocation(GeolocationRequestModel requestModel)
        {
            using (var httpCllient = new HttpClient())
            {
                httpCllient.BaseAddress = new Uri("http://ip-api.com");

                StringBuilder uriBuilder = new StringBuilder("/json");

                if (!string.IsNullOrEmpty(requestModel.Query))
                {
                    uriBuilder.Append("/");
                    uriBuilder.Append($"{requestModel.Query}");
                }

                var queryStringDict = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(requestModel.Lang))
                {
                    queryStringDict[nameof(requestModel.Lang).ToLower()] = requestModel.Lang;
                }

                if (!string.IsNullOrEmpty(requestModel.Fields))
                {
                    queryStringDict[nameof(requestModel.Fields).ToLower()] = requestModel.Fields;
                }

                if (queryStringDict.Count > 0)
                {
                    uriBuilder.Append("?");
                    uriBuilder.Append(string.Join("&", queryStringDict.Select(x => $"{x.Key}={x.Value}")));
                }

                httpCllient.DefaultRequestHeaders.Accept.Clear();
                httpCllient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpCllient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

                var httpResponse = await httpCllient.GetAsync(new Uri(uriBuilder.ToString(), UriKind.Relative));

                if (httpResponse.IsSuccessStatusCode)
                {
                    var data = httpResponse.Content.ReadAsStringAsync().Result;
                    var geolocationResponse = JsonConvert.DeserializeObject<GeolocationResponse>(data);

                    return geolocationResponse;
                }
            }

            return null;
        }
    }
}