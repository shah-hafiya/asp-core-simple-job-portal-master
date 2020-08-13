using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace JobPortal.ViewModels.Home
{
    public class ServiceBusSender
    {
        private readonly QueueClient _queueClient;
        private readonly IConfiguration _configuration;
        private const string QUEUE_NAME = "simplequeue";

        public ServiceBusSender(IConfiguration configuration)
        {
            _configuration = configuration;
            
        }

    }
}
