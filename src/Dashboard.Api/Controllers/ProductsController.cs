using CosmicWorks.Models;
using Dashboard.Api.Options;
using Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Dashboard.Api.Controllers;

[ApiController]
[Route("/products")]
public class ProductsController : ControllerBase
{
    private readonly CosmosClient _client;
    private readonly CosmosDb _options;

    public ProductsController(CosmosClient client, IOptions<CosmosDb> options)
    {
        _client = client;
        _options = options.Value;
    }

    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<Payload<Product>> GetAsync(string? tag)
    {
        Container container;
        try
        {
            container = _client.GetDatabase(_options.DatabaseName).GetContainer(_options.ContainerName);
            await container.ReadContainerAsync();
        }
        catch (CosmosException)
        {
            return new Payload<Product>(0, Enumerable.Empty<Product>());
        }

        QueryDefinition query;
        if (tag is not null)
        {
            query = new QueryDefinition(
                query: """
            SELECT
                p.id,
                p.name,
                p.description,
                p.category,
                p.sku,
                p.tags,
                p.cost,
                p.price,
                p.type
            FROM
                products p
            JOIN 
                (SELECT VALUE t FROM t IN p.tags WHERE STRINGEQUALS(t, @tag, true)) tag
            WHERE
                STRINGEQUALS(p.type, @type, true)
            ORDER BY
                p.category.name ASC,
                p.category.subCategory.name ASC
            """
            )
                .WithParameter("@type", nameof(Product))
                .WithParameter("@tag", tag);
        }
        else
        {
            query = new QueryDefinition(
                query: """
                SELECT
                    *
                FROM
                    products p
                WHERE
                    STRINGEQUALS(p.type, @type, true)
                ORDER BY
                    p.category.name ASC,
                    p.category.subCategory.name ASC
            """
            )
                .WithParameter("@type", nameof(Product));
        }
        var iterator = container.GetItemQueryIterator<Product>(
            queryDefinition: query
        );

        List<Product> results = new();
        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync();
            results.AddRange(page);
        }

        return new Payload<Product>(results.Count, results);
    }
}