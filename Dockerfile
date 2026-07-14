# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/HomeFinance.Core/HomeFinance.Core.csproj src/HomeFinance.Core/
COPY src/HomeFinance.Data/HomeFinance.Data.csproj src/HomeFinance.Data/
COPY src/HomeFinance.Web/HomeFinance.Web.csproj  src/HomeFinance.Web/
RUN dotnet restore src/HomeFinance.Web/HomeFinance.Web.csproj

COPY src/ src/
RUN dotnet publish src/HomeFinance.Web/HomeFinance.Web.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "HomeFinance.Web.dll"]
