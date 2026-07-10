using Volo.Abp.Modularity;

namespace KatKat;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class KatKatApplicationTestBase<TStartupModule> : KatKatTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
