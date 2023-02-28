using Moq;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Services.Users;

namespace SchoolWebRegister.Tests.Modal_Tests
{
    public class UserServiceTests
    {
        private ApplicationUser GetTestUser()
        {
            return new ApplicationUser
            {
                Email = "xxxx@example.com",
                NormalizedEmail = "XXXX@EXAMPLE.COM",
                UserName = "TestUser123",
                NormalizedUserName = "TESTUSER123",
                PhoneNumber = "+111111111111",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };
        }

        [Fact]
        public async void Find_User_After_Create_In_Database()
        {
            // Arrange
            string testUserId = "1";
            ApplicationUser testUser = GetTestUser();
            testUser.Id = testUserId;

            var mock = new Mock<IUserService>();
            mock.Setup(service => service.CreateUser(testUser));

            // Act
            var createResult = await mock.Object.CreateUser(testUser);

            // Assert
            var findResult = await mock.Object.GetUserById(testUserId);

            Assert.True(createResult.StatusCode == Domain.StatusCode.Successful);
            Assert.True(findResult.StatusCode == Domain.StatusCode.Successful);
            Assert.NotNull(findResult.Data);
            Assert.Equal(testUserId, findResult.Data.Id);
        }
    }
}
