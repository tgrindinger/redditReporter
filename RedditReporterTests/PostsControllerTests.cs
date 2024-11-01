using FluentAssertions;
using RedditReporterTests.Builders;

namespace RedditReporterTests
{
    [TestClass]
    public class PostsControllerTests
    {
        [TestMethod]
        public void GivenThereAreNoPosts_WhenIRequestThePosts_ThenIGetNoPosts()
        {
            // arrange
            var sut = new PostsControllerBuilder()
                .IsFlushed().Result
                .Build();

            // act
            var result = sut.GetTopPosts();

            // assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GivenThereIsAPost_WhenIRequestThePosts_ThenIGetTheCorrectPost()
        {
            // arrange
            var postSummary = DataGenerators.RandomPostSummary();
            var sut = new PostsControllerBuilder()
                .WithPost(postSummary).Result
                .IsFlushed().Result
                .Build();

            // act
            var result = sut.GetTopPosts();

            // assert
            result.Count().Should().Be(1);
            result.First().Id.Should().Be(postSummary.Id);
            result.First().Title.Should().Be(postSummary.Title);
            result.First().Upvotes.Should().Be(postSummary.Upvotes);
        }

        [TestMethod]
        public void GivenThereArePosts_WhenIRequestThePosts_ThenIGetTheCorrectPosts()
        {
            // arrange
            var numPosts = 100000;
            var sut = new PostsControllerBuilder()
                .WithPosts(numPosts).Result
                .IsFlushed(numPosts, 1000).Result
                .Build();

            // act
            var result = sut.GetTopPosts();

            // assert
            result.Count().Should().Be(numPosts);
        }
    }
}
