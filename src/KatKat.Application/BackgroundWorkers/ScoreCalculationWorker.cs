using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.DomainServices;
using KatKat.Entities;
using KatKat.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace KatKat.BackgroundWorkers;

/// <summary>
/// Recalculates every Complex's KatKat Score (Financial/Social/Resolution) once a day around
/// 03:00 local time. Checks every 30 minutes rather than sleeping until 03:00 directly, since
/// AsyncPeriodicBackgroundWorkerBase only supports a fixed repeating period - this is the
/// standard ABP pattern for "run once a day at roughly a given hour" without a full cron-style
/// scheduler.
/// </summary>
public class ScoreCalculationWorker : AsyncPeriodicBackgroundWorkerBase
{
    private const int TargetHour = 3;
    private const int CheckIntervalMinutes = 30;
    private const int TrailingWindowDays = 30;

    private DateTime? _lastRunDate;

    public ScoreCalculationWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory)
        : base(timer, serviceScopeFactory)
    {
        Timer.Period = (int)TimeSpan.FromMinutes(CheckIntervalMinutes).TotalMilliseconds;
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        var now = DateTime.Now;
        if (now.Hour != TargetHour || _lastRunDate?.Date == now.Date)
        {
            return;
        }

        _lastRunDate = now;

        var complexRepository = workerContext.ServiceProvider.GetRequiredService<IComplexRepository>();
        var dataFilter = workerContext.ServiceProvider.GetRequiredService<IDataFilter>();
        var currentTenant = workerContext.ServiceProvider.GetRequiredService<ICurrentTenant>();
        var unitOfWorkManager = workerContext.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

        List<Complex> complexes;
        using (dataFilter.Disable<IMultiTenant>())
        {
            complexes = await complexRepository.GetListAsync();
        }

        var since = DateTime.UtcNow.AddDays(-TrailingWindowDays);

        foreach (var complex in complexes)
        {
            using var tenantScope = currentTenant.Change(complex.TenantId);
            using var uow = unitOfWorkManager.Begin();

            var scoreManager = workerContext.ServiceProvider.GetRequiredService<ScoreManager>();
            var financialScore = await scoreManager.CalculateFinancialScoreAsync(complex.Id, since);
            var socialScore = await scoreManager.CalculateSocialScoreAsync(complex.Id, since);
            var resolutionScore = await scoreManager.CalculateResolutionScoreAsync(complex.Id, since);

            await scoreManager.UpsertAsync(
                complex.Id,
                complex.TenantId!.Value,
                complex.Name,
                complex.City,
                complex.District,
                complex.Latitude,
                complex.Longitude,
                financialScore,
                socialScore,
                resolutionScore
            );

            await uow.CompleteAsync();
        }
    }
}
