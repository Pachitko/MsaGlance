FROM mcr.microsoft.com/dotnet/sdk:6.0 AS debug

# RUN apt-get update
# RUN apt-get install -y unzip
# RUN curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l ~/vsdbg

WORKDIR /src

COPY Identity.Api.csproj .
RUN dotnet restore
COPY . .
RUN ["dotnet", "build"]

CMD ["dotnet", "run", "--no-build"]

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

COPY Identity.Api.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as prod
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Identity.Api.dll"]