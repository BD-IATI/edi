Feature: Results

  Scenario: User can see results in the IATI import interface
    Given User uses the IATI import module to import a project
    And User proceeds to the `5. Set import preferences` step
    Then the page includes the number of `/result/indicator` (e.g. "5 results")

  Scenario: User can import results data
    Given User imports a project from IATI data
    And the project contains at least one `result/indicator`
    Then on the `Results` tab the table contains at least one indicator.