using AutoMapper;
using Moq;
using SplitExpense.Data.Factory;
using SplitExpense.Logger;
using SplitExpense.Logic;
using SplitExpense.Logic.Email;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO;
using Xunit;

namespace SplitExpense.UnitTest.Logic
{
    public class GroupsLogicTests
    {
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IGroupFactory> _mockFactory;
        private readonly Mock<IEmailLogic> _mockEmailLogic;
        private readonly Mock<ISplitExpenseLogger> _mockLogger;
        private readonly GroupsLogic _groupsLogic;

        public GroupsLogicTests()
        {
            _mockMapper = new Mock<IMapper>();
            _mockFactory = new Mock<IGroupFactory>();
            _mockEmailLogic = new Mock<IEmailLogic>();
            _mockLogger = new Mock<ISplitExpenseLogger>();
            _groupsLogic = new GroupsLogic(_mockMapper.Object, _mockFactory.Object, _mockEmailLogic.Object,_mockLogger.Object);
        }

        [Fact]
        public async Task AddFriendInGroupAsync_WhenValidRequest_ReturnsTrue()
        {
            // Arrange
            var request = new AddFriendInGroupRequest() {FriendId=1,GroupId=1 };
            var groupMapping = new UserGroupMapping { Id = 1 };
            var mappingData = new UserGroupMappingResponse
            {
                GroupName = "Test Group",
                AddedByUser = "Test User",
                AddedUser = new UserResponse(),
                CreatedAt = DateTime.Now
            };

            _mockFactory.Setup(f => f.AddFriendInGroupAsync(request))
                .ReturnsAsync(groupMapping);
            _mockMapper.Setup(m => m.Map<UserGroupMappingResponse>(It.IsAny<object>()))
                .Returns(mappingData);

            // Act
            var result = await _groupsLogic.AddFriendInGroupAsync(request);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task AddFriendInGroupAsync_WhenFactoryReturnsNull_ReturnsFalse()
        {
            // Arrange
            var request = new AddFriendInGroupRequest() { FriendId = 1, GroupId = 1 }; ;
            _mockFactory.Setup(f => f.AddFriendInGroupAsync(request))
                .ReturnsAsync((UserGroupMapping)null);

            // Act
            var result = await _groupsLogic.AddFriendInGroupAsync(request);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task CreateAsync_WithValidRequest_ReturnsGroupResponse(int id)
        {
            // Arrange
            var request = new GroupRequest { Name = "Test Group" };
            var group = new Group { Id = id, Name = "Test Group",Icon=string.Empty,UserId=1 };
            var response = new GroupResponse { Id = id, Name = "Test Group" };

            _mockMapper.Setup(m => m.Map<Group>(request)).Returns(group);
            _mockMapper.Setup(m => m.Map<GroupResponse>(group)).Returns(response);
            _mockFactory.Setup(f => f.CreateAsync(group)).ReturnsAsync(group);

            // Act
            var result = await _groupsLogic.CreateAsync(request);

            // Assert
            Assert.Equal(id, result.Id);
            Assert.Equal(request.Name, result.Name);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task DeleteAsync_WithValidId_ReturnsTrue(int id)
        {
            // Arrange
            _mockFactory.Setup(f => f.DeleteAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _groupsLogic.DeleteAsync(id);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetAsync_WithValidId_ReturnsGroupResponse(int id)
        {
            // Arrange
            var group = new Group { Id = id, Name = "Test Group", Icon = string.Empty, UserId = 1 };
            var response = new GroupResponse { Id = id, Name = "Test Group" };

            _mockFactory.Setup(f => f.GetAsync(id)).ReturnsAsync(group);
            _mockMapper.Setup(m => m.Map<GroupResponse>(group)).Returns(response);

            // Act
            var result = await _groupsLogic.GetAsync(id);

            // Assert
            Assert.Equal(id, result.Id);
            Assert.Equal(group.Name, result.Name);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsPagedResponse()
        {
            // Arrange
            var request = new PagingRequest { PageNo = 1, PageSize = 10 };
            var pagedGroups = new PagingResponse<Group>
            {
                Data = [new Group() { Icon="",Name="",Id=1,UserId=3}],
                RecordCounts = 1
            };
            var response = new PagingResponse<GroupResponse>
            {
                Data = new List<GroupResponse> { new() { Id = 1 } },
                RecordCounts = 1
            };

            _mockFactory.Setup(f => f.GetAllAsync(request)).ReturnsAsync(pagedGroups);
            _mockMapper.Setup(m => m.Map<PagingResponse<GroupResponse>>(pagedGroups))
                .Returns(response);

            // Act
            var result = await _groupsLogic.GetAllAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Data);
            Assert.Equal(1, result.RecordCounts);
        }

        [Fact]
        public async Task AddFriendInGroupAsync_SendsEmail_WhenSuccessful()
        {
            // Arrange
            var request = new AddFriendInGroupRequest() { FriendId=1,GroupId=1};
            var groupMapping = new UserGroupMapping { Id = 1 };
            var mappingData = new UserGroupMappingResponse
            {
                GroupName = "Test Group",
                AddedByUser = "Test User",
                AddedUser = new UserResponse(),
                CreatedAt = DateTime.Now
            };

            _mockFactory.Setup(f => f.AddFriendInGroupAsync(request))
                .ReturnsAsync(groupMapping);
            _mockMapper.Setup(m => m.Map<UserGroupMappingResponse>(It.IsAny<object>()))
                .Returns(mappingData);

            // Act
            await _groupsLogic.AddFriendInGroupAsync(request);

            // Assert
            _mockEmailLogic.Verify(e => e.SendEmailOnUserAddedInGroup(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithNullRequest_ThrowsException()
        {
            // Arrange
            GroupRequest request = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _groupsLogic.CreateAsync(request));
        }

        [Fact]
        public async Task GetAllAsync_WithInvalidPageSize_ThrowsException()
        {
            // Arrange
            var request = new PagingRequest { PageNo = 1, PageSize = -1 };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _groupsLogic.GetAllAsync(request));
        }
    }
}