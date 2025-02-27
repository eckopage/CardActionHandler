using System.ComponentModel.DataAnnotations;

namespace Card.API.Models
{
    public class CardRequest
    {
        [Required(ErrorMessage = "UserIdRequired")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "CardNumberRequired")]
        public string CardNumber { get; set; } = string.Empty;
    }
}
