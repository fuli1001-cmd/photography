#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-nanoserver-1809 AS base
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

#FROM mcr.microsoft.com/dotnet/core/sdk:3.1-nanoserver-1809 AS build
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Src/Services/User/User.API/User.API.csproj", "Src/Services/User/User.API/"]
COPY ["Src/ApplicationMessages/ApplicationMessages.csproj", "Src/ApplicationMessages/"]
COPY ["Src/Services/User/User.Infrastructure/User.Infrastructure.csproj", "Src/Services/User/User.Infrastructure/"]
COPY ["Src/Services/User/User.Domain/User.Domain.csproj", "Src/Services/User/User.Domain/"]
COPY ["Src/Services/User/User.API.Query/User.API.Query.csproj", "Src/Services/User/User.API.Query/"]
COPY ["Src/Services/User/User.API.Query.EF/User.API.Query.EF.csproj", "Src/Services/User/User.API.Query.EF/"]
RUN dotnet restore "Src/Services/User/User.API/User.API.csproj"
COPY . .
WORKDIR "/src/Src/Services/User/User.API"
RUN dotnet build "User.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "User.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "User.API.dll"]