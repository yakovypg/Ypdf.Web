FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app/src

# Copy certificates for HTTPS
COPY Certificates/localhost.crt /https/localhost.crt
COPY Certificates/localhost.key /https/localhost.key

# Copy configuration files
COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY stylecop.json ./
COPY .ruleset ./
COPY .editorconfig ./

# Copy .csproj files
COPY Services/Ypdf.Web.FilesAPI/Ypdf.Web.FilesAPI.csproj ./Services/Ypdf.Web.FilesAPI/
COPY Domain/Ypdf.Web.Domain/Ypdf.Web.Domain.csproj ./Domain/Ypdf.Web.Domain/

# Restore dependencies
RUN dotnet restore ./Services/Ypdf.Web.FilesAPI/Ypdf.Web.FilesAPI.csproj

# Copy other files
COPY Services/Ypdf.Web.FilesAPI/. ./Services/Ypdf.Web.FilesAPI/
COPY Domain/Ypdf.Web.Domain/. ./Domain/Ypdf.Web.Domain/

# Build project
WORKDIR /app/src/Services/Ypdf.Web.FilesAPI
RUN dotnet build -c Release -o /app/build

# Publish project
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish ./Ypdf.Web.FilesAPI.csproj

# Create final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

# Run project
ENTRYPOINT ["dotnet", "Ypdf.Web.FilesAPI.dll"]
