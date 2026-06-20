using XeroDemo.Interfaces;
using XeroDemo.Models;
using XeroDemo.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


builder.Services.Configure<XeroConfigurationDto>(builder.Configuration.GetSection("Xero"));

builder.Services.AddHttpClient<IXeroTokenService, XeroTokenService>();
builder.Services.AddHttpClient<IXeroContactService, XeroContactService>();
builder.Services.AddHttpClient<IXeroInvoiceService, XeroInvoiceService>();

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
