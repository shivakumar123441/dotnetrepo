namespace InvestTrackerWebApi.HttpApi.Models;
public class ValidationErrorsDto
{
    public string ExceptionMessage { get; set; } = default!;
    public Guid ErrorId { get; set; } = default!;
    public string SupportMessage { get; set; } = default!;
    public IDictionary<string, string[]> ValidationErrors { get; set; } = default!;
}
