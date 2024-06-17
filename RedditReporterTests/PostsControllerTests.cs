using FluentAssertions;
using Microsoft.Extensions.Logging;
using RedditReporter.Controllers;
using DataAccess;
using Contracts;

namespace RedditReporterTests
{
    [TestClass]
    public class PostsControllerTests
    {
        [TestMethod]
        public void GivenThereAreNoPosts_WhenIRequestThePosts_TheResultIsEmpty()
        {
            // arrange
            var logger = new LoggerFactory()
                .CreateLogger<PostsController>();
            var postsRepository = new MemoryPostsRepository();
            var sut = new PostsController(logger, postsRepository);

            // act
            var result = sut.GetTopPosts();

            // assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GivenThereArePosts_WhenIRequestThePosts_ThenIGetAListOfPostSummaries()
        {
            // arrange
            var logger = new LoggerFactory()
                .CreateLogger<PostsController>();
            var postsRepository = new MemoryPostsRepository();
            var summary = new PostSummary(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 5);
            postsRepository.UpdatePostSummary(summary);
            var sut = new PostsController(logger, postsRepository);

            // act
            var result = sut.GetTopPosts();

            // assert
            result.Count().Should().Be(1);
            result.First().Id.Should().Be(summary.Id);
            result.First().Upvotes.Should().Be(summary.Upvotes);
        }
    }
}
