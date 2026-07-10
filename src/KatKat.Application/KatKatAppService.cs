using KatKat.Localization;
using Volo.Abp.Application.Services;

namespace KatKat;

public abstract class KatKatAppService : ApplicationService
{
    protected KatKatAppService()
    {
        LocalizationResource = typeof(KatKatResource);
        ObjectMapperContext = typeof(KatKatApplicationModule);
    }
}
