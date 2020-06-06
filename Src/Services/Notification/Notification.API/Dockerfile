#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-nanoserver-1809 AS base
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

#FROM mcr.microsoft.com/dotnet/core/sdk:3.1-nanoserver-1809 AS build
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Src/Services/Notification/Notification.API/Notification.API.csproj", "Src/Services/Notification/Notification.API/"]
COPY ["Src/ApplicationMessages/ApplicationMessages.csproj", "Src/ApplicationMessages/"]
COPY ["Src/Services/Notification/Notification.Infrastructure/Notification.Infrastructure.csproj", "Src/Services/Notification/Notification.Infrastructure/"]
COPY ["Src/Services/Notification/Notification.API.Query/Notification.API.Query.csproj", "Src/Services/Notification/Notification.API.Query/"]
COPY ["Src/Services/Notification/Notification.Domain/Notification.Domain.csproj", "Src/Services/Notification/Notification.Domain/"]
RUN dotnet restore "Src/Services/Notification/Notification.API/Notification.API.csproj"
COPY . .
WORKDIR "/src/Src/Services/Notification/Notification.API"
RUN dotnet build "Notification.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Notification.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notification.API.dll"]