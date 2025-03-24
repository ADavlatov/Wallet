FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY Wallet.Server.Presentation/Wallet.Server.Presentation.csproj ./Wallet.Server.Presentation/
RUN dotnet restore Wallet.Server.Presentation/Wallet.Server.Presentation.csproj

COPY Wallet.Server.Application/Wallet.Server.Application.csproj ./Wallet.Server.Application/
RUN dotnet restore Wallet.Server.Application/Wallet.Server.Application.csproj

COPY Wallet.Server.Infrastructure/Wallet.Server.Infrastructure.csproj ./Wallet.Server.Infrastructure/
RUN dotnet restore Wallet.Server.Infrastructure/Wallet.Server.Infrastructure.csproj

COPY Wallet.Server.Domain/Wallet.Server.Domain.csproj ./Wallet.Server.Domain/
RUN dotnet restore ./Wallet.Server.Domain/Wallet.Server.Domain.csproj

COPY Wallet.Server.Presentation/. ./Wallet.Server.Presentation/
COPY Wallet.Server.Application/. ./Wallet.Server.Application/
COPY Wallet.Server.Infrastructure/. ./Wallet.Server.Infrastructure/
COPY Wallet.Server.Domain/. ./Wallet.Server.Domain/

WORKDIR /app/Wallet.Server.Presentation
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build-env /app/out ./
ENTRYPOINT ["dotnet", "Wallet.Server.Presentation.dll"]
EXPOSE 80