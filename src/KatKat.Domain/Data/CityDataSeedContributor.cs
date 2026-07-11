using System.Threading.Tasks;
using KatKat.DomainServices;
using KatKat.Repositories;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace KatKat.Data;

/// <summary>
/// Seeds Turkey's 81 provinces into the City lookup table on first run. Idempotent - a
/// non-empty table means a previous run (or another instance racing at startup) already seeded
/// it, so this is a safe no-op on every subsequent application start.
/// </summary>
public class CityDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private static readonly string[] Provinces =
    {
        "Adana", "Adıyaman", "Afyonkarahisar", "Ağrı", "Amasya", "Ankara", "Antalya", "Artvin",
        "Aydın", "Balıkesir", "Bilecik", "Bingöl", "Bitlis", "Bolu", "Burdur", "Bursa",
        "Çanakkale", "Çankırı", "Çorum", "Denizli", "Diyarbakır", "Edirne", "Elazığ", "Erzincan",
        "Erzurum", "Eskişehir", "Gaziantep", "Giresun", "Gümüşhane", "Hakkari", "Hatay", "Isparta",
        "Mersin", "İstanbul", "İzmir", "Kars", "Kastamonu", "Kayseri", "Kırklareli", "Kırşehir",
        "Kocaeli", "Konya", "Kütahya", "Malatya", "Manisa", "Kahramanmaraş", "Mardin", "Muğla",
        "Muş", "Nevşehir", "Niğde", "Ordu", "Rize", "Sakarya", "Samsun", "Siirt",
        "Sinop", "Sivas", "Tekirdağ", "Tokat", "Trabzon", "Tunceli", "Şanlıurfa", "Uşak",
        "Van", "Yozgat", "Zonguldak", "Aksaray", "Bayburt", "Karaman", "Kırıkkale", "Batman",
        "Şırnak", "Bartın", "Ardahan", "Iğdır", "Yalova", "Karabük", "Kilis", "Osmaniye",
        "Düzce"
    };

    private readonly ICityRepository _cityRepository;
    private readonly CityManager _cityManager;

    public CityDataSeedContributor(ICityRepository cityRepository, CityManager cityManager)
    {
        _cityRepository = cityRepository;
        _cityManager = cityManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _cityRepository.GetCountAsync() > 0)
        {
            return;
        }

        foreach (var province in Provinces)
        {
            await _cityRepository.InsertAsync(await _cityManager.CreateAsync(province));
        }
    }
}
