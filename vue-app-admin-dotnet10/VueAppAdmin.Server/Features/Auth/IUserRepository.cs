namespace VueAppAdmin.Server.Features.Auth;

public interface IUserRepository
{
    bool ExistsWithCredentials(string username, string hashedPassword);
    string? FindDisplayNameByUsername(string username);
}
