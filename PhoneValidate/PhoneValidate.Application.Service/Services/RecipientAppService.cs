using PhoneValidate.Application.Services.Dto;
using PhoneValidate.Application.Services.Interfaces;
using PhoneValidate.Application.Services.Mapping;
using PhoneValidate.Domain.Service.Interfaces;
using PhoneValidate.Domain.Service.Models;

namespace PhoneValidate.Application.Service.Services
{
    public class RecipientAppService : IRecipientAppService
    {
        private readonly IRecipientService _recipientService;

        public RecipientAppService(IRecipientService recipientService)
        {
            _recipientService = recipientService;
        }

        public async Task<RecipientsDto> GetRecipientAsync(string phoneNumber)
        {
            var result = await _recipientService.GetByPhoneNumberAsync(phoneNumber);
            return result.ToDto();
        }

        public async Task<Result<RecipientsDto>> CreateRecipient(RecipientsDto recipientsDto)
        {
            var result = await _recipientService.CreateAsync(recipientsDto.ToModel());

            if (!result.Success)
                return Result<RecipientsDto>.Fail(result.ErrorMessage!);

            return Result<RecipientsDto>.Ok(result.Data!.ToDto());
        }
    }
}
