using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Entities;
using KatKat.Enums;
using KatKat.Repositories;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace KatKat.DomainServices;

public class ExpenseManager : DomainService
{
    private const int AmountDecimalPlaces = 2;

    private readonly IComplexRepository _complexRepository;
    private readonly IFlatRepository _flatRepository;

    public ExpenseManager(IComplexRepository complexRepository, IFlatRepository flatRepository)
    {
        _complexRepository = complexRepository;
        _flatRepository = flatRepository;
    }

    /// <summary>
    /// Creates the Expense and calculates every flat's ExpenseShare in one step - splitting
    /// needs the full flat list of the Complex, a second aggregate, so it belongs here rather
    /// than in the Application Service.
    /// </summary>
    public virtual async Task<(Expense Expense, List<ExpenseShare> Shares)> CreateAsync(
        Guid complexId,
        string title,
        string? description,
        decimal totalAmount,
        ExpenseDistributionModes distributionModes,
        DateTime issuedAt,
        string? receiptImageUrl)
    {
        var complex = await _complexRepository.GetAsync(complexId);
        var flats = await _flatRepository.GetListByComplexAsync(complexId);

        if (flats.Count == 0)
        {
            throw new BusinessException(KatKatErrorCodes.ExpenseComplexHasNoFlatsToDistributeTo);
        }

        var expense = new Expense(
            GuidGenerator.Create(), complex.TenantId, complexId, title, description,
            totalAmount, distributionModes, issuedAt, receiptImageUrl);

        var shares = distributionModes == ExpenseDistributionModes.ShareFactorBased
            ? CreateShareFactorBasedShares(expense, flats)
            : CreateEqualSplitShares(expense, flats);

        return (expense, shares);
    }

    private List<ExpenseShare> CreateShareFactorBasedShares(Expense expense, List<Flat> flats)
    {
        var totalShareFactor = flats.Sum(f => f.ShareFactor);

        return flats.Select(flat => new ExpenseShare(
            GuidGenerator.Create(),
            expense.TenantId,
            expense.Id,
            flat.Id,
            Math.Round(expense.TotalAmount * (flat.ShareFactor / totalShareFactor), AmountDecimalPlaces)
        )).ToList();
    }

    private List<ExpenseShare> CreateEqualSplitShares(Expense expense, List<Flat> flats)
    {
        var perFlatAmount = Math.Round(expense.TotalAmount / flats.Count, AmountDecimalPlaces);

        return flats.Select(flat => new ExpenseShare(
            GuidGenerator.Create(),
            expense.TenantId,
            expense.Id,
            flat.Id,
            perFlatAmount
        )).ToList();
    }
}
