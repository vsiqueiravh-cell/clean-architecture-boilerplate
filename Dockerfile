FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY NuGet.Config ./
COPY CleanArchitectureBoilerplate.slnx ./
COPY Directory.Build.props ./
COPY src/CleanArchitecture.Api/CleanArchitecture.Api.csproj src/CleanArchitecture.Api/
COPY src/CleanArchitecture.Application/CleanArchitecture.Application.csproj src/CleanArchitecture.Application/
COPY src/CleanArchitecture.Domain/CleanArchitecture.Domain.csproj src/CleanArchitecture.Domain/
COPY src/CleanArchitecture.Infrastructure/CleanArchitecture.Infrastructure.csproj src/CleanArchitecture.Infrastructure/
COPY src/CleanArchitecture.Shared/CleanArchitecture.Shared.csproj src/CleanArchitecture.Shared/
COPY tests/CleanArchitecture.Tests/CleanArchitecture.Tests.csproj tests/CleanArchitecture.Tests/

RUN dotnet restore CleanArchitectureBoilerplate.slnx

COPY . .
RUN dotnet publish src/CleanArchitecture.Api/CleanArchitecture.Api.csproj \
    --configuration Release \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CleanArchitecture.Api.dll"]
