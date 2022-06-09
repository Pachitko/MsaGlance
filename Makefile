DOCKER_COMPOSE_BUILD = docker compose -f docker-compose.yml build
DOCKER_COMPOSE_BUILD_PROD = docker compose -f docker-compose.prod.yml build

DOCKER_COMPOSE_UP = docker compose up --build -d
DOCKER_COMPOSE_UP_PROD = docker compose -f docker-compose.prod.yml up --build -d

DOCKER_COMPOSE_DOWN = docker compose down
DOCKER_COMPOSE_LOGS = docker compose logs -f
DOCKER_DROP_DB = docker volume rm db

DOTNET_PUBLISH = dotnet publish -c Release src/SoaGlance.sln

IDENTITY_API = Identity.Api
IDENTITY_PROJECT = ./src/Services/Identity/${IDENTITY_API}/${IDENTITY_API}.csproj
DISK_API = Disk.Api
DISK_PROJECT = ./src/Services/Disk/${DISK_API}/${DISK_API}.csproj

pgadmin:
	docker run --rm -d -p 8080:80 --name pgadmin -v pgadmin:/var/lib/pgadmin -e PGADMIN_DEFAULT_EMAIL="a@a.aa" -e PGADMIN_DEFAULT_PASSWORD="b" dpage/pgadmin4:6.9
dropdb:
	$(DOCKER_DROP_DB)

prune:
	docker system prune
publish_docker:
#	docker build ./api/ -t pachitko/minimal_api:1.0
#	docker build ./server/ -t pachitko/proxy:1.0
#	docker push pachitko/minimal_api:1.0
#	docker push pachitko/proxy:1.0

build:
	$(DOCKER_COMPOSE_BUILD)
build-prod:
	$(DOCKER_COMPOSE_BUILD_PROD)

up:
	$(DOCKER_COMPOSE_UP)
up-prod:
	$(DOCKER_COMPOSE_UP_PROD)

down:
	$(DOCKER_COMPOSE_DOWN)
logs:
	$(DOCKER_COMPOSE_LOGS) $(s)
restart: 
	$(DOCKER_COMPOSE_DOWN)
# $(DOCKER_COMPOSE_BUILD)
	$(DOCKER_COMPOSE_UP)