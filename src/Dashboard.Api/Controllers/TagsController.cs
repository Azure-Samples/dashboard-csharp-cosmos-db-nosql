using Dashboard.Api.Options;
using Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Dashboard.Api.Controllers;

[ApiController]
[Route("/tags")]
public class TagsController
{
    private readonly CosmosClient _client;
    private readonly CosmosDb _options;

    public TagsController(CosmosClient client, IOptions<CosmosDb> options)
    {
        _client = client;
        _options = options.Value;
    }

    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<Payload<string>> GetAsync()
    {
        Container container;
        try
        {
            container = _client.GetDatabase(_options.DatabaseName).GetContainer(_options.ContainerName);
            await container.ReadContainerAsync();
        }
        catch (CosmosException)
        {
            return new Payload<string>(0, Enumerable.Empty<string>());
        }

        QueryDefinition query = new(
            query: """
                SELECT DISTINCT VALUE
                    t
                FROM
                    products p
                JOIN
                    t in p.tags
            """
        );

        var iterator = container.GetItemQueryIterator<string>(
            queryDefinition: query
        );

        List<string> results = new();
        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync();
            results.AddRange(page);
        }

        return new Payload<string>(results.Count, results.OrderBy(t => t));
    }
}
