using Contracts;
using DataAccess;
using DataAccessTests.Builders;
using FluentAssertions;

namespace DataAccessTests
{
    [TestClass]
    public class MemoryUsersRepositoryTests
    {
        [TestMethod]
        public void GivenThereAreNoUsers_WhenIReadTheUsers_ThenThereAreNoUsers()
        {
            // arrange
            var sut = new MemoryUsersRepository();

            // act
            var result = sut.GetUserSummaries();

            // assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GivenThereAreNoUsers_WhenIAddAUser_ThenICanGetTheUserSummary()
        {
            // arrange
            var sut = new MemoryUsersRepository();
            var summary = new UserSummary(Guid.NewGuid().ToString(), 5);

            // act
            sut.UpdateUserSummary(summary);

            // assert
            var result = sut.GetUserSummaries();
            result.Count().Should().Be(1);
            result.First().Posts.Should().Be(summary.Posts);
        }

        [TestMethod]
        public void GivenThereIsAUser_WhenIUpdateTheUser_TheValueIsUpdated()
        {
            // arrange
            var sut = new MemoryUsersRepositoryBuilder()
                .WithUsers(1)
                .Build();
            var existingSummary = sut.GetUserSummaries().First();
            var updatedSummary = new UserSummary(existingSummary.Name, 1);

            // act
            sut.UpdateUserSummary(updatedSummary);

            // assert
            var result = sut.GetUserSummaries();
            result.Count().Should().Be(1);
            result.First().Posts.Should().Be(existingSummary.Posts + updatedSummary.Posts);
        }

        [TestMethod]
        public void GivenThereAreUsers_WhenIRequestTheSummaries_ThenTheyAreSortedInDescendingOrderByPosts()
        {
            // arrange
            int numUsers = 10;
            var sut = new MemoryUsersRepositoryBuilder()
                .WithUsers(numUsers)
                .Build();

            // act
            var result = sut.GetUserSummaries();

            // assert
            result.Count().Should().Be(numUsers);
            result.Should().BeInDescendingOrder(summary => summary.Posts);
        }
    }
}
