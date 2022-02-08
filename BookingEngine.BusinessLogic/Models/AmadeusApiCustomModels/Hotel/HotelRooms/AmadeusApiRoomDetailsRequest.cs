namespace BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels;

public class AmadeusApiRoomDetailsRequest
{
    public string OfferId { get; set; }
    //public string Lang { get; set; } = "en";

    public AmadeusApiRoomDetailsRequest(string offerId)
    {
        OfferId = offerId;
    }

    public async Task<string> ToUrlParamsString()
    {
        var urlParams = new Dictionary<string, string>();

        urlParams.Add("offerId", OfferId);
        //urlParams.Add("lang", Lang);

        using (HttpContent content = new FormUrlEncodedContent(urlParams))
        {
            return await content.ReadAsStringAsync();
        }
    }
}