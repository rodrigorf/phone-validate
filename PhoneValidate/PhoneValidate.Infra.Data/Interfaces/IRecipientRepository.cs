using PhoneValidate.Domain.Service.Interfaces;
using PhoneValidate.Domain.Service.Models;

namespace PhoneValidate.Infra.Data.Interfaces
{
    public interface IRecipientRepository : IBaseRepository<Recipients>
    {
        Task<Recipients?> GetByPhoneNumberAsync(string phoneNumber);
        Task<Recipients?> GetByIdWithHistoriesAsync(Guid id);
        Task<Recipients?> GetByPhoneNumberWithHistoriesAsync(string phoneNumber);
    }
}