using Volo.Abp.Modularity;

namespace KatKat;

[DependsOn(
    typeof(KatKatDomainModule),
    typeof(KatKatTestBaseModule)
)]
public class KatKatDomainTestModule : AbpModule
{

}
