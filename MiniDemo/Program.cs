using System.Text.Json;
using System.Text.Json.Serialization;
using Audit.Core;
using Audit.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniDemo.Model;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AppDb");
builder.Services.AddTransient<DataSeeder>();
//Add Repository Pattern
builder.Services.AddScoped<IDataRepository, DataRepository>();
builder.Services.AddDbContext<EmployeeDbContext>(x => x.UseSqlServer(connectionString));
builder.Services.AddDbContext<AuditableDbContext>(x => x.UseSqlServer(connectionString));
//Add Swagger Support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

SetupAudit();

var app = builder.Build();
app.UseSwaggerUI();

if (args.Length == 1 && args[0].ToLower() == "seeddata")
    SeedData(app);

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


void SetupAudit()
{
    Audit.Core.Configuration.AddCustomAction(ActionType.OnScopeCreated, scope =>
    {
        var efEvent = scope.GetEntityFrameworkEvent();
        foreach (var entry in efEvent.Entries)
        {
            if (entry.GetEntry().Entity is not AuditEntity auditEntity)
                continue;

            var now = DateTime.UtcNow;
            if (entry.Action == "Insert")
            {
                auditEntity.CreatedDate = now;
            }
            else
            {
                entry.GetEntry().Property("CreatedDate").IsModified = false;
                auditEntity.ModifiedDate = now;   
            }
        }
    });

    Audit.EntityFramework.Configuration.Setup()
        .ForContext<AuditableDbContext>(config => config
            .AuditEventType("EF:{context}")
        )
        .UseOptIn()
        ;

    Audit.Core.Configuration.Setup()
        .UseEntityFramework(ef => ef
            .UseDbContext<AuditableDbContext>()
            .AuditTypeMapper(t => typeof(AuditTrail))
            .AuditEntityAction<AuditTrail>((ev, entry, entity) =>
            {
                entity.JsonData = entry.ToJson();
                entity.Action = entry.Action.ToString();
                entity.ModifiedDate = DateTime.UtcNow;
                entity.TableName = entry.Table;
                entity.EventType = ev.EventType.ToString();
                entity.HostUserName = ev.Environment.UserName;
                
                var efe = ev.GetEntityFrameworkEvent();

                return efe.Success;
            })
            .IgnoreMatchedProperties()
        );

}

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