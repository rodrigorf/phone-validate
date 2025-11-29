using PhoneValidate.Domain.Service.Models;

namespace PhoneValidate.Domain.Service.Interfaces
{
    public interface IRecipientService
    {
        Task<Recipient?> GetByPhoneNumberAsync(string phoneNumber);
        Task<Recipient?> GetByIdAsync(Guid id);
        Task<Result<Recipient>> CreateAsync(Recipient recipient);
        Task<Result<Recipient?>> UpdateAsync(Guid id, Recipient recipients);
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}