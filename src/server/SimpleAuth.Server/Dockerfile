#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SimpleAuth.Server/SimpleAuth.Server.csproj", "SimpleAuth.Server/"]
COPY ["SimpleAuth.Application/SimpleAuth.Application.csproj", "SimpleAuth.Application/"]
COPY ["SimpleAuth.Domain/SimpleAuth.Domain.csproj", "SimpleAuth.Domain/"]
COPY ["SimpleAuth.Infrastructure/SimpleAuth.Infrastructure.csproj", "SimpleAuth.Infrastructure/"]
RUN dotnet restore "SimpleAuth.Server/SimpleAuth.Server.csproj"
COPY . .
WORKDIR "/src/SimpleAuth.Server"
RUN dotnet build "SimpleAuth.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleAuth.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleAuth.Server.dll"]