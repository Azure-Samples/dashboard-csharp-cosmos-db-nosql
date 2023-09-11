using CosmicWorks.Data;
using CosmicWorks.Generator;
using CosmicWorks.Generator.DataSource;
using CosmicWorks.Models;
using Dashboard.Api.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<CosmosDb>()
    .Bind(builder.Configuration.GetSection(nameof(CosmosDb)));

builder.Services.AddSingleton<CosmosClient>((IServiceProvider serviceProvider) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<CosmosDb>>();
    CosmosSerializationOptions serializerOptions = new()
    {
        IgnoreNullValues = true,
        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
    };

    var clientBuilder = new CosmosClientBuilder(options.Value.ConnectionString);

    return clientBuilder
        .WithSerializerOptions(serializerOptions)
        .WithBulkExecution(true)
        .Build();
});

builder.Services.AddSingleton<ICosmosContext, CosmosContext>();
builder.Services.AddTransient<IDataSource<Product>, ProductsDataSource>();
builder.Services.AddTransient<ICosmosDataGenerator<Product>, ProductsCosmosDataGenerator>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

var app = builder.Build();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("./swagger/v1/swagger.json", "v1");
    o.RoutePrefix = String.Empty;
});

await app.RunAsync();