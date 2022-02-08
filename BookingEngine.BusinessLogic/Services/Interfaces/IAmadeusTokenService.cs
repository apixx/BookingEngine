using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.BusinessLogic.Services.Interfaces
{
    public interface IAmadeusTokenService
    {
        /// <summary>
        /// Gets the authorization token using asynchronous POST request
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Access Token</returns>
        Task<string> GetAmadeusToken(CancellationToken cancellationToken);
    }
}
