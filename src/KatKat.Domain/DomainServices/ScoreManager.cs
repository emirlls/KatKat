using System;
using System.Threading.Tasks;
using KatKat.Entities;
using KatKat.Repositories;
using Volo.Abp.Domain.Services;

namespace KatKat.DomainServices;

/// <summary>
/// The KatKat Score gamification engine: Score = Financial*0.40 + Social*0.35 + Resolution*0.25.
/// Financial is driven by ExpenseShare payment delay, Social by fulfilled P2PRequests, and
/// Resolution by Issue resolution delay - all computed here from real data.
/// </summary>
public class ScoreManager : DomainService
{
    public const decimal FinancialWeight = 0.40m;
    public const decimal SocialWeight = 0.35m;
    public const decimal ResolutionWeight = 0.25m;

    /// <summary>How far back Financial/Social/Resolution scores look when recalculating - both the
    /// nightly worker and the manual recalculate-now endpoint use the same window.</summary>
    public const int TrailingWindowDays = 30;

    public const int DefaultFulfillmentsForMaxScore = 20;

    /// <summary>Floor of every normalized 0-100 sub-score (Financial/Social/Resolution).</summary>
    public const decimal MinScore = 0m;

    /// <summary>Ceiling of every normalized 0-100 sub-score (Financial/Social/Resolution).</summary>
    public const decimal MaxScore = 100m;

    /// <summary>Score used when there is no data yet to judge by (new Complex) - assume healthy.</summary>
    public const decimal DefaultScoreWhenNoData = MaxScore;

    /// <summary>Points deducted per day an expense share is paid late, floored at 0.</summary>
    public const decimal FinancialScorePenaltyPerDelayDay = 10m;

    /// <summary>Points deducted per hour an issue takes to resolve, floored at 0.</summary>
    public const decimal ResolutionScorePenaltyPerDelayHour = 1m;

    private readonly IComplexScoreRepository _complexScoreRepository;
    private readonly IP2PRequestRepository _p2pRequestRepository;
    private readonly IExpenseShareRepository _expenseShareRepository;
    private readonly IIssueRepository _issueRepository;
    private readonly LocationHierarchyResolver _locationHierarchyResolver;

    public ScoreManager(
        IComplexScoreRepository complexScoreRepository,
        IP2PRequestRepository p2pRequestRepository,
        IExpenseShareRepository expenseShareRepository,
        IIssueRepository issueRepository,
        LocationHierarchyResolver locationHierarchyResolver)
    {
        _complexScoreRepository = complexScoreRepository;
        _p2pRequestRepository = p2pRequestRepository;
        _expenseShareRepository = expenseShareRepository;
        _issueRepository = issueRepository;
        _locationHierarchyResolver = locationHierarchyResolver;
    }

    public virtual decimal CalculateComposite(decimal financialScore, decimal socialScore, decimal resolutionScore)
    {
        return (financialScore * FinancialWeight) + (socialScore * SocialWeight) + (resolutionScore * ResolutionWeight);
    }

    /// <summary>
    /// Normalizes fulfilled P2P requests in the trailing window to a 0-100 score, capping at
    /// <paramref name="fulfillmentsForMaxScore"/> fulfillments = 100.
    /// </summary>
    public virtual async Task<decimal> CalculateSocialScoreAsync(
        Guid complexId, DateTime since, int fulfillmentsForMaxScore = DefaultFulfillmentsForMaxScore)
    {
        var fulfilledCount = await _p2pRequestRepository.GetFulfilledCountAsync(complexId, since);
        return Math.Min(MaxScore, fulfilledCount * (MaxScore / fulfillmentsForMaxScore));
    }

    /// <summary>
    /// Normalizes average expense-share payment delay to a 0-100 score:
    /// 100 minus <see cref="FinancialScorePenaltyPerDelayDay"/> points per day late, floored at 0.
    /// </summary>
    public virtual async Task<decimal> CalculateFinancialScoreAsync(Guid complexId, DateTime since)
    {
        var averageDelayDays = await _expenseShareRepository.GetAveragePaymentDelayDaysAsync(complexId, since);
        if (averageDelayDays == null)
        {
            return DefaultScoreWhenNoData;
        }

        return Math.Max(MinScore, MaxScore - (averageDelayDays.Value * FinancialScorePenaltyPerDelayDay));
    }

    /// <summary>
    /// Normalizes average issue resolution delay to a 0-100 score:
    /// 100 minus <see cref="ResolutionScorePenaltyPerDelayHour"/> point per hour taken, floored at 0.
    /// </summary>
    public virtual async Task<decimal> CalculateResolutionScoreAsync(Guid complexId, DateTime since)
    {
        var averageResolutionHours = await _issueRepository.GetAverageResolutionHoursAsync(complexId, since);
        if (averageResolutionHours == null)
        {
            return DefaultScoreWhenNoData;
        }

        return Math.Max(MinScore, MaxScore - (averageResolutionHours.Value * ResolutionScorePenaltyPerDelayHour));
    }

    public virtual async Task<ComplexScore> UpsertAsync(
        Guid complexId,
        Guid? tenantId,
        string name,
        int neighborhoodId,
        decimal latitude,
        decimal longitude,
        decimal financialScore,
        decimal socialScore,
        decimal resolutionScore,
        bool isActive)
    {
        var (cityId, districtId, resolvedNeighborhoodId) = await _locationHierarchyResolver.ResolveAsync(neighborhoodId);
        var totalScore = CalculateComposite(financialScore, socialScore, resolutionScore);
        var calculatedAt = DateTime.UtcNow;

        var existing = await _complexScoreRepository.FindByComplexIdAsync(complexId);
        if (existing != null)
        {
            existing.Update(name, cityId, districtId, resolvedNeighborhoodId, latitude, longitude, financialScore, socialScore, resolutionScore, totalScore, calculatedAt, isActive);
            return await _complexScoreRepository.UpdateAsync(existing);
        }

        var complexScore = new ComplexScore(
            GuidGenerator.Create(),
            complexId,
            tenantId,
            name,
            cityId,
            districtId,
            resolvedNeighborhoodId,
            latitude,
            longitude,
            financialScore,
            socialScore,
            resolutionScore,
            totalScore,
            calculatedAt,
            isActive
        );

        return await _complexScoreRepository.InsertAsync(complexScore, autoSave: true);
    }

    /// <summary>
    /// Calculates and upserts one Complex's KatKat Score. Shared by the nightly
    /// ScoreCalculationWorker and the manual "recalculate now" endpoint so the scoring logic
    /// itself lives in exactly one place.
    /// </summary>
    public virtual async Task<ComplexScore> RecalculateAsync(Complex complex, DateTime since)
    {
        var financialScore = await CalculateFinancialScoreAsync(complex.Id, since);
        var socialScore = await CalculateSocialScoreAsync(complex.Id, since);
        var resolutionScore = await CalculateResolutionScoreAsync(complex.Id, since);

        return await UpsertAsync(
            complex.Id, complex.TenantId, complex.Name, complex.NeighborhoodId,
            complex.Latitude, complex.Longitude, financialScore, socialScore, resolutionScore, complex.IsActive);
    }
}
