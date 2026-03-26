using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Controllers.Base;
using UrlShortener.Application.Dtos;
using UrlShortener.Application.Features.Urls.Commands;
using UrlShortener.Application.Features.Urls.Queries;

namespace UrlShortener.Api.Controllers;

[Route("api/urls")]
public class UrlsController : BaseController
{
    public UrlsController(ISender mediator) : base(mediator) {}
    
    [HttpPost]
    public async Task<IActionResult> CreateUrl([FromBody] CreateUrlDto dto)
    {
        var command = new CreateUrlCommand(dto.OriginalUrl, dto.CustomAlias, dto.ExpiresAt);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    [HttpGet("{shortCode}")]
    public async Task<IActionResult> GetUrl(string shortCode)
    {
        var result = await _mediator.Send(new GetUrlByShortCodeQuery(shortCode));
        return Ok(result);
    }
    
    [HttpPut("{shortCode}")]
    public async Task<IActionResult> UpdateUrl(string shortCode, [FromBody] UpdateUrlDto dto)
    {
        await _mediator.Send(new UpdateUrlCommand(shortCode, dto.OriginalUrl, dto.ExpiresAt));
        return NoContent();
    }
    
    [HttpDelete("{shortCode}")]
    public async Task<IActionResult> DeleteUrl(string shortCode)
    {
        await _mediator.Send(new DeleteUrlCommand(shortCode));
        return NoContent();
    }
}