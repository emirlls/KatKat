using System.Threading.Tasks;
using KatKat.Dtos;
using Volo.Abp.Application.Services;

namespace KatKat.Services;

public interface IUserPreferenceAppService : IApplicationService
{
    Task<UserPreferenceDto> GetMyPreferenceAsync();

    Task<UserPreferenceDto> SetMyPreferenceAsync(SetUserPreferenceDto input);
}
