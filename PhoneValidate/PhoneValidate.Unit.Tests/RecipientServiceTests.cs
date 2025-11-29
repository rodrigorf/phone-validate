using Microsoft.Extensions.Logging;
using Moq;
using PhoneValidate.Domain.Service.Interfaces;
using PhoneValidate.Domain.Service.Models;
using PhoneValidation.Domain.Service.Services;
using System.Linq.Expressions;

namespace PhoneValidation.Tests
{
    public class RecipientServiceTests
    {
        private readonly RecipientService _service;
        private readonly Mock<ILogger<RecipientService>> _mockLogger;
        private readonly Mock<IBaseRepository<Recipients>> _mockRepository;

        public RecipientServiceTests()
        {
            _mockLogger = new Mock<ILogger<RecipientService>>();
            _mockRepository = new Mock<IBaseRepository<Recipients>>();
            _service = new RecipientService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetForecastAsync_ShouldReturnFirstForecast_WhenForecastsExist()
        {
            // Arrange
            var phone = "+1234567890";
            var recipients = new Recipients
            {
                Id = Guid.NewGuid(),
                PhoneNumber = phone,
                UpdatedAt = DateTime.UtcNow
            };

            _mockRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Recipients, bool>>>()))
                .ReturnsAsync(recipients);

            // Act
            var result = await _service.GetByPhoneNumberAsync(phone);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(recipients.Id, result.Id);
            Assert.Equal(phone, result.PhoneNumber);
            _mockRepository.Verify(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Recipients, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task GetForecastAsync_ShouldReturnNull_WhenNoForecastsExist()
        {
            // Arrange
            _mockRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Recipients, bool>>>()))
                .ReturnsAsync((Recipients)null);

            // Act
            var result = await _service.GetByPhoneNumberAsync(It.IsAny<string>());

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Recipients, bool>>>()), Times.Once);
        }
    }
}