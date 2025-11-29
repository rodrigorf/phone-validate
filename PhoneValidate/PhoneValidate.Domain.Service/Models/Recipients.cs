using System.ComponentModel.DataAnnotations;

namespace PhoneValidate.Domain.Service.Models
{
    public class Recipient
    {
        [Required]
        public Guid Id { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(50)]
        [MinLength(14)]
        public string PhoneNumber { get; set; } = string.Empty;

        public ICollection<History> Histories { get; set; } = new List<History>();
    }
}