using Microsoft.Extensions.Logging;
using PhoneValidate.Domain.Service.Interfaces;
using PhoneValidate.Domain.Service.Models;
using System.Text.RegularExpressions;

namespace PhoneValidate.Domain.Service.Services
{
    public class RecipientService : IRecipientService
    {
        private readonly IBaseRepository<Recipient> _repository;
        private readonly ILogger<RecipientService> _logger;

        public RecipientService(
            IBaseRepository<Recipient> repository,
            ILogger<RecipientService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Recipient?> GetByPhoneNumberAsync(string phoneNumber)
        {
            _logger.LogInformation("Getting recipient by phone number: {PhoneNumber}", phoneNumber);

            var recipient = await _repository.FirstOrDefaultAsync(r => r.PhoneNumber == phoneNumber);

            return recipient;
        }

        public async Task<Recipient?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting recipient by ID: {Id}", id);
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Result<Recipient>> CreateAsync(Recipient recipient)
        {
            _logger.LogInformation("Creating new recipient with phone: {PhoneNumber}", recipient.PhoneNumber);

            var normalizedPhone = NormalizePhoneNumber(recipient.PhoneNumber);
            if (normalizedPhone is null)
            {
                var message = $"Phone number '{recipient.PhoneNumber}' is invalid.";
                _logger.LogWarning(message);
                return Result<Recipient>.Fail(message);
            }

            recipient.PhoneNumber = normalizedPhone;
            var exists = await _repository.AnyAsync(r => r.PhoneNumber == recipient.PhoneNumber);
            if (exists)
            {
                _logger.LogWarning("Recipient already exists with phone: {PhoneNumber}", recipient.PhoneNumber);
                return Result<Recipient>.Fail($"A recipient with phone number '{recipient.PhoneNumber}' already exists.");
            }

            recipient.Id = Guid.NewGuid();
            recipient.UpdatedAt = DateTime.UtcNow;

            await _repository.AddAsync(recipient);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Recipient created successfully with ID: {Id}", recipient.Id);
            return Result<Recipient>.Ok(recipient);
        }

        public async Task<Result<Recipient?>> UpdateAsync(Guid id, Recipient recipients)
        {
            _logger.LogInformation("Updating recipient ID: {Id}", id);

            var foundRecipient = await _repository.GetByIdAsync(id);
            if (foundRecipient == null)
            {
                _logger.LogWarning("Recipient not found for update, ID: {Id}", id);
                return Result<Recipient?>.Fail($"A recipient with ID '{id}' does not exist.");
            }

            var exists = await _repository.AnyAsync(r => r.PhoneNumber == foundRecipient.PhoneNumber && r.Id != id);
            if (exists)
            {
                _logger.LogWarning("Phone number {PhoneNumber} already exists for another recipient", foundRecipient.PhoneNumber);
                return Result<Recipient?>.Fail($"A recipient with phone number '{foundRecipient.PhoneNumber}' already exists.");
            }

            foundRecipient.UpdatedAt = DateTime.UtcNow;
            foundRecipient.PhoneNumber = recipients.PhoneNumber;

            _repository.Update(foundRecipient);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Recipient updated successfully, ID: {Id}", id);
            return Result<Recipient?>.Ok(foundRecipient);
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

        private string? NormalizePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return null;

            var digitsOnly = Regex.Replace(phoneNumber, @"\D", "");

            if (digitsOnly.Length < 10 || digitsOnly.Length > 17)
                return null;

            return $"+{digitsOnly}";
        }
    }
}