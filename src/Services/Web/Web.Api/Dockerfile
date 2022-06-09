FROM mcr.microsoft.com/dotnet/sdk:6.0 AS debug

WORKDIR /src

COPY Web.Api.csproj .
RUN dotnet restore
COPY . .

CMD ["dotnet", "run"]

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

COPY Web.Api.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as prod
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Web.Api.dll"]