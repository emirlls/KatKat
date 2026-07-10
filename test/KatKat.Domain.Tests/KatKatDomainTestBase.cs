using Volo.Abp.Modularity;

namespace KatKat;

/* Inherit from this class for your domain layer tests. */
public abstract class KatKatDomainTestBase<TStartupModule> : KatKatTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
