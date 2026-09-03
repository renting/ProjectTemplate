namespace VueAppAdmin.Server.Features.Auth;

public interface IAuthService
{
    bool ValidateCredentials(string username, string password);
    string GetUserDisplayName(string username);
    string[] GetUserGroups(string username);
    string[] GetUserFeatures(string username);
}
