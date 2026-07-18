using System;
using System.Threading.Tasks;
using KatKat.Constants;
using KatKat.Entities;
using KatKat.Repositories;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace KatKat.DomainServices;

public class ResourceReservationManager : DomainService
{
    private readonly IResourceRepository _resourceRepository;
    private readonly IResourceReservationRepository _resourceReservationRepository;

    public ResourceReservationManager(
        IResourceRepository resourceRepository, IResourceReservationRepository resourceReservationRepository)
    {
        _resourceRepository = resourceRepository;
        _resourceReservationRepository = resourceReservationRepository;
    }

    /// <summary>
    /// Creates a pending booking, rejecting it up front if it already overlaps a Confirmed
    /// reservation for the same Resource. Other pending requests for the same slot are allowed to
    /// coexist - the manager picks which one to approve.
    /// </summary>
    public virtual async Task<ResourceReservation> CreateAsync(
        Guid resourceId, Guid? flatId, Guid reservedByUserId, DateTime startTime, DateTime endTime)
    {
        var resource = await _resourceRepository.GetAsync(resourceId);

        if (await _resourceReservationRepository.HasOverlapAsync(resourceId, startTime, endTime))
        {
            throw new BusinessException(KatKatErrorCodes.ReservationOverlapsWithAnExistingReservation);
        }

        return new ResourceReservation(GuidGenerator.Create(), resource.TenantId, resourceId, flatId, reservedByUserId, startTime, endTime);
    }

    /// <summary>
    /// Approves a pending reservation, re-checking overlap first so a manager can't confirm two
    /// requests that would collide (a competing request may have been approved in the meantime).
    /// </summary>
    public virtual async Task ApproveAsync(ResourceReservation reservation)
    {
        if (await _resourceReservationRepository.HasOverlapAsync(
                reservation.ResourceId, reservation.StartTime, reservation.EndTime, reservation.Id))
        {
            throw new BusinessException(KatKatErrorCodes.ReservationOverlapsWithAnExistingReservation);
        }

        reservation.Approve();
    }
}
