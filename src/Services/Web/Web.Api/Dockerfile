FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY Services/Web/Web.Api/Web.Api.csproj .
RUN dotnet restore

COPY Services/Web/Web.Api .
RUN dotnet publish -c Release -o /app/bin

FROM mcr.microsoft.com/dotnet/aspnet:6.0

COPY Services/Web/Web.Api/certs/idsrv.crt /usr/local/share/ca-certificates/idsrv.crt
COPY Services/Web/Web.Api/certs/disk.crt /usr/local/share/ca-certificates/disk.crt
RUN update-ca-certificates

WORKDIR /app
COPY --from=build /app/bin ./
ENTRYPOINT ["dotnet", "Web.Api.dll"]