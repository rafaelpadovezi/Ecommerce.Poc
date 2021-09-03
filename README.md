# Ecommerce.Poc

## TODO

- [x] Messaging between services
- [x] Idempotent messages
- [x] Cap Filter
- [x] Multiple consumers - same assembly
- [ ] Multiple consumers - same message
- [ ] Change Qos
- [ ] Error handling
- [ ] Propagate tracecontext
- [ ] Load test

## Running locally

```bash
# Run dependencies
docker compose up -d db es queue

# Run migrations for Sale
dotnet ef database update --project .\src\Ecommerce.Poc.Sale\Ecommerce.Poc.Sale.csproj --context SaleDbContext

# Run migrations for Catalog
dotnet run --project .\src\Ecommerce.Poc.Catalog\ -- seed
```


## Useful commands
dotnet new sln --name Ecommerce.Poc 

dotnet new webapi --name Ecommerce.Poc.Sale -o src/Ecommerce.Poc.Sale

dotnet new webapi --name Ecommerce.Poc.Catalog -o src/Ecommerce.Poc.Catalog

dotnet new webapi --name Ecommerce.Poc.Search -o src/Ecommerce.Poc.Search

dotnet sln add .\src\Ecommerce.Poc.Search\Ecommerce.Poc.Search.csproj

docker compose up -d db es queue

dotnet ef database update --project .\src\Ecommerce.Poc.Sale\Ecommerce.Poc.Sale.csproj --context SaleDbContext

dotnet ef database update --project .\src\Ecommerce.Poc.Catalog\Ecommerce.Poc.Catalog.csproj --context CatalogDbContext

dotnet ef migrations add InitialCreate --project .\src\Ecommerce.Poc.Catalog\Ecommerce.Poc.Catalog.csproj -o Infrastructure/Migrations

dotnet ef migrations add InitialCreate --project .\src\Ecommerce.Poc.Sale\Ecommerce.Poc.Sale.csproj -o Infrastructure/Migrations

dotnet run --project .\src\Ecommerce.Poc.Catalog\ -- seed