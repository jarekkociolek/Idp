FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS BUILD
WORKDIR /app
COPY . .
RUN dotnet publish Idp.Server -o output

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app/output .
ENV ASPNETCORE_URLS http://*:80
ENV ASPNETCORE_ENVIRONMENT docker
ENTRYPOINT dotnet Idp.Server.dll