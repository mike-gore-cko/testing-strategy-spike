FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

ARG BUILDCONFIG=Release
ARG VERSION=1.0.0

COPY ./src ./src

RUN dotnet restore "./src/Api/Api.csproj"

WORKDIR "./src/Api/"
RUN dotnet publish -c ${BUILDCONFIG} -o out --self-contained=true -r alpine-x64 /p:Version=${VERSION}

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/src/Api/out ./ 
ENTRYPOINT ["dotnet", "Api.dll"]