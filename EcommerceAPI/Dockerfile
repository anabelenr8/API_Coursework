# Use official .NET SDK as a build environment
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy and restore dependencies
COPY . .
RUN dotnet restore

# Copy the certificate
COPY certificate.pfx /app/certificate.pfx

RUN dotnet publish -c Release -o out

# Use runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy built application from the build stage
COPY --from=build /app/out ./

# Copy the certificate into the runtime image
COPY --from=build /app/certificate.pfx /app/certificate.pfx

# Expose ports and start the app
EXPOSE 5001
CMD ["dotnet", "EcommerceAPI.dll"]
