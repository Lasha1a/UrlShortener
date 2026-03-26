using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.Api.Controllers.Base;

[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    protected readonly ISender _mediator;
    
    public  BaseController(ISender mediator)
    {
        _mediator = mediator;
    }
}