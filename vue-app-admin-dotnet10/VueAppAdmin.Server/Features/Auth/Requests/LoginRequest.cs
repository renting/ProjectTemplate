using System.ComponentModel.DataAnnotations;
using VueAppAdmin.Server.Shared.Logging;

namespace VueAppAdmin.Server.Features.Auth.Requests;

public class LoginRequest
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    [LogMask]
    public string Password { get; set; } = string.Empty;
}
