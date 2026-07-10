using System.Threading.Tasks;
using KatKat.DomainServices;
using KatKat.Dtos;
using KatKat.Entities;
using Volo.Abp.Users;

namespace KatKat.Services;

public class UserPreferenceAppService : KatKatAppService, IUserPreferenceAppService
{
    private readonly UserPreferenceManager _userPreferenceManager;

    public UserPreferenceAppService(UserPreferenceManager userPreferenceManager)
    {
        _userPreferenceManager = userPreferenceManager;
    }

    public async Task<UserPreferenceDto> GetMyPreferenceAsync()
    {
        var preference = await _userPreferenceManager.GetOrCreateAsync(CurrentUser.GetId());
        return ObjectMapper.Map<UserPreference, UserPreferenceDto>(preference);
    }

    public async Task<UserPreferenceDto> SetMyPreferenceAsync(SetUserPreferenceDto input)
    {
        var preference = await _userPreferenceManager.SetNeighborRequestNotificationsAsync(
            CurrentUser.GetId(), input.ReceiveNeighborRequestNotifications);

        return ObjectMapper.Map<UserPreference, UserPreferenceDto>(preference);
    }
}
