# VueAppAdmin.Server.Tests

`VueAppAdmin.Server` 的 xUnit 測試專案。

## 技術棧

| 套件 | 用途 |
|---|---|
| xUnit 2.9.2 | 測試框架 |
| NSubstitute 5.3.0 | Mock / Stub |
| Coverlet | 程式碼覆蓋率收集 |

## 執行測試

```sh
# 執行所有測試
dotnet test

# 顯示詳細輸出
dotnet test --logger "console;verbosity=detailed"

# 收集覆蓋率報告
dotnet test --collect:"XPlat Code Coverage"
```

## 目錄結構

測試目錄結構鏡射 server 專案的 `Features/` 結構：

```
VueAppAdmin.Server.Tests/
└── Features/
    ├── Auth/
    │   └── AuthServiceTests.cs
    ├── ExampleItems/
    │   └── ExampleItemsServiceTests.cs
    ├── ExampleCategories/
    │   └── ExampleCategoriesServiceTests.cs
    └── Menu/
        └── MenuServiceTests.cs
```

新增測試時，請依照 feature 資料夾放置，例如：

```
Features/
└── Products/
    └── ProductsServiceTests.cs
```

## 撰寫慣例

**命名規則**：`方法名稱_情境_預期結果`

```csharp
// ✓ 清楚說明：什麼方法、什麼條件、預期什麼
ValidateCredentials_ValidCredentials_ReturnsTrue()
ValidateCredentials_WrongPassword_ReturnsFalse()
```

**測試結構**：AAA（Arrange / Act / Assert）

```csharp
[Fact]
public void MyMethod_SomeCondition_ExpectedResult()
{
    // Arrange
    var sut = new MyService();

    // Act
    var result = sut.MyMethod(input);

    // Assert
    Assert.Equal(expected, result);
}
```

**有外部依賴時使用 NSubstitute**：

```csharp
public class MyServiceTests
{
    private readonly IMyRepository _repository = Substitute.For<IMyRepository>();
    private readonly MyService _sut;

    public MyServiceTests()
    {
        _sut = new MyService(_repository);
    }

    [Fact]
    public void DoSomething_WhenDataExists_ReturnsResult()
    {
        // Arrange
        _repository.GetById(1).Returns(new MyEntity { Id = 1 });

        // Act
        var result = _sut.DoSomething(1);

        // Assert
        Assert.NotNull(result);
    }
}
```

**無外部依賴時直接實例化**（參考 `AuthServiceTests`）：

```csharp
public class AuthServiceTests
{
    private readonly AuthService _sut = new();
    // ...
}
```
