namespace KatKat.Hubs;

public static class KatKatHubConsts
{
    public const string RoutePath = "/signalr-hubs/katkat";

    public const string ComplexGroupPrefix = "complex-";

    public static class EventNames
    {
        public const string P2PRequestCreated = "ReceiveP2PRequestCreated";
        public const string P2PRequestFulfilled = "ReceiveP2PRequestFulfilled";
        public const string ResourceReservationCreated = "ReceiveResourceReservationCreated";
        public const string ResourceReservationCancelled = "ReceiveResourceReservationCancelled";
        public const string SosAlertReported = "ReceiveSosAlert";
        public const string SosAlertResolved = "ReceiveSosAlertResolved";
        public const string IssueResolved = "ReceiveIssueResolved";
    }
}
