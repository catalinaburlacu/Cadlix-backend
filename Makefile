-include .env

.PHONY: docker-up docker-down run migration-down migration-up update

export ConnectionStrings__DefaultConnection := Server=localhost;Database=Cadlix;User Id=$(DB_USER);Password=$(DB_PASSWORD);TrustServerCertificate=True;

docker-up:
	@ docker compose up -d --build

docker-down:
	@ docker compose down

run:
	@ dotnet run --project ./Cadlix_backend.Api/

migration-down:
	@ dotnet ef migrations remove --project Cadlix_backend.DataAccess --startup-project Cadlix_backend.Api

migration-up: migration-down
	@ dotnet ef migrations add InitialCreate --project Cadlix_backend.DataAccess --startup-project Cadlix_backend.Api

update: migration-up
	@ dotnet ef database update --project Cadlix_backend.DataAccess --startup-project Cadlix_backend.Api
