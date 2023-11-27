using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string ProcesoSchema = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "proceso.schema.json"));
string OrganizacionSchema = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/organizacion.schema.json");
builder.Services.AddMemoryCache();
var app = builder.Build();
var cache = app.Services.GetRequiredService<IMemoryCache>();
cache.Set("Proceso", ProcesoSchema);
cache.Set("Organizacion", ProcesoSchema);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
