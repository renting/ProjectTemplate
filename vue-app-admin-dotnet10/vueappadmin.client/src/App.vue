<script setup lang="ts">
import { onMounted } from 'vue';
import NavigationProgress from '@/components/NavigationProgress.vue';
import { init } from '@/composables/useTheme';
import { useAuthStore } from '@/stores/auth-store';
import { useUserInfoStore } from '@/stores/user-info-store';

const authStore = useAuthStore();
const userInfoStore = useUserInfoStore();

// 應用程式掛載時初始化主題（讀取 localStorage 偏好或跟隨系統設定）
// 重新整理頁面後 Pinia store 會重置，但登入 Token 仍在 localStorage 中，故需重新載入使用者資訊
onMounted(() => {
    init();
    if (authStore.isAuthenticated) {
        userInfoStore.fetchUserInfo();
    }
});
</script>

<template>
    <!-- NavigationProgress 固定在視窗頂端，RouterView 渲染目前路由對應的頁面 -->
    <NavigationProgress />
    <RouterView />
</template>
