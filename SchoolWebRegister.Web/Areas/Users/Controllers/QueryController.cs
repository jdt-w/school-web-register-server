using Microsoft.AspNetCore.Mvc;
using SchoolWebRegister.DAL.Repositories;
using SchoolWebRegister.Domain.QuerySpecifications;
using SchoolWebRegister.Domain.Specifications;
using SchoolWebRegister.Services.Users;

namespace SchoolWebRegister.Web.Areas.Users.Controllers
{
    [ApiController]
    public class QueryController : Controller
    {
        private readonly IUserService _userRepository;
        public QueryController(IUserService userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("/users/[controller]/search")]
        public async Task<IActionResult> Search()
        {
            if (Request.Query == null || Request.Query.Count == 0) 
                return BadRequest();

            var result = _userRepository.GetUsers().Specify(new UserQuerySpecification(Request.Query));

            return result.Count() > 0 ? Ok(result) : NoContent();
        }
        
        [HttpGet("/users/[controller]/filter")]
        public async Task<IActionResult> Filter()
        {
            IQueryCollection query = HttpContext.Request.Query;

            if (query == null || query.Count == 0) return BadRequest();

            return Ok();
        }
    }
}
