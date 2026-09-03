import { defineStore } from 'pinia';

// TOKEN_KEY 包含 APP_NAME 前綴，與 axios.ts 保持一致
const TOKEN_KEY = `${import.meta.env.VITE_APP_NAME}_authToken`;

// 管理登入狀態與 Token 存取
// isAuthenticated 初始值從 localStorage 讀取，支援頁面重新整理後保持登入
export const useAuthStore = defineStore('auth', {
    state: () => ({
        isAuthenticated: !!localStorage.getItem(TOKEN_KEY)
    }),
    actions: {
        login(token: string) {
            localStorage.setItem(TOKEN_KEY, token);
            this.isAuthenticated = true;
        },
        logout() {
            localStorage.removeItem(TOKEN_KEY);
            this.isAuthenticated = false;
        }
    }
});
