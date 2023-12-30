#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Authentication.Api/Authentication.Api.csproj", "Authentication.Api/"]
RUN dotnet restore "Authentication.Api/Authentication.Api.csproj"
COPY . .
WORKDIR "/src/Authentication.Api"
RUN dotnet build "Authentication.Api.csproj" -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS migration
WORKDIR /src
COPY . .
RUN dotnet restore "Authentication.Migration/Authentication.Migration.csproj"
COPY . .
WORKDIR "/src/Authentication.Migration"
RUN dotnet build "Authentication.Migration.csproj" -c Release -o /app/migration

FROM build AS publish
RUN dotnet publish "Authentication.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
FROM base AS final
WORKDIR /migration
COPY --from=migration /app/migration .

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Authentication.Api.dll"]