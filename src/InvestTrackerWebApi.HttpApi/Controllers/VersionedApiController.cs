namespace InvestTrackerWebApi.HttpApi.Controllers;
using Microsoft.AspNetCore.Mvc;

[Route("api/v{version:apiVersion}/[controller]")]
public class VersionedApiController : BaseApiController
{
}
