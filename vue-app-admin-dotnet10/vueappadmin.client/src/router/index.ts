import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '@/stores/auth-store';
import { closeSidebar } from '@/composables/useSidebarDrawer';

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        {
            path: '/login',
            name: 'login',
            component: () => import('@/views/LoginView.vue'),
            // noAuthRequired: true → navigation guard 跳過驗證
            meta: { noAuthRequired: true, title: '登入' }
        },
        {
            path: '/',
            component: () => import('@/views/layouts/MainLayout.vue'),
            redirect: { name: 'dashboard' },
            children: [
                {
                    path: 'dashboard',
                    name: 'dashboard',
                    component: () => import('@/views/DashboardView.vue'),
                    meta: { showInSidebar: true, sidebarLabel: 'Dashboard', sidebarIcon: 'bi-speedometer2', title: 'Dashboard' }
                },
                {
                    path: 'example-items',
                    name: 'example-items',
                    component: () => import('@/views/ExampleItemsView.vue'),
                    meta: { showInSidebar: true, sidebarLabel: 'Example Items', sidebarIcon: 'bi-list-ul', title: 'Example Items' }
                }
            ]
        },
        {
            path: '/:pathMatch(.*)*',
            name: 'not-found',
            component: () => import('@/views/NotFoundView.vue'),
            meta: { noAuthRequired: true, title: '404' }
        }
    ]
});

// 路由保護：未登入時重導至 /login；已登入時不允許進入 /login
router.beforeEach((to) => {
    const authStore = useAuthStore();

    if (!to.meta.noAuthRequired && !authStore.isAuthenticated) {
        return { name: 'login' };
    }

    if (to.name === 'login' && authStore.isAuthenticated) {
        return { name: 'dashboard' };
    }
});

// 路由切換後更新瀏覽器標題，並關閉窄螢幕下開啟中的側邊欄 Drawer
router.afterEach((to) => {
    const title = to.meta.title;
    document.title = title ? `${title} | VueAppAdmin` : 'VueAppAdmin';
    closeSidebar();
});

export default router;
