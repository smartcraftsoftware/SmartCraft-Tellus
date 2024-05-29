FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
EXPOSE 8080
EXPOSE 8081

COPY ["src/SmartCraft.Core.Tellus.Api/SmartCraft.Core.Tellus.Api.csproj", "SmartCraft.Core.Tellus.Api/"]
COPY ["src/SmartCraft.Core.Tellus.Application/SmartCraft.Core.Tellus.Application.csproj", "SmartCraft.Core.Tellus.Application/"]
COPY ["src/SmartCraft.Core.Tellus.Domain/SmartCraft.Core.Tellus.Domain.csproj", "SmartCraft.Core.Tellus.Domain/"]
COPY ["src/SmartCraft.Core.Tellus.Infrastructure/SmartCraft.Core.Tellus.Infrastructure.csproj", "SmartCraft.Core.Tellus.Infrastructure/"]

RUN dotnet restore "SmartCraft.Core.Tellus.Api/SmartCraft.Core.Tellus.Api.csproj"
COPY . ../
WORKDIR /src/SmartCraft.Core.Tellus.Api
RUN dotnet build "SmartCraft.Core.Tellus.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartCraft.Core.Tellus.Api.dll"]