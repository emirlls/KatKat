using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;

namespace KatKat.Hubs;

/// <summary>
/// Real-time channel for a Complex: clients join their complex's group on connect and receive
/// P2P request, resource reservation, SOS and issue-resolution pushes.
/// </summary>
public class KatKatHub : AbpHub
{
    public async Task JoinComplexGroupAsync(Guid complexId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(complexId));
    }

    public async Task LeaveComplexGroupAsync(Guid complexId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(complexId));
    }

    public static string GroupName(Guid complexId) => $"{KatKatHubConsts.ComplexGroupPrefix}{complexId}";
}
