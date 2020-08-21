using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace RESTfulApi_Reddit.Controllers
{
    public class BaseApiController : ControllerBase
    {
        private IMediator _mediator;

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        protected IActionResult FromResult(Result result)
        {
            if (result.IsSuccess)
                return Ok();

            return NotFound(result.Error);
        }
    }
}
