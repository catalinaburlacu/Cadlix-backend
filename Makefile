-include .env

start:
	@ docker compose up -d --build

stop:
	@ docker compose down

run:
	@ dotnet run --project ./Cadlix_backend.Api/

migration:
	@ dotnet ef migrations add InitialCreate --project Cadlix_backend.DataAccess --startup-project Cadlix_backend.Api

update:
	@ dotnet ef database update --project Cadlix_backend.DataAccess --startup-project Cadlix_backend.Api