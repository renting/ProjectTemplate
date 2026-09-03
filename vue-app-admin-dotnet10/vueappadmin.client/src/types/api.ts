// 對應後端 ApiResponse<T>，所有 API 回應的基礎型別
// result 與 results 擇一有值：單筆用 result，清單用 results
export interface ApiResponse<T> {
    success: boolean;
    message: string | null;
    result: T | null;
    results: T[] | null;
}

// 對應後端 ApiPagedResponse<T>，加上分頁總筆數
export interface ApiPagedResponse<T> extends ApiResponse<T> {
    total: number;
}

export interface LoginRequest {
    username: string;
    password: string;
}

export interface LoginResponse {
    token: string;
}

export interface MeResponse {
    username: string;
    displayName: string;
    // 使用者所屬群組（即時查詢）
    groups: string[];
    // 使用者擁有的功能識別字（從 JWT claims 解析）
    features: string[];
}

export interface FeatureResponse {
    // 功能識別字，格式：<資源>:<動作>，例如 "items:read"
    code: string;
    description: string;
}

// 對應後端 MenuNode，支援多層巢狀
export interface MenuNode {
    id: number;
    label: string;
    icon: string;
    // null 表示群組節點（不可點擊）
    route: string | null;
    // null 表示所有登入使用者均可見
    requiredFeature: string | null;
    children: MenuNode[];
}

export interface ExampleCategoryResponse {
    id: number;
    name: string;
}

export interface ItemResponse {
    id: number;
    name: string;
    description: string;
    categoryId: number;
    categoryName: string;
    createdDate: string;
}

export interface ExampleItemsSearchRequest {
    // 從 1 開始
    page: number;
    pageSize: number;
    // 排序欄位：id / name / description / categoryName / createdDate
    sortField: string;
    // 排序方向：asc / desc
    sortOrder: string;
    name?: string;
    description?: string;
    // 多選類別過濾；空陣列表示不限類別
    categoryIds: number[];
    // createdDate 查詢區間（含首尾兩端點）；僅帶一端代表開放區間
    dateFrom?: string;
    dateTo?: string;
}
