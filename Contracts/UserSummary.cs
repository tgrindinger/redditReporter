namespace Contracts
{
    public class UserSummary
    {
        public string Name { get; private set; }
        public int Posts { get; private set; }

        public UserSummary(string name, int posts)
        {
            Name = name;
            Posts = posts;
        }
    }
}
