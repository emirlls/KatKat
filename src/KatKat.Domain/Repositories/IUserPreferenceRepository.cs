using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Entities;

namespace KatKat.Repositories;

public interface IUserPreferenceRepository : IKatKatRepository<UserPreference, Guid>
{
    Task<UserPreference?> FindByUserIdAsync(Guid userId);

    Task<List<UserPreference>> GetListByUserIdsAsync(IEnumerable<Guid> userIds);
}
