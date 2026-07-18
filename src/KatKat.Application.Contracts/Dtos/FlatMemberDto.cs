using System;
using KatKat.Constants;
using Volo.Abp.Application.Dtos;

namespace KatKat.Dtos;

public class FlatMemberDto : FullAuditedEntityDto<Guid>
{
    public Guid FlatId { get; set; }

    public Guid UserId { get; set; }

    public string UserName { get; set; } = null!;

    /// <summary>Denormalized from IdentityUser.Name - the resident's real first name, for display alongside their username.</summary>
    public string? Name { get; set; }

    /// <summary>Denormalized from IdentityUser.Surname.</summary>
    public string? Surname { get; set; }

    public FlatMemberRole Role { get; set; }
}
