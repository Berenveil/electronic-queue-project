using App.Data;   
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Professions.Any())
    {
        var sql = File.ReadAllText("Data/Seed_all.sql"); 
        db.Database.ExecuteSqlRaw(sql);
    }
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/health", () => new { status = "ok" });

app.Run();