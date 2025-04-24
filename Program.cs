using TSF_mustidisProj.Services;
using TSF_mustidisProj.Data;
using Microsoft.EntityFrameworkCore;
using TSF_mustidisProj.Models;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);
Env.Load();

var connectionString = builder.Configuration.GetConnectionString("Default");
//test connection
Console.WriteLine("======================================================");
if (connectionString != null){
    bool isConnected = DatabaseConnectionTest.TestConnection(connectionString);

    if (!isConnected)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("ERROR: Could not connect to MySQL database. Application will not function correctly.");
        Console.ResetColor();
        // Optionally, you could exit here if connection is critical
        // Environment.Exit(1);
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("MySQL connection test successful!");
        Console.ResetColor();
    }
}
else{
    Console.WriteLine("NULL CONNECTION STRING");
}

Console.WriteLine("======================================================");


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<AdafruitService>(); // Register the servic

var app = builder.Build();

// Configure the HTTP request pipeline. 
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        DataSeeder.SeedData(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
