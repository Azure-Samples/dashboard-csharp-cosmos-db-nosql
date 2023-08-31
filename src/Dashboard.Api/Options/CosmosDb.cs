namespace Dashboard.Api.Options;

public record CosmosDb
{
    public string ConnectionString { get; init; } = String.Empty;

    public string DatabaseName { get; init; } = String.Empty;

    public string ContainerName { get; init; } = String.Empty;
}