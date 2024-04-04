# Introduction

Regis Pay is a example microservice project built with dotnet. 

# Getting Started

The following prerequisites are required to build and run the solution:
- [Install Azure Cosmos DB Emulator](https://learn.microsoft.com/en-us/azure/cosmos-db/how-to-develop-emulator?tabs=windows%2Ccsharp&pivots=api-nosql#install-the-emulator)
- [Docker Desktop](https://docs.docker.com/desktop/) - For RabbitMQ.

Run the following to get RabbitMQ running:

```
$ docker run -p 15672:15672 -p 5672:5672 masstransit/rabbitmq
```

Run the solution from Visual Studio, which should start the three programs, Api, ChangeFeed and EventConsumer.

Once up and running you can test the solution by using the [payment.http](local/payment.http) file to make a API request.

# Architecture

## Patterns: 
- [Event Sourcing pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/event-sourcing)
- [Transactional Outbox pattern (with Azure Cosmos DB)](https://learn.microsoft.com/en-us/azure/architecture/databases/guide/transactional-outbox-cosmos)

## Technologies: 

- [Azure Cosmos DB](https://azure.microsoft.com/en-gb/products/cosmos-db) - A NoSQL database for storing data. 
- [RabbitMQ](https://www.rabbitmq.com/) - A reliable and mature messaging and streaming broker.

## Nuget Packages: 

- [MassTransit](https://masstransit.io/) - A framework that provides a abstraction on top of message transports, ie. in this example RabbitMQ. It can also be used with Azure Service Bus and Amazon SQS.
- [FastEndpoints](https://fast-endpoints.com/)  


# Testing Strategy

This section will go over the testing strategy for this example project and the reasoning behind the each approach.

## Unit Tests

Testing each unit at a granular level which subscribes more to the solitary unit test definition as opposed to a sociable unit test.

Ideally there should be few of these type of tests as possible as component tests should offer the same coverage with fewer tests to maintain. These are useful when it's hard to write test coverage for a unit in a component test.

## Component Tests

Component tests could be described as a sociable unit test. I won't focus on naming but instead the characteristics in this example. Tests are written in a given-when-then format.

Ideally giving greater code coverage with fewer tests to maintain. Tests as much of a unit/component as possible until it's difficult to do so and falling back to mocking or stubbing.

A few examples:
- Testing a endpoint up until the point where it hits a database. 
- Testing a handler that does a HTTP call with the HTTP call mocked/stubbed out.
- Testing a handler that publishes a message with the message broker mocked/stubbed out.

## Integration Tests

Focusing on ensuring a component has integrated with it's infrastructure dependencies such as databases or messaging systems.

Should not be confused with testing external or third party dependencies. Third party providers can be mocked/stubbed out with an out of process mock.

## Performance Tests



## End To End Tests

Testing specific scenarios against the full stack including third party providers. These tests should be run only if the previous tests pass to reduce the amount of noise created on third party providers systems unless you have communication indicating otherwise.

# External References

- https://github.com/amolenk/CosmosEventSourcing/ - Example used for Event Store.