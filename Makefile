ENV = --env-file .env.dev

restore:
	dotnet restore ./MsaGlance.sln
dropdb:
	docker volume rm idsrv_db

prune:
	docker system prune
publish_docker:

build:
	docker compose -f docker-compose.yml -f docker-compose.override.yml ${ENV} build
build-prod:
	docker compose -f docker-compose.yml -f docker-compose.prod up --build

up:
	docker compose ${ENV} up --build -d ${s}
reup:
	docker compose ${ENV} restart --build $(s)
up-prod:
	docker compose -f docker-compose.yml -f docker-compose.prod ${ENV} up --build ${s}
down:
	docker compose  ${ENV} down
logs:
	docker compose ${ENV} logs -f $(s)
restart: down up

gitlog:
	git log --grapgh --decorate --all --oneline