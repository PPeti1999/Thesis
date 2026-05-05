# 🍏 HealthyApp - Lifestyle & Nutrition Tracker

A comprehensive Full-Stack Single Page Application (SPA) designed to support a healthy lifestyle, diet management, and physical activity tracking. Developed as a **University Thesis Project**.

[![.NET Core](https://img.shields.io/badge/.NET_Core-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/en-us/sql-server/)
[![TypeScript](https://img.shields.io/badge/TypeScript-007ACC?style=for-the-badge&logo=typescript&logoColor=white)](https://www.typescriptlang.org/)

---

## 📚 Hungarian Documentation / Magyar Dokumentáció

*   📄 **Dokumentáció:** [Megnyitás GitHub nézetben](docs/Egeszseges-eletmodot-tamogato-webalkalmazas.pdf) 
*   📊 **Prezentáció:** [Letöltés / megnyitás](docs/Net-alapu-fejlesztes-prezentacio.pptx) 
---

## 📖 About The Project

**HealthyApp** is a modern web application that helps users track their daily meals, create custom recipes, monitor physical activities, and analyze weight changes. The system automatically calculates daily caloric and macronutrient needs (**BMR & TDEE**) based on user goals (cutting, bulking, maintaining) and dynamically updates statistics.

### ✨ Key Technical Features

*   **Real-Time "Recalculation Chain":** Engineered a complex, real-time database-level algorithm using Entity Framework Core and MS SQL for precise macronutrient synchronization across meals, recipes, and daily notes.
*   **Secure Authentication & Identity:** Implemented robust JWT-based ASP.NET Core Identity authentication, featuring role-based access control and automated email confirmation/password reset via the **Mailjet API**.
*   **Type-Safe API Communication:** Fully automated frontend-backend data exchange and client code generation using the **NSwag** toolchain, eliminating manual DTO mapping errors and ensuring strict type safety.
*   **Smart Profile & Food Management:** Automatic calculation of daily macro goals using the **Mifflin-St Jeor equation**, paired with CRUD operations for a dynamic Recipe Builder.

---

## 🛠️ Technology Stack

### **Backend**
*   **Framework:** C# / ASP.NET Core Web API (N-Tier Architecture)
*   **Database & ORM:** Microsoft SQL Server, Entity Framework Core (Code-First)
*   **Security:** ASP.NET Core Identity, JWT Bearer Tokens
*   **API Documentation:** Swagger (OpenAPI), NSwag
*   **Testing:** xUnit, Moq, FluentAssertions, EF Core InMemory Database

### **Frontend**
*   **Framework:** Angular 19, TypeScript
*   **UI/UX:** HTML5, SCSS, Bootstrap 5, Ngx-Bootstrap (Modals)
*   **State Management & Data Visualization:** RxJS (Observables, Subjects), Chart.js

---

## 🏗️ System Architecture & Design

The application follows a strict separation of concerns using the **DTO (Data Transfer Object)** pattern to communicate between the Client and Server, ensuring data security and optimized payloads.

### Use Case Diagram
This diagram illustrates the role-based access control (Guest vs. Registered Member) and the core functionalities of the system.

![Use Case Diagram](docs/usecase.png)

### Entity Relationship (ER) Diagram
The database is built using a Code-First approach. It handles complex Many-to-Many (N:M) relationships (e.g., Recipes containing multiple Foods, Meals containing both Foods and Recipes).

![ER Diagram](docs/plantextuml.png)

---

## ⚙️ Quick Start (Local Setup)

### Prerequisites
*   [.NET 9.0 SDK](https://dotnet.microsoft.com/) (or newer)
*   [Node.js & npm](https://nodejs.org/)
*   [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or Developer edition)

### Installation

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/PPeti1999/Thesis.git](https://github.com/PPeti1999/Thesis.git)
    ```

2.  **Backend Setup:**
    *   Navigate to the API directory.
    *   Update the connection string in `appsettings.json`.
    *   *Note: Use .NET User Secrets to avoid exposing real API keys (e.g., Mailjet).*
    *   Run EF Core migrations to create the database:
    ```bash
    dotnet ef database update
    ```
    *   Start the API:
    ```bash
    dotnet run
    ```

3.  **Frontend Setup:**
    *   Navigate to the Angular client directory.
    *   Install dependencies and start the development server:
    ```bash
    npm install
    ng serve
    ```

4.  **Access the app:**
    Open your browser and navigate to `http://localhost:4200`
