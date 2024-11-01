using Contracts;
using DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace RedditReporter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController(IUsersConsumer usersConsumer) : ControllerBase
    {
        private readonly IUsersConsumer _usersConsumer = usersConsumer;

        [HttpGet(Name = "GetTopUsers")]
        public IEnumerable<UserSummary> GetTopUsers()
        {
            var userSummaries = _usersConsumer.GetUserSummaries();
            return userSummaries.ToArray();
        }
    }
}
