-include .env

run:
	@ ConnectionStrings__DefaultConnection="Server=localhost;Database=Cadlix;User Id=${DB_USER};Password=${DB_PASSWORD};TrustServerCertificate=True;" \
	dotnet run --project ./Cadlix_backend.Api/

migration:
	@ ConnectionStrings__DefaultConnection="Server=localhost;Database=Cadlix;User Id=${DB_USER};Password=${DB_PASSWORD};TrustServerCertificate=True;" \
	dotnet ef migrations add InitialCreate --project Cadlix_backend.DataAccess --startup-project Cadlix_backend.Api

update:
	@ ConnectionStrings__DefaultConnection="Server=localhost;Database=Cadlix;User Id=${DB_USER};Password=${DB_PASSWORD};TrustServerCertificate=True;" \
	dotnet ef database update --project Cadlix_backend.DataAccess --startup-project Cadlix_backend.Api