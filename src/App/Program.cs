using App.Data;   
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

var connectionString = new SqliteConnectionStringBuilder(
    builder.Configuration.GetConnectionString("DefaultConnection"));
if (!Path.IsPathRooted(connectionString.DataSource))
{
    connectionString.DataSource = Path.Combine(builder.Environment.ContentRootPath, connectionString.DataSource);
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString.ToString()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Professions.Any())
    {
        var sql = File.ReadAllText(Path.Combine(app.Environment.ContentRootPath, "Data", "Seed_all.sql"));
        using var transaction = db.Database.BeginTransaction();
        db.Database.ExecuteSqlRaw(sql);
        transaction.Commit();
    }
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/health", () => new { status = "ok" });

app.MapGet("/api/doctors", async (AppDbContext db) =>
{
    var doctors = await db.Employees
        .AsNoTracking()
        .OrderBy(e => e.LastName)
        .ThenBy(e => e.FirstName)
        .Select(e => new
        {
            id = e.Id,
            lastName = e.LastName,
            firstName = e.FirstName,
            middleName = e.MiddleName,
            profession = e.Profession.Name,
            services = e.EmployeeServices.Select(es => new
            {
                id = es.Service.Id,
                name = es.Service.Name,
                price = es.Service.Price,
                durationMin = es.Service.DurationMin
            }).ToList()
        })
        .ToListAsync();

    return Results.Ok(doctors);
});

app.Run();
