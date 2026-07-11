using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Entities;
using KatKat.EntityFrameworkCore;
using KatKat.Repositories;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace KatKat.EntityFrameworkCore.Repositories;

public class EfCoreFlatRepository : KatKatEfCoreRepository<Flat, Guid>, IFlatRepository
{
    public EfCoreFlatRepository(IDbContextProvider<KatKatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<bool> FlatNumberExistsInBuildingAsync(Guid buildingId, string flatNumber, Guid? excludedId = null)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(x =>
            x.BuildingId == buildingId &&
            x.FlatNumber == flatNumber &&
            (excludedId == null || x.Id != excludedId));
    }

    public async Task<List<Flat>> GetListByBuildingAsync(Guid buildingId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(x => x.BuildingId == buildingId).ToListAsync();
    }

    public async Task<List<Flat>> GetListByComplexAsync(Guid complexId)
    {
        var dbContext = await GetDbContextAsync();
        return await (
            from flat in dbContext.Flats
            join building in dbContext.Buildings on flat.BuildingId equals building.Id
            where building.ComplexId == complexId
            select flat
        ).ToListAsync();
    }

    public async Task<List<Flat>> GetListByUserAndComplexAsync(Guid userId, Guid complexId)
    {
        var dbContext = await GetDbContextAsync();
        return await (
            from flatMember in dbContext.FlatMembers
            join flat in dbContext.Flats on flatMember.FlatId equals flat.Id
            join building in dbContext.Buildings on flat.BuildingId equals building.Id
            where flatMember.UserId == userId && building.ComplexId == complexId
            select flat
        ).Distinct().ToListAsync();
    }
}
