FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS runtime
WORKDIR /app
COPY . .
ENTRYPOINT ["dotnet", "Api.dll"]