using System;
using KatKat.Constants;
using Volo.Abp.Application.Dtos;

namespace KatKat.Dtos;

public class FlatMemberDto : FullAuditedEntityDto<Guid>
{
    public Guid FlatId { get; set; }

    public Guid UserId { get; set; }

    public string UserName { get; set; } = null!;

    public FlatMemberRole Role { get; set; }
}
