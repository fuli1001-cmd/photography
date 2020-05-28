#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Src/Services/Post/Post.API/Post.API.csproj", "Src/Services/Post/Post.API/"]
COPY ["Src/ApplicationMessages/ApplicationMessages.csproj", "Src/ApplicationMessages/"]
COPY ["Src/Services/Post/Post.Infrastructure/Post.Infrastructure.csproj", "Src/Services/Post/Post.Infrastructure/"]
COPY ["Src/Services/Post/Post.Domain/Post.Domain.csproj", "Src/Services/Post/Post.Domain/"]
COPY ["Src/Services/Post/Post.API.Query/Post.API.Query.csproj", "Src/Services/Post/Post.API.Query/"]
COPY ["Src/Services/Post/Post.API.Query.EF/Post.API.Query.EF.csproj", "Src/Services/Post/Post.API.Query.EF/"]
RUN dotnet restore "Src/Services/Post/Post.API/Post.API.csproj"
COPY . .
WORKDIR "/src/Src/Services/Post/Post.API"
RUN dotnet build "Post.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Post.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN sed -i "s|DEFAULT@SECLEVEL=2|DEFAULT@SECLEVEL=1|g" /etc/ssl/openssl.cnf
ENTRYPOINT ["dotnet", "Post.API.dll"]