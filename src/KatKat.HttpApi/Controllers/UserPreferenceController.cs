using System.Threading.Tasks;
using KatKat.Dtos;
using KatKat.Services;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace KatKat.Controllers;

/// <summary>
/// The current user's own notification opt-in/opt-out preferences.
/// </summary>
[Area(KatKatRemoteServiceConsts.ModuleName)]
[RemoteService(Name = KatKatRemoteServiceConsts.RemoteServiceName)]
[Route(KatKatRemoteServiceConsts.RoutePathPrefix + "/my-preference")]
public class UserPreferenceController : KatKatController, IUserPreferenceAppService
{
    private readonly IUserPreferenceAppService _userPreferenceAppService;

    public UserPreferenceController(IUserPreferenceAppService userPreferenceAppService)
    {
        _userPreferenceAppService = userPreferenceAppService;
    }

    /// <summary>Gets the current user's notification preference (created with defaults on first read).</summary>
    [HttpGet]
    public Task<UserPreferenceDto> GetMyPreferenceAsync() => _userPreferenceAppService.GetMyPreferenceAsync();

    /// <summary>Sets whether the current user wants to receive neighbor P2P request notifications.</summary>
    [HttpPut]
    public Task<UserPreferenceDto> SetMyPreferenceAsync(SetUserPreferenceDto input) =>
        _userPreferenceAppService.SetMyPreferenceAsync(input);
}
