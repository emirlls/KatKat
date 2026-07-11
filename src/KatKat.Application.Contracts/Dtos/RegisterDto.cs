namespace KatKat.Dtos;

/// <summary>
/// IsManager chooses which self-registration role is granted - see KatKatRoleConsts and
/// KatKatRoleDataSeedContributor. No email/SMS verification step: this deployment has no
/// mail/SMS provider configured, so the account is active immediately upon registration.
/// </summary>
public class RegisterDto
{
    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool IsManager { get; set; }
}
