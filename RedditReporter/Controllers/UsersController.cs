using Contracts;
using DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace RedditReporter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUsersRepository _usersRepository;

        public UsersController(
            ILogger<UsersController> logger,
            IUsersRepository usersRepository)
        {
            _logger = logger;
            _usersRepository = usersRepository;
        }

        [HttpGet(Name = "GetTopUsers")]
        public IEnumerable<UserSummary> GetTopUsers()
        {
            var userSummaries = _usersRepository.GetUserSummaries();
            return userSummaries.ToArray();
        }
    }
}