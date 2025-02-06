using Microsoft.EntityFrameworkCore;
using trackr_api.Data;
using trackr_api.Filters;
using trackr_api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

//builder.Services.AddControllers(options =>
//{
//    options.Filters.Add<BaseController>();  // Make sure this line is present
//});


builder.Services.AddDbContext<TrackrDbContext>(options =>
{
    var connectionStringName = builder.Environment.IsDevelopment() ? "LocalConnection" : "ProductionConnection";
    options.UseSqlServer(builder.Configuration.GetConnectionString(connectionStringName));
});

// Register DbContext with SQL Server connection
//builder.Services.AddDbContext<TrackrDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Adding Filters
builder.Services.AddScoped<ActionLoggingFilter>();
builder.Services.AddScoped<AuthFilter>();



builder.Services.AddScoped<PopulateData>();

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Trackr API V1");
        c.RoutePrefix = string.Empty;
    });
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        //c.SwaggerEndpoint("/swagger/v1/swagger.json", "Trackr API V1");
        //c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
