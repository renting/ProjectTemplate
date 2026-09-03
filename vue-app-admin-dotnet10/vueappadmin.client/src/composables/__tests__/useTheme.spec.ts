import { describe, it, expect, beforeEach, vi } from 'vitest';

// Reset module state between tests by re-importing
beforeEach(() => {
    localStorage.clear();
    document.documentElement.removeAttribute('data-bs-theme');
    document.documentElement.classList.remove('p-dark');
    vi.resetModules();
});

describe('useTheme', () => {
    it('init reads dark from localStorage', async () => {
        localStorage.setItem('theme', 'dark');
        const { isDark, init } = await import('../useTheme');
        init();
        expect(isDark.value).toBe(true);
        expect(document.documentElement.getAttribute('data-bs-theme')).toBe('dark');
    });

    it('init reads light from localStorage', async () => {
        localStorage.setItem('theme', 'light');
        const { isDark, init } = await import('../useTheme');
        init();
        expect(isDark.value).toBe(false);
        expect(document.documentElement.getAttribute('data-bs-theme')).toBe('light');
    });

    it('toggle switches isDark and writes to localStorage', async () => {
        localStorage.setItem('theme', 'light');
        const { isDark, init, toggle } = await import('../useTheme');
        init();
        expect(isDark.value).toBe(false);

        toggle();
        expect(isDark.value).toBe(true);
        expect(localStorage.getItem('theme')).toBe('dark');

        toggle();
        expect(isDark.value).toBe(false);
        expect(localStorage.getItem('theme')).toBe('light');
    });
});
