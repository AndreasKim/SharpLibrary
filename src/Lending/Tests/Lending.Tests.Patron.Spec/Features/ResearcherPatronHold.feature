Feature: Researcher PatronHold

Scenario: Researcher Patron holds more than 5 books
	Given a researcher patron
		And an available book
	When the patron tries to place his 6th hold
	Then the close ended bookhold suceeds

Scenario: Researcher Patron tries to hold a book that is already on hold
	Given a researcher patron
		And a book that is already on hold
	When the patron tries to hold the book
	Then the close ended bookhold fails

Scenario: Researcher Patron has 2 overdue checkouts at library branch
	Given a researcher patron
		And an available book
	When the patron has two overdue checkouts at a library branch
		And the patron tries to hold the book
	Then the close ended bookhold fails

Scenario: Researcher Patron tries to place a restriced book on hold
	Given a researcher patron
		And a restricted book
	When the patron tries to hold the book
	Then the close ended bookhold suceeds

Scenario: Researcher Patron tried an open ended hold
	Given a researcher patron
		And an open ended book
	When the patron tries to hold the book
	Then the close ended bookhold suceeds

Scenario: Researcher Patron succeeds at placing an close ended hold
	Given a researcher patron
		And an available book
	When the patron places a hold on the book
	Then the close ended bookhold suceeds