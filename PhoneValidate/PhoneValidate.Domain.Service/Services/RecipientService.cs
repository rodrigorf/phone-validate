using Microsoft.Extensions.Logging;
using PhoneValidate.Domain.Service.Interfaces;
using PhoneValidate.Domain.Service.Models;

namespace PhoneValidate.Domain.Service.Services
{
    public class RecipientService : IRecipientService
    {
        private readonly IBaseRepository<Recipients> _repository;
        private readonly ILogger<RecipientService> _logger;

        public RecipientService(
            IBaseRepository<Recipients> repository,
            ILogger<RecipientService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Recipients?> GetByPhoneNumberAsync(string phoneNumber)
        {
            _logger.LogInformation("Getting recipient by phone number: {PhoneNumber}", phoneNumber);

            var recipient = await _repository.FirstOrDefaultAsync(r => r.PhoneNumber == phoneNumber);

            return recipient;
        }

        public async Task<Recipients?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting recipient by ID: {Id}", id);
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Result<Recipients>> CreateAsync(Recipients recipient)
        {
            _logger.LogInformation("Creating new recipient with phone: {PhoneNumber}", recipient.PhoneNumber);

            var exists = await _repository.AnyAsync(r => r.PhoneNumber == recipient.PhoneNumber);
            if (exists)
            {
                _logger.LogWarning("Recipient already exists with phone: {PhoneNumber}", recipient.PhoneNumber);
                return Result<Recipients>.Fail($"A recipient with phone number '{recipient.PhoneNumber}' already exists.");
            }

            recipient.Id = Guid.NewGuid();
            recipient.UpdatedAt = DateTime.UtcNow;

            await _repository.AddAsync(recipient);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Recipient created successfully with ID: {Id}", recipient.Id);
            return Result<Recipients>.Ok(recipient);
        }

        public async Task<Recipients?> UpdateAsync(Guid id, string phoneNumber)
        {
            _logger.LogInformation("Updating recipient ID: {Id}", id);

            var recipient = await _repository.GetByIdAsync(id);
            if (recipient == null)
            {
                _logger.LogWarning("Recipient not found for update, ID: {Id}", id);
                return null;
            }

            var exists = await _repository.AnyAsync(r => r.PhoneNumber == phoneNumber && r.Id != id);
            if (exists)
            {
                _logger.LogWarning("Phone number {PhoneNumber} already exists for another recipient", phoneNumber);
                throw new InvalidOperationException($"Phone number {phoneNumber} already exists");
            }

            recipient.PhoneNumber = phoneNumber;
            recipient.UpdatedAt = DateTime.UtcNow;

            _repository.Update(recipient);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Recipient updated successfully, ID: {Id}", id);
            return recipient;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Deleting recipient ID: {Id}", id);

            var recipient = await _repository.GetByIdAsync(id);
            if (recipient == null)
            {
                _logger.LogWarning("Recipient not found for deletion, ID: {Id}", id);
                return false;
            }

            _repository.Remove(recipient);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Recipient deleted successfully, ID: {Id}", id);

            return true;
        }
    }
}