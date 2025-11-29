Feature: PhoneValidate E2E

Background:
    Given the API is running

Scenario: Get Recipient returns unauthorized without token
    When I call GET "/Recipient" without authentication
    Then I should receive status code 401

Scenario: Get Recipient with valid authentication
    When I login with username "test" and password "test"
    Then I should receive a valid JWT token
    When I create a recipient with a unique phone number
    Then I should receive status code 201
    When I call GET "/Recipient" with "phoneNumber" equals to "+5579999810687"
    Then I should receive status code 200
    And the response should contain recipient data

Scenario: Create Recipient successfully with valid phone number
    When I login with username "test" and password "test"
    Then I should receive a valid JWT token
    When I create a recipient with a unique phone number
    Then I should receive status code 201
    And the response should contain the created recipient

Scenario: Create Recipient fails with invalid phone number
    When I login with username "test" and password "test"
    Then I should receive a valid JWT token
    When I create a recipient with phone number "123"
    Then I should receive status code 400  

Scenario: Health check returns healthy status
    When I call GET "/healthystatus" without authentication
    Then I should receive status code 200
    And the response should contain "Healthy"