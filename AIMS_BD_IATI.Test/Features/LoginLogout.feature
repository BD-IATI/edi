Feature: Log in / Log out

@web
Scenario: Login/Logout
	Given open a browser 
	And goto http://localhost/IATIImportSite 
	And input UserName = "admin", password = "123456"
	And click login button
	Then the DP selection page should appear

Scenario: Logout
	Given loggen in browser window
	When click on user button
	And click on logout button
	Then the login page should appear