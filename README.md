# awesome-aspnet-templates
A collection of ASP.NET templates

# Structure of the Projects

Each project is structured using clean architecture. The domain and application layers return a result object, which is handled by the upper layer. Once it reaches the Web API layer, it is mapped into a response or problem details.  
The 'use cases' are defined in the application layer and use the CQRS pattern.  
To implement cross-cutting logic when handling a request, decorators are used to handle the validation of the request using Fluent Validation and a logging decorator to log each request, avoiding code duplication.

## Clean Architecture

### Benefits of Clean Architecture:
- **Separation of Concerns**: Clean architecture enforces clear boundaries between layers, making the code easier to understand and maintain. Every layer has its own responsibility.
- **Testability**: By isolating the business logic from external dependencies, it becomes easier to write unit tests for the core functionality.
- **Flexibility**: The architecture allows for swapping out frameworks, databases, or UI layers with minimal impact on the rest of the application, reducing the risk of vendor lock-in.
- **Scalability**: Clean architecture promotes modularity, making it easier to scale the application as requirements grow.
- **Maintainability**: With a well-organized structure, developers can quickly locate and modify code, reducing the time spent on debugging and enhancements.
- **Reusability**: Core business logic can be reused across different projects or platforms without significant changes.
- **Improved Collaboration**: Clear boundaries and responsibilities make it easier for teams to work on different parts of the application simultaneously.
- **Long-Term Viability**: Clean architecture supports the development of robust and future-proof applications that can adapt to changing requirements.

---

### **1. Domain Layer**
- **Purpose**: This is the core of the application. It contains the **business logic** and **entities** that define the rules and behavior of your application.
- **Key Components**:
    - **Entities**: Represent the core data and behavior of your application (e.g., `Order`, `Customer`).
    - **Value Objects**: Immutable objects that represent concepts (e.g., `Money`, `Address`).
    - **Domain Services**: Contain business logic that doesn’t naturally fit within a single entity.
    - **Aggregates**: Group related entities and enforce consistency rules.
- **Dependencies**: The Domain layer is **independent** of all other layers. It doesn’t depend on frameworks, databases, or external libraries. This ensures that your business logic is isolated and reusable.

---

### **2. Application Layer**
- **Purpose**: Acts as the **bridge** between the Domain and Infrastructure/Web API layers. It defines the **interfaces** and **contracts** for the infrastructure and orchestrates the use of domain logic.
- **Key Components**:
    - **Interfaces**: Define contracts for external dependencies (e.g., `IEmailService`).
    - **Use Cases**: Implement application-specific logic (e.g., "Create Order", "Process Payment"). These are implemented as **Command Handlers** or **Query Handlers** in a CQRS pattern.
    - **DTOs (Data Transfer Objects)**: Define the data structures used to transfer data between layers.
- **Dependencies**: The Application layer depends only on the Domain layer. It doesn’t directly depend on Infrastructure or Web API, ensuring flexibility and testability.

---

### **3. Infrastructure Layer**
- **Purpose**: Provides the **concrete implementations** of the interfaces defined in the Application layer. It handles external concerns like databases, file systems, APIs, and third-party services.
- **Key Components**:
    - **Repositories**: Implement data access logic (e.g., using Entity Framework Core for database operations).
    - **Services**: Implement external integrations (e.g., sending emails, logging, or calling external APIs).
- **Dependencies**: The Infrastructure layer depends on both the Domain and Application layers. It implements the contracts defined in the Application layer and may use Domain entities for persistence.

---

### **4. Web API Layer**
- **Purpose**: This is the **entry point** of your application. It defines the **endpoints** (controllers) and sets up the application (e.g., dependency injection, middleware, routing).
- **Key Components**:
    - **Controllers**: Define the HTTP endpoints (e.g., `GET /api/orders`, `POST /api/customers`).
    - **Dependency Injection (DI)**: Registers services, repositories, and other dependencies.
    - **Middleware**: Handles cross-cutting concerns like authentication, logging, and error handling.
- **Dependencies**: The Web API layer depends on the Application layer to execute use cases and on the Infrastructure layer (only to register the services defined in the Infrastructure layer).

---

### **How the Layers Interact**
1. **Web API Layer**: Receives an HTTP request and forwards it to the Application layer.
2. **Application Layer**: Executes the use case by invoking domain logic (from the Domain layer) and infrastructure services (from the Infrastructure layer).
3. **Domain Layer**: Processes the business logic and returns the result.
4. **Infrastructure Layer**: Handles external concerns like database access or API calls as needed.