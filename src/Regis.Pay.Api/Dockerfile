FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["src/Regis.Pay.Api/Regis.Pay.Api.csproj", "src/Regis.Pay.Api/"]
COPY ["src/Regis.Pay.Domain/Regis.Pay.Domain.csproj", "src/Regis.Pay.Domain/"]
COPY ["src/Regis.Pay.Common/Regis.Pay.Common.csproj", "src/Regis.Pay.Common/"]
RUN dotnet restore "src/Regis.Pay.Api/Regis.Pay.Api.csproj"
COPY . .
WORKDIR "/src/src/Regis.Pay.Api"
RUN dotnet build "Regis.Pay.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Regis.Pay.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# cert workaround

RUN apk add --no-cache bash
RUN apk add --no-cache curl

COPY ./api.sh ./api.sh

# convert line endings to unix
RUN dos2unix ./api.sh 
RUN chmod +x ./api.sh

ENTRYPOINT ["./api.sh", "azurecosmosemulator", "8081"]