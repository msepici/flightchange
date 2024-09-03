<pre>1. Database Design (Data Types, Keys, Indexes)
Tables and Relationships:
 •	Routes Table:
  o	Columns: route_id (Primary Key, INT), origin_city_id (INT), destination_city_id (INT), departure_date (DATE)
  o	Indexes: Composite index on (origin_city_id, destination_city_id) to optimize queries that filter by route.
•	Flights Table:
  o	Columns: flight_id (Primary Key, INT), route_id (Foreign Key referencing Routes, INT), departure_time (TIMESTAMP), arrival_time (TIMESTAMP), airline_id (INT)
  o	Indexes: Index on departure_time to optimize queries that filter flights by time. A composite index on (departure_time, route_id) can also be used to optimize queries that join  flights with routes.
•	Subscriptions Table:
 o	Columns: subscription_id (Primary Key, INT), agency_id (INT), origin_city_id (INT), destination_city_id (INT)
 o	Indexes: Composite index on (agency_id, origin_city_id, destination_city_id) to speed up queries that filter by agency and route.
Data Types:
INT: Used for identifiers and foreign keys.
TIMESTAMP: Used for departure_time and arrival_time, allowing precise time filtering and comparisons.
DATE: Used for departure_date in Routes, where only the date (without time) is relevant.

Keys:
Primary Keys: Uniquely identify records in each table (route_id, flight_id, subscription_id).
Foreign Keys: Ensure referential integrity between Flights and Routes, and between Subscriptions and Routes.

Indexes:
Single-Column Indexes: Used for frequently queried columns like departure_time.
Composite Indexes: Used for columns that are often queried together, such as (origin_city_id, destination_city_id) in Routes.

2. Overall Structure of the Application (Layers, Data Flow, Dependencies, (De)Coupling)
Layers:
 Presentation Layer: The user interface or API layer that interacts with the users or external systems. This layer handles input validation and forwards requests to the application layer.
 Application Layer: Contains business logic, including the FlightChangeDetector service. This layer orchestrates operations, manages workflows, and interacts with the data access layer.
 Data Access Layer: Responsible for interacting with the database. It includes repositories that encapsulate the logic for accessing data sources, providing an abstraction over database  operations.

Data Flow:
 Input: The user or system provides parameters such as start date, end date, and agency ID.
 Processing: The application layer (e.g., FlightChangeDetector) processes the input, retrieves data through the data access layer, and applies the change detection algorithm.
 Output: The results are returned to the presentation layer, which may output them in a desired format (e.g., CSV file).

Dependencies:
 Dependency Injection: Used to inject services and repositories into controllers and services, promoting loose coupling and testability.
 Decoupling: Interfaces are used to decouple the application layer from the data access layer, allowing for easier testing and maintenance.
3. Data Access Layer Implementation

Repository Pattern:
Interfaces: Defines contracts for data access operations (e.g., IFlightRepository, IRouteRepository).
Repositories: Implemented these interfaces using Entity Framework Core to interact with the database. 
Each repository handles CRUD operations for a specific entity (e.g., FlightRepository for Flights).

4. Change Detection Algorithm Implementation

Input: Start date, end date, agency ID.

Process:
1.	Retrieve subscriptions for the specified agency.
2.	Fetch flights within the date range, filtered by the subscriptions.
3.	Identify new flights by checking if a similar flight existed 7 days before.
4.	Identify discontinued flights by checking if a similar flight exists 7 days after.

Output: A list of flight changes (new or discontinued) is returned.
 
5. Data Structures Used

FlightChange Class: Represents the result of the change detection, containing the flight details and status (new or discontinued).

Collections:
 o	List<T>: Used for in-memory storage of flights, routes, and changes.
 o	ConcurrentBag<T>: Used for thread-safe accumulation of results during parallel processing.

6. Optimizations Applied
Batch Processing:
 The DetectChanges method was refactored to process flights in batches, reducing memory usage and improving performance.

Parallel Processing:
 Implemented parallel processing using Parallel.For to process batches concurrently, significantly reducing execution time.

Thread-Safe DbContext:
Ensured thread-safe database operations by creating a new DbContext instance for each thread using IServiceProvider.CreateScope().

Indexing:
Created indexes on key columns like departure_time and composite indexes on frequently queried columns to optimize database query performance.

Query Optimization:
Used EXPLAIN ANALYZE to monitor query performance and ensure that indexes were being used effectively.
</pre>
