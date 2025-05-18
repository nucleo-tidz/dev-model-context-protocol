# Model Context Protocol – Shipment and Vessel AI Integration

This repository demonstrates a real-world use case of the **Model Context Protocol (MCP)** using .NET and [Semantic Kernel](https://github.com/microsoft/semantic-kernel). It integrates multiple MCP servers and a client that can invoke services dynamically through natural language chat.

## ✨ Overview

The project sets up a multi-server environment with the following components:

### 🚢 Vessel Server
An MCP server that responds with **number of legs** for a given **vessel ID**.

### 📦 Shipment Server
An MCP server focused on **container information**. It provides:
- **Container type** (e.g., DRY) based on ID
- **Container state** (e.g., Damaged) based on ID

### 🤖 Shipment Client
A Semantic Kernel-based MCP client that:
- Connects to both the Vessel and Shipment servers
- Embeds a **chat interface**
- Automatically routes user queries to the appropriate server using tool calling

## 🧠 How It Works

1. You start a Semantic Kernel-powered chat from the shipment client.
2. The user asks a question like:
   - “What type of container is CONT123?”
   - “How many legs does vessel VSL456 have?”
3. The SK planner chooses the appropriate MCP tool and routes the query to the correct server via Server-Sent Events (SSE).
4. The result is returned in the chat seamlessly.

## 🛠 Technologies Used

- [.NET](https://dotnet.microsoft.com/)
- [Semantic Kernel](https://github.com/microsoft/semantic-kernel)
- Model Context Protocol (via `ModelContextProtocol` NuGet package)
- Docker & `docker-compose`
- Server-Sent Events (SSE) for live connections


