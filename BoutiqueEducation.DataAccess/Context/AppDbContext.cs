using BoutiqueEducation.DataAccess.Interfaces;
using BoutiqueEducation.Entity.Entities;
using BoutiqueEducation.Entity.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BoutiqueEducation.DataAccess.Context;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>, IAppDbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Question> Questions { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        Guid? userId = null;
        var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdStr, out Guid parsedUserId))
        {
            userId = parsedUserId;
        }

        foreach (var entry in ChangeTracker.Entries())
        {
            // Ekleme İşlemleri
            if (entry.State == EntityState.Added && entry.Entity is ICreatableEntity creatable)
            {
                creatable.CreatedDate = DateTime.Now; // Yerel saat dilimi düzeltmesi
                creatable.CreatedById = userId;
            }

            // Güncelleme İşlemleri
            if (entry.State == EntityState.Modified && entry.Entity is IUpdatableEntity updatable)
            {
                // Soft delete esnasında UpdateDate'in tetiklenmesini engelleyebiliriz veya bırakabiliriz. Genelde update olarak geçerlidir.
                // Biz sadece SoftDelete'in ana silme esnasında Modified olarak override edildiğinin farkındayız.
                updatable.UpdatedDate = DateTime.Now;
                updatable.ModifiedById = userId;
            }

            // Silme (Soft Delete) İşlemleri
            if (entry.State == EntityState.Deleted && entry.Entity is ISoftDeletableEntity deletable)
            {
                entry.State = EntityState.Modified;
                deletable.IsDeleted = true;
                deletable.DeletedDate = DateTime.Now;
                deletable.DeletedById = userId;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
