# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /app

# Copy the project file and restore dependencies
COPY ["WebAPP.csproj", "./"]
RUN dotnet restore "./WebAPP.csproj"

# Copy the rest of the application code
COPY . .

# Build the application
RUN dotnet build "WebAPP.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "WebAPP.csproj" -c Release -o /app/publish

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS final
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Expose the ports (optional)
EXPOSE 80
EXPOSE 443

# Entry point to run the application
ENTRYPOINT ["dotnet", "WebAPP.dll"]
