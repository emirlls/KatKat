using System;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Repositories;
using Microsoft.AspNetCore.SignalR;
using Volo.Abp.DependencyInjection;

namespace KatKat.Hubs;

/// <summary>
/// Pushes complex-wide real-time events while honoring each resident's notification opt-out
/// (UserPreference.ReceiveNeighborRequestNotifications) - official notices bypass this, but
/// ad-hoc P2P request pushes never go to someone who muted them.
/// </summary>
public class KatKatHubNotifier : ITransientDependency
{
    private readonly IHubContext<KatKatHub> _hubContext;
    private readonly IFlatMemberRepository _flatMemberRepository;
    private readonly IUserPreferenceRepository _userPreferenceRepository;

    public KatKatHubNotifier(
        IHubContext<KatKatHub> hubContext,
        IFlatMemberRepository flatMemberRepository,
        IUserPreferenceRepository userPreferenceRepository)
    {
        _hubContext = hubContext;
        _flatMemberRepository = flatMemberRepository;
        _userPreferenceRepository = userPreferenceRepository;
    }

    public async Task NotifyP2PRequestEventAsync(Guid complexId, string eventName, object payload)
    {
        var userIds = await _flatMemberRepository.GetUserIdsByComplexAsync(complexId);
        if (userIds.Count == 0)
        {
            return;
        }

        var preferences = await _userPreferenceRepository.GetListByUserIdsAsync(userIds);
        var optedOutUserIds = preferences
            .Where(p => !p.ReceiveNeighborRequestNotifications)
            .Select(p => p.UserId)
            .ToHashSet();

        var recipientIds = userIds
            .Where(id => !optedOutUserIds.Contains(id))
            .Select(id => id.ToString())
            .ToList();

        if (recipientIds.Count > 0)
        {
            await _hubContext.Clients.Users(recipientIds).SendAsync(eventName, payload);
        }
    }
}
