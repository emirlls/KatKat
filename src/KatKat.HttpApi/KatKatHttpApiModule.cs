using Localization.Resources.AbpUi;
using KatKat.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace KatKat;

[DependsOn(
    typeof(KatKatApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class KatKatHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(KatKatHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<KatKatResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
