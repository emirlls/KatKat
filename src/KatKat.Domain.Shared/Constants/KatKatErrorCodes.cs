namespace KatKat.Constants;

public static class KatKatErrorCodes
{
    private const string Prefix = "KatKat:";

    // Complex (01xxxx)
    public const string ComplexNameAlreadyExists = Prefix + "010002";
    public const string SubscriptionEndDateMustBeLaterThanCurrentEndDate = Prefix + "010003";
    public const string ComplexLatitudeOutOfRange = Prefix + "010004";
    public const string ComplexLongitudeOutOfRange = Prefix + "010005";

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

    // P2PRequest (05xxxx)
    public const string P2PRequestMustBeOpenToFulfill = Prefix + "050001";
    public const string P2PRequestCannotBeFulfilledByRequester = Prefix + "050002";
    public const string P2PRequestMustBeOpenToCancel = Prefix + "050003";

    // Expense / ExpenseShare (06xxxx)
    public const string ExpenseTotalAmountMustBePositive = Prefix + "060001";
    public const string ExpenseComplexHasNoFlatsToDistributeTo = Prefix + "060002";
    public const string ExpenseShareAlreadyPaid = Prefix + "060003";
    public const string ExpenseShareCanOnlyBePaidByAFlatMember = Prefix + "060004";

    // Issue (07xxxx)
    public const string IssueMustBeOpenToStartProgress = Prefix + "070001";
    public const string IssueMustBeInProgressToResolve = Prefix + "070002";

    // Resource / ResourceReservation (08xxxx)
    public const string ReservationEndTimeMustBeAfterStartTime = Prefix + "080001";
    public const string ReservationOverlapsWithAnExistingReservation = Prefix + "080002";
    public const string ReservationMustBeConfirmedToCancel = Prefix + "080003";
    public const string ReservationCanOnlyBeCancelledByReserver = Prefix + "080004";

    // SosAlert (09xxxx)
    public const string SosAlertMustBeHelpNeededToResolve = Prefix + "090001";

    // Leaderboard (10xxxx)
    public const string NearbyLeaderboardRadiusMustBePositive = Prefix + "100001";

    // Location: City / District / Neighborhood (11xxxx)
    public const string CityNameAlreadyExists = Prefix + "110001";
    public const string DistrictNameAlreadyExistsInCity = Prefix + "110002";
    public const string NeighborhoodNameAlreadyExistsInDistrict = Prefix + "110003";
    public const string CityHasDistrictsCannotBeDeleted = Prefix + "110004";
    public const string DistrictHasNeighborhoodsCannotBeDeleted = Prefix + "110005";
    public const string NeighborhoodInUseByComplexCannotBeDeleted = Prefix + "110006";
}
