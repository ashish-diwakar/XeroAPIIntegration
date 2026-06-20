# Xero Custom Connection Invoice Demo

A .NET 9 ASP.NET Core Razor Pages application that connects to Xero using a **Custom Connection (Client Credentials Grant)**, retrieves contacts from Xero, displays them in a dropdown list, and creates a sample invoice for the selected contact.

## Features

* Authentication using Xero Custom Connection (OAuth 2.0 Client Credentials Grant)
* Automatic access token retrieval and caching
* Retrieve contacts from Xero
* Display contacts in a dropdown list
* Select a contact and create a demo invoice
* Strongly typed DTO models
* ASP.NET Core Razor Pages UI
* Dependency Injection using typed HttpClient services
* Structured logging and error handling

---

## Prerequisites

### 1. .NET SDK

Install .NET 9 SDK or later.

### 2. Xero Developer Account

Create a Xero developer account and configure a Custom Connection application.

### 3. Xero App Permissions

The Custom Connection should have the required scopes, for example:

* accounting.contacts
* accounting.contacts.read
* accounting.invoices
* accounting.invoices.read
* accounting.settings
* accounting.settings.read
* projects
* projects.read

---

## Configuration

Update `appsettings.json`:

```json
{
  "XERO": {
    "TokenServiceEndPoint": "https://identity.xero.com/connect/token",
    "ContactServiceEndPoint": "https://api.xero.com/api.xro/2.0/Contacts",
    "InvoiceServiceEndPoint": "https://api.xero.com/api.xro/2.0/Invoices",
    "ClientID": "ClientID here",
    "ClientSecret": "ClientSecret here"
  },
}
```

---

## Dependency Injection Configuration

Register services in `Program.cs`:

```csharp
builder.Services.Configure<XeroConfigurationDto>(
    builder.Configuration.GetSection("Xero"));

builder.Services.AddHttpClient<IXeroTokenService, XeroTokenService>();

builder.Services.AddHttpClient<IXeroContactService, XeroContactService>();

builder.Services.AddHttpClient<IXeroInvoiceService, XeroInvoiceService>();

builder.Services.AddRazorPages();
```

---

## Running the Application

Restore packages:

```bash
dotnet restore
```

Build the application:

```bash
dotnet build
```

Run the application:

```bash
dotnet run
```

Open the application in your browser using the URL displayed by ASP.NET Core.

---

## Application Workflow

### Step 1 - Fetch Contacts

Click:

```text
Fetch Contacts
```

The application:

1. Requests an access token from Xero.
2. Calls the Xero Contacts API.
3. Loads all available contacts.
4. Populates the dropdown list.

### Step 2 - Select Contact

Choose a contact from the dropdown list.

The selected contact will be used when creating the invoice.

### Step 3 - Create Demo Invoice

Click:

```text
Create Invoice
```

The application creates a sample Accounts Receivable invoice using:

* Selected Contact
* Demo line item
* Current invoice date
* Due date (+15 days)
* Status = AUTHORISED

### Step 4 - Review Response

The raw Xero API response is displayed on the page.

---

## Architecture

### Services

#### XeroTokenService

Responsible for:

* Retrieving access tokens
* Caching tokens until expiration
* Authenticating using Client Credentials Grant

#### XeroContactService

Responsible for:

* Fetching contacts from Xero
* Returning contact data for dropdown binding

#### XeroInvoiceService

Responsible for:

* Creating invoices
* Sending invoice payloads to Xero

---

## Important Notes

### Custom Connection

This application uses a Xero Custom Connection.

Because of this:

* No user login is required
* No authorization code flow is required
* No refresh tokens are required
* No tenant selection is required
* No xero-tenant-id header is required

### Contacts Instead of Tenants

Earlier versions of the project referenced tenants and organization selection.

The current implementation retrieves contacts from Xero and displays them in a dropdown. The selected contact is used when creating the demo invoice.

---

## Troubleshooting

### Unable to Obtain Access Token

Verify:

* Client ID is correct
* Client Secret is correct
* Custom Connection is active
* Required scopes have been granted

### No Contacts Returned

Verify:

* Contacts exist in the connected Xero organization
* The application has `accounting.contacts.read` permission

### Invoice Creation Fails

Verify:

* Selected contact exists
* Account code exists in Xero
* Invoice payload is valid
* The application has `accounting.invoices` permission

Check application logs for detailed Xero API error messages.

---

## Disclaimer

This project creates demonstration invoices for testing purposes. Review and adapt the invoice payload structure before using in a production environment.
