FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Wallet.Server.Infrastructure/Wallet.Server.Infrastructure.csproj", "Wallet.Server.Infrastructure/"]
RUN dotnet restore "Wallet.Server.Infrastructure/Wallet.Server.Infrastructure.csproj"
COPY . .
WORKDIR "/src/Wallet.Server.Infrastructure"
RUN dotnet build "Wallet.Server.Infrastructure.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Wallet.Server.Infrastructure.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wallet.Server.Infrastructure.dll"]
