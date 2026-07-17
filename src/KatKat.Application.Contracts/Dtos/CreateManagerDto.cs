namespace KatKat.Dtos;

/// <summary>
/// Admin-only: provisions a brand new Manager account together with their site, each scoped to
/// its own new Tenant (one site's worth of data, fully isolated from every other
/// Manager/Resident's). A Manager never creates their own site - only the admin does, at the
/// same time the Manager account itself is created. No email/SMS verification step - this
/// deployment has no mail/SMS provider configured, so the account is active immediately upon
/// creation.
/// </summary>
public class CreateManagerDto
{
    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    /// <summary>
    /// KVKK note: collected only for the platform admin to reach this Manager, never returned by
    /// any endpoint a Manager/Resident can call - only the admin-only manager directory
    /// (<see cref="IAccountAppService.GetManagersAsync"/>) surfaces it.
    /// </summary>
    public string PhoneNumber { get; set; } = null!;

    public string Password { get; set; } = null!;

    /// <summary>The site created for this Manager, inside their new Tenant.</summary>
    public CreateComplexDto Site { get; set; } = null!;
}
