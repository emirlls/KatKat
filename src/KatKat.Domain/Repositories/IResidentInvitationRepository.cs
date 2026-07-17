using System;
using System.Threading.Tasks;
using KatKat.Entities;

namespace KatKat.Repositories;

public interface IResidentInvitationRepository : IKatKatRepository<ResidentInvitation, Guid>
{
    Task<ResidentInvitation?> FindByCodeAsync(string code);
}
