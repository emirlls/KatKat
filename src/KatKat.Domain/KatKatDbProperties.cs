namespace KatKat;

public static class KatKatDbProperties
{
    public static string DbTablePrefix { get; set; } = "KatKat";

    public static string? DbSchema { get; set; } = null;

    public const string ConnectionStringName = "KatKat";
}
