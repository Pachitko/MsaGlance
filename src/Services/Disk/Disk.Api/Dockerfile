FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

# RUN ["dotnet", "build"]
# CMD ["dotnet", "run", "--no-build"]

COPY . .
RUN dotnet publish -c Release -o /app/bin --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0

COPY ./certs/idsrv.crt /usr/local/share/ca-certificates/idsrv.crt
RUN update-ca-certificates

WORKDIR /app
COPY --from=build /app/bin ./
ENTRYPOINT ["dotnet", "Disk.Api.dll"]