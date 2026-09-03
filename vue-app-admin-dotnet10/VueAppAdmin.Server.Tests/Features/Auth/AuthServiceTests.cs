using VueAppAdmin.Server.Features.Auth;

namespace VueAppAdmin.Server.Tests.Features.Auth;

// AuthService 直接 new()，不需 mock（無外部相依）
// 測試命名格式：方法名稱_情境_預期結果
public class AuthServiceTests
{
    private readonly AuthService _sut = new();

    [Fact]
    public void ValidateCredentials_ValidCredentials_ReturnsTrue()
    {
        // Arrange
        // Act
        var result = _sut.ValidateCredentials("admin", "password");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateCredentials_WrongPassword_ReturnsFalse()
    {
        // Arrange
        // Act
        var result = _sut.ValidateCredentials("admin", "wrong");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateCredentials_ViewerWithCorrectPassword_ReturnsTrue()
    {
        var result = _sut.ValidateCredentials("viewer", "password");
        Assert.True(result);
    }

    [Fact]
    public void ValidateCredentials_UnknownUser_ReturnsFalse()
    {
        var result = _sut.ValidateCredentials("unknown", "password");
        Assert.False(result);
    }

    [Fact]
    public void GetUserDisplayName_AdminUser_ReturnsAdministrator()
    {
        // Arrange
        // Act
        var result = _sut.GetUserDisplayName("admin");

        // Assert
        Assert.Equal("Administrator", result);
    }
}
