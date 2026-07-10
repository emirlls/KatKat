using KatKat.Buildings;
using KatKat.Complexes;
using KatKat.EntityFrameworkCore.Buildings;
using KatKat.EntityFrameworkCore.Complexes;
using KatKat.EntityFrameworkCore.FlatMembers;
using KatKat.EntityFrameworkCore.Flats;
using KatKat.FlatMembers;
using KatKat.Flats;
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
        context.Services.AddAbpDbContext<KatKatDbContext>(options =>
        {
            options.AddRepository<Complex, EfCoreComplexRepository>();
            options.AddRepository<Building, EfCoreBuildingRepository>();
            options.AddRepository<Flat, EfCoreFlatRepository>();
            options.AddRepository<FlatMember, EfCoreFlatMemberRepository>();
        });
    }
}
