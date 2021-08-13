#!/usr/bin/env bash

echo "## init catalog db"

dotnet run --project ./src/Ecommerce.Poc.Catalog/ -- seed

echo "## init sale db"

dotnet ef database update --project ./src/Ecommerce.Poc.Sale/ --context SaleDbContext
