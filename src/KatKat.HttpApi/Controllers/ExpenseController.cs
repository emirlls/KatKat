using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatKat.Dtos;
using KatKat.Services;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace KatKat.Controllers;

/// <summary>
/// Smart Billing: shared expenses raised against a Complex and split into per-flat dues.
/// </summary>
[Area(KatKatRemoteServiceConsts.ModuleName)]
[RemoteService(Name = KatKatRemoteServiceConsts.RemoteServiceName)]
[Route(KatKatRemoteServiceConsts.RoutePathPrefix + "/expenses")]
public class ExpenseController : KatKatController, IExpenseAppService
{
    private readonly IExpenseAppService _expenseAppService;

    public ExpenseController(IExpenseAppService expenseAppService)
    {
        _expenseAppService = expenseAppService;
    }

    /// <summary>Gets an Expense by id.</summary>
    [HttpGet("{id}")]
    public Task<ExpenseDto> GetAsync(Guid id) => _expenseAppService.GetAsync(id);

    /// <summary>Lists Expenses raised against a Complex.</summary>
    [HttpGet]
    public Task<List<ExpenseDto>> GetListByComplexAsync(Guid complexId) =>
        _expenseAppService.GetListByComplexAsync(complexId);

    /// <summary>Creates a new Expense and splits it into per-flat shares.</summary>
    [HttpPost]
    public Task<ExpenseDto> CreateAsync(CreateExpenseDto input) => _expenseAppService.CreateAsync(input);

    /// <summary>Lists every flat's share of an Expense.</summary>
    [HttpGet("{expenseId}/shares")]
    public Task<List<ExpenseShareDto>> GetSharesByExpenseAsync(Guid expenseId) =>
        _expenseAppService.GetSharesByExpenseAsync(expenseId);

    /// <summary>Lists a Flat's dues across all Expenses.</summary>
    [HttpGet("shares/by-flat/{flatId}")]
    public Task<List<ExpenseShareDto>> GetSharesByFlatAsync(Guid flatId) =>
        _expenseAppService.GetSharesByFlatAsync(flatId);

    /// <summary>Marks a share as paid - only a member of the owning Flat may call this.</summary>
    [HttpPost("shares/{shareId}/pay")]
    public Task<ExpenseShareDto> PayShareAsync(Guid shareId) => _expenseAppService.PayShareAsync(shareId);
}
