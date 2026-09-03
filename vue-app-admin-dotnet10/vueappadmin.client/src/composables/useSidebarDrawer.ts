import { ref } from 'vue';

const isSidebarOpen = ref(false);

function openSidebar() {
    isSidebarOpen.value = true;
}

function closeSidebar() {
    isSidebarOpen.value = false;
}

function toggleSidebar() {
    isSidebarOpen.value = !isSidebarOpen.value;
}

export { isSidebarOpen, openSidebar, closeSidebar, toggleSidebar };
