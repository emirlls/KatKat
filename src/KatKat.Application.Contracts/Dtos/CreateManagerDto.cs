namespace KatKat.Dtos;

/// <summary>
/// Admin-only: provisions a brand new Manager account, each scoped to its own new Tenant (one
/// site's worth of data, fully isolated from every other Manager/Resident's). No email/SMS
/// verification step - this deployment has no mail/SMS provider configured, so the account is
/// active immediately upon creation.
/// </summary>
public class CreateManagerDto
{
    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}
