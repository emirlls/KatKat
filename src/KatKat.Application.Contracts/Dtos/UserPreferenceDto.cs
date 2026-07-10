using System;
using Volo.Abp.Application.Dtos;

namespace KatKat.Dtos;

public class UserPreferenceDto : EntityDto<Guid>
{
    public Guid UserId { get; set; }

    public bool ReceiveNeighborRequestNotifications { get; set; }
}
