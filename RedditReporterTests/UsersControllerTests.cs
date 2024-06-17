using FluentAssertions;
using Microsoft.Extensions.Logging;
using RedditReporter.Controllers;
using DataAccess;
using Contracts;

namespace RedditReporterTests
{
    [TestClass]
    public class UsersControllerTests
    {
        [TestMethod]
        public void GivenThereAreNoUsers_WhenIRequestTheUsers_ThenTheResultIsEmpty()
        {
            // arrange
            var logger = new LoggerFactory()
                .CreateLogger<UsersController>();
            var usersRepository = new MemoryUsersRepository();
            var sut = new UsersController(logger, usersRepository);

            // act
            var result = sut.GetTopUsers();

            // assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GivenThereAreUsers_WhenIRequestTheUsers_ThenIGetAListOfUserSummaries()
        {
            // arrange
            var logger = new LoggerFactory()
                .CreateLogger<UsersController>();
            var usersRepository = new MemoryUsersRepository();
            var userSummary = new UserSummary(Guid.NewGuid().ToString(), 5);
            usersRepository.UpdateUserSummary(userSummary);
            var sut = new UsersController(logger, usersRepository);

            // act
            var result = sut.GetTopUsers();

            // assert
            result.Count().Should().Be(1);
            result.First().Posts.Should().Be(userSummary.Posts);
        }
    }
}
