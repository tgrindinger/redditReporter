namespace Services
{
    internal class RedditSettings
    {
        public string TrackedSub { get; private set; }
        public bool SeedTopPosts { get; private set; }
        public bool SeedNewPosts { get; private set; }

        public RedditSettings(string name, bool seedTopPosts, bool seedNewPosts)
        {
            this.TrackedSub = name;
            SeedTopPosts = seedTopPosts;
            SeedNewPosts = seedNewPosts;
        }
    }
}
