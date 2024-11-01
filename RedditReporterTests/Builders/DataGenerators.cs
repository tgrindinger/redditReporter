using Contracts;

namespace RedditReporterTests.Builders
{
    internal static class DataGenerators
    {
        private static readonly Random _random = new Random();

        public static int RandomInt()
        {
            return _random.Next();
        }

        public static string RandomString()
        {
            return Guid.NewGuid().ToString();
        }

        public static PostSummary RandomPostSummary()
        {
            return new PostSummary(RandomString(), RandomString(), RandomInt());
        }

        public static UserSummary RandomUserSummary()
        {
            return new UserSummary(RandomString(), RandomInt());
        }
    }
}
