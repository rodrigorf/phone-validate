using PhoneValidate.Domain.Service.Interfaces;
using PhoneValidate.Domain.Service.Models;

namespace PhoneValidate.Infra.Data.Interfaces
{
    public interface IRecipientRepository : IBaseRepository<Recipient>
    {
        Task<Recipient?> GetByPhoneNumberAsync(string phoneNumber);
        Task<Recipient?> GetByIdWithHistoriesAsync(Guid id);
        Task<Recipient?> GetByPhoneNumberWithHistoriesAsync(string phoneNumber);
    }
}