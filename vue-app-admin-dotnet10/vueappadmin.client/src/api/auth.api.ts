import apiClient from '@/lib/axios';
import type { ApiResponse, LoginRequest, LoginResponse, MeResponse } from '@/types/api';

// 登入並取得 JWT Token
export async function login(request: LoginRequest): Promise<LoginResponse> {
    const { data } = await apiClient.post<ApiResponse<LoginResponse>>('/api/Auth/Login', request);
    return data.result!;
}

// 取得目前登入使用者的資訊（username、displayName、groups、features）
// 需要有效的 JWT Token（由 axios interceptor 自動注入）
export async function getMe(): Promise<MeResponse> {
    const { data } = await apiClient.get<ApiResponse<MeResponse>>('/api/Auth/Me');
    return data.result!;
}
