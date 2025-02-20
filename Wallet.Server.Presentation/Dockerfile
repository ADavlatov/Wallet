FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Wallet.Server.Presentation/Wallet.Server.Presentation.csproj", "Wallet.Server.Presentation/"]
RUN dotnet restore "Wallet.Server.Presentation/Wallet.Server.Presentation.csproj"
COPY . .
WORKDIR "/src/Wallet.Server.Presentation"
RUN dotnet build "Wallet.Server.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Wallet.Server.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wallet.Server.Presentation.dll"]
