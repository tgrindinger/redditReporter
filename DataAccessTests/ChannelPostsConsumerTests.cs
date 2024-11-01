using DataAccessTests.Builders;
using FluentAssertions;

namespace DataAccessTests
{
    [TestClass]
    public class ChannelPostsConsumerTests
    {
        [TestMethod]
        public void GivenThereAreNoPosts_WhenIGetPosts_ThenIGetNoPosts()
        {
            // arrange
            var sut = new ChannelPostsConsumerBuilder()
                .Build().Result;

            // act
            var result = sut.GetPostSummaries();

            // assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GivenThereArePosts_WhenIGetPosts_ThenIGetThePosts()
        {
            // arrange
            var numPosts = 10;
            var sut = new ChannelPostsConsumerBuilder()
                .WithPosts(numPosts).Result
                .Build().Result;

            // act
            var result = sut.GetPostSummaries();

            // assert
            result.Count().Should().Be(numPosts);
        }

        [TestMethod]
        public void GivenThereIsAPost_WhenIGetPosts_ThenIGetTheCorrectPost()
        {
            // arrange
            var post = DataGenerators.RandomPostSummary();
            var sut = new ChannelPostsConsumerBuilder()
                .WithPost(post).Result
                .Build().Result;

            // act
            var result = sut.GetPostSummaries();

            // assert
            result.Should().NotBeEmpty();
            var newSummary = result.First();
            newSummary.Id.Should().Be(post.Id);
            newSummary.Title.Should().Be(post.Title);
            newSummary.Upvotes.Should().Be(post.Upvotes);
        }

        [TestMethod]
        public void GivenThereArePostsAndTheConsumerIsCancelled_WhenIGetPosts_ThenIGetNoPosts()
        {
            // arrange
            var sut = new ChannelPostsConsumerBuilder()
                .IsCancelled()
                .WithPosts(1).Result
                .Build().Result;

            // act
            var result = sut.GetPostSummaries();

            // assert
            result.Should().BeEmpty();
        }
    }
}
