FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY *.sln ./
COPY . .
RUN dotnet restore ei8-Event-Sourcing-Master-Core.sln
WORKDIR /src/main/Port.Adapter/In/Api
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app /property:PublishWithAspNetCoreTargetManifest=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ei8.EventSourcing.Port.Adapter.In.Api.dll"]
