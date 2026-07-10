using System;
using KatKat.Constants;
using KatKat.Enums;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace KatKat.Entities;

/// <summary>
/// A resident-reported building issue/complaint ("Garaj kapısı sıkışıyor"). Resolution speed
/// feeds the Resolution metric (25%) of the KatKat Score.
/// </summary>
public class Issue : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual Guid ComplexId { get; protected set; }

    public virtual Guid ReporterUserId { get; protected set; }

    public virtual string Title { get; protected set; } = null!;

    public virtual string? Description { get; protected set; }

    public virtual string? PhotoUrl { get; protected set; }

    public virtual IssueStatuses Statuses { get; protected set; }

    public virtual DateTime? ResolvedAt { get; protected set; }

    public virtual Guid? ResolvedByUserId { get; protected set; }

    protected Issue()
    {
        /* EF Core */
    }

    internal Issue(
        Guid id,
        Guid? tenantId,
        Guid complexId,
        Guid reporterUserId,
        string title,
        string? description,
        string? photoUrl)
        : base(id)
    {
        TenantId = tenantId;
        ComplexId = complexId;
        ReporterUserId = reporterUserId;
        SetTitle(title);
        SetDescription(description);
        SetPhotoUrl(photoUrl);
        Statuses = IssueStatuses.Open;
    }

    public void SetTitle(string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(title), IssueConsts.MaxTitleLength);
    }

    public void SetDescription(string? description)
    {
        Description = Check.Length(description, nameof(description), IssueConsts.MaxDescriptionLength);
    }

    public void SetPhotoUrl(string? photoUrl)
    {
        PhotoUrl = Check.Length(photoUrl, nameof(photoUrl), IssueConsts.MaxPhotoUrlLength);
    }

    public void StartProgress()
    {
        if (Statuses != IssueStatuses.Open)
        {
            throw new BusinessException(KatKatErrorCodes.IssueMustBeOpenToStartProgress);
        }

        Statuses = IssueStatuses.InProgress;
    }

    public void Resolve(Guid resolvedByUserId)
    {
        if (Statuses != IssueStatuses.InProgress)
        {
            throw new BusinessException(KatKatErrorCodes.IssueMustBeInProgressToResolve);
        }

        Statuses = IssueStatuses.Resolved;
        ResolvedAt = DateTime.UtcNow;
        ResolvedByUserId = resolvedByUserId;
    }
}
