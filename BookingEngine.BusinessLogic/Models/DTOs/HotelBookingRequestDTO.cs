using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.BusinessLogic.Models
{
    public class HotelBookingRequestDTO
    {
        [Required]
        public string OfferId { get; set; }

        [Required]
        public List<GuestDTO> Guests { get; set; }

        // Assuming 'credit card' will be the default for a while
        public List<PaymentDTO> Payments { get; set; }

        public List<RoomDTO> Rooms { get; set; }
    }

    public class GuestDTO
    {
        [Required]
        public int Id { get; set; } // Consider if this should be a GUID for uniqueness

        [Required]
        public NameDTO Name { get; set; }

        [Required]
        public ContactDTO Contact { get; set; }
    }

    public class ContactDTO
    {
        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class NameDTO
    {
        [Required]
        public string Title { get; set; } // Could be an enum: Mr, Mrs, Ms, etc.

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }

    public class PaymentDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [RegularExpression("^(creditCard)$")]  // Ensures only "CC" for credit card
        public string Method { get; set; }

        [Required]
        public CardDTO Card { get; set; }
    }

    public class CardDTO
    {
        [Required]
        public string VendorCode { get; set; } // E.g., "VI", "MC", "AX"

        [Required]
        [CreditCard]
        public string CardNumber { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{4}-([0][1-9]|[1][0-2])$")]
        public string ExpiryDate { get; set; }
    }

    public class RoomDTO
    {
        public List<int> GuestIds { get; set; }

        public int PaymentId { get; set; }

        public string SpecialRequest { get; set; }
    }

}
