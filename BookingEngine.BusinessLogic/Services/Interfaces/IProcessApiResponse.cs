namespace BookingEngine.BusinessLogic.Services.Interfaces;

public interface IProcessApiResponse
{
    Task<T> ProcessResponse<T>(HttpResponseMessage response);

    Task<T> ProcessError<T>(HttpResponseMessage response);
}