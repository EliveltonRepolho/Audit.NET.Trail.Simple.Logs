using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Audit.EntityFramework;

namespace MiniDemo.Model;

[AuditIgnore]
public class AuditTrail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }
    public string? EventType { get; set; }
    public string? TableName { get; set; }
    public string? Action { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? HostUserName { get; set; }
    public string? JsonData { get; set; }
}