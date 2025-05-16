# User App Integration Demo (.NET 8)

This project demonstrates how to build a .NET 8 component (class library) that interacts with an external API (https://reqres.in). It includes:
- A reusable class library to fetch user data.
- A console application to run and test the integration.
- A unit test project using xUnit and Moq.

## Projects

| Project | Description |
|--------|-------------|
| **UserWebApp.Integration** | Class library containing the API client, service layer, models, and configuration. |
| **UserWebApp.Console** | Console application to demonstrate usage of the class library. |
| **UserWebApp.Test** | Unit test project for the service layer using xUnit and Moq. |

## Features

- Fetch single or all users from the ReqRes API  
- Handle pagination when retrieving all users internally and return all data  
- In-memory caching for all users  
- Configurable base IOptions
- Unit tested with mocked HttpClient

## Setup Instructions

### 1. Clone the Repository

- git clone https://github.com/vijaysonidiyan/UseAppReqres.git
- cd reqres-integration-demo

### 2. Build the Solution
- open sln file in visual studio 2022
- dotnet build execute this command or enter Ctrl + Shift + B
- install nessasary library if missing and install it if needed

### 3. Run the Console App
- cd UserWebApp.Test
- Run UserWebApp.Console using (dotnet run) command 
- Example output:
- Fetched 12 users:
- 1: George Bluth - george.bluth@reqres.in
- ...
- Do you want to use cached user list? (y/n)
- y
- Fetching from cache...
- 1: George Bluth - george.bluth@reqres.in
- ...
- Fetched single user:
- 1: George Bluth - george.bluth@reqres.in

### 4. Run Unit Tests
- cd UserWebApp.Test
- dotnet test
- Expected output:
- Passed!  - Failed: 0, Passed: 3

