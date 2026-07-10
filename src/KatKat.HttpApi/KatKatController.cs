using KatKat.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace KatKat;

public abstract class KatKatController : AbpControllerBase
{
    protected KatKatController()
    {
        LocalizationResource = typeof(KatKatResource);
    }
}
