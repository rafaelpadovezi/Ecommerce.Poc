# Ecommerce.Poc

A microservice example application using asynchronous communication.

## TODO

- [x] Messaging between services
- [x] Idempotent messages (sql server)
- [x] Cap Filter
- [x] Multiple consumers - same assembly
- [x] Multiple consumers - same message
- [x] Idempotent messages (mongodb)
- [x] Open Telemetry
- [ ] Error handling
- [ ] ~~Change Qos~~ (at least for now)
- [ ] Load test

## Preparing the enviroment

```bash
# Run dependencies
docker compose up -d mongoclustersetup db es queue otel-collector

# Run migrations for Sale
dotnet ef database update --project ./src/Ecommerce.Poc.Sale/ --context SaleDbContext

# Run migrations for Catalog
dotnet run --project ./src/Ecommerce.Poc.Catalog/ -- seed
```

## Run all apps

```sh
dotnet run --project ./src/Ecommerce.Poc.Sale

dotnet run --project ./src/Ecommerce.Poc.Catalog -- api

dotnet run --project ./src/Ecommerce.Poc.Catalog -- order-canceled-consumer

dotnet run --project ./src/Ecommerce.Poc.Catalog -- order-created-consumer

dotnet run --project ./src/Ecommerce.Poc.Search -- api

dotnet run --project ./src/Ecommerce.Poc.Search -- order-created-consumer

dotnet run --project ./src/Ecommerce.Poc.Search -- product-created-consumer
```

or

```sh
dotnet run --project ./src/Ecommerce.Poc.Sale
# Start APIs and consumers on the same process
dotnet run --project ./src/Ecommerce.Poc.Catalog -- debug
# Start APIs and consumers on the same process
dotnet run --project ./src/Ecommerce.Poc.Search -- debug
```

## Useful commands
dotnet new sln --name Ecommerce.Poc

dotnet new webapi --name Ecommerce.Poc.Sale -o src/Ecommerce.Poc.Sale

dotnet new webapi --name Ecommerce.Poc.Catalog -o src/Ecommerce.Poc.Catalog

dotnet new webapi --name Ecommerce.Poc.Search -o src/Ecommerce.Poc.Search

dotnet sln add .\src\Ecommerce.Poc.Search\Ecommerce.Poc.Search.csproj

docker compose up -d db es queue

dotnet ef database update --project .\src\Ecommerce.Poc.Sale\ --context SaleDbContext

dotnet ef database update --project .\src\Ecommerce.Poc.Catalog\ --context CatalogDbContext

dotnet ef migrations add InitialCreate --project .\src\Ecommerce.Poc.Catalog\ -o Infrastructure/Migrations

dotnet ef migrations add InitialCreate --project .\src\Ecommerce.Poc.Sale\ -o Infrastructure/Migrations

dotnet run --project .\src\Ecommerce.Poc.Catalog\ -- seed

# Useful links

- https://github.com/dotnetcore/CAP/pull/976
- https://github.com/dotnetcore/CAP/issues/932
- https://github.com/dotnetcore/CAP/issues/800
- https://github.com/dotnetcore/CAP/issues/638
- https://www.elastic.co/guide/en/elasticsearch/client/net-api/master/lifetimes.html
- https://github.com/dotnetcore/CAP/issues/347 - CAP message Id
- https://cap.dotnetcore.xyz/user-guide/en/getting-started/quick-start/
- https://github.com/dotnetcore/CAP/issues/860#issuecomment-838020712
- https://github.com/dotnetcore/CAP/issues/1118
- https://github.com/luizhlelis/cap-playground