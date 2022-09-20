FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY Services/Disk/Disk.Api/Disk.Api.csproj ./
RUN dotnet restore

# RUN ["dotnet", "build"]
# CMD ["dotnet", "run", "--no-build"]

COPY Services/Disk/Disk.Api .
RUN dotnet publish -c Release -o /app/bin

FROM mcr.microsoft.com/dotnet/aspnet:6.0

COPY Services/Disk/Disk.Api/certs/idsrv.crt /usr/local/share/ca-certificates/idsrv.crt
RUN update-ca-certificates

WORKDIR /app
COPY --from=build /app/bin ./
ENTRYPOINT ["dotnet", "Disk.Api.dll"]