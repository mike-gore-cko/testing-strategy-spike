# Testing Strategy Spike
Provides two different approaches to [sociable unit testing](https://martinfowler.com/articles/2021-test-shapes.html)
- Testing from the [service layer downwards](./test/Api.Domain.Tests)
- Testing from the [API layer downwards](./test/Api.Tests)

Both sets of tests have exactly the same acceptance criteria and assertions. 
It's worth experimenting by changing or adding tests and the implementation
to get a feel for the trade-offs of these approaches.

# Further reading
- https://martinfowler.com/articles/2021-test-shapes.html
- https://martinfowler.com/articles/microservice-testing/
- https://engineering.atspotify.com/2018/01/testing-of-microservices/

# Videos
- https://www.youtube.com/watch?v=EZ05e7EMOLM
- https://www.youtube.com/watch?v=vOO3hulIcsY