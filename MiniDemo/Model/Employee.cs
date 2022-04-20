using Audit.EntityFramework;

namespace MiniDemo.Model
{
    [AuditInclude]
    public class Employee : AuditEntity
    {
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string Citizenship { get; set; }
    }

}
