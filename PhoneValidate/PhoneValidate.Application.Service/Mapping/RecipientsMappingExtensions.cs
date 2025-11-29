using PhoneValidate.Application.Services.Dto;
using PhoneValidate.Domain.Service.Models;

namespace PhoneValidate.Application.Services.Mapping
{
    public static class RecipientsMappingExtensions
    {
        public static RecipientsDto? ToDto(this Recipient? source)
        {
            if (source is null)
                return null;

            return new RecipientsDto
            {
                Id = source.Id,
                PhoneNumber = source.PhoneNumber,
                UpdatedAt = source.UpdatedAt
            };
        }

        public static Recipient ToModel(this RecipientsDto? dto)
        {
            var recipient = new Recipient
            {
                Id = Guid.NewGuid(),
                PhoneNumber = dto.PhoneNumber,
                UpdatedAt = DateTime.UtcNow,
                Histories = new List<History>()
            };

            recipient.Histories.Add(new History
            {
                Id = Guid.NewGuid(),
                RecipientId = recipient.Id,
                CreatedAt = DateTime.UtcNow
            });

            return recipient;
        }
    }
}