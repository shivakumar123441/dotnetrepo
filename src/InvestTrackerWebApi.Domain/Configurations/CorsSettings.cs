namespace InvestTrackerWebApi.Domain.Configurations;

public class CorsSettings
{
    public string? Policy { get; set; }
    public List<string>? AllowedOrigins { get; set; }
}
