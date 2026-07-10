using Volo.Abp.Modularity;

namespace KatKat;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class KatKatDomainTestBase<TStartupModule> : KatKatTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
