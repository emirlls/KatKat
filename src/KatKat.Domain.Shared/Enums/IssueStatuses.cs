namespace KatKat.Enums;

public enum IssueStatuses
{
    /// <summary>Reported, awaiting the manager to pick it up.</summary>
    Open = 0,

    /// <summary>The manager has started handling it.</summary>
    InProgress = 1,

    /// <summary>Fixed. The resolution delay feeds the Resolution metric of the KatKat Score.</summary>
    Resolved = 2
}
