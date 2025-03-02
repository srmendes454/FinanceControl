FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /runtime-app
COPY --from=build /app/out .

ENV UrlMongoDB=mongodb://localhost:27017/FinanceControlDev

EXPOSE 8080
ENTRYPOINT ["dotnet", "FinanceControl.dll"]
