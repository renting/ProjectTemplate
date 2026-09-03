## Purpose

規範 feature-based 垂直切片的資料夾組織方式與每個 feature 的擴充方法慣例。

## Requirements

### Requirement: Feature 資料夾組織規範
每個業務功能 SHALL 放在 `Features/<FeatureName>/` 資料夾下，包含該功能所有相關檔案。跨功能共用的基礎設施 SHALL 放在 `Shared/<TopicName>/` 下，以主題為子資料夾單位。

#### Scenario: 新增一個 Feature
- **WHEN** 開發者新增一個業務功能（如 Products）
- **THEN** 所有相關檔案（Controller、Service、Interface、Repository、Requests、Responses）SHALL 放在 `Features/Products/` 下，不得分散到其他頂層資料夾

#### Scenario: Feature 內部子資料夾
- **WHEN** 一個 Feature 有 Request 或 Response DTO
- **THEN** Request DTO SHALL 放在 `Features/<Name>/Requests/`，Response DTO SHALL 放在 `Features/<Name>/Responses/`

---

### Requirement: Feature 自我 DI 註冊
每個 Feature SHALL 提供自己的 `IServiceCollection` 擴充方法，負責註冊該 Feature 的所有 Service 與 Repository。

#### Scenario: Feature 擴充方法命名
- **WHEN** 為 Auth Feature 建立 DI 註冊方法
- **THEN** 擴充方法 SHALL 命名為 `AddAuthFeature`，放在 `Features/Auth/AuthExtensions.cs`

#### Scenario: Program.cs 呼叫
- **WHEN** 啟動應用程式
- **THEN** `Program.cs` SHALL 以 `builder.Services.AddAuthFeature()` 形式呼叫各 Feature 的註冊方法，不得直接在 `Program.cs` 內逐一呼叫 `AddScoped<>`

---

### Requirement: Shared 資料夾邊界規則
`Shared/` 資料夾 SHALL 只存放被兩個以上 Feature 使用的元件，或跨切面基礎設施（Middleware、Logging 初始化、DB 連線）。

#### Scenario: 單一 Feature 使用的元件
- **WHEN** 某元件只有一個 Feature 在使用
- **THEN** 該元件 SHALL 留在該 Feature 資料夾內，不得移入 Shared/

#### Scenario: 跨 Feature 共用元件
- **WHEN** 某元件被兩個以上 Feature 引用
- **THEN** 該元件 SHALL 移至 `Shared/<TopicName>/` 對應主題資料夾

---

### Requirement: Interface 強制規範
所有 Service 與 Repository SHALL 定義對應的 Interface，與實作檔案放在同一資料夾。

#### Scenario: Service Interface 命名
- **WHEN** 建立 `AuthService`
- **THEN** SHALL 同時建立 `IAuthService`，放在 `Features/Auth/IAuthService.cs`

#### Scenario: Controller 注入型別
- **WHEN** Controller 注入 Service
- **THEN** SHALL 注入 Interface 型別（`IAuthService`），不得直接注入 concrete class
