import { defineStore } from 'pinia';
import { getMe } from '@/api/auth.api';

// 快取登入使用者的詳細資訊（username、displayName、groups、features）
// 登入後或頁面重新整理後呼叫 fetchUserInfo() 載入
export const useUserInfoStore = defineStore('userInfo', {
    state: () => ({
        username: '',
        displayName: '',
        groups: [] as string[],
        features: [] as string[],
        isLoading: false,
        error: null as string | null
    }),
    actions: {
        async fetchUserInfo() {
            this.isLoading = true;
            this.error = null;
            try {
                const me = await getMe();
                this.username = me.username;
                this.displayName = me.displayName;
                this.groups = me.groups ?? [];
                this.features = me.features ?? [];
            } catch (err) {
                this.error = err instanceof Error ? err.message : '無法取得使用者資訊';
            } finally {
                this.isLoading = false;
            }
        },
        // 檢查使用者是否擁有特定功能識別字，供元件控制按鈕/選單可見性
        hasFeature(feature: string): boolean {
            return this.features.includes(feature);
        }
    }
});
