Feature: Policy markers from IATI

  Scenario: User can see policy markers in the IATI import interface
    Given User uses the IATI import module to import a project for DP=`Netherlands (Netherlands)`
    And User proceeds to the `5. Set import preferences` step
    Then the page includes the list of policy markers that have a significance code that is not `0`.

  Scenario: User can import a policy marker
    Given User imports a project from IATI data
    And the project contains at least one policy marker with a significance code that is not `0`
    Then on the `Sector Contribution` tab the `Policy Marker` collapsible contains the same policy markers