using Volo.Abp.Modularity;

namespace KatKat;

[DependsOn(
    typeof(KatKatApplicationModule),
    typeof(KatKatDomainTestModule)
    )]
public class KatKatApplicationTestModule : AbpModule
{

}
