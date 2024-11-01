FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
EXPOSE 8080
EXPOSE 8081

COPY . ../

RUN dotnet restore "SmartCraft.Core.Tellus.Api/SmartCraft.Core.Tellus.Api.csproj"
WORKDIR /src/SmartCraft.Core.Tellus.Api
RUN dotnet build "SmartCraft.Core.Tellus.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartCraft.Core.Tellus.Api.dll"]