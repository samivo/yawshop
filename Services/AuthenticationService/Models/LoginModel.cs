using System.ComponentModel.DataAnnotations;

public class LoginModel
{
    /// <summary>
    /// The email or username of the user attempting to log in.
    /// </summary>
    [EmailAddress]
    [Length(5, 50)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The user's password.
    /// </summary>
    [Length(5, 100)]
    public string Password { get; set; } = string.Empty;
}