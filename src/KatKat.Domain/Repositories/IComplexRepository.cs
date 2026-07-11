using System;
using System.Threading.Tasks;
using KatKat.Entities;

namespace KatKat.Repositories;

public interface IComplexRepository : IKatKatRepository<Complex, Guid>
{
    Task<bool> NameExistsAsync(string name, Guid? excludedId = null);

    Task<bool> ExistsForNeighborhoodAsync(int neighborhoodId);
}
