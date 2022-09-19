ENV_DEV = --env-file .env.dev

IDENTITY_PROJECT = ./src/Services/Identity/Identity.Api/Identity.Api.csproj
DISK_PROJECT = ./src/Services/Disk/Disk.Api/Disk.Api.csproj
TEMPODISK_PROJECT = ./src/Services/TempoDisk/TempoDisk.Api/TempoDisk.Api.csproj

restore:
	dotnet restore ./MsaGlance.sln
pgadmin:
	docker run --rm -d -p 8080:80 --name pgadmin -v pgadmin:/var/lib/pgadmin -e PGADMIN_DEFAULT_EMAIL="a@a.aa" -e PGADMIN_DEFAULT_PASSWORD="b" dpage/pgadmin4:6.9
dropdb:
	docker volume rm idsrv_db

prune:
	docker system prune
publish_docker:

build:
	docker compose -f docker-compose.yml ${ENV_DEV} build
build-prod:
# docker compose -f docker-compose.yml -f docker-compose.prod build
	docker compose -f docker-compose.yml -f docker-compose.prod up --build

up:
	docker compose ${ENV_DEV} up --build -d ${s}
reup:
	docker compose ${ENV_DEV} restart --build $(s)
up-prod:
	docker compose -f docker-compose.yml -f docker-compose.prod ${ENV_DEV} up --build ${s}
down:
	docker compose  ${ENV_DEV} down
logs:
	docker compose ${ENV_DEV} logs -f $(s)
restart: down up