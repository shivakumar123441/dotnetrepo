namespace InvestTrackerWebApi.HttpApi.Models;
public class ErrorDto
{
    public string ExceptionMessage { get; set; } = default!;
    public Guid ErrorId { get; set; } = default!;
    public string SupportMessage { get; set; } = default!;
}
