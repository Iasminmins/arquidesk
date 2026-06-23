FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore ArquiDesk.sln
RUN dotnet publish src/ArquiDesk.Web/ArquiDesk.Web.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["sh", "-c", "dotnet ArquiDesk.Web.dll --urls http://0.0.0.0:${PORT:-10000}"]
