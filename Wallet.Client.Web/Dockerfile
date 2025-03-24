FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
EXPOSE 80
WORKDIR /src
COPY Wallet.Client.Web.csproj .
RUN dotnet restore Wallet.Client.Web.csproj
COPY . .
RUN dotnet build Wallet.Client.Web.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish Wallet.Client.Web.csproj -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf