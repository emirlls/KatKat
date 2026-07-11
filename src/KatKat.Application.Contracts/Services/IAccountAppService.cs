using System.Threading.Tasks;
using KatKat.Dtos;

namespace KatKat.Services;

public interface IAccountAppService
{
    Task RegisterAsync(RegisterDto input);
}
