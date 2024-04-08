# Testing Strategy

In this documentation, we’ll delve into the testing strategy for our example project and explore the rationale behind each approach.

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

## Load Tests

//TODO

## End To End Tests

Tests specific scenarios against the full stack including third party providers if possible. These tests should be run only if the previous tests pass to reduce the amount of noise created on third party providers systems unless you have permission to let lose. It is also ok to mock or stub out dependencies if using a dependency is a no go. 

End to end tests are considered to be more flakey and can be difficult to maintain due to testing multiple components interacting with each other. Which is why these tests should be kept to a minimum.