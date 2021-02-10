#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Idp.Server/Idp.Server.csproj", "Idp.Server/"]
RUN dotnet restore "Idp.Server/Idp.Server.csproj"
COPY . .
WORKDIR "/src/Idp.Server"
RUN dotnet build "Idp.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Idp.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT docker
ENTRYPOINT ["dotnet", "Idp.Server.dll"]