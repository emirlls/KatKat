using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace KatKat.Complexes;

public interface IComplexRepository : IRepository<Complex, Guid>
{
    Task<bool> NameExistsAsync(string name, Guid? excludedId = null);
}
