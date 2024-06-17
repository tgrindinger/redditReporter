namespace Contracts
{
    public class PostSummary
    {
        public string Id { get; private set; }
        public string Title { get; private set; }
        public int Upvotes { get; private set; }

        public PostSummary(string id, string title, int upvotes)
        {
            Id = id;
            Title = title;
            Upvotes = upvotes;
        }
    }
}