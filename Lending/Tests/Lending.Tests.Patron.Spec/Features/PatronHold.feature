Feature: PatronHold

@mytag
Scenario: Patron hold more than 5 books
	Given a regular patron
	When the patron tries to place his 6th hold
	Then the close ended bookhold fails