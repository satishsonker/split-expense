
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using SplitExpense.Data;
using SplitExpense.EmailManagement.Service;
using SplitExpense.ExceptionManagement.Exceptions;
using SplitExpense.Logger;
using SplitExpense.Models.DbModels;
using SplitExpense.SharedResource;
using Xunit;

namespace SplitExpense.UnitTest.EmailManagement
{
    public class EmailQueueServiceTests
    {
        private readonly Mock<SplitExpenseDbContext> _mockContext;
        private readonly Mock<ISplitExpenseLogger> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<EmailServiceFactory> _mockEmailServiceFactory;
        private readonly EmailQueueService _service;
        private readonly Mock<DbSet<EmailQueue>> _mockEmailQueueDbSet;

        public EmailQueueServiceTests()
        {
            _mockContext = new Mock<SplitExpenseDbContext>();
            _mockLogger = new Mock<ISplitExpenseLogger>();
            _mockConfig = new Mock<IConfiguration>();
            _mockEmailServiceFactory = new Mock<EmailServiceFactory>();
            _mockEmailQueueDbSet = new Mock<DbSet<EmailQueue>>();

            _mockContext.Setup(c => c.EmailQueues).Returns(_mockEmailQueueDbSet.Object);

            _service = new EmailQueueService(
                _mockContext.Object,
                _mockLogger.Object,
                _mockConfig.Object,
                _mockEmailServiceFactory.Object
            );
        }

        [Fact]
        public async Task AddEmailToQueue_ValidInputs_ReturnsTrue()
        {
            // Arrange
            var email = "test@test.com";
            var subject = "Test Subject";
            var body = "Test Body";

            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.AddEmailToQueue(email, subject, body);

            // Assert
            Assert.True(result);
            _mockEmailQueueDbSet.Verify(d => d.AddAsync(
                It.IsAny<EmailQueue>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [InlineData("", "subject", "body")]
        [InlineData(" ", "subject", "body")]
        [InlineData(null, "subject", "body")]
        public async Task AddEmailToQueue_InvalidEmail_ThrowsException(string email, string subject, string body)
        {
            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleViolationException>(
                () => _service.AddEmailToQueue(email, subject, body));
        }

        [Theory]
        [InlineData("test@test.com", "", "body")]
        [InlineData("test@test.com", " ", "body")]
        [InlineData("test@test.com", null, "body")]
        public async Task AddEmailToQueue_InvalidSubject_ThrowsException(string email, string subject, string body)
        {
            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleViolationException>(
                () => _service.AddEmailToQueue(email, subject, body));
        }

        [Theory]
        [InlineData("test@test.com", "subject", "")]
        [InlineData("test@test.com", "subject", " ")]
        [InlineData("test@test.com", "subject", null)]
        public async Task AddEmailToQueue_InvalidBody_ThrowsException(string email, string subject, string body)
        {
            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleViolationException>(
                () => _service.AddEmailToQueue(email, subject, body));
        }

        [Fact]
        public async Task AddEmailToQueue_DefaultRetryCount_SetsToThree()
        {
            // Arrange
            var email = "test@test.com";
            var subject = "Test Subject";
            var body = "Test Body";

            _mockConfig.Setup(c => c.GetSection("EmailSettings:SendEmailRetryCount").Get<int>())
                .Returns(0);

            // Act
            await _service.AddEmailToQueue(email, subject, body);

            // Assert
            _mockEmailQueueDbSet.Verify(d => d.AddAsync(
                It.Is<EmailQueue>(e => e.RetryCount == 3),
                It.IsAny<CancellationToken>()));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public async Task AddEmailToQueue_CustomRetryCount_SetsCorrectly(int retryCount)
        {
            // Arrange
            var email = "test@test.com";
            var subject = "Test Subject";
            var body = "Test Body";

            _mockConfig.Setup(c => c.GetSection("EmailSettings:SendEmailRetryCount").Get<int>())
                .Returns(retryCount);

            // Act
            await _service.AddEmailToQueue(email, subject, body);

            // Assert
            _mockEmailQueueDbSet.Verify(d => d.AddAsync(
                It.Is<EmailQueue>(e => e.RetryCount == retryCount),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task AddEmailToQueue_SaveChangesFails_ReturnsFalse()
        {
            // Arrange
            var email = "test@test.com";
            var subject = "Test Subject";
            var body = "Test Body";

            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            // Act
            var result = await _service.AddEmailToQueue(email, subject, body);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddEmailToQueue_SetsCorrectStatus()
        {
            // Arrange
            var email = "test@test.com";
            var subject = "Test Subject";
            var body = "Test Body";

            // Act
            await _service.AddEmailToQueue(email, subject, body);

            // Assert
            _mockEmailQueueDbSet.Verify(d => d.AddAsync(
                It.Is<EmailQueue>(e => e.Status == EmailStatus.Pending),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task AddEmailToQueue_SetsCreatedAtToCurrentTime()
        {
            // Arrange
            var email = "test@test.com";
            var subject = "Test Subject";
            var body = "Test Body";
            var beforeTest = DateTime.Now;

            // Act
            await _service.AddEmailToQueue(email, subject, body);

            // Assert
            _mockEmailQueueDbSet.Verify(d => d.AddAsync(
                It.Is<EmailQueue>(e => e.CreatedAt >= beforeTest && e.CreatedAt <= DateTime.Now),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task AddEmailToQueue_VeryLongInputs_Succeeds()
        {
            // Arrange
            var email = "very.long.email.address@really.long.domain.name.com";
            var subject = new string('a', 1000); // Very long subject
            var body = new string('b', 10000); // Very long body

            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.AddEmailToQueue(email, subject, body);

            // Assert
            Assert.True(result);
        }
    }
}