using DataAccessTests.Builders;
using FluentAssertions;

namespace DataAccessTests
{
    [TestClass]
    public class ChannelPostsProducerTests
    {
        [TestMethod]
        public async Task GivenThereAreNoPosts_WhenIUpdatePosts_ThenTheChannelHasAPost()
        {
            // arrange
            var builder = new ChannelPostsProducerBuilder();
            var sut = builder.Build();

            // act
            await sut.UpdatePostSummary(DataGenerators.RandomPostSummary());

            // assert
            builder.Channel.Reader.Count.Should().Be(1);
        }

        [TestMethod]
        public async Task GivenThereArePosts_WhenIUpdatePosts_ThenTheChannelGetsMorePosts()
        {
            // arrange
            var numPosts = 10;
            var builder = new ChannelPostsProducerBuilder();
            var sut = builder
                .WithPosts(numPosts).Result
                .Build();

            // act
            await sut.UpdatePostSummary(DataGenerators.RandomPostSummary());

            // assert
            builder.Channel.Reader.Count.Should().Be(numPosts + 1);
        }

        [TestMethod]
        public async Task GivenThereAreNoPosts_WhenIUpdatePosts_ThenTheChannelHasTheCorrectPost()
        {
            // arrange
            var summary = DataGenerators.RandomPostSummary();
            var builder = new ChannelPostsProducerBuilder();
            var sut = builder.Build();

            // act
            await sut.UpdatePostSummary(summary);

            // assert
            var channelPost = await builder.Channel.Reader.ReadAsync();
            channelPost.Id.Should().Be(summary.Id);
            channelPost.Upvotes.Should().Be(summary.Upvotes);
            channelPost.Title.Should().Be(summary.Title);
        }
    }
}
