<script setup lang="ts">
import { ref, onMounted } from 'vue';
import Drawer from 'primevue/drawer';
import { getMenuItems } from '@/api/menu.api';
import type { MenuNode } from '@/types/api';
import SidebarMenuItem from './SidebarMenuItem.vue';
import { isSidebarOpen } from '@/composables/useSidebarDrawer';

const menuNodes = ref<MenuNode[]>([]);

// 掛載時向後端請求依使用者權限過濾後的選單，後端依 JWT claims 中的 features 過濾
onMounted(async () => {
    menuNodes.value = await getMenuItems();
});
</script>

<template>
    <!-- ≥768px：固定欄位呈現，維持現行行為 -->
    <nav class="border-end d-none d-md-block">
        <ul class="nav flex-column pt-3">
            <!-- SidebarMenuItem 支援遞迴渲染，可處理多層巢狀選單 -->
            <SidebarMenuItem v-for="node in menuNodes" :key="node.id" :node="node" />
        </ul>
    </nav>

    <!-- <768px：以 Drawer 覆蓋層呈現，重用同一份 menuNodes -->
    <Drawer v-model:visible="isSidebarOpen" position="left" class="sidebar-drawer">
        <ul class="nav flex-column">
            <SidebarMenuItem v-for="node in menuNodes" :key="node.id" :node="node" />
        </ul>
    </Drawer>
</template>

<style scoped>
.sidebar-drawer {
    width: 16rem;
}

/* 寬螢幕不使用 Drawer 覆蓋層，避免手動 resize 造成殘留開啟狀態 */
@media (min-width: 768px) {
    .sidebar-drawer {
        display: none;
    }
}
</style>
