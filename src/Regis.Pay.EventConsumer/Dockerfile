FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["src/Regis.Pay.EventConsumer/Regis.Pay.EventConsumer.csproj", "src/Regis.Pay.EventConsumer/"]
COPY ["src/Regis.Pay.Domain/Regis.Pay.Domain.csproj", "src/Regis.Pay.Domain/"]
COPY ["src/Regis.Pay.Common/Regis.Pay.Common.csproj", "src/Regis.Pay.Common/"]
RUN dotnet restore "src/Regis.Pay.EventConsumer/Regis.Pay.EventConsumer.csproj"
COPY . .
WORKDIR "/src/src/Regis.Pay.EventConsumer"
RUN dotnet build "Regis.Pay.EventConsumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Regis.Pay.EventConsumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# cert workaround

RUN apk add --no-cache bash
RUN apk add --no-cache curl

COPY ./entrypoint.sh ./entrypoint.sh

# convert line endings to unix
RUN dos2unix ./entrypoint.sh 
RUN chmod +x ./entrypoint.sh

ENTRYPOINT ["./entrypoint.sh", "azurecosmosemulator", "8081", "./Regis.Pay.EventConsumer.dll"]