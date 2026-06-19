# Xero Multi-Tenant Invoice Manager

A .NET 9 ASP.NET Core Razor Pages application that authenticates with the Xero API using a **Custom Integration (Client Credentials Grant)**, allows you to dynamically choose from connected organizations, and creates or updates business invoices.

## 🚀 Features

* **Phase-Based UI Layout**: Prevents invalid actions by hiding the invoice creation form until a valid Xero Organization Connection is verified.
* **Multi-Tenant Dropdown**: Dynamically queries Xero's connection endpoint and populates an HTML dropdown list if your credentials link to multiple business profiles.
* **Secure Token Handshake**: Performs the automated backend OAuth2 handshake using Client Credentials (machine-to-machine integration).
* **Robust Form Validations**: Employs client-side and server-side model validation constraints, including a 36-character Regex match ensuring Xero Guid fields are accurate before dispatch.

---

## 🛠️ Prerequisites

1. **.NET SDK**: Ensure you have the latest .NET Core SDK installed.
2. **Xero Developer Account**: You must register an app in the [Xero Developer Portal](https://xero.com).
3. **App Type Requirement**: Because this app uses backend client credentials, you **must create a "Custom Integration" app type**, not a standard "Web App". During setup, you must manually check and assign the specific organizations your app can access.

---

## ⚙️ Project Setup & Configuration

### 1. Update Configuration
Open your local `appsettings.json` configuration file, fix any JSON structural formatting bugs, and supply your custom application values:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "XERO": {
    "EndPoint": "https://xero.com",
    "EndPointInvoice": "https://xero.comInvoices",
    "ClientID": "YOUR_XERO_CLIENT_ID",
    "ClientSecret": "YOUR_XERO_CLIENT_SECRET"
  },
  "AllowedHosts": "*"
}
```

### 2. Register Services in `Program.cs`
Ensure your core HTTP Clients and Services are correctly wired into your application's dependency container:

```csharp
using YourNamespace.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Configure the Named/Typed HttpClient Factory for Xero
builder.Services.AddHttpClient<XeroInvoiceService>(client =>
{
    var invoiceEndpoint = builder.Configuration["XERO:EndPointInvoice"];
    if (!string.IsNullOrEmpty(invoiceEndpoint))
    {
        client.BaseAddress = new Uri(invoiceEndpoint);
    }
});
```

---

## 🏁 Running the Application

Execute the following standard CLI instructions from your terminal in the directory where the `.csproj` file resides:

```bash
# Restore project dependencies
dotnet restore

# Build and start the development server
dotnet run
```

Once launched, navigate to the local hosting environment link printed in your terminal (typically `https://localhost:7001` or `http://localhost:5001`).

---

## 🧭 Step-by-Step Workflow Guide

1. **Verify Session Authorization**: On first load, the app only renders the **🔌 Fetch Connected Organizations** button. Click it to query Xero.
2. **Select Target Organization**: If authentication is successful, the app unlocks the lower section, rendering a dropdown selector displaying all your connected Xero business profiles.
3. **Provide Contact GUID**: Input a valid 36-character Xero Contact Guid (e.g., `1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d`).
4. **Submit Payload Data**: Click **📄 Create Invoice for Selection** to dispatch the strongly-typed DTO structure to Xero's core ledger endpoints. Check the feedback card for the raw API confirmation response.

---

## ⚠️ Troubleshooting Connection Issues

If your application displays the error: *"Failed to fetch any connected Xero profiles. Check your developer credentials or API status."*, check the following:

* **Mismatched Scopes**: Make sure you added `accounting.transactions` to your app configuration permissions.
* **App Type Mismatch**: Ensure your app in the Xero Portal is a **Custom Integration**. A standard *Web App* type requires user login context redirects and will fail when using this codebase.
* **Check the Console Output**: The underlying network execution services are configured with verbose exception logging. Open your terminal window or debug output stream to view the exact error codes rejected by Xero's gateway servers.
