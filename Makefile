DOCKER_COMPOSE_BUILD = docker compose -f docker-compose.yml build
DOCKER_COMPOSE_UP = docker compose up -d --build
DOCKER_COMPOSE_DOWN = docker compose down
DOCKER_COMPOSE_LOGS = docker compose logs -f
DOCKER_DROP_DB = docker volume rm db

DOTNET_PUBLISH = dotnet publish -c Release src/WebWorld.sln

IDENTITY = ./src/Services/Identity/Identity.Api/Identity.Api.csproj
DISK = ./src/Services/Disk/Disk.Api/Disk.Api.csproj

pgadmin:
	docker run --rm -d -p 8080:80 --name pgadmin -v pgadmin:/var/lib/pgadmin -e PGADMIN_DEFAULT_EMAIL="a@a.aa" -e PGADMIN_DEFAULT_PASSWORD="b" dpage/pgadmin4:6.9
prune:
	docker system prune
dropdb:
	$(DOCKER_DROP_DB)
run:
	gnome-terminal -- dotnet run --project ${IDENTITY}
	gnome-terminal -- dotnet run --project ${DISK}
publish_dotnet:
	$(DOTNET_PUBLISH)
publish_docker:
	docker build ./api/ -t pachitko/minimal_api:1.0
	docker build ./server/ -t pachitko/proxy:1.0
	docker push pachitko/minimal_api:1.0
	docker push pachitko/proxy:1.0
build:
	$(DOCKER_COMPOSE_BUILD)
up:
	$(DOCKER_COMPOSE_UP)
down:
	$(DOCKER_COMPOSE_DOWN)
logs:
	$(DOCKER_COMPOSE_LOGS) $(s)
reset: 
	$(DOCKER_COMPOSE_DOWN)
	$(DOTNET_PUBLISH)
# $(DOCKER_COMPOSE_BUILD)
	$(DOCKER_COMPOSE_UP)