# Build context is the repository root (see docker-compose.yml `build.context: .`),
# so every COPY/path below is relative to the solution root, not this file's folder.

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore "host/KatKat.HttpApi.Host/KatKat.HttpApi.Host.csproj"
RUN dotnet publish "host/KatKat.HttpApi.Host/KatKat.HttpApi.Host.csproj" \
    --no-restore -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
RUN chown -R $APP_UID /app
USER $APP_UID

ENTRYPOINT ["dotnet", "KatKat.HttpApi.Host.dll"]
