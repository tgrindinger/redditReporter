using DataAccessTests.Builders;
using FluentAssertions;

namespace DataAccessTests
{
    [TestClass]
    public class ChannelUsersProducerTests
    {
        [TestMethod]
        public async Task GivenThereAreNoUsers_WhenIUpdateUsers_ThenTheChannelHasAUser()
        {
            // arrange
            var builder = new ChannelUsersProducerBuilder();
            var sut = builder.Build();

            // act
            await sut.UpdateUserSummary(DataGenerators.RandomUserSummary());

            // assert
            builder.Channel.Reader.Count.Should().Be(1);
        }

        [TestMethod]
        public async Task GivenThereAreUsers_WhenIUpdateUsers_ThenTheChannelGetsMoreUsers()
        {
            // arrange
            var numUsers = 10;
            var builder = new ChannelUsersProducerBuilder();
            var sut = builder
                .WithUsers(numUsers).Result
                .Build();

            // act
            await sut.UpdateUserSummary(DataGenerators.RandomUserSummary());

            // assert
            builder.Channel.Reader.Count.Should().Be(numUsers + 1);
        }

        [TestMethod]
        public async Task GivenThereAreNoUsers_WhenIUpdateUsers_ThenTheChannelHasTheCorrectUser()
        {
            // arrange
            var summary = DataGenerators.RandomUserSummary();
            var builder = new ChannelUsersProducerBuilder();
            var sut = builder.Build();

            // act
            await sut.UpdateUserSummary(summary);

            // assert
            var channelUser = await builder.Channel.Reader.ReadAsync();
            channelUser.Name.Should().Be(summary.Name);
            channelUser.Posts.Should().Be(summary.Posts);
        }
    }
}
