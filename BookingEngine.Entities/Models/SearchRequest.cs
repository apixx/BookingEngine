namespace BookingEngine.Entities.Models
{
    public class SearchRequest
    {
        public int SearchRequestId { get; set; }
        public string CityCode { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public DateTime ValidUntil { get; set; }
        public int Adults { get; set; }
        public string NextItemsLink { get; set; }
        public ICollection<SearchRequestHotel> SearchRequestHotels { get; set; }
    }
}