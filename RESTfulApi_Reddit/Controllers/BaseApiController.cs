using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;

namespace RESTfulApi_Reddit.Controllers
{
    public class BaseApiController : ControllerBase
    {
        protected IActionResult FromResult(Result result)
        {
            if (result.IsSuccess)
                return Ok();

            return NotFound(result.Error);
        }
    }
}
