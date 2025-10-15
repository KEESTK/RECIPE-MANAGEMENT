# 🧾 Evaluation License

**Copyright © 2025 Kees Toukam — All Rights Reserved**  
📧 *kees.toukam@gmail.com*  

This project and all accompanying source code are provided **solely for evaluation and recruitment purposes** related to the **Bewerberaufgabe**.  

The receiving company (hereafter *the Evaluating Party*) is granted a **non-exclusive, non-transferable, and revocable right** to:
- View, execute, and analyze the project code,  
- Evaluate the design, architecture, and implementation for recruitment purposes.  

Any other use, modification, redistribution, or publication — including commercial, educational, or internal development usage — is **strictly prohibited** without my prior written consent.  

This license does **not** confer any transfer of ownership or intellectual property rights.  
The software is provided *“as is”*, without warranty of any kind.

---

# 🍳 RecipeManagement Library & TestHarness  
*A Bewerberaufgabe Implementation by Kees Toukam*  

---

## 🚀 Quickstart

### Prerequisites
- .NET SDK **9.0** or later installed  
- No external dependencies required (SQLite database is created automatically)

### Clone and Build
```bash
git clone <repository-url>
cd recipe-management
dotnet build
```

### Run the TestHarness
```bash
cd TestHarness
dotnet run
```

This will:
- Create a SQLite database file (`recipes_test.db`)
- Seed test data
- Run all validation and functional tests
- Print a detailed summary of passed and failed cases

**Expected Output Example:**
```
=== RecipeManagement TestHarness (enhanced report mode) ===
Seeded recipe 'Pasta' (GUID) by demoUser.
✅ Register duplicate username should fail
✅ Recipe must have at least 1 step
...
=== SUMMARY ===
Total: 17 tests executed.
✅ Passed: 15
❌ Failed: 2
⚪ Skipped: 0
=== TestHarness finished ===
```

---

## 🧩 1. Purpose & Scope

This project implements a modular **.NET 9 class library** for managing users and recipes, including related entities such as ingredients, categories, steps, and favorites.  

The goal was to design and build a **clean, reusable, and extensible backend library** that fulfills the functional and persistence requirements of the Bewerberaufgabe, while maintaining architectural clarity and testability.  
To demonstrate the system, I created a **TestHarness console application** that initializes the database and verifies all business rules end-to-end.

---

## 🧱 2. Solution Structure

```
RECIPE-MANAGEMENT/
├── RecipeManagement/        # Core .NET class library
│   ├── Domain/              # Entities, Value Objects, Domain Services
│   ├── Application/         # Interfaces and DTOs (Application Layer)
│   ├── Infrastructure/      # Persistence (EF Core, Repositories, Configurations)
│   ├── RecipeManagement.csproj
│   └── RecipeManagement.sln
│
└── TestHarness/             # Console app demonstrating and verifying functionality
    ├── Program.cs
    ├── TestHarness.csproj
    └── recipes_test.db      # SQLite database created automatically
```

### Library vs. Executable

- **RecipeManagement** is a **class library** containing all domain, application, and persistence logic.  
- **TestHarness** is a **console demo application** that references the library to test it in realistic scenarios.

---

## ⚙️ 3. Technologies & Dependencies

