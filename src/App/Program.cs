using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
builder.Services.AddDbContext<DbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DbContext>();
    db.Database.Migrate();
}
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/health", () => new { status = "ok" });

app.Run();