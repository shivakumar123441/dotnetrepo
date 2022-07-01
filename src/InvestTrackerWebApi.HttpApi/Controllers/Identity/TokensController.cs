namespace InvestTrackerWebApi.HttpApi.Controllers.Identity;
using InvestTrackerWebApi.Application.Identity.Tokens;
using InvestTrackerWebApi.HttpApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

public sealed class TokensController : VersionNeutralApiController
{
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpPost]
    [AllowAnonymous]
    [OpenApiOperation("Request an access token using credentials.", "")]
    public async Task<TokenResponse> Get(GetTokenQuery request) =>
        await this.Mediator.Send(request);

    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationErrorsDto))]
    [ProducesDefaultResponseType(typeof(ErrorDto))]
    [HttpPost("refresh")]
    [AllowAnonymous]
    [OpenApiOperation("Request an access token using a refresh token.", "")]
    public async Task<TokenResponse> Refresh(GetRefreshTokenQuery request) =>
        await this.Mediator.Send(request);
}
