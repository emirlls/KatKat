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
    /// Rejects the booking if it overlaps any existing Confirmed reservation for the same
    /// Resource - the core conflict-prevention rule ("otopark kavgaları kökten çözülür").
    /// </summary>
    public virtual async Task<ResourceReservation> CreateAsync(
        Guid resourceId, Guid reservedByUserId, DateTime startTime, DateTime endTime)
    {
        var resource = await _resourceRepository.GetAsync(resourceId);

        if (await _resourceReservationRepository.HasOverlapAsync(resourceId, startTime, endTime))
        {
            throw new BusinessException(KatKatErrorCodes.ReservationOverlapsWithAnExistingReservation);
        }

        return new ResourceReservation(GuidGenerator.Create(), resource.TenantId, resourceId, reservedByUserId, startTime, endTime);
    }
}
