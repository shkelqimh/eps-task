
# EpsTask

A TCP-based discount code generation and validation system using .NET. The solution is organized with clear separation of concerns between the client, server, persistence layer, and shared logic.

---

## 🗂 Solution Structure

```
EpsTask/
│
├── src/
│   ├── Client                       # TCP client that sends requests
│   ├── Server                       # TCP server that handles requests
│   ├── Infrastructure.Persistence.Sqlite  # SQLite persistence for codes
│   └── Shared                       # Shared models and enums (e.g. MessageType)
│
└── tests/                           # Unit and integration tests
```

---

## 🛠 Requirements

- .NET 8 SDK
- OS: Windows, macOS, or Linux
- SQLite installed (or included via `Microsoft.Data.Sqlite` NuGet package)

---

## 🚀 Getting Started

### 1. Build the solution

```bash
dotnet build
```

---

## 🖥️ Running the Server

1. Navigate to the `Server` project directory:

```bash
cd src/Server
```

2. Run the server:

```bash
dotnet run
```

The server will start listening for TCP requests on the configured port (default: `50051`) and will output messages to the console.

---

## 💻 Running the Client

1. In a new terminal window, navigate to the `Client` project:

```bash
cd src/Client
```

2. Run the client:

```bash
dotnet run
```

3. You’ll see the following options:

```
1. Generate codes
2. Use code
3. Press Q to quit
Choose option:
```

4. You can generate or use discount codes through interactive prompts.

---

## 🧠 How It Works

- The **Client** connects to the **Server** via TCP and sends a message (e.g., generate codes, use code).
- The **Server** verifies the request, processes it, and responds.
- All generated codes and their usage status are stored in a **SQLite database** (`Infrastructure.Persistence.Sqlite` project).
- Only clients with **whitelisted IP addresses** can access the server.

---

## 💾 Database

- Codes are stored in a local **SQLite** database.
- The schema and database handling are implemented in the `Infrastructure.Persistence.Sqlite` project.
- No manual setup required — database is created automatically on first use.

---

## ✅ Running Tests

```bash
cd tests
dotnet test
```

Unit and integration tests will validate the behavior of code generation, usage, validation, and TCP communication.

---

## 🔐 Security Notes

- The server restricts access to a list of **whitelisted IP addresses**.
- Make sure to configure your local IP in the `Settings` section before starting the server if running locally.

---

## 📬 Message Types

```csharp
enum MessageType : byte
{
    GenerateCode = 1,
    UseCode = 2
}
```

These message types define the protocol between client and server.

---

## 📂 Settings

Server and client settings are configured using the `Settings` class (in Shared). You can define:

- Port
- Whitelisted IPs
- Code generation rules

---
