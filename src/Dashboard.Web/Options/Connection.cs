namespace Dashboard.Web.Options;

public record Connection
{
    public string ApiUrl { get; init; } = String.Empty;
}