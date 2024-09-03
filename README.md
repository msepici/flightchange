# Flight Change Detection System

## 1. Database Design

### Tables and Relationships

#### Routes Table
- **Columns:**
  - `route_id` (Primary Key, `INT`)
  - `origin_city_id` (`INT`)
  - `destination_city_id` (`INT`)
  - `departure_date` (`DATE`)
- **Indexes:**
  - Composite index on (`origin_city_id`, `destination_city_id`) to optimize route queries.

#### Flights Table
- **Columns:**
  - `flight_id` (Primary Key, `INT`)
  - `route_id` (Foreign Key referencing Routes, `INT`)
  - `departure_time` (`TIMESTAMP`)
  - `arrival_time` (`TIMESTAMP`)
  - `airline_id` (`INT`)
- **Indexes:**
  - Index on `departure_time` to optimize time-based queries.
  - Composite index on (`departure_time`, `route_id`) to optimize joins with Routes.

#### Subscriptions Table
- **Columns:**
  - `subscription_id` (Primary Key, `INT`)
  - `agency_id` (`INT`)
  - `origin_city_id` (`INT`)
  - `destination_city_id` (`INT`)
- **Indexes:**
  - Composite index on (`agency_id`, `origin_city_id`, `destination_city_id`) to speed up agency and route queries.

### Data Types
- **`INT`:** Used for identifiers and foreign keys.
- **`TIMESTAMP`:** Used for precise time filtering (e.g., `departure_time`, `arrival_time`).
- **`DATE`:** Used for `departure_date` where only the date is relevant.

### Keys
- **Primary Keys:** Unique identifiers for each table (e.g., `route_id`, `flight_id`, `subscription_id`).
- **Foreign Keys:** Ensure referential integrity between tables (e.g., `Flights` to `Routes`).

### Indexes
- **Single-Column Indexes:** Optimize queries on frequently queried columns like `departure_time`.
- **Composite Indexes:** Optimize queries that involve multiple columns, such as (`origin_city_id`, `destination_city_id`) in `Routes`.

## 2. Overall Structure of the Application

### Layers

- **Presentation Layer:** The UI or API layer that handles user interaction and input validation, forwarding requests to the application layer.
- **Application Layer:** Contains business logic, such as the `FlightChangeDetector` service, orchestrating operations and interacting with the data access layer.
- **Data Access Layer:** Manages database interactions, including repositories that abstract database operations.

### Data Flow

1. **Input:** User/system provides parameters like start date, end date, and agency ID.
2. **Processing:** The `FlightChangeDetector` processes input, retrieves data via the data access layer, and applies the change detection algorithm.
3. **Output:** Results are returned to the presentation layer, which formats them as needed (e.g., CSV).

### Dependencies

- **Dependency Injection:** Injects services and repositories into controllers and services, promoting loose coupling and testability.
- **Decoupling:** Interfaces are used to decouple layers, allowing for easier testing and maintenance.

## 3. Data Access Layer Implementation

### Repository Pattern

- **Interfaces:** Define contracts for data access operations (e.g., `IFlightRepository`, `IRouteRepository`).
- **Repositories:** Implement these interfaces using Entity Framework Core to interact with the database, handling CRUD operations for entities.

## 4. Change Detection Algorithm Implementation

### Input
- **Parameters:** Start date, end date, agency ID.

### Process

1. Retrieve subscriptions for the specified agency.
2. Fetch flights within the date range, filtered by subscriptions.
3. Identify new flights by checking for similar flights 7 days prior.
4. Identify discontinued flights by checking for similar flights 7 days later.

### Output
- Returns a list of flight changes (new or discontinued).

## 5. Data Structures Used

### FlightChange Class
- Represents the result of change detection, including flight details and status (new or discontinued).

### Collections
- **List:** Used for in-memory storage of flights, routes, and changes.
- **ConcurrentBag:** Used for thread-safe accumulation of results during parallel processing.

## 6. Optimizations Applied

### Batch Processing
- **Refactoring:** The `DetectChanges` method processes flights in batches, reducing memory usage and improving performance.

### Parallel Processing
- **Implementation:** Parallel processing via `Parallel.For` to process batches concurrently, reducing execution time.

### Thread-Safe DbContext
- **Safety:** Ensures thread-safe database operations by creating a new `DbContext` instance per thread using `IServiceProvider.CreateScope()`.

### Indexing
- **Indexes:** Created on key columns like `departure_time` and composite indexes on frequently queried columns to optimize query performance.

### Query Optimization
- **Monitoring:** Used `EXPLAIN ANALYZE` to monitor and ensure effective use of indexes in queries.

