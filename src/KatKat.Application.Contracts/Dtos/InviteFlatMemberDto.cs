using System;

namespace KatKat.Dtos;

/// <summary>
/// The joining user is always the caller (CurrentUser) - a resident attaches themselves to the
/// flat they selected during registration. There is no field to name a different user here.
/// </summary>
public class InviteFlatMemberDto
{
    public Guid FlatId { get; set; }
}
