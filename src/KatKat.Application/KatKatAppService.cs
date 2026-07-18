using System;
using System.Threading.Tasks;
using KatKat.DomainServices;
using KatKat.Localization;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Services;

namespace KatKat;

public abstract class KatKatAppService : ApplicationService
{
    protected KatKatAppService()
    {
        LocalizationResource = typeof(KatKatResource);
        ObjectMapperContext = typeof(KatKatApplicationModule);
    }

    protected ScoreManager ScoreManager => LazyServiceProvider.LazyGetRequiredService<ScoreManager>();

    /// <summary>
    /// Recalculates a Complex's KatKat Score right now, swallowing any failure into a warning log -
    /// a scoring hiccup must never block the primary action (fulfilling a request, resolving an
    /// issue, paying a share) that triggered it. Call this after any action that affects the
    /// Financial/Social/Resolution sub-scores, so the resident/manager sees the effect immediately
    /// instead of waiting for the once-daily ScoreCalculationWorker.
    /// </summary>
    protected async Task RecalculateScoreSafelyAsync(Guid complexId)
    {
        try
        {
            await ScoreManager.RecalculateByComplexIdAsync(complexId);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to recalculate the KatKat Score for Complex {ComplexId}", complexId);
        }
    }
}
