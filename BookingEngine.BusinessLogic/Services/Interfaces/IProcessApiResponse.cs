namespace BookingEngine.BusinessLogic.Services.Interfaces;

public interface IProcessApiResponse
{
    /// <summary>
    /// Deserialize the valid response into AmadeusApiHotelsSearchResponse.
    /// </summary>
    /// <param name="response"></param>
    /// <returns>AmadeusApiHotelsSearchResponse object</returns>
    /// <exception cref="Exception"></exception>
    Task<T> ProcessResponse<T>(HttpResponseMessage response);

    /// <summary>
    /// Deserialize the error (BadRequest) response.
    /// </summary>
    /// <param name="response"></param>
    /// <returns>AmadeusApiErrorResponse object</returns>
    /// <exception cref="Exception"></exception>
    Task<T> ProcessError<T>(HttpResponseMessage response);
}