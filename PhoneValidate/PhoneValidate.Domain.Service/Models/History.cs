using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneValidate.Domain.Service.Models
{
    public class History
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid RecipientId { get; set; }

        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(RecipientId))]
        public Recipient Recipient { get; set; } = null!;
    }
}
