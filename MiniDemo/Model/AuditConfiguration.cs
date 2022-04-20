using System.Text.Json;
using System.Text.Json.Serialization;
using Audit.Core;
using MiniDemo.Model;
using Configuration = Audit.EntityFramework.Configuration;

namespace Echope.Infrastructure;

public static class AuditConfiguration
{
        /// <summary>
        /// Global Audit configuration
        /// </summary>
        public static IServiceCollection ConfigureAudit(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            
            Configuration.Setup()
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
            
            return serviceCollection;
        }

}