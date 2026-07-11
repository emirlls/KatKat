namespace KatKat.ResponseWrapping;

/// <summary>
/// The single envelope shape every KatKat API response is wrapped in - success or failure alike -
/// so API consumers always parse the same { message, success, status, data } structure.
/// </summary>
public class ApiResponse
{
    public string Message { get; set; } = null!;

    public bool Success { get; set; }

    public int Status { get; set; }

    public object? Data { get; set; }
}
