using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Dtos;
using Volo.Abp.Application.Services;

namespace KatKat.Services;

public interface IFlatMemberAppService : IApplicationService
{
    Task<FlatMemberDto> GetAsync(Guid id);

    Task<List<FlatMemberDto>> GetListByFlatAsync(Guid flatId);

    /// <summary>
    /// Attaches the current user to a flat as an UnverifiedResident.
    /// </summary>
    Task<FlatMemberDto> InviteAsync(InviteFlatMemberDto input);

    /// <summary>
    /// Manager-only: verifies an UnverifiedResident, moving them to Resident.
    /// </summary>
    Task<FlatMemberDto> ApproveAsync(Guid id);

    /// <summary>
    /// Manager-only: promotes an already-verified member to Manager.
    /// </summary>
    Task<FlatMemberDto> PromoteToManagerAsync(Guid id);
}
