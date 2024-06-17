using Contracts;
using DataAccess;
using DataAccessTests.Builders;
using FluentAssertions;

namespace DataAccessTests
{
    [TestClass]
    public class MemoryPostsRepositoryTests
    {
        [TestMethod]
        public void GivenThereAreNoPosts_WhenIReadThePosts_ThenThereAreNoPosts()
        {
            // arrange
            var sut = new MemoryPostsRepository();

            // act
            var result = sut.GetPostSummaries();

            // assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GivenThereAreNoPosts_WhenIAddAPost_ThenICanGetThePostSummary()
        {
            // arrange
            var sut = new MemoryPostsRepository();
            var summary = new PostSummary(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 5);

            // act
            sut.UpdatePostSummary(summary);

            // assert
            var result = sut.GetPostSummaries();
            result.Count().Should().Be(1);
            result.First().Id.Should().Be(summary.Id);
            result.First().Upvotes.Should().Be(summary.Upvotes);
        }

        [TestMethod]
        public void GivenThereIsAPost_WhenIUpdateThePost_TheValueIsUpdated()
        {
            // arrange
            var sut = new MemoryPostsRepositoryBuilder()
                .WithPosts(1)
                .Build();
            var existingSummary = sut.GetPostSummaries().First();
            var summaryUpdate = new PostSummary(existingSummary.Id, existingSummary.Title, existingSummary.Upvotes + 10);

            // act
            sut.UpdatePostSummary(summaryUpdate);

            // assert
            var result = sut.GetPostSummaries();
            result.Count().Should().Be(1);
            result.First().Id.Should().Be(existingSummary.Id);
            result.First().Upvotes.Should().Be(summaryUpdate.Upvotes);
        }

        [TestMethod]
        public void GivenThereAreMultiplePosts_WhenIRequestTheSummaries_TheyAreSortedByUpvotesInDescendingOrder()
        {
            // arrange
            int numSummaries = 10;
            var sut = new MemoryPostsRepositoryBuilder()
                .WithPosts(numSummaries)
                .Build();

            // act
            var result = sut.GetPostSummaries();

            // assert
            result.Count().Should().Be(numSummaries);
            result.Should().BeInDescendingOrder(summary => summary.Upvotes);
        }
    }
}
