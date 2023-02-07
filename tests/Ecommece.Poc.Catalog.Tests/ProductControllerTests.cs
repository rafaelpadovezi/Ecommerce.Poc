using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using Ecommerce.Poc.Catalog.Commands;
using Ecommerce.Poc.Catalog.Domain.Models;
using Ecommerce.Poc.Catalog.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Ecommerce.Poc.Catalog.Tests;

public class ProductControllerTests : IAsyncLifetime
{
    private readonly WebApplicationFactory<ApiCommand.Startup> _factory;
    private readonly CatalogDbContext _context;

    public ProductControllerTests()
    {
        _factory = new();

        _context = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<CatalogDbContext>();
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    // Initialize CAP
    public Task InitializeAsync() => _factory.Services.GetRequiredService<IBootstrapper>().BootstrapAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Create_ShouldSaveProductAndEnqueueMessage()
    {
        // Arrange
        var client = _factory.CreateClient();

        var product = new Product("1111", 1)
        {
            Name = "MyProduct",
            Description = "Product for tests"
        };
        var expectedSavedProduct = new
        {
            MaterialCode = "1111",
            Stock = 1,
            Name = "MyProduct",
            Description = "Product for tests"
        };

        // Act
        var response = await client.PostAsJsonAsync("Product", product);

        // Assert - API returned OK
        response.Should().Be200Ok();
        // Assert - Product saved on DB
        _context.Products
            .Single(x => x.MaterialCode == "1111")
            .Should()
            .BeEquivalentTo(expectedSavedProduct);
        // Assert - Product "sent" to the queue
        var messageBody = await GetMessagePersistedByCap<Product>(_context);
        messageBody.Should().BeEquivalentTo(expectedSavedProduct);
    }

    private static async Task<T?> GetMessagePersistedByCap<T>(DbContext context)
    {
        var connection = context.Database.GetDbConnection();
        await connection.OpenAsync();

        using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = "Select top 1 Content FROM [cap].Published Where [Name] = 'product.created'";

        using var reader = await command.ExecuteReaderAsync();
        var table = new DataTable();
        table.Load(reader);
        var content = table.Rows[0]["Content"].ToString();

        var messageBody = JsonDocument.Parse(content).RootElement.GetProperty("Value").Deserialize<T>();
        return messageBody;
    }
}