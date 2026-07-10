namespace KatKat.P2PRequests;

public enum P2PRequestStatus
{
    /// <summary>
    /// Actively broadcast to opted-in neighbors, awaiting a helper.
    /// </summary>
    Open = 0,

    /// <summary>
    /// A neighbor has helped; the exchange is complete and counts toward the Social Score.
    /// </summary>
    Fulfilled = 1,

    /// <summary>
    /// Withdrawn by the requester before anyone helped.
    /// </summary>
    Cancelled = 2
}
