# 📦 Container Booking POC with Agent Orchestration and MCP Server

This repository presents a proof of concept (PoC) for orchestrating a multi-step **container booking workflow** using the **Model Context Protocol (MCP)**, built with **.NET** and [Semantic Kernel](https://github.com/microsoft/semantic-kernel).

## 🧭 Workflow Overview

The container booking process is broken into three distinct steps, each handled by a specialized agent:

1. **Find Vessel** – Searches available vessels based on user input  
2. **Check Capacity** – Checks space availability on the selected vessel  
3. **Complete Booking** – Finalizes the booking process with validated details  

Each agent connects to a separate **MCP server**, enabling decentralized knowledge and function isolation. An **orchestrator** interprets user intent using natural language and invokes the appropriate agent accordingly.

## 🔐 Security

All MCP servers are secured using **Microsoft Entra ID (formerly Azure Active Directory)**.  
- **Authentication**: Agents authenticate using OAuth2 client credentials  
- **Authorization**: Role-based access control (RBAC) ensures agents only access permitted scopes  

## 🧠 Tech Stack

- **C# (.NET)** – Core implementation of agents, orchestration logic, and MCP clients  
- **Azure OpenAI Service** – For natural language understanding and orchestration logic  
- **GPT-4** – Model used via Semantic Kernel for interpreting user prompts and selecting agents  
- **Semantic Kernel** – Enables intelligent planning and multi-agent coordination  
- **Microsoft Entra ID (Azure AD)** – Secure access and identity management for MCP servers  



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


