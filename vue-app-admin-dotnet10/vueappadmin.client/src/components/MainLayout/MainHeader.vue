<script setup lang="ts">
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth-store';
import { useUserInfoStore } from '@/stores/user-info-store';
import { isDark, toggle } from '@/composables/useTheme';
import { toggleSidebar } from '@/composables/useSidebarDrawer';

const router = useRouter();
const authStore = useAuthStore();
const userInfoStore = useUserInfoStore();

function logout() {
    if (!confirm('確定要登出嗎？')) return;
    authStore.logout();
    router.push({ name: 'login' });
}
</script>

<template>
    <header class="navbar navbar-expand px-3 border-bottom bg-body-tertiary">
        <!-- 選單開關按鈕：僅小螢幕（<768px）顯示，切換側邊欄 Drawer 開合 -->
        <button class="btn btn-link text-body p-0 me-3 d-md-none" @click="toggleSidebar" title="開啟選單">
            <i class="bi bi-list" style="font-size: 1.3rem;"></i>
        </button>
        <span class="navbar-brand mb-0 h1">VueAppAdmin</span>
        <div class="ms-auto d-flex align-items-center gap-3">
            <!-- 深淺色切換按鈕：圖示跟隨目前模式變化 -->
            <button class="btn btn-link text-body-secondary p-0" @click="toggle" :title="isDark ? '切換淺色模式' : '切換深色模式'">
                <i :class="isDark ? 'bi bi-sun' : 'bi bi-moon-stars-fill'" style="font-size: 1.1rem;"></i>
            </button>
            <span class="text-body-secondary small">{{ userInfoStore.displayName }}</span>
            <button class="btn btn-outline-secondary btn-sm" @click="logout">登出</button>
        </div>
    </header>
</template>
