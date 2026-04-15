using BoutiqueEducation.Entity.Interfaces;

namespace BoutiqueEducation.Entity.Entities;

public sealed class AuditLog : ICreatableEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string HttpMethod { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string? QueryString { get; set; }
    public string? RequestBody { get; set; } // Body içerisindeki DTO Jsonu (CURL)
    public int StatusCode { get; set; }
    public string? IpAddress { get; set; }
    public long ExecutionDurationMs { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public Guid? CreatedById { get; set; }
}
