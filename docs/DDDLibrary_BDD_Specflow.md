# Design Level Implementation
Since the domain and bounded context are now known, we can shift our attention to the implementation of features identified through the [design level interactions](https://github.com/ddd-by-examples/library/blob/master/docs/design-level.md). The idea is to identitfy relevant features from there and then transfer those to a human readable format close to text, which can then be used to write acceptance tests against. This format in our case is gonna be Gherkin.

## What is Gherkin?
Gherkin is a simple, human-readable language used for writing executable specifications in Behavior-Driven Development (BDD). It serves as a structured format for describing the behavior and expected outcomes of a software system. Gherkin acts as a bridge between business stakeholders, developers, and testers, facilitating effective collaboration and communication.

Gherkin scenarios are structured using the "Given-When-Then" format, also known as the Gherkin keywords. Each scenario typically starts with one or more Given steps, followed by a When step, and concludes with one or more Then steps.

Given: The Given step defines the initial state or preconditions for the scenario. It describes the context in which the scenario takes place.

When: The When step specifies the action or event that is being performed by the user or system.

Then: The Then step describes the expected outcome or behavior of the system after the action specified in the When step has occurred. It defines the expected result or assertion.

## What is BDD?
BDD focuses on defining and automating executable specifications, which are written in a common language easily understandable by all stakeholders involved. It emphasizes collaboration between developers, testers, and business stakeholders. 

We will use BDD to write the acceptance tests according to our features **first** and then implement the functionality.

## Specflow