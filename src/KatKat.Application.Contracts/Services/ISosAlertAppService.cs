using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Dtos;
using Volo.Abp.Application.Services;

namespace KatKat.Services;

public interface ISosAlertAppService : IApplicationService
{
    /// <summary>One tap: "Güvendeyim" or "Yardım Lazım". Broadcasts to the Complex immediately.</summary>
    Task<SosAlertDto> ReportAsync(ReportSosAlertDto input);

    /// <summary>Latest status per Flat - the manager's digital floor plan.</summary>
    Task<List<SosAlertDto>> GetActiveByComplexAsync(Guid complexId);

    /// <summary>Manager-only: acknowledges help has reached a HelpNeeded alert.</summary>
    Task<SosAlertDto> ResolveAsync(Guid id);
}
