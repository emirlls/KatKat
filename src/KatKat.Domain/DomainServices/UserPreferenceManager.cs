using System;
using System.Threading.Tasks;
using KatKat.Entities;
using KatKat.Repositories;
using Volo.Abp.Domain.Services;

namespace KatKat.DomainServices;

public class UserPreferenceManager : DomainService
{
    private readonly IUserPreferenceRepository _userPreferenceRepository;

    public UserPreferenceManager(IUserPreferenceRepository userPreferenceRepository)
    {
        _userPreferenceRepository = userPreferenceRepository;
    }

    public virtual async Task<UserPreference> GetOrCreateAsync(Guid userId)
    {
        var preference = await _userPreferenceRepository.FindByUserIdAsync(userId);
        if (preference != null)
        {
            return preference;
        }

        preference = new UserPreference(GuidGenerator.Create(), CurrentTenant.Id, userId);
        return await _userPreferenceRepository.InsertAsync(preference, autoSave: true);
    }

    public virtual async Task<UserPreference> SetNeighborRequestNotificationsAsync(Guid userId, bool enabled)
    {
        var preference = await GetOrCreateAsync(userId);
        preference.SetNeighborRequestNotifications(enabled);
        return await _userPreferenceRepository.UpdateAsync(preference);
    }
}
