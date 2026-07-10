using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace KatKat;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(KatKatHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class KatKatConsoleApiClientModule : AbpModule
{

}
