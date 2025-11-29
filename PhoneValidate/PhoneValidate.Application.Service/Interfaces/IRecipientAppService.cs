using PhoneValidate.Application.Services.Dto;

namespace PhoneValidate.Application.Services.Interfaces
{
    public interface IRecipientAppService
    {
        Task<RecipientsDto> GetRecipientAsync(string phoneNumber);

    }
}