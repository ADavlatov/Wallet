FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Wallet.Quartz.Presentation/Wallet.Quartz.Presentation.csproj", "Wallet.Quartz.Presentation/"]
COPY ["Wallet.Quartz.Application/Wallet.Quartz.Application.csproj", "Wallet.Quartz.Application/"]
COPY ["Wallet.Client.Web/Wallet.Client.Web.csproj", "Wallet.Client.Web/"]
COPY ["Wallet.Quartz.Domain/Wallet.Quartz.Domain.csproj", "Wallet.Quartz.Domain/"]
COPY ["Wallet.Quartz.Infrastructure/Wallet.Quartz.Infrastructure.csproj", "Wallet.Quartz.Infrastructure/"]
RUN dotnet restore "Wallet.Quartz.Presentation/Wallet.Quartz.Presentation.csproj"
COPY . .
WORKDIR "/src/Wallet.Quartz.Presentation"
RUN dotnet build "./Wallet.Quartz.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Wallet.Quartz.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wallet.Quartz.Presentation.dll"]
EXPOSE 80