| Component | Technology | Purpose |
|------------|-------------|----------|
| **.NET 9.0 (C# 12)** | Core runtime | Modern language and tooling support |
| **Entity Framework Core 9.0** | ORM | Data persistence and relationship mapping |
| **SQLite** | Embedded DB | Portable database for demo and testing |
| **Microsoft.Extensions.DependencyInjection** *(optional)* | Dependency Injection | Enables flexible wiring of repositories/services |

---

## 🧠 4. Architecture Overview

The architecture follows **Domain-Driven Design (DDD)** and **Clean Architecture** principles.

| Layer | Responsibility | Example Components |
|-------|----------------|-------------------|
| **Domain** | Core business entities & rules | `User`, `Recipe`, `Ingredient`, `Favorite`, `Step` |
| **Application** | Abstractions & coordination logic | `IUserRepository`, `IRecipeRepository`, `RecipeService` |
| **Infrastructure** | Persistence & data access | EF Core DbContext, repositories, entity configurations |
| **Presentation (TestHarness)** | Verification layer | Integration tests, results reporting |

### Data Flow
1. `TestHarness` calls domain services (`UserService`, `RecipeService`, etc.)  
2. Services interact via repository interfaces (defined in the Application layer).  
3. Infrastructure implements those interfaces with EF Core.  
4. Domain entities validate state and maintain business invariants.  
5. The database schema enforces uniqueness and foreign key constraints.

---

## 🧭 5. Key Design Decisions

| Decision | Reason |
|-----------|--------|
| **Domain-Driven Structure** | Keeps core business logic independent from database or UI. |
| **Repository Pattern** | Clean abstraction between logic and data access. |
| **EF Core (SQLite)** | Ideal for lightweight demonstration and testing. |
| **Service Layer** | Centralizes validation logic before persistence. |
| **Console-based Testing** | Enables easy, self-contained verification. |
| **Unique Constraints in DB** | Adds consistency guarantees beyond application logic. |

---

## 🧪 6. TestHarness Overview

The **TestHarness** acts as a simple yet powerful integration test runner.  
It automatically sets up a clean database and verifies each functional requirement.

### Tested Scenarios
- User creation and duplicate detection  
- Recipe validation (steps, ingredients, categories)  
- Unique constraints (recipe, ingredient, category)  
- Foreign key integrity  
- Favoriting rules (cannot favorite own recipes, idempotency)  
- Querying by user, category, and ingredient  
- Cascade deletion of related entities  

### Output Summary
The harness reports:
- ✅ Passed tests  
- ❌ Failed tests with exception type and message  
- ⚪ Skipped (none by default)  
- Totals (executed, passed, failed)

---

## ✅ 7. Requirement Fulfillment Summary

| Requirement | Implementation | Verified By |
|--------------|----------------|-------------|
| **Nutzerverwaltung** | Implemented in `UserService` & `UserRepository`. | Duplicate username test. |
| **Rezeptverwaltung** | Implemented in `RecipeService` with validation. | Step, ingredient, category tests. |
| **Eindeutiger Rezeptname** | Unique index in `RecipeConfiguration`. | “Duplicate recipe name fails.” |
| **Mindestens 1 Schritt, 1 Zutat, 1 Kategorie** | Validated before saving. | Validation tests. |
| **Eindeutige Zutaten & Kategorien** | Unique indices in configurations. | “Duplicate ingredient/category fails.” |
| **Rezepte nur für registrierte Nutzer** | Enforced by FK on `AuthorId`. | FK constraint tests. |
| **Abfragen nach User, Kategorie, Zutat** | Repository methods implemented. | Query tests. |
| **Favoriten anderer Nutzer** | Implemented in `FavoriteService`. | Favoriting tests. |
| **Favoritenliste anzeigen** | `GetFavoritesByUserAsync()`. | “Verify favorites list” test. |
| **Kaskadierende Löschung** | EF Core cascade rules. | Cascade delete test. |

---

## ⚠️ 8. Known Pitfalls & Limitations

| Issue | Explanation | Mitigation |
|--------|--------------|------------|
| **SQLite FK timing** | EF may insert join entities too early in deep graphs. | Use `Attach()` or switch to PostgreSQL. |
| **Plain password hashes** | No full authentication stack. | Integrate ASP.NET Identity. |
| **Manual test execution** | Harness must be run manually. | Convert to automated xUnit tests. |
| **Lack of transaction management** | No explicit transaction wrapping. | Add transactional service layer. |
| **No Web/API layer** | Console demo only. | Extend via ASP.NET Web API. |

---

## 🚧 9. Future Enhancements

- Add a **REST API** using ASP.NET Core  
- Integrate **user authentication** (Identity)  
- Implement **unit/integration tests** with xUnit  
- Add **pagination and filters** to recipe queries  
- Support **PostgreSQL** for production deployment  
- Add **DTO mapping** and **logging**

---

## 🕒 10. Development Time

| Activity | Hours |
|-----------|--------|
| Requirements & Architecture | 4 |
| Entity Modeling & Configurations | 5 |
| Repository & Service Implementation | 5 |
| TestHarness & Validation Scenarios | 4 |
| Debugging & Refinement | 2 |
| **Total** | **20 hours** |

---

## 🏁 11. Summary

I designed and implemented **RecipeManagement** as a modular .NET library demonstrating clean architecture, domain-driven design, and EF Core persistence.  

The **TestHarness** validates all business rules, ensuring the system is both functionally complete and architecturally robust.  
This project provides a solid foundation for further development into a full-featured web application while fulfilling all given requirements for the Bewerberaufgabe.
