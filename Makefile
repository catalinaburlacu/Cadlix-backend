-include .env

# export ConnectionStrings__DefaultConnection := Server=localhost;Database=Cadlix;User Id=$(DB_USER);Password=$(DB_PASSWORD);TrustServerCertificate=True;

start:
	@ docker compose up -d --build

stop:
	@ docker compose down

run:
	@ dotnet run --project ./Cadlix_backend.Api/

migration-delete:
	@ dotnet ef migrations remove --project Cadlix_backend.DataAccess --startup-project Cadlix_backend.Api

migration: migration-delete
	@ dotnet ef migrations add InitialCreate --project Cadlix_backend.DataAccess --startup-project Cadlix_backend.Api

update: migration
	@ dotnet ef database update --project Cadlix_backend.DataAccess --startup-project Cadlix_backend.Api