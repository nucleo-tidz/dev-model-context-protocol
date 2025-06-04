# Model Context Protocol – Shipment and Vessel AI Integration

This repository demonstrates a real-world use case of the **Model Context Protocol (MCP)** using .NET and [Semantic Kernel](https://github.com/microsoft/semantic-kernel). It integrates multiple MCP servers and a client that can invoke services dynamically through natural language chat. Read more about it [here](https://www.linkedin.com/posts/nucleotidz_orchestrating-multi-agent-ai-applictaion-activity-7334108268723609601-Hrj_?utm_source=share&utm_medium=member_desktop&rcm=ACoAAA8W40wBqxt9tXu5xBQinNefCMjDPWwL9Oc)

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

# 🔐 Azure AD Setup for MCP Authentication (Client Credentials Flow)

## 🧩 Step-by-Step Setup in Azure

### 🔹 Step 1: Register an Application (Client App)

1. Go to [https://portal.azure.com](https://portal.azure.com)
2. Navigate to **Microsoft Entra ID > App registrations**
3. Click **+ New registration**
4. Fill out the form:
   - **Name**: `mcp-client-app`
   - **Supported account types**: *Single tenant* (or as per your needs)
   - Leave **Redirect URI** empty for now
5. Click **Register**

---

### 🔹 Step 2: Generate Client Secret

1. In the registered app, go to **Certificates & secrets**
2. Click **+ New client secret**
3. Provide a name and expiry
4. Click **Add**
5. Copy the **secret value** immediately — you won't be able to see it again

---

### 🔹 Step 3: Note Down App Info

| Field        | Source             |
|--------------|--------------------|
| **Tenant ID**   | From the app's **Overview** tab |
| **Client ID**   | Also from the **Overview** tab |
| **Client Secret** | From Step 2 |
| **Token Endpoint** |  https://login.microsoftonline.com/{tenant-id}/oauth2/v2.0/token 


---
### 🔹 Step 4: (Optional but Recommended) Create a Scope for Your API

If you have registered your **MCP Server** as another Azure AD application (API app), follow these steps:

1. **Register your API App**  
   - Example name: `mcp-server-api`

2. **Expose an API**  
   - Go to the app > **Expose an API**
   - Set an **Application ID URI** (e.g. `api://<api-client-id>`)

3. **Add a Scope**
   - Click **+ Add a scope**
   - Fill in the form:
     - **Scope name**: `mcp.readwrite`
     - **Who can consent**: *Admins only* (or as needed)
     - **Admin consent display name**: `Full MCP access`
     - **Admin consent description**: `Allows client apps to read/write to the MCP server.`
     - **State**: Enabled
   - Click **Add scope**

4. **Assign the Scope to the Client App**
   - Go to your client app (e.g. `mcp-client-app`)
   - Navigate to **API permissions**
   - Click **+ Add a permission**
   - Choose **My APIs**
   - Select your API app
   - Select the scope you created (`mcp.readwrite`)
   - Click **Add permissions**
   - Click **Grant admin consent** if required

---

### 🛠️ Step 5: Request a Token Using Client Credentials Flow

Now that your client app is registered and scoped, request a token using the **OAuth 2.0 client credentials flow**.

You can do this using `curl`, Postman, or your preferred HTTP client:

```bash
curl -X POST https://login.microsoftonline.com/<tenant-id>/oauth2/v2.0/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "client_id=<client-id>" \
  -d "client_secret=<client-secret>" \
  -d "scope=api://<api-client-id>/.default" \
  -d "grant_type=client_credentials"


