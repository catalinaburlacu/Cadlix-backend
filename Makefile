-include .env

.PHONY: docker-up docker-down run migration-down migration-clean migration-create update drop-db create-db fresh seed

export ConnectionStrings__DefaultConnection := Server=localhost;Database=Cadlix;User Id=$(DB_USER);Password=$(DB_PASSWORD);TrustServerCertificate=True;

docker-up:
	@ docker compose up -d --build

docker-down:
docker-down:
	@ docker compose down

run:
	@ dotnet run --project ./Cadlix_backend.Api/

migration-down:
	@ dotnet ef migrations remove --project Cadlix_backend.DataAccess --startup-project Cadlix_backend.Api 2>/dev/null || true

migration-clean:
	@ rm -rf Cadlix_backend.DataAccess/Migrations/
	@ echo "  ✓ Migrations folder cleaned"

migration-create: migration-clean
	@ dotnet ef migrations add InitialCreate --project Cadlix_backend.DataAccess --startup-project Cadlix_backend.Api

update: migration-create
	@ dotnet ef database update --project Cadlix_backend.DataAccess --startup-project Cadlix_backend.Api

drop-db:
	@ dotnet ef database drop --project Cadlix_backend.DataAccess --startup-project Cadlix_backend.Api --force

create-db:
	@ dotnet ef database update --project Cadlix_backend.DataAccess --startup-project Cadlix_backend.Api

fresh: create-db drop-db migration-create update

seed:
	@ dotnet run --project ./Cadlix_backend.SeedData/ -- "$(ConnectionStrings__DefaultConnection)"
