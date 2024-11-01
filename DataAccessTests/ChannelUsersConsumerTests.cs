using DataAccessTests.Builders;
using FluentAssertions;

namespace DataAccessTests
{
    [TestClass]
    public class ChannelUsersConsumerTests
    {
        [TestMethod]
        public void GivenThereAreNoUsers_WhenIGetUsers_ThenIGetNoUsers()
        {
            // arrange
            var sut = new ChannelUsersConsumerBuilder()
                .Build().Result;

            // act
            var result = sut.GetUserSummaries();

            // assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GivenThereAreUsers_WhenIGetUsers_ThenIGetTheUsers()
        {
            // arrange
            var numUsers = 10;
            var sut = new ChannelUsersConsumerBuilder()
                .WithUsers(numUsers).Result
                .Build().Result;

            // act
            var result = sut.GetUserSummaries();

            // assert
            result.Count().Should().Be(numUsers);
        }

        [TestMethod]
        public void GivenThereIsAUser_WhenIGetUsers_ThenIGetTheCorrectUser()
        {
            // arrange
            var post = DataGenerators.RandomUserSummary();
            var sut = new ChannelUsersConsumerBuilder()
                .WithUser(post).Result
                .Build().Result;

            // act
            var result = sut.GetUserSummaries();

            // assert
            result.Should().NotBeEmpty();
            var newSummary = result.First();
            newSummary.Name.Should().Be(post.Name);
            newSummary.Posts.Should().Be(post.Posts);
        }

        [TestMethod]
        public void GivenThereAreUsersAndTheConsumerIsCancelled_WhenIGetUsers_ThenIGetNoUsers()
        {
            // arrange
            var sut = new ChannelUsersConsumerBuilder()
                .IsCancelled()
                .WithUsers(1).Result
                .Build().Result;

            // act
            var result = sut.GetUserSummaries();

            // assert
            result.Should().BeEmpty();
        }
    }
}
