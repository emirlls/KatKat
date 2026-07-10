using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace KatKat;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(KatKatDomainSharedModule)
)]
public class KatKatDomainModule : AbpModule
{

}
