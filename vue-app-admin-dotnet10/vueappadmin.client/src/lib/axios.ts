import axios from 'axios';
import type { ApiResponse } from '@/types/api';

// TOKEN_KEY 包含 APP_NAME 前綴，避免多個應用共用 localStorage 時 key 衝突
const TOKEN_KEY = `${import.meta.env.VITE_APP_NAME}_authToken`;

const apiClient = axios.create();

// Request interceptor：自動從 localStorage 讀取 Token 並注入 Authorization header
apiClient.interceptors.request.use((config) => {
    const token = localStorage.getItem(TOKEN_KEY);
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

// 防止 401 觸發多次重導（例如同時發出多個請求時）
let isRedirectingToLogin = false;

// Response interceptor：
//   成功回應但 success = false → 轉為 rejected（顯示 API 回傳的錯誤訊息）
//   HTTP 401（非 Login 端點）→ 清除 Token 並跳轉 /login
apiClient.interceptors.response.use(
    (response) => {
        const body = response.data as ApiResponse<unknown>;
        if (body && typeof body === 'object' && 'success' in body && !body.success) {
            return Promise.reject(new Error(body.message ?? '操作失敗'));
        }
        return response;
    },
    (error) => {
        const body = error.response?.data as ApiResponse<unknown> | undefined;
        const message = body?.message;

        if (error.response?.status === 401) {
            const isLoginEndpoint = (error.config?.url as string | undefined)?.includes('/api/Auth/Login');
            if (!isLoginEndpoint && !isRedirectingToLogin) {
                isRedirectingToLogin = true;
                localStorage.removeItem(TOKEN_KEY);
                window.location.href = '/login';
            }
        }

        return Promise.reject(message ? new Error(message) : error);
    }
);

export default apiClient;
