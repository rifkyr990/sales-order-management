var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Registrasi Client ke Backend Services
builder.Services.AddHttpClient("CustomerService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/");
});

builder.Services.AddHttpClient("SalesOrderService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5002/");
});

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Order}/{action=Index}/{id?}");

app.Run();