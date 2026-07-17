using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.DomainServices;
using KatKat.Dtos;
using KatKat.Entities;
using KatKat.Permissions;
using KatKat.Repositories;
using Volo.Abp;
using Volo.Abp.Users;

namespace KatKat.Services;

public class ExpenseAppService : KatKatAppService, IExpenseAppService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IExpenseShareRepository _expenseShareRepository;
    private readonly IFlatMemberRepository _flatMemberRepository;
    private readonly IFlatRepository _flatRepository;
    private readonly ExpenseManager _expenseManager;

    public ExpenseAppService(
        IExpenseRepository expenseRepository,
        IExpenseShareRepository expenseShareRepository,
        IFlatMemberRepository flatMemberRepository,
        IFlatRepository flatRepository,
        ExpenseManager expenseManager)
    {
        _expenseRepository = expenseRepository;
        _expenseShareRepository = expenseShareRepository;
        _flatMemberRepository = flatMemberRepository;
        _flatRepository = flatRepository;
        _expenseManager = expenseManager;
    }

    public async Task<ExpenseDto> GetAsync(Guid id)
    {
        var expense = await _expenseRepository.GetAsync(id);
        return ObjectMapper.Map<Expense, ExpenseDto>(expense);
    }

    public async Task<List<ExpenseDto>> GetListByComplexAsync(Guid complexId)
    {
        var expenses = await _expenseRepository.GetListByComplexAsync(complexId);
        return expenses.Select(e => ObjectMapper.Map<Expense, ExpenseDto>(e)).ToList();
    }

    public async Task<ExpenseDto> CreateAsync(CreateExpenseDto input)
    {
        var (expense, shares) = await _expenseManager.CreateAsync(
            input.ComplexId, input.Title, input.Description, input.TotalAmount,
            input.DistributionModes, input.IssuedAt, input.ReceiptImageUrl);

        await _expenseRepository.InsertAsync(expense, autoSave: true);
        await _expenseShareRepository.InsertManyAsync(shares, autoSave: true);

        return ObjectMapper.Map<Expense, ExpenseDto>(expense);
    }

    public async Task<List<ExpenseShareDto>> GetSharesByExpenseAsync(Guid expenseId)
    {
        var shares = await _expenseShareRepository.GetListByExpenseAsync(expenseId);
        return await MapToShareDtosAsync(shares);
    }

    public async Task<List<ExpenseShareDto>> GetSharesByFlatAsync(Guid flatId)
    {
        var shares = await _expenseShareRepository.GetListByFlatAsync(flatId);
        return await MapToShareDtosAsync(shares);
    }

    public async Task<ExpenseShareDto> PayShareAsync(Guid shareId)
    {
        var share = await _expenseShareRepository.GetAsync(shareId);

        if (!await _flatMemberRepository.ExistsAsync(share.FlatId, CurrentUser.GetId()))
        {
            throw new BusinessException(KatKatErrorCodes.ExpenseShareCanOnlyBePaidByAFlatMember);
        }

        share.MarkAsPaid();
        await _expenseShareRepository.UpdateAsync(share);

        return (await MapToShareDtosAsync(new List<ExpenseShare> { share }))[0];
    }

    /// <summary>Batches the Flat -> FlatNumber lookup for a whole list, avoiding N+1 queries.</summary>
    private async Task<List<ExpenseShareDto>> MapToShareDtosAsync(List<ExpenseShare> shares)
    {
        if (shares.Count == 0)
        {
            return new List<ExpenseShareDto>();
        }

        var flats = await _flatRepository.GetListByIdsAsync(shares.Select(s => s.FlatId).Distinct());
        var flatNumberById = flats.ToDictionary(f => f.Id, f => f.FlatNumber);

        return shares.Select(share =>
        {
            var dto = ObjectMapper.Map<ExpenseShare, ExpenseShareDto>(share);
            dto.FlatNumber = flatNumberById.GetValueOrDefault(share.FlatId, share.FlatId.ToString());
            return dto;
        }).ToList();
    }
}
