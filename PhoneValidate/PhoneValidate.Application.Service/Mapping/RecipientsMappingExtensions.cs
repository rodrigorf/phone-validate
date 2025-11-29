using PhoneValidate.Application.Services.Dto;
using PhoneValidate.Domain.Service.Models;

namespace PhoneValidate.Application.Services.Mapping
{
    public static class RecipientsMappingExtensions
    {
        public static RecipientsDto? ToDto(this Recipients? source)
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

        public static Recipients ToModel(this RecipientsDto? dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            return new Recipients
            {
                Id = dto.Id,
                PhoneNumber = dto.PhoneNumber,
                UpdatedAt = dto.UpdatedAt
            };
        }
    }
}