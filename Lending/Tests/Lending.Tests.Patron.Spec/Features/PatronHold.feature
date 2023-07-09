Feature: PatronHold

Scenario: Patron hold more than 5 books
	Given a regular patron
		And an available book
	When the patron tries to place his 6th hold
	Then the close ended bookhold fails

Scenario: Patron tries to hold a book that is already on hold
	Given a regular patron
		And a book that is already on hold
	When the patron tries to hold the book
	Then the close ended bookhold throws an invalid operation exception.

Scenario: Patron has 2 overdue checkouts at library branch
	Given a regular patron
		And an available book
	When the patron has two overdue checkouts at a library branch
		And the patron tries to hold the book
	Then the close ended bookhold fails