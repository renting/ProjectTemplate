import apiClient from '@/lib/axios';
import type { ApiResponse, FeatureResponse } from '@/types/api';

// 取得系統所有功能識別字清單（供管理介面顯示「可分配功能」使用）
export async function getFeatures(): Promise<FeatureResponse[]> {
    const { data } = await apiClient.get<ApiResponse<FeatureResponse>>('/api/Features');
    return data.results ?? [];
}
