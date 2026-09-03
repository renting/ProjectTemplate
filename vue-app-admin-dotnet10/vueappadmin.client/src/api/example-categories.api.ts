import apiClient from '@/lib/axios';
import type { ApiResponse, ExampleCategoryResponse } from '@/types/api';

// 取得所有類別清單（供 ExampleItemsView 的篩選下拉選單使用）
export async function getExampleCategories(): Promise<ExampleCategoryResponse[]> {
    const { data } = await apiClient.post<ApiResponse<ExampleCategoryResponse>>('/api/ExampleCategories');
    return data.results ?? [];
}
