using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

namespace KatKat;

[DependsOn(
    typeof(KatKatDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class KatKatApplicationContractsModule : AbpModule
{

}
