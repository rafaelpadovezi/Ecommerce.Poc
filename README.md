# Setup

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