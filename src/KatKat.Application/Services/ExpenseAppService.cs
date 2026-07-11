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
    private readonly ExpenseManager _expenseManager;

    public ExpenseAppService(
        IExpenseRepository expenseRepository,
        IExpenseShareRepository expenseShareRepository,
        IFlatMemberRepository flatMemberRepository,
        ExpenseManager expenseManager)
    {
        _expenseRepository = expenseRepository;
        _expenseShareRepository = expenseShareRepository;
        _flatMemberRepository = flatMemberRepository;
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

        await _expenseRepository.InsertAsync(expense);
        await _expenseShareRepository.InsertManyAsync(shares);

        return ObjectMapper.Map<Expense, ExpenseDto>(expense);
    }

    public async Task<List<ExpenseShareDto>> GetSharesByExpenseAsync(Guid expenseId)
    {
        var shares = await _expenseShareRepository.GetListByExpenseAsync(expenseId);
        return shares.Select(s => ObjectMapper.Map<ExpenseShare, ExpenseShareDto>(s)).ToList();
    }

    public async Task<List<ExpenseShareDto>> GetSharesByFlatAsync(Guid flatId)
    {
        var shares = await _expenseShareRepository.GetListByFlatAsync(flatId);
        return shares.Select(s => ObjectMapper.Map<ExpenseShare, ExpenseShareDto>(s)).ToList();
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

        return ObjectMapper.Map<ExpenseShare, ExpenseShareDto>(share);
    }
}
