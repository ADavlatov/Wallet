FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Wallet.Client.Bot/Wallet.Client.Bot.csproj", "Wallet.Client.Bot/"]
RUN dotnet restore "Wallet.Client.Bot/Wallet.Client.Bot.csproj"
COPY . .
WORKDIR "/src/Wallet.Client.Bot"
RUN dotnet build "./Wallet.Client.Bot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Wallet.Client.Bot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wallet.Client.Bot.dll"]
EXPOSE 80
