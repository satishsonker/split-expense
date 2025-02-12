using Microsoft.EntityFrameworkCore;
using Moq;
using SplitExpense.Data;
using SplitExpense.EmailManagement.Service;
using SplitExpense.ExceptionManagement.Exceptions;
using SplitExpense.Models.DbModels;
using SplitExpense.SharedResource;
using Xunit;

namespace SplitExpense.UnitTest.EmailManagement
{
    public class EmailTemplateServiceTests
    {
        private readonly Mock<SplitExpenseDbContext> _mockContext;
        private readonly EmailTemplateService _service;
        private readonly Mock<DbSet<EmailTemplate>> _mockSet;

        public EmailTemplateServiceTests()
        {
            _mockContext = new Mock<SplitExpenseDbContext>();
            _mockSet = new Mock<DbSet<EmailTemplate>>();
            //_mockContext.Setup(c => c.EmailTemplates).Returns(_mockSet.Object);
            _service = new EmailTemplateService(_mockContext.Object);
        }

        [Fact]
        public async Task AddTemplateAsync_ValidTemplate_ReturnsId()
        {
            var entity = new EmailTemplate { Id = 1, TemplateCode = "TestEntity", Subject = string.Empty, Body = string.Empty };

            // Mock AddAsync method
            _mockSet.Setup(m => m.AddAsync(It.IsAny<EmailTemplate>(), It.IsAny<CancellationToken>()))
                     .Returns((EmailTemplate e, CancellationToken ct) =>
                         ValueTask.FromResult((Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<EmailTemplate>)null));

            _mockContext.Setup(c => c.EmailTemplates).Returns(_mockSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            

            // Act
            var result = await _service.AddTemplateAsync(entity);

            // Assert
            _mockSet.Verify(m => m.AddAsync(It.IsAny<EmailTemplate>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task AddTemplateAsync_NullTemplate_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddTemplateAsync(null));
        }

        [Fact]
        public async Task AddTemplateAsync_InvalidTemplateCode_ThrowsBusinessRuleViolationException()
        {
            // Arrange
            var template = new EmailTemplate { TemplateCode = "InvalidCode",Subject=string.Empty,Body=string.Empty };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _service.AddTemplateAsync(template));
        }

        [Fact]
        public async Task AddTemplateAsync_DuplicateTemplate_ThrowsBusinessRuleViolationException()
        {
            // Arrange
            var template = new EmailTemplate { TemplateCode = "WelcomeEmail", Subject = string.Empty, Body = string.Empty };
            var existingTemplates = new List<EmailTemplate>
            {
                new EmailTemplate { Id = 1, TemplateCode = "WelcomeEmail",Subject=string.Empty,Body=string.Empty }
            };
            _mockSet.Setup(x => x.FirstOrDefaultAsync(default))
                .ReturnsAsync(existingTemplates[0]);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _service.AddTemplateAsync(template));
        }

        [Fact]
        public async Task DeleteTemplateAsync_ExistingTemplate_ReturnsTrue()
        {
            // Arrange
            var template = new EmailTemplate { Id = 1, TemplateCode = "WelcomeEmail", Subject = string.Empty, Body = string.Empty };
            _mockSet.Setup(x => x.FirstOrDefaultAsync(default))
                .ReturnsAsync(template);
            _mockContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteTemplateAsync(1);

            // Assert
            Assert.True(result);
            Assert.True(template.IsDeleted);
        }

        [Fact]
        public async Task DeleteTemplateAsync_NonExistingTemplate_ThrowsBusinessRuleViolationException()
        {
            // Arrange
            _mockSet.Setup(x => x.FirstOrDefaultAsync(default))
                .ReturnsAsync((EmailTemplate)null);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _service.DeleteTemplateAsync(1));
        }

        [Fact]
        public async Task GetAllTemplatesAsync_ReturnsAllNonDeletedTemplates()
        {
            // Arrange
            var templates = new List<EmailTemplate>
            {
                new EmailTemplate { Id = 1, TemplateCode = "WelcomeEmail", IsDeleted = false,Subject=string.Empty,Body=string.Empty },
                new EmailTemplate { Id = 2, TemplateCode = "ForgotPassword", IsDeleted = false,Subject=string.Empty,Body=string.Empty }
            };
            _mockSet.Setup(x => x.Where(It.IsAny<Func<EmailTemplate, bool>>()))
                .Returns(templates.AsQueryable());

            // Act
            var result = await _service.GetAllTemplatesAsync();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetTemplateByCodeAsync_ExistingCode_ReturnsTemplate()
        {
            // Arrange
            var template = new EmailTemplate { Id = 1, TemplateCode = "UserAddedToExpenseGroup", Subject = string.Empty, Body = string.Empty };
            _mockSet.Setup(x => x.FirstOrDefaultAsync(default))
                .ReturnsAsync(template);

            // Act
            var result = await _service.GetTemplateByCodeAsync(EmailTemplateCode.UserAddedToExpenseGroup);

            // Assert
            Assert.Equal(template.Id, result.Id);
        }

        [Fact]
        public async Task GetTemplateByIdAsync_ExistingId_ReturnsTemplate()
        {
            // Arrange
            var template = new EmailTemplate {Id = 1, TemplateCode = "WelcomeEmail", Subject = string.Empty, Body = string.Empty };
            _mockSet.Setup(x => x.FirstOrDefaultAsync(default))
                .ReturnsAsync(template);

            // Act
            var result = await _service.GetTemplateByIdAsync(1);

            // Assert
            Assert.Equal(template.Id, result.Id);
        }
    }
}