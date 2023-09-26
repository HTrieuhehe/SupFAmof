	# Start with a base image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the SupFAmof.API project file
COPY ["SupFAmof_Project_API/SupFAmof.API.csproj", "SupFAmof_Project_API/"]

# Copy the BusinessObject project file
COPY ["SupFAmof.Data/SupFAmof.Data.csproj", "SupFAmof.Data/"]

# Copy the DataAccess project file
COPY ["SupFAmof.Serviec/SupFAmof.Service.csproj", "SupFAmof.Serviec/"]

# Copy the Repository project file
COPY ["SupFAmof.Serviec/MailTemplate/VeryficationEmailTemplate.html" ,"/app/MailTemplate/VeryficationEmailTemplate.html"]
# Copy the Client project file



# Restore the project dependencies
RUN dotnet restore "SupFAmof_Project_API/SupFAmof.API.csproj"

# Copy the source code
COPY . .

# Build the application
RUN dotnet build "SupFAmof_Project_API/SupFAmof.API.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "SupFAmof_Project_API/SupFAmof.API.csproj" -c Release -o /app/publish 


# Create a runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime

# Set the working directory inside the container
WORKDIR /app

# Copy the published files from the build image
COPY --from=build /app/publish .

# Expose the port(s) that the application will listen on
EXPOSE 7049

# Set the entry point for the container
ENTRYPOINT ["dotnet", "SupFAmof.API.dll"]
