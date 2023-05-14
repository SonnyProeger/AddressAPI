# AddressAPI
AddressAPI Project

The AddressAPI project is an API for managing addresses. It provides endpoints to retrieve, create, update, and delete address records. It also includes functionality to search addresses, sort them, and calculate the distance between two addresses.

# Technologies Used
C#
ASP.NET Core
Entity Framework Core
Newtonsoft.Json
HttpClient

# Getting Started
To get started with the AddressAPI project, follow the steps below:

# Prerequisites
.NET Core SDK (version 6.0 or higher)
A database engine supported by Entity Framework Core (e.g., SQL Server, MySQL, PostgreSQL)

# Installation
Clone the repository or download the project files.
Navigate to the project's root directory.
Configuration
Rename the .env.example file in the root directory to .env.
Open the .env file and provide the necessary configuration values, such as the Google Maps API key.

# Running the Application
Open a terminal or command prompt.
Navigate to the project's root directory.
Run the following command to start the application:
bash
Copy code
dotnet run
The application will start running on your localhost.

# Usage
API Endpoints
GET /api/addresses - Retrieve a list of addresses.
GET /api/addresses/{id} - Retrieve a single address by ID.
POST /api/addresses - Create a new address.
PUT /api/addresses/{id} - Update an existing address by ID.
DELETE /api/addresses/{id} - Delete an address by ID.
GET /api/addresses/distance?originId={originId}&destinationId={destinationId} - Calculate the distance between two addresses by their IDs.

# Query Parameters
Search - Search for addresses based on a search query.
SortBy - Sort the addresses by a specific field.
SortDescending - Set to true to sort the addresses in descending order.

  # Example Usage
Retrieve all addresses: GET /api/addresses
Retrieve a specific address: GET /api/addresses/{id}
Create a new address: POST /api/addresses
Update an existing address: PUT /api/addresses/{id}
Delete an address: DELETE /api/addresses/{id}
Calculate the distance between two addresses: GET /api/addresses/distance?originId={originId}&destinationId={destinationId}

# Reflection

**Proud Achievements

Modular and Extensible Architecture: The project follows a modular architecture, with separate classes

**Improvements

- The GetAddresses method in the AddressController class could be refactored further. It currently handles both search filtering and sorting logic. I should extract the search filtering logic and sorting logic into separate methods or classes.

- The code could benefit from additional error handling and validation. I need to ensure that appropriate checks are in place for null references, invalid inputs, or potential exceptions that may occur during API requests or database operations. Properly handle and communicate errors to the client through appropriate HTTP status codes and error messages.

- The code does not include unit tests. I should start writing unit tests to verify the behavior and functionality of individual methods and classes. This will help ensure the correctness of the code, facilitate easier maintenance, and enhance confidence in making changes.



Interface Segregation Principle (ISP):
