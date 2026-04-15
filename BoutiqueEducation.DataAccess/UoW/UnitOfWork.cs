using BoutiqueEducation.DataAccess.Context;
using BoutiqueEducation.DataAccess.Interfaces;

namespace BoutiqueEducation.DataAccess.UoW;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
