# Microservice Architecture Glance

## Services:

- <b>Disk</b> - disk service with an OIDC and file CRUD operations
- <b>Identity</b> - identity server for OIDC support between the services
- <b>Telegram Bot</b> - telegram bot with FSM (Finite State Machine based on ASP.NET Core routing) for interaction with other services via Telegram App
- <b>Web</b> - web API service for interaction with other services via REST API
- <b>TempoDisk (Go lang)</b> - web API for sharing temporary files by unqiue code without user authorization
