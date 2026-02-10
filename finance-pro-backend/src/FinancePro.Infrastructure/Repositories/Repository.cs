using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using FinancePro.Domain.Entities;
using FinancePro.Domain.Interfaces;
using FinancePro.Infrastructure.Data;

namespace FinancePro.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly FinanceProDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(FinanceProDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _dbSet.FindAsync(new object[] { id }, ct);

    public async Task<IReadOnlyList<T>> GetAllAsync(int organizationId, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();
        var orgProp = typeof(T).GetProperty("OrganizationId");
        if (orgProp != null)
        {
            var param = Expression.Parameter(typeof(T), "e");
            var prop = Expression.Property(param, "OrganizationId");
            var value = Expression.Constant(organizationId);
            var equals = Expression.Equal(prop, value);
            var lambda = Expression.Lambda<Func<T, bool>>(equals, param);
            query = query.Where(lambda);
        }
        return await query.OrderByDescending(e => e.CreatedAt).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _dbSet.Where(predicate).ToListAsync(ct);

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        entity.IsDeleted = true;
        await UpdateAsync(entity, ct);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _dbSet.CountAsync(predicate, ct);

    public IQueryable<T> Query() => _dbSet.AsQueryable();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly FinanceProDbContext _context;

    public UnitOfWork(FinanceProDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);

    public void Dispose() => _context.Dispose();
}
