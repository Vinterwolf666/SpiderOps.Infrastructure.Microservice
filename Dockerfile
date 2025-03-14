#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Infrastructure.Microservice.API/Infrastructure.Microservice.API.csproj", "Infrastructure.Microservice.API/"]
COPY ["Customer.Infrastructure.Microservice/Infrastructure.Microservice.Domain.csproj", "Customer.Infrastructure.Microservice/"]
COPY ["Infrastructure.Microservice.APP/Infrastructure.Microservice.APP.csproj", "Infrastructure.Microservice.APP/"]
COPY ["Infrastructure.Microservice.Infrastructure/Infrastructure.Microservice.Infrastructure.csproj", "Infrastructure.Microservice.Infrastructure/"]
RUN dotnet restore "Infrastructure.Microservice.API/Infrastructure.Microservice.API.csproj"
COPY . .
WORKDIR "/src/Infrastructure.Microservice.API"
RUN dotnet build "Infrastructure.Microservice.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Infrastructure.Microservice.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Infrastructure.Microservice.API.dll"]