using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Controllers.Base;
using UrlShortener.Application.Features.Urls.Commands;
using UrlShortener.Application.Features.Urls.Queries;

namespace UrlShortener.Api.Controllers;

[Route("/")]
public class RedirectController : BaseController
{
    public RedirectController(ISender mediator) : base(mediator){}

    [HttpGet("{shortCode}")]
    public async Task<IActionResult> RedirectToUrl(string shortCode)
    {
        var originalUrl = await _mediator.Send(new RedirectQuery(shortCode));

        var userAgent = Request.Headers["User-Agent"].ToString();
        var ipAddres = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        await _mediator.Send(new RecordClickCommand(shortCode,userAgent,ipAddres));

        return Redirect(originalUrl);
    }
}