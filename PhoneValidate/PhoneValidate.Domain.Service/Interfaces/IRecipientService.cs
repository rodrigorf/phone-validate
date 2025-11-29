using PhoneValidate.Domain.Service.Models;

namespace PhoneValidate.Domain.Service.Interfaces
{
    public interface IRecipientService
    {
        Task<Recipients?> GetByPhoneNumberAsync(string phoneNumber);
        Task<Recipients?> GetByIdAsync(Guid id);
        Task<Result<Recipients>> CreateAsync(Recipients recipient);
        Task<Recipients?> UpdateAsync(Guid id, string phoneNumber);
        Task<bool> DeleteAsync(Guid id);
    }
}