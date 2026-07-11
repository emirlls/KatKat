using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Dtos;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Guids;
using Volo.Abp.Identity;

namespace KatKat.Services;

public class AccountAppService : KatKatAppService, IAccountAppService
{
    private readonly IdentityUserManager _userManager;
    private readonly IGuidGenerator _guidGenerator;

    public AccountAppService(IdentityUserManager userManager, IGuidGenerator guidGenerator)
    {
        _userManager = userManager;
        _guidGenerator = guidGenerator;
    }

    public async Task RegisterAsync(RegisterDto input)
    {
        var user = new Volo.Abp.Identity.IdentityUser(_guidGenerator.Create(), input.UserName, input.Email);

        (await _userManager.CreateAsync(user, input.Password)).CheckErrors();

        var roleName = input.IsManager ? KatKatRoleConsts.ManagerRoleName : KatKatRoleConsts.ResidentRoleName;
        (await _userManager.AddToRoleAsync(user, roleName)).CheckErrors();
    }
}
