<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import type { MenuNode } from '@/types/api';

const props = defineProps<{ node: MenuNode }>();
const router = useRouter();
// 群組節點預設展開
const expanded = ref(true);

function toggle() {
    expanded.value = !expanded.value;
}

function navigate() {
    if (props.node.route) {
        router.push(props.node.route);
    }
}
</script>

<template>
    <li class="nav-item">
        <!-- route === null：群組節點（資料夾），顯示展開/收合按鈕並遞迴渲染子節點 -->
        <template v-if="node.route === null">
            <button
                class="nav-link text-body d-flex align-items-center gap-2 w-100 border-0 bg-transparent"
                @click="toggle"
            >
                <i v-if="node.icon" class="bi" :class="node.icon"></i>
                <span class="flex-grow-1 text-start">{{ node.label }}</span>
                <i class="bi" :class="expanded ? 'bi-chevron-down' : 'bi-chevron-right'" style="font-size: 0.75rem;"></i>
            </button>
            <!-- 遞迴渲染子節點（SidebarMenuItem 自我引用） -->
            <ul v-if="expanded" class="nav flex-column ms-3">
                <SidebarMenuItem v-for="child in node.children" :key="child.id" :node="child" />
            </ul>
        </template>
        <!-- route 有值：一般頁面連結 -->
        <template v-else>
            <RouterLink
                :to="node.route"
                class="nav-link text-body d-flex align-items-center gap-2"
                active-class="fw-bold text-primary"
                @click.prevent="navigate"
            >
                <i v-if="node.icon" class="bi" :class="node.icon"></i>
                {{ node.label }}
            </RouterLink>
        </template>
    </li>
</template>
