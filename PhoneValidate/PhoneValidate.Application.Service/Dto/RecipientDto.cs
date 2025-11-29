using System.ComponentModel.DataAnnotations;

namespace PhoneValidate.Application.Services.Dto
{
    public class RecipientsDto
    {
        public Guid Id { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Phone number is required.")]
        [MinLength(14, ErrorMessage = "Phone number must be at least 14 characters.")]
        [MaxLength(50, ErrorMessage = "Phone number must not exceed 50 characters.")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}