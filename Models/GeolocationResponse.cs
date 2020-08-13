﻿namespace JobPortal.Models
{
    public class GeolocationResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string Continent { get; set; }
        public string ContinentCode { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Region { get; set; }
        public string RegionName { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Zip { get; set; }
        public float Lat { get; set; }
        public float Lon { get; set; }
        public string Timezone { get; set; }
        public int Offset { get; set; }
        public string Currency { get; set; }
        public string Isp { get; set; }
        public string Org { get; set; }
        public string As { get; set; }
        public string Asname { get; set; }
        public string Reverse { get; set; }
        public string Mobile { get; set; }
        public string Proxy { get; set; }
        public string Hosting { get; set; }
        public string Query { get; set; }
    }

    public class DnsResponse
    {
        public NestedDnsResponse Dns { get; set; }

        public class NestedDnsResponse
        {
            public string Ip { get; set; }
            public string Geo { get; set; }
        }
    }
}
