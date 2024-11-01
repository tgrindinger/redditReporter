using FluentAssertions;
using RedditReporterTests.Builders;

namespace RedditReporterTests
{
    [TestClass]
    public class UsersControllerTests
    {
        [TestMethod]
        public void GivenThereAreNoUsers_WhenIRequestTheUsers_ThenIGetNoUsers()
        {
            // arrange
            var sut = new UsersControllerBuilder()
                .IsFlushed().Result
                .Build();

            // act
            var result = sut.GetTopUsers();

            // assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GivenThereIsAUser_WhenIRequestTheUsers_ThenIGetTheCorrectUser()
        {
            // arrange
            var userSummary = DataGenerators.RandomUserSummary();
            var sut = new UsersControllerBuilder()
                .WithUser(userSummary).Result
                .IsFlushed().Result
                .Build();

            // act
            var result = sut.GetTopUsers();

            // assert
            result.Count().Should().Be(1);
            result.First().Name.Should().Be(userSummary.Name);
            result.First().Posts.Should().Be(userSummary.Posts);
        }

        [TestMethod]
        public void GivenThereAreUsers_WhenIRequestThePosts_ThenIGetTheCorrectPosts()
        {
            // arrange
            var numUsers = 10000;
            var sut = new UsersControllerBuilder()
                .WithUsers(numUsers).Result
                .IsFlushed(numUsers, 1000).Result
                .Build();

            // act
            var result = sut.GetTopUsers();

            // assert
            result.Count().Should().Be(numUsers);
        }
    }
}
