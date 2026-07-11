using System.Threading.Tasks;
using KatKat.Entities;

namespace KatKat.Repositories;

public interface ICityRepository : IKatKatRepository<City, int>
{
    Task<bool> NameExistsAsync(string name, int? excludedId = null);
}
