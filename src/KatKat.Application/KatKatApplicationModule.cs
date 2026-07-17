using System.Threading.Tasks;
using KatKat.BackgroundWorkers;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Application;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.FluentValidation;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;

namespace KatKat;

[DependsOn(
    typeof(KatKatDomainModule),
    typeof(KatKatApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule),
    typeof(AbpFluentValidationModule),
    typeof(AbpAspNetCoreSignalRModule),
    typeof(AbpBackgroundWorkersModule),
    typeof(AbpMultiTenancyModule),
    typeof(AbpTenantManagementDomainModule)
    )]
public class KatKatApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<KatKatApplicationModule>();
    }

    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        await context.AddBackgroundWorkerAsync<ScoreCalculationWorker>();
    }
}
