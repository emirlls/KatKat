using System;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.Timing;

namespace KatKat;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(KatKatDomainSharedModule)
)]
public class KatKatDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // PostgreSQL's "timestamp with time zone" columns reject anything but Kind=Utc; ABP's
        // Clock defaults to Kind=Unspecified, which Npgsql then treats as local time and rejects.
        Configure<AbpClockOptions>(options =>
        {
            options.Kind = DateTimeKind.Utc;
        });
    }
}
