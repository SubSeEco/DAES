using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using DAES.API.BackOffice.Modelos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure DbContext
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SistemaIntegrado")));



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API Name", Version = "v1" });
});

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
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API Name V1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
