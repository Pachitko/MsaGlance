FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# RUN apt-get update
# RUN apt-get install -y unzip
# RUN curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l ~/vsdbg

WORKDIR /app

COPY Services/Identity/Identity.Domain/Identity.Domain.csproj Identity.Domain/
COPY Services/Identity/Identity.DAL/Identity.DAL.csproj Identity.DAL/
COPY Services/Identity/Identity.Api/Identity.Api.csproj Identity.Api/

RUN dotnet restore Identity.Api/Identity.Api.csproj

COPY Services/Identity .
RUN dotnet publish Identity.Api/Identity.Api.csproj -c Release -o /app/bin

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/bin ./
ENTRYPOINT ["dotnet", "Identity.Api.dll"]