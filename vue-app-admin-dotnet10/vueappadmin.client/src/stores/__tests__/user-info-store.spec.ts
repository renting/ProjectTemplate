import { describe, it, expect, beforeEach } from 'vitest';
import { setActivePinia, createPinia } from 'pinia';
import { useUserInfoStore } from '../user-info-store';

beforeEach(() => {
    setActivePinia(createPinia());
});

describe('user-info-store', () => {
    it('hasFeature returns true when feature exists', () => {
        const store = useUserInfoStore();
        store.features = ['items:read', 'items:write'];
        expect(store.hasFeature('items:read')).toBe(true);
    });

    it('hasFeature returns false when feature does not exist', () => {
        const store = useUserInfoStore();
        store.features = ['items:read'];
        expect(store.hasFeature('items:write')).toBe(false);
    });

    it('hasFeature returns false when features is empty', () => {
        const store = useUserInfoStore();
        expect(store.hasFeature('items:read')).toBe(false);
    });
});
