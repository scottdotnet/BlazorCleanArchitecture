using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorCleanArchitecture.WebUI.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        private Mediator.Mediator _mediator = null!;

        protected Mediator.Mediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<Mediator.Mediator>();
    }
}
