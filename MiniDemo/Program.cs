using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MiniDemo.Model;
using Audit.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AppDb");
builder.Services.AddTransient<DataSeeder>();
//Add Repository Pattern
builder.Services.AddScoped<IDataRepository, DataRepository>();
builder.Services.AddDbContext<EmployeeDbContext>(x => x.UseSqlServer(connectionString));
//Add Swagger Support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Audit.EntityFramework.Configuration.Setup()
    .ForContext<EmployeeDbContext>(config => config
        .IncludeEntityObjects()
        .AuditEventType("EF:{context}"))
    .UseOptIn()
    ;

Audit.Core.Configuration.Setup()
    .UseEntityFramework(ef => ef
        .AuditTypeMapper(t => typeof(AuditTrail))
        .AuditEntityAction<AuditTrail>((ev, entry, entity) =>
        {
            var now = DateTime.UtcNow;
            
            if (entry.Entity is AuditEntity auditEntity) {
                if (entry.Action == "Insert") {
                    auditEntity.CreatedDate = now;    
                }
                auditEntity.ModifiedDate = now;
            }
                        
            entity.JsonData = entry.ToJson();
            entity.Action = entry.Action.ToString();
            entity.ModifiedDate = now;
            entity.TableName = entry.Table;
            entity.EventType = ev.EventType.ToString();
            entity.HostUserName = ev.Environment.UserName;
        })
        .IgnoreMatchedProperties()
    );
            
Audit.Core.Configuration.JsonSettings = new JsonSerializerOptions
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
};

var app = builder.Build();
app.UseSwaggerUI();

SeedData(app);

//Seed Data
void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<DataSeeder>();
        service.Seed();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger(x => x.SerializeAsV2 = true);
app.MapGet("/employee/{id}", ([FromServices] IDataRepository db, string id) =>
{
    return db.GetEmployeeById(id);
});


app.MapGet("/employees", ([FromServices] IDataRepository db) =>
    {
        return db.GetEmployees();
    }
);

app.MapPut("/employee/{id}", ([FromServices] IDataRepository db, Employee employee) =>
{
    return db.PutEmployee(employee);
});

app.MapPost("/employee", ([FromServices] IDataRepository db, Employee employee) =>
{
    return db.AddEmployee(employee);
});

app.Run();
