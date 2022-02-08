namespace BookingEngine.BusinessLogic.Models
{
    public class HotelsSearchResponse
    {
        public List<HotelSearchItemResponse> Items { get; set; }   
        public int CurrentPageSize { get; set; }    
        public int CurrentPageOffset { get; set; }  
        public bool HasNextPage { get; set; }

        public HotelsSearchResponse(HotelsSearchUserRequest hotelSearchRequest)
        {
            Items = new List<HotelSearchItemResponse>();
            CurrentPageSize = hotelSearchRequest.PageSize;
            CurrentPageOffset = hotelSearchRequest.PageOffset;  
        }
    }
}
