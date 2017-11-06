Feature: SpecFlowFeature1
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

@mytag
Scenario: Login/Logout
	Given open a browser 
	And goto http://localhost/IATIImportSite 
	And input UserName = "admin", password = "123456"
	And click login button
	Then the DP selection page should appear

Scenario: Logout
	Given loggen in browser window
	And click login button
	Then the DP selection page should appear
