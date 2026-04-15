using BoutiqueEducation.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BoutiqueEducation.DataAccess.Interfaces;

public interface IAppDbContext
{
    DbSet<AppUser> Users { get; set; }
    DbSet<AppRole> Roles { get; set; }
    DbSet<Question> Questions { get; set; }
    DbSet<TaskItem> Tasks { get; set; }
    DbSet<Message> Messages { get; set; }
    DbSet<AuditLog> AuditLogs { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
