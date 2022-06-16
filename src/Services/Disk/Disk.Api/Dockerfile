FROM mcr.microsoft.com/dotnet/sdk:6.0 AS debug

WORKDIR /src

COPY ./certs/idsrv.crt /usr/local/share/ca-certificates/idsrv.crt
RUN update-ca-certificates

COPY Disk.Api.csproj .
RUN dotnet restore
COPY . .
RUN ["dotnet", "build"]

CMD ["dotnet", "run", "--no-build"]

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

COPY Disk.Api.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as prod
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Disk.Api.dll"]