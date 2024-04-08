# Introduction

Regis Pay is a example event-driven microservice architecture project built with dotnet, making use of the [Transactional Outbox pattern (with Azure Cosmos DB)](https://learn.microsoft.com/en-us/azure/architecture/databases/guide/transactional-outbox-cosmos) and [Event Sourcing pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/event-sourcing).

# Getting Started

There are two ways to get started and up and running.

## Docker

> [!NOTE]  
> This option offer a all in one solution. No need to install/run CosmosDB or RabbitMQ individually.

### Prerequisites

- [Docker Desktop](https://www.docker.com/get-started/) - preferred docker solution.

### Steps

1. `cd` into the `local` folder and run the [localSetup.ps1](local/localSetup.ps1) from your terminal. This is to setup the cert on docker for HTTPS support.

```
.\localSetup.ps1
```

2. Then run docker compose

```
docker-compose up build
```

This should run all the services as-well all the required dependencies in a pre-configured working state. 

## Visual Studio

### Prerequisites

The following prerequisites are required to build and run the solution. You can either install them individually or via docker:
- [Azure Cosmos DB Emulator](https://learn.microsoft.com/en-us/azure/cosmos-db/how-to-develop-emulator?tabs=windows%2Ccsharp&pivots=api-nosql#install-the-emulator)
- [RabbitMQ](https://www.rabbitmq.com/docs/download)

### Steps

1. Run the services as mentioned in the prerequisites.

2. Run the solution from Visual Studio, which should start the three programs, `Api`, `ChangeFeed` and `EventConsumer`.



## Manually Testing

Once up and running you can test the solution by using the [payment.http](local/payment.http) file to make a API request and observe the logs. As this solution uses CosmosDB and RabbitMQ, you can also inspect these systems to verify integration.

Here's an example gif showcasing the services running in docker and me manually submitting a payment request.

![Manually Testing](./docs/images/manual-test.gif)

Observe how the logs emitted as the payment goes through the services.

# Architecture

This diagram depicts the architecture of this solution.

![Architecture diagram](./docs/images/architecture.drawio.png)

- **API**: Represents the application programming interface that exposes endpoints for creating, updating, and deleting data.
- **Change Feed Processor**: Monitors changes to data (e.g., database records) and publishes events when changes occur.
- **Event Consumer**: Subscribes events and processes them (e.g., updating databases, sending notifications, etc.).

## Technologies

Core infrastructure technologies in this solution are:

- [Azure Cosmos DB](https://azure.microsoft.com/en-gb/products/cosmos-db) - A NoSQL database for storing data. 
- [RabbitMQ](https://www.rabbitmq.com/) - A reliable and mature messaging and streaming broker.

## Nuget Packages

Notable packages used in this solution are:

- [MassTransit](https://masstransit.io/) - A framework that provides a abstraction on top of message transports, ie. in this example RabbitMQ. It can also be used with Azure Service Bus and Amazon SQS.
- [FastEndpoints](https://fast-endpoints.com/) - A developer friendly alternative to Minimal APIs & MVC.