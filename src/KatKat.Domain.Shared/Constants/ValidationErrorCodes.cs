namespace KatKat.Constants;

public static class ValidationErrorCodes
{
    private const string Prefix = "Validation:";

    public static class Building
    {
        public const string FloorCountMustBePositive = Prefix + nameof(FloorCountMustBePositive);
    }

    public static class Expense
    {
        public const string TotalAmountMustBePositive = Prefix + nameof(TotalAmountMustBePositive);
    }

    public static class Flat
    {
        public const string ShareFactorMustBePositive = Prefix + nameof(ShareFactorMustBePositive);
    }

    public static class P2PRequest
    {
        public const string NeededUntilMustBeFuture = Prefix + nameof(NeededUntilMustBeFuture);
    }
    
    public static class ResourceReservation
    {
        public const string EndTimeMustBeAfterStartTime = Prefix + nameof(EndTimeMustBeAfterStartTime);
    }    
    public static class ComplexSubscription
    {
        public const string ExtendSubscriptionDateMustBeFuture = Prefix + nameof(ExtendSubscriptionDateMustBeFuture);
    }
}