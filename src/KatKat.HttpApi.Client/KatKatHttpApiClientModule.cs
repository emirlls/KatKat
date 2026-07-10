using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace KatKat;

[DependsOn(
    typeof(KatKatApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class KatKatHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(KatKatApplicationContractsModule).Assembly,
            KatKatRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<KatKatHttpApiClientModule>();
        });

    }
}
