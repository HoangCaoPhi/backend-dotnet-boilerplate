# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Boilerplate.slnx Directory.Build.props Directory.Packages.props global.json ./
COPY src/Boilerplate.Api/Boilerplate.Api.csproj src/Boilerplate.Api/
COPY src/Boilerplate.Application/Boilerplate.Application.csproj src/Boilerplate.Application/
COPY src/Boilerplate.Domain/Boilerplate.Domain.csproj src/Boilerplate.Domain/
COPY src/Boilerplate.Infrastructure/Boilerplate.Infrastructure.csproj src/Boilerplate.Infrastructure/
COPY src/Boilerplate.SharedKernel/Boilerplate.SharedKernel.csproj src/Boilerplate.SharedKernel/
RUN dotnet restore Boilerplate.slnx

COPY src/ src/
RUN dotnet publish src/Boilerplate.Api/Boilerplate.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Boilerplate.Api.dll"]
