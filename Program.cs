using XeroDemo.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register HttpClient factory and bind the Xero Invoice Service
builder.Services.AddHttpClient<XeroInvoiceService>(client =>
{
    // Fetches the 'EndPointInvoice' URL string from appsettings.json
    var invoiceEndpoint = builder.Configuration["XERO:EndPointInvoice"];

    if (!string.IsNullOrEmpty(invoiceEndpoint))
    {
        client.BaseAddress = new Uri(invoiceEndpoint);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
