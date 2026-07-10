using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Entities;

namespace KatKat.Repositories;

public interface IResourceRepository : IKatKatRepository<Resource, Guid>
{
    Task<List<Resource>> GetListByComplexAsync(Guid complexId);
}
