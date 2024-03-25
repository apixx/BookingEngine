using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookingEngine.BusinessLogic.Models.AmadeusApiCustomModels.Hotel.Booking
{

    public class AmadeusApiHotelBookingRequest
    {
        [JsonPropertyName("Data")] 
        public AmadeusApiHotelBookingRequestItem Data { get; set; }

        public AmadeusApiHotelBookingRequest(AmadeusApiHotelBookingRequestItem data)
        {
            this.Data = data;
        }
    }

    public class AmadeusApiHotelBookingRequestItem
    {
        [JsonPropertyName("offerId")]
        public string OfferId { get; set; }
        [JsonPropertyName("guests")]
        public List<StakeHolder> Guests { get; set; }
        [JsonPropertyName("payments")]
        public List<PaymentItem> Payments { get; set; }
        [JsonPropertyName("rooms")]
        public List<Room>? Rooms { get; set; }
    }

    public class StakeHolder // Guest
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public NameItem Name { get; set; }
        [JsonPropertyName("contact")]
        public Contact Contact { get; set; }
    }

    public class Contact
    {
        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    public class NameItem
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
    }

    //public enum Title
    //{
    //    MR,
    //    MRS,
    //    MS
    //}

    public class PaymentItem
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("method")]
        public string Method { get; set; } // only credit card (CC) available
        [JsonPropertyName("card")]
        public CardItem Card { get; set; }
    }

    public class CardItem
    {
        [JsonPropertyName("vendorCode")]
        public string VendorCode { get; set; }
        [JsonPropertyName("cardNumber")]
        public string CardNumber { get; set; }
        [JsonPropertyName("expiryDate")]
        public string ExpiryDate { get; set; }
    }


    public class Room
    {
        [JsonPropertyName("guestIds")]
        public List<int> GuestIds { get; set; }
        [JsonPropertyName("paymentId")]
        public int PaymentId { get; set; }
        [JsonPropertyName("specialRequest")]
        public string SpecialRequest { get; set; }
    }

}
