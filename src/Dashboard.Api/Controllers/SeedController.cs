using CosmicWorks.Generator;
using CosmicWorks.Models;
using Dashboard.Api.Options;
using Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Dashboard.Api.Controllers;

[ApiController]
[Route("/seed")]
public class SeedController : ControllerBase
{
    private readonly ICosmosDataGenerator<Product> _generator;
    private readonly CosmosDb _options;

    public SeedController(ICosmosDataGenerator<Product> generator, IOptions<CosmosDb> options)
    {
        _generator = generator;
        _options = options.Value;
    }

    [HttpPost]
    [ProducesResponseType(201)]
    public async Task<List<string>> PostAsync([FromBody] Seed configuration)
    {
        List<string> output = new();
        await _generator.GenerateAsync(
            connectionString: _options.ConnectionString,
            databaseName: _options.DatabaseName,
            containerName: _options.ContainerName,
            count: configuration.Count,
            onItemCreate: (item) => output.Add($"{item}")
        );
        return output;
    }
}