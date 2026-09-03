import { ref } from 'vue';

const isDark = ref(false);

// 同步套用深淺色模式：
//   PrimeVue 以 .p-dark class 控制
//   Bootstrap 以 data-bs-theme 屬性控制
function apply() {
    document.documentElement.classList.toggle('p-dark', isDark.value);
    document.documentElement.setAttribute('data-bs-theme', isDark.value ? 'dark' : 'light');
}

// 初始化：優先讀取 localStorage 記錄的偏好，若無則跟隨系統設定
function init() {
    const saved = localStorage.getItem('theme');
    if (saved) {
        isDark.value = saved === 'dark';
    } else {
        isDark.value = window.matchMedia('(prefers-color-scheme: dark)').matches;
    }
    apply();
}

function toggle() {
    isDark.value = !isDark.value;
    localStorage.setItem('theme', isDark.value ? 'dark' : 'light');
    apply();
}

export { isDark, init, toggle };
