FROM mcr.microsoft.com/dotnet/sdk:6.0 AS debug

# RUN apt-get update
# RUN apt-get install -y unzip
# RUN curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l ~/vsdbg

WORKDIR /src

COPY Disk.Api.csproj .
RUN dotnet restore
COPY . .

CMD ["dotnet", "run"]

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