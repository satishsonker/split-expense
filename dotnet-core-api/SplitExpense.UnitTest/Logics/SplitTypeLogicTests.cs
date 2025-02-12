using AutoMapper;
using Moq;
using SplitExpense.Data.Factory;
using SplitExpense.Logger;
using SplitExpense.Logic.Masters;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;
using SplitExpense.Models.DTO;
using Xunit;

namespace SplitExpense.UnitTest.Logic
{
    public class SplitTypeLogicTests
    {
        private readonly Mock<ISplitTypeFactory> _mockSplitTypeFactory;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ISplitExpenseLogger> _mockLogger;
        private readonly SplitTypeLogic _splitTypeLogic;

        public SplitTypeLogicTests()
        {
            _mockSplitTypeFactory = new Mock<ISplitTypeFactory>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ISplitExpenseLogger>();
            _splitTypeLogic = new SplitTypeLogic(_mockSplitTypeFactory.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateAsync_ValidRequest_ReturnsSplitTypeResponse()
        {
            // Arrange
            var request = new SplitTypeRequest { Name = "Test Split" };
            var splitType = new SplitType { Id = 1, Name = "Test Split" };
            var response = new SplitTypeResponse { Id = 1, Name = "Test Split" };

            _mockMapper.Setup(m => m.Map<SplitType>(request)).Returns(splitType);
            _mockSplitTypeFactory.Setup(f => f.CreateAsync(splitType)).ReturnsAsync(splitType);
            _mockMapper.Setup(m => m.Map<SplitTypeResponse>(splitType)).Returns(response);

            // Act
            var result = await _splitTypeLogic.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(response.Id, result.Id);
            Assert.Equal(response.Name, result.Name);
        }

        [Fact]
        public async Task CreateAsync_NullRequest_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _splitTypeLogic.CreateAsync(null));
        }

        [Fact]
        public async Task CreateAsync_FactoryThrowsException_LogsAndRethrows()
        {
            // Arrange
            var request = new SplitTypeRequest { Name = "Test" };
            var splitType = new SplitType { Name = "Test" };
            var exception = new Exception("Test exception");

            _mockMapper.Setup(m => m.Map<SplitType>(request)).Returns(splitType);
            _mockSplitTypeFactory.Setup(f => f.CreateAsync(splitType)).ThrowsAsync(exception);

            // Act & Assert
            var thrownException = await Assert.ThrowsAsync<Exception>(() => _splitTypeLogic.CreateAsync(request));
            _mockLogger.Verify(l => l.LogError(exception, "Error creating split type"), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        public async Task DeleteAsync_ValidId_ReturnsTrue(int id)
        {
            // Arrange
            _mockSplitTypeFactory.Setup(f => f.DeleteAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _splitTypeLogic.DeleteAsync(id);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task DeleteAsync_InvalidId_ThrowsArgumentException(int id)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _splitTypeLogic.DeleteAsync(id));
        }

        [Fact]
        public async Task GetAllAsync_ValidRequest_ReturnsPagingResponse()
        {
            // Arrange
            var request = new PagingRequest { PageNo = 1, PageSize = 10 };
            var response = new PagingResponse<SplitType>
            {
                Data = [new() { Id = 1, Name = "Test" }],
                RecordCounts = 1
            };
            var mappedResponse = new PagingResponse<SplitTypeResponse>
            {
                Data = [new() { Id = 1, Name = "Test" }],
                RecordCounts = 1
            };

            _mockSplitTypeFactory.Setup(f => f.GetAllAsync(request)).ReturnsAsync(response);
            _mockMapper.Setup(m => m.Map<PagingResponse<SplitTypeResponse>>(response)).Returns(mappedResponse);

            // Act
            var result = await _splitTypeLogic.GetAllAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Data);
            Assert.Equal(1, result.RecordCounts);
        }

        [Fact]
        public async Task GetAsync_ValidId_ReturnsSplitTypeResponse()
        {
            // Arrange
            var id = 1;
            var splitType = new SplitType { Id = id, Name = "Test" };
            var response = new SplitTypeResponse { Id = id, Name = "Test" };

            _mockSplitTypeFactory.Setup(f => f.GetAsync(id)).ReturnsAsync(splitType);
            _mockMapper.Setup(m => m.Map<SplitTypeResponse>(splitType)).Returns(response);

            // Act
            var result = await _splitTypeLogic.GetAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetAsync_InvalidId_ThrowsArgumentException(int id)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _splitTypeLogic.GetAsync(id));
        }

        [Fact]
        public async Task SearchAsync_ValidRequest_ReturnsPagingResponse()
        {
            // Arrange
            var request = new SearchRequest { SearchTerm = "Test", PageNo = 1, PageSize = 10 };
            var response = new PagingResponse<SplitType>
            {
                Data = [new() { Id = 1, Name = "Test" }],
                RecordCounts = 1
            };
            var mappedResponse = new PagingResponse<SplitTypeResponse>
            {
                Data = [new() { Id = 1, Name = "Test" }],
                RecordCounts = 1
            };

            _mockSplitTypeFactory.Setup(f => f.SearchAsync(request)).ReturnsAsync(response);
            _mockMapper.Setup(m => m.Map<PagingResponse<SplitTypeResponse>>(response)).Returns(mappedResponse);

            // Act
            var result = await _splitTypeLogic.SearchAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Data);
            Assert.Equal(1, result.RecordCounts);
        }

        [Fact]
        public async Task UpdateAsync_ValidRequest_ReturnsUpdatedId()
        {
            // Arrange
            var request = new SplitTypeRequest { Id = 1, Name = "Updated Test" };
            var splitType = new SplitType { Id = 1, Name = "Updated Test" };

            _mockMapper.Setup(m => m.Map<SplitType>(request)).Returns(splitType);
            _mockSplitTypeFactory.Setup(f => f.UpdateAsync(splitType)).ReturnsAsync(1);

            // Act
            var result = await _splitTypeLogic.UpdateAsync(request);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task UpdateAsync_NullRequest_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _splitTypeLogic.UpdateAsync(null));
        }
    }
}
