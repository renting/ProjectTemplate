<script setup lang="ts">
import MainHeader from '@/components/MainLayout/MainHeader.vue';
import MainSidebar from '@/components/MainLayout/MainSidebar.vue';
</script>

<template>
    <!-- 整體版面：Header 固定頂部，下方左側 Sidebar + 右側 RouterView -->
    <!-- ≥768px：Sidebar 寬度自適應內容（minmax(8rem, max-content)）並排顯示，主要區域佔剩餘空間 -->
    <!-- <768px：單欄版面，主要區域佔滿寬度，Sidebar 改由 Drawer 覆蓋層呈現，不佔版面空間 -->
    <div class="d-flex flex-column vh-100">
        <MainHeader />
        <div class="flex-grow-1 overflow-hidden main-content-grid">
            <MainSidebar />
            <main class="flex-grow-1 overflow-auto p-4">
                <RouterView />
            </main>
        </div>
    </div>
</template>

<style scoped>
.main-content-grid {
    display: grid;
    grid-template-columns: 1fr;
}

@media (min-width: 768px) {
    .main-content-grid {
        grid-template-columns: minmax(8rem, max-content) 1fr;
    }
}
</style>
