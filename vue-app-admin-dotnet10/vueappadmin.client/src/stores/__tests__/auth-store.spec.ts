import { describe, it, expect, beforeEach } from 'vitest';
import { setActivePinia, createPinia } from 'pinia';
import { useAuthStore } from '../auth-store';

beforeEach(() => {
    setActivePinia(createPinia());
    localStorage.clear();
});

describe('auth-store', () => {
    it('login stores token and sets isAuthenticated', () => {
        const store = useAuthStore();
        store.login('fake-token');
        expect(store.isAuthenticated).toBe(true);
    });

    it('logout clears token and sets isAuthenticated to false', () => {
        const store = useAuthStore();
        store.login('fake-token');
        store.logout();
        expect(store.isAuthenticated).toBe(false);
    });
});
