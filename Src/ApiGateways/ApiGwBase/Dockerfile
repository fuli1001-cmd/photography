#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Src/ApiGateways/ApiGwBase/ApiGwBase.csproj", "Src/ApiGateways/ApiGwBase/"]
RUN dotnet restore "Src/ApiGateways/ApiGwBase/ApiGwBase.csproj"
COPY . .
WORKDIR "/src/Src/ApiGateways/ApiGwBase"
RUN dotnet build "ApiGwBase.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiGwBase.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#RUN sed -i "s|DEFAULT@SECLEVEL=2|DEFAULT@SECLEVEL=1|g" /etc/ssl/openssl.cnf
ENTRYPOINT ["dotnet", "ApiGwBase.dll"]