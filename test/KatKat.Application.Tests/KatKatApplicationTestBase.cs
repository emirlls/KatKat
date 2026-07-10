using Volo.Abp.Modularity;

namespace KatKat;

/* Inherit from this class for your application layer tests. */
public abstract class KatKatApplicationTestBase<TStartupModule> : KatKatTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
