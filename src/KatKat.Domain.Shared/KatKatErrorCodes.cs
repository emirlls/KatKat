namespace KatKat;

public static class KatKatErrorCodes
{
    private const string Prefix = "KatKat:";

    // Complex (01xxxx)
    public const string ComplexMustBeCreatedInsideTenantScope = Prefix + "010001";
    public const string ComplexNameAlreadyExists = Prefix + "010002";
    public const string SubscriptionEndDateMustBeLaterThanCurrentEndDate = Prefix + "010003";

    // Building (02xxxx)
    public const string BuildingNameAlreadyExistsInComplex = Prefix + "020001";
    public const string BuildingFloorCountMustBePositive = Prefix + "020002";

    // Flat (03xxxx)
    public const string FlatNumberAlreadyExistsInBuilding = Prefix + "030001";
    public const string FlatShareFactorMustBeGreaterThanZero = Prefix + "030002";

    // FlatMember (04xxxx)
    public const string FlatMemberAlreadyExistsForFlat = Prefix + "040001";
    public const string FlatMemberMustBeUnverifiedToApprove = Prefix + "040002";
    public const string FlatMemberCannotBePromotedWhileUnverified = Prefix + "040003";
}
