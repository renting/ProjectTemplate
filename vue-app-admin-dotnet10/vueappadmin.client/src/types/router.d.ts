import 'vue-router';

declare module 'vue-router' {
    interface RouteMeta {
        noAuthRequired?: boolean;
        showInSidebar?: boolean;
        sidebarLabel?: string;
        sidebarIcon?: string;
        title?: string;
    }
}
