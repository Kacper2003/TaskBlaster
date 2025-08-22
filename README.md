## TaskBlaster Backend

### Overview
TaskBlaster is a task management platform inspired by Trello/JIRA.  
The backend handles tasks, assignments, tags, comments, and scheduled email notifications.

Built with modern best practices, TaskBlaster focuses on **security**, **maintainability**, and clear separation of concerns.

---

### Architecture
The backend runs on **.NET 8** with **Entity Framework Core**, containerized via **Docker** and backed by **PostgreSQL**.

Projects:
- **TaskBlaster.TaskManagement.API** – tasks, tags, users, comments, priorities
- **TaskBlaster.TaskManagement.Notifications** – email sending + Hangfire background jobs
- **TaskBlaster.TaskManagement.DAL** – repositories & DbContext
- **TaskBlaster.TaskManagement.Models** – DTOs & input models
- **taskblaster-web** – minimal Next.js client (login + acquiring JWT; useful for testing authenticated flows)

**System Architecture**

![Architecture diagram](docs/architecture.png)

---

### Security
- **JWT authentication** (Auth0) across APIs  
- **M2M tokens** for API-to-API calls (Task API → Notification API)  
- Secrets via configuration/environment (no secrets in code)  
- On first authenticated call, the backend can persist the user profile from token claims (e.g., email, name)

---

### Deployment
- **Local**: Docker Compose (APIs, PostgreSQL, Hangfire)  
- **Ports (default)**:
  - Task API → `http://localhost:5010`
  - Notification API → `http://localhost:5020`
  - Swagger for each API at `/swagger`
  - Hangfire Dashboard (dev only) typically at `/hangfire` on the Notifications API

---

### Setup

#### Prerequisites
- Docker & Docker Compose  
- .NET SDK 8.0 (if running outside Docker)  
- Node.js (only if you run the frontend)

#### Steps
CODEBLOCK START
# Clone the repository
git clone git@github.com:Kacper2003/TaskBlaster.git
cd TaskBlaster

# Copy example configs (fill in values)
cp TaskBlaster.TaskManagement/TaskBlaster.TaskManagement.API/appsettings.Development.json.example \
   TaskBlaster.TaskManagement/TaskBlaster.TaskManagement.API/appsettings.Development.json

cp TaskBlaster.TaskManagement/TaskBlaster.TaskManagement.Notifications/appsettings.Development.json.example \
   TaskBlaster.TaskManagement/TaskBlaster.TaskManagement.Notifications/appsettings.Development.json

# (Optional) populate taskblaster-web/.env.local for Auth0 if you plan to use the frontend

# Run with Docker
docker compose up --build
CODEBLOCK END

- Task API → `http://localhost:5010`  
- Notification API → `http://localhost:5020`

---

### API Documentation
The backend uses **OpenAPI** with **Swagger UI** for interactive docs/testing.

Local:
- Task API → `http://localhost:5010/swagger`  
- Notification API → `http://localhost:5020/swagger`

---

### Example Endpoint Categories

- **Tasks**  
  - `GET /tasks` – paginated list (filters)  
  - `GET /tasks/{id}` – task details  
  - `POST /tasks` – create task  
  - `DELETE /tasks/{id}` – archive task  
  - `PATCH /tasks/{id}/assign/{userId}` – assign (triggers email)  
  - `PATCH /tasks/{id}/unassign/{userId}` – unassign (triggers email)  
  - `PATCH /tasks/{id}/status` – update status  
  - `PATCH /tasks/{id}/priority` – update priority

- **Comments**  
  - `GET /tasks/{id}/comments`  
  - `POST /tasks/{id}/comments`  
  - `DELETE /tasks/{id}/comments/{commentId}`

- **Tags / Status / Priority / Users**  
  - `GET /tags` · `POST /tags`  
  - `GET /status` · `GET /priorities`  
  - `GET /users`

- **Notifications**  
  - `POST /emails/basic`  
  - `POST /emails/template` (optional)

---

### Database Schema

The following ER diagram shows the PostgreSQL schema used by TaskBlaster:

![Database schema](docs/db-schema.png)

---

## License
This project is part of my personal portfolio.
