using Microsoft.Extensions.Logging;
using Moq;
using PhoneValidate.Domain.Service.Interfaces;
using PhoneValidate.Domain.Service.Models;
using PhoneValidate.Domain.Service.Services;
using System.Linq.Expressions;

namespace PhoneValidate.Tests
{
    public class RecipientServiceTests
    {
        private readonly RecipientService _service;
        private readonly Mock<ILogger<RecipientService>> _mockLogger;
        private readonly Mock<IBaseRepository<Recipient>> _mockRepository;

        public RecipientServiceTests()
        {
            _mockLogger = new Mock<ILogger<RecipientService>>();
            _mockRepository = new Mock<IBaseRepository<Recipient>>();
            _service = new RecipientService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetRecipientAsync_ShouldReturnFirstRecipient_WhenRecipientsExist()
        {
            // Arrange
            var phone = "+1234567890";
            var recipients = new Recipient
            {
                Id = Guid.NewGuid(),
                PhoneNumber = phone,
                UpdatedAt = DateTime.UtcNow
            };

            _mockRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Recipient, bool>>>()))
                .ReturnsAsync(recipients);

            // Act
            var result = await _service.GetByPhoneNumberAsync(phone);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(recipients.Id, result.Id);
            Assert.Equal(phone, result.PhoneNumber);
            _mockRepository.Verify(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Recipient, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task GetRecipientAsync_ShouldReturnNull_WhenNoRecipientsExist()
        {
            // Arrange
            _mockRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Recipient, bool>>>()))
                .ReturnsAsync((Recipient)null);

            // Act
            var result = await _service.GetByPhoneNumberAsync(It.IsAny<string>());

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Recipient, bool>>>()), Times.Once);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_WhenRecipientIsValid()
        {
            // Arrange
            var recipient = new Recipient
            {
                PhoneNumber = "+55 11 98765-4321",
                Histories = new List<History>
                {
                    new History
                    {
                        CreatedAt = DateTime.UtcNow
                    }
                }
            };

            _mockRepository.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Recipient, bool>>>()))
                .ReturnsAsync(false);

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Recipient>()))
                .Returns(Task.CompletedTask);

            _mockRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(recipient);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotEqual(Guid.Empty, result.Data.Id);
            Assert.NotNull(result.Data.UpdatedAt);
            Assert.Equal("+5511987654321", result.Data.PhoneNumber);

            // Verify
            _mockRepository.Verify(r => r.AnyAsync(It.IsAny<Expression<Func<Recipient, bool>>>()), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Recipient>()), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnFailure_WhenRecipientAlreadyExists()
        {
            // Arrange
            var existingPhoneNumber = "+55 11 98765-4321";
            var recipient = new Recipient
            {
                PhoneNumber = existingPhoneNumber,
                Histories = new List<History>()
            };

            _mockRepository.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Recipient, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.CreateAsync(recipient);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("already exists", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("+5511987654321", result.ErrorMessage);

            _mockRepository.Verify(r => r.AnyAsync(It.IsAny<Expression<Func<Recipient, bool>>>()), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Recipient>()), Times.Never);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ShouldSetIdAndUpdatedAt_WhenRecipientIsCreated()
        {
            // Arrange
            var recipient = new Recipient
            {
                PhoneNumber = "+55 21 99999-8888",
                Histories = new List<History>()
            };

            var beforeCreation = DateTime.UtcNow;

            _mockRepository.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Recipient, bool>>>()))
                .ReturnsAsync(false);

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Recipient>()))
                .Returns(Task.CompletedTask);

            _mockRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(recipient);

            // Assert
            Assert.True(result.Success);
            Assert.NotEqual(Guid.Empty, result.Data!.Id);
            Assert.NotNull(result.Data.UpdatedAt);
            Assert.True(result.Data.UpdatedAt >= beforeCreation);
            Assert.True(result.Data.UpdatedAt <= DateTime.UtcNow.AddSeconds(1));
        }

        [Theory]
        [InlineData("+5511987654321")]
        [InlineData("5511987654321")]
        public async Task CreateAsync_ShouldAddPlusSign_WhenNotPresent(string phoneNumber)
        {
            // Arrange
            var recipient = new Recipient
            {
                PhoneNumber = phoneNumber,
                Histories = new List<History>()
            };

            _mockRepository.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Recipient, bool>>>()))
                .ReturnsAsync(false);

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Recipient>()))
                .Returns(Task.CompletedTask);

            _mockRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(recipient);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.StartsWith("+", result.Data.PhoneNumber);  
        }

        [Theory]
        [InlineData("")]
        [InlineData("+")]
        [InlineData("  ")]
        [InlineData("123ABC456")]
        public async Task CreateAsync_ShouldReturnNUll_IfNumberInvalid(string phoneNumber)
        {
            // Arrange
            var recipient = new Recipient
            {
                PhoneNumber = phoneNumber,
                Histories = new List<History>()
            };

            _mockRepository.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Recipient, bool>>>()))
                .ReturnsAsync(false);

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Recipient>()))
                .Returns(Task.CompletedTask);

            _mockRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(recipient);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.NotNull(result.ErrorMessage);
        }
        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenRecipientExists()
        {
            // Arrange
            var recipientId = Guid.NewGuid();
            var existingRecipient = new Recipient
            {
                Id = recipientId,
                PhoneNumber = "+5511987654321",
                UpdatedAt = DateTime.UtcNow
            };

            _mockRepository.Setup(r => r.GetByIdAsync(recipientId))
                .ReturnsAsync(existingRecipient);

            _mockRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.DeleteAsync(recipientId);

            // Assert
            Assert.True(result.Success);
            Assert.True(result.Data);

            // Verify
            _mockRepository.Verify(r => r.GetByIdAsync(recipientId), Times.Once);
            _mockRepository.Verify(r => r.Remove(existingRecipient), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFailure_WhenRecipientDoesNotExist()
        {
            // Arrange
            var recipientId = Guid.NewGuid();

            _mockRepository.Setup(r => r.GetByIdAsync(recipientId))
                .ReturnsAsync((Recipient?)null);

            // Act
            var result = await _service.DeleteAsync(recipientId);

            // Assert
            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("not found", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(recipientId.ToString(), result.ErrorMessage);

            _mockRepository.Verify(r => r.GetByIdAsync(recipientId), Times.Once);
            _mockRepository.Verify(r => r.Remove(It.IsAny<Recipient>()), Times.Never);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        #endregion
    }
}