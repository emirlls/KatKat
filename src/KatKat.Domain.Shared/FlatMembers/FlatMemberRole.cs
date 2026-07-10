namespace KatKat.FlatMembers;

public enum FlatMemberRole
{
    /// <summary>
    /// Just signed up via an invitation link. Read-only until a Manager approves them.
    /// </summary>
    UnverifiedResident = 0,

    /// <summary>
    /// Approved dweller with full access to community features.
    /// </summary>
    Resident = 1,

    /// <summary>
    /// Official board/manager. Can post expenses, publish notices, and verify residents.
    /// </summary>
    Manager = 2,

    /// <summary>
    /// The creator of the tenant (Complex). Handles subscription and initial layout setup.
    /// </summary>
    Owner = 3
}
