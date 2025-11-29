using PhoneValidate.Application.Services.Dto;
using PhoneValidate.Domain.Service.Models;

namespace PhoneValidate.Application.Services.Interfaces
{
    public interface IRecipientAppService
    {
        Task<RecipientsDto> GetRecipientAsync(string phoneNumber);
        Task<Result<RecipientsDto>> CreateRecipient(RecipientsDto recipientsDto);
        Task<Result<RecipientsDto>> UpdateRecipientAsync(Guid id, RecipientsDto recipientsDto);
    }
}