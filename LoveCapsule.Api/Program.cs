using LoveCapsule.Api.Services;
using LoveCapsule.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<PdfService>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});
var app = builder.Build();
app.UseCors();


app.UseSwagger();
app.UseSwaggerUI();



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
app.Run("http://0.0.0.0:80");


