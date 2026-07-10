using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace KatKat.EntityFrameworkCore;

[DependsOn(
    typeof(KatKatDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class KatKatEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Custom repositories (EfCoreComplexRepository etc.) are NOT registered here:
        // ABP's conventional registrar auto-exposes any class ending in "Repository" that
        // implements an IRepository-derived interface for every such interface it implements,
        // as long as it lives in a module-loaded assembly - which this project's is.
        context.Services.AddAbpDbContext<KatKatDbContext>();
    }
}
