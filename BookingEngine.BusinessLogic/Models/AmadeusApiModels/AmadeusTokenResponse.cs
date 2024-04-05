using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels
{
    public class AmadeusTokenResponse
    {
        public string Type { get; set; }
        public string Username { get; set; }
        public string Application_name { get; set; }
        public string Client_id { get; set; }
        public string Token_type { get; set; }
        public string Access_token { get; set; }
        public int Expires_in { get; set; } // seconds
        public string State { get; set; }
        public string Scope { get; set; }
    }
}
