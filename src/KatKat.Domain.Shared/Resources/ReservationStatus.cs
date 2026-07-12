namespace KatKat.Resources;

public enum ReservationStatus
{
    // Numeric values are deliberately NOT sequential: Confirmed/Cancelled keep their original
    // values so reservation rows created before the approval workflow are not reinterpreted.
    Confirmed = 0,

    Cancelled = 1,

    /// <summary>Awaiting a manager's decision - the state every new reservation starts in.</summary>
    Pending = 2,

    /// <summary>A manager declined the request.</summary>
    Rejected = 3
}
