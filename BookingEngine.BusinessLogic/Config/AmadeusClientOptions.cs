using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.BusinessLogic.Config
{
    public class AmadeusClientOptions
    {
        public string Url { get; set; }

        public string AuthTokenUrl { get; set; }

        public string ApiKey { get; set; }

        public string ApiSecret { get; set; }
    }
}
