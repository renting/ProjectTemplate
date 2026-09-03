import apiClient from '@/lib/axios';
import type { ApiResponse, MenuNode } from '@/types/api';

// 取得依目前使用者功能權限過濾後的選單節點清單
// 後端從 JWT claims 讀取 features 並過濾，前端不需要額外傳遞參數
export async function getMenuItems(): Promise<MenuNode[]> {
    const { data } = await apiClient.post<ApiResponse<MenuNode>>('/api/Menu/Items');
    return data.results ?? [];
}
