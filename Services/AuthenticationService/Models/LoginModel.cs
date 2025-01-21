public class LoginModel
{
    /// <summary>
    /// The email or username of the user attempting to log in.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The user's password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}