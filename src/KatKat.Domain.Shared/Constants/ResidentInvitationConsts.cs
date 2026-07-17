namespace KatKat.Constants;

public static class ResidentInvitationConsts
{
    public const string TableName = "ResidentInvitations";

    /// <summary>Codes are a GUID with the dashes stripped ("N" format) - 32 hex characters.</summary>
    public const int CodeLength = 32;

    public const int ValidityDays = 7;
}
