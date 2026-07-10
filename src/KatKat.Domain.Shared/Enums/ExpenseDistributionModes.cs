namespace KatKat.Enums;

public enum ExpenseDistributionModes
{
    /// <summary>
    /// Each flat's share is proportional to its ShareFactor (arsa payı).
    /// </summary>
    ShareFactorBased = 0,

    /// <summary>
    /// The total amount is split evenly across every flat in the Complex.
    /// </summary>
    EqualSplit = 1
}
