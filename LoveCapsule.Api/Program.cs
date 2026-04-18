using LoveCapsule.Api.Services;
using LoveCapsule.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
var databaseUrl = Environment.GetEnvironmentVariable("DB_CONNECTION");
if (string.IsNullOrEmpty(databaseUrl))
{
    throw new Exception("DB_CONNECTION is missing !");
}
var uri = new Uri(databaseUrl);
var userInfo = uri.UserInfo.Split(':');

var npgsqlBuilder = new NpgsqlConnectionStringBuilder
{
    Host = uri.Host,
    Port = uri.Port > 0 ? uri.Port : 5432,
    Database = uri.AbsolutePath.Trim('/'),
    Username = userInfo[0],
    Password = userInfo[1],
    SslMode = SslMode.Require,
    TrustServerCertificate = true
};

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
// Add services to the container.

builder.Services.AddControllers();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(npgsqlBuilder.ConnectionString));
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


