using AutoMapper;
using Moq;
using SplitExpense.EmailManagement.Service;
using SplitExpense.ExceptionManagement.Exceptions;
using SplitExpense.Logger;
using SplitExpense.Logic.Email;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;
using SplitExpense.Models.DTO.Response;
using SplitExpense.SharedResource;
using Xunit;

namespace SplitExpense.UnitTest.Logic
{
    public class EmailLogicTests
    {
        private readonly Mock<IEmailTemplateService> _emailTemplateServiceMock;
        private readonly Mock<IEmailQueueService> _emailQueueServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ISplitExpenseLogger> _loggerMock;
        private readonly EmailLogic _emailLogic;

        public EmailLogicTests()
        {
            _emailTemplateServiceMock = new Mock<IEmailTemplateService>();
            _emailQueueServiceMock = new Mock<IEmailQueueService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ISplitExpenseLogger>();
            _emailLogic = new EmailLogic(_emailTemplateServiceMock.Object, _emailQueueServiceMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetEmailQueueAsync_ShouldReturnMappedResponse()
        {
            // Arrange
            var request = new PagingRequest();
            var queueResponse = new EmailQueueResponse();
            _emailQueueServiceMock.Setup(x => x.FilterEmails(It.IsAny<PagingRequest>())).ReturnsAsync(new PagingResponse<EmailQueue>());
            _mapperMock.Setup(x => x.Map<EmailQueueResponse>(It.IsAny<object>())).Returns(queueResponse);

            // Act
            var result = await _emailLogic.GetEmailQueueAsync(request);

            // Assert
            Assert.Equal(queueResponse, result);
        }

        [Fact]
        public async Task SendEmailOnUserAddedInGroup_WithValidTemplate_ShouldAddToQueue()
        {
            // Arrange
            var template = new EmailTemplate { Subject = "Test ##addedByUserName## ##groupName##", Body = "Test ##addedByUserName## ##addedByUserEmail## ##groupName##" };
            _emailTemplateServiceMock.Setup(x => x.GetTemplateByCodeAsync(EmailTemplateCode.UserAddedToExpenseGroup)).ReturnsAsync(template);

            // Act
            await _emailLogic.SendEmailOnUserAddedInGroup("test@email.com", "User1", "User2", DateTime.Now, new Dictionary<string, string> { { "groupName", "Group1" }, { "addedByUserEmail", "user1@email.com" } });

            // Assert
            _emailQueueServiceMock.Verify(x => x.AddEmailToQueue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task SendEmailOnUserAddedInGroup_WithNullTemplate_ShouldLogError()
        {
            // Arrange
            _emailTemplateServiceMock.Setup(x => x.GetTemplateByCodeAsync(EmailTemplateCode.UserAddedToExpenseGroup)).ReturnsAsync((EmailTemplate)null);

            // Act
            await _emailLogic.SendEmailOnUserAddedInGroup("test@email.com", "User1", "User2", DateTime.Now, new Dictionary<string, string>());

            // Assert
            _loggerMock.Verify(x => x.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task SendEmailOnUserAddedInGroup_WithException_ShouldLogError()
        {
            // Arrange
            _emailTemplateServiceMock.Setup(x => x.GetTemplateByCodeAsync(EmailTemplateCode.UserAddedToExpenseGroup)).ThrowsAsync(new Exception("Test Exception"));

            // Act
            await _emailLogic.SendEmailOnUserAddedInGroup("test@email.com", "User1", "User2", DateTime.Now, new Dictionary<string, string>());

            // Assert
            _loggerMock.Verify(x => x.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetEmailQueueAsync_WithNullResponse_ShouldReturnNull()
        {
            // Arrange
            var request = new PagingRequest();
            _emailQueueServiceMock.Setup(x => x.FilterEmails(request)).ReturnsAsync((PagingResponse<EmailQueue>)null);
            _mapperMock.Setup(x => x.Map<EmailQueueResponse>(null)).Returns((EmailQueueResponse)null);

            // Act
            var result = await _emailLogic.GetEmailQueueAsync(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SendEmailOnUserAddedInGroup_WithEmptyData_ShouldThrowException()
        {
            // Arrange
            var template = new EmailTemplate { Subject = "Test", Body = "Test" };
            _emailTemplateServiceMock.Setup(x => x.GetTemplateByCodeAsync(EmailTemplateCode.UserAddedToExpenseGroup)).ReturnsAsync(template);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _emailLogic.SendEmailOnUserAddedInGroup("test@email.com", "User1", "User2", DateTime.Now, new Dictionary<string, string>()));
        }

        [Fact]
        public async Task SendEmailOnUserAddedInGroup_WithNullData_ShouldThrowException()
        {
            // Arrange
            var template = new EmailTemplate { Subject = "Test", Body = "Test" };
            _emailTemplateServiceMock.Setup(x => x.GetTemplateByCodeAsync(EmailTemplateCode.UserAddedToExpenseGroup)).ReturnsAsync(template);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _emailLogic.SendEmailOnUserAddedInGroup("test@email.com", "User1", "User2", DateTime.Now, null));
        }

        [Fact]
        public async Task GetEmailQueueAsync_ShouldCallFilterEmails()
        {
            // Arrange
            var request = new PagingRequest();

            // Act
            await _emailLogic.GetEmailQueueAsync(request);

            // Assert
            _emailQueueServiceMock.Verify(x => x.FilterEmails(request), Times.Once);
        }

        [Fact]
        public async Task SendEmailOnUserAddedInGroup_ShouldReplaceAllPlaceholders()
        {
            // Arrange
            var template = new EmailTemplate
            {
                Subject = "##addedByUserName## added you to ##groupName##",
                Body = "##addedByUserName## (##addedByUserEmail##) added you to ##groupName##"
            };
            _emailTemplateServiceMock.Setup(x => x.GetTemplateByCodeAsync(EmailTemplateCode.UserAddedToExpenseGroup)).ReturnsAsync(template);

            var data = new Dictionary<string, string>
            {
                { "groupName", "TestGroup" },
                { "addedByUserEmail", "test@email.com" }
            };

            // Act
            await _emailLogic.SendEmailOnUserAddedInGroup("recipient@email.com", "TestUser", "AddedUser", DateTime.Now, data);

            // Assert
            _emailQueueServiceMock.Verify(x => x.AddEmailToQueue(
                "recipient@email.com",
                It.Is<string>(s => s.Contains("TestUser") && s.Contains("TestGroup")),
                It.Is<string>(s => s.Contains("TestUser") && s.Contains("test@email.com") && s.Contains("TestGroup"))
            ), Times.Once);
        }

        [Fact]
        public async Task SendEmailOnUserAddedInGroup_WithInvalidTemplateCode_ShouldThrowException()
        {
            // Arrange
            _emailTemplateServiceMock.Setup(x => x.GetTemplateByCodeAsync(It.IsAny<EmailTemplateCode>())).ReturnsAsync((EmailTemplate)null);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleViolationException>(() =>
                _emailLogic.SendEmailOnUserAddedInGroup("test@email.com", "User1", "User2", DateTime.Now, new Dictionary<string, string>()));
        }
    }
}
