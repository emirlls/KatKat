using System;
using Volo.Abp.Application.Dtos;

namespace KatKat.Dtos;

public class ResidentInvitationDto : FullAuditedEntityDto<Guid>
{
    public Guid FlatId { get; set; }

    public string Code { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime? RedeemedAt { get; set; }
}
