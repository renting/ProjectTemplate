<script setup lang="ts">
import { ref } from 'vue';
import { isDark } from '@/composables/useTheme';
import { useForm } from 'vee-validate';
import { object, string } from 'yup';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth-store';
import { useUserInfoStore } from '@/stores/user-info-store';
import { login } from '@/api/auth.api';

const router = useRouter();
const authStore = useAuthStore();
const userInfoStore = useUserInfoStore();

const loginError = ref('');
const isSubmitting = ref(false);

// 使用 vee-validate + yup 進行表單驗證
const { handleSubmit, defineField, errors } = useForm({
    validationSchema: object({
        username: string().required('帳號為必填'),
        password: string().required('密碼為必填')
    })
});

const [username, usernameAttrs] = defineField('username');
const [password, passwordAttrs] = defineField('password');

// 登入流程：呼叫 API → 儲存 Token → 預先載入使用者資訊 → 跳轉 dashboard
const onSubmit = handleSubmit(async (values) => {
    loginError.value = '';
    isSubmitting.value = true;
    try {
        const response = await login({ username: values.username, password: values.password });
        authStore.login(response.token);
        await userInfoStore.fetchUserInfo();
        router.push({ name: 'dashboard' });
    } catch (err) {
        loginError.value = err instanceof Error ? err.message : '帳號或密碼錯誤';
    } finally {
        isSubmitting.value = false;
    }
});
</script>

<template>
    <div class="d-flex justify-content-center align-items-center vh-100 bg-body">
        <div class="card shadow" style="width: 360px">
            <div class="card-body p-4">
                <h4 class="card-title mb-4 text-center">VueAppAdmin</h4>
                <form @submit.prevent="onSubmit">
                    <div class="mb-3">
                        <label class="form-label">帳號</label>
                        <input
                            v-model="username"
                            v-bind="usernameAttrs"
                            type="text"
                            class="form-control"
                            :class="{ 'is-invalid': errors.username }"
                            autocomplete="username"
                            :disabled="isSubmitting"
                        />
                        <div class="invalid-feedback">{{ errors.username }}</div>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">密碼</label>
                        <input
                            v-model="password"
                            v-bind="passwordAttrs"
                            type="password"
                            class="form-control"
                            :class="{ 'is-invalid': errors.password }"
                            autocomplete="current-password"
                            :disabled="isSubmitting"
                        />
                        <div class="invalid-feedback">{{ errors.password }}</div>
                    </div>
                    <div v-if="loginError" class="alert alert-danger py-2">{{ loginError }}</div>
                    <button type="submit" class="btn w-100" :class="isDark ? 'btn-light' : 'btn-primary'" :disabled="isSubmitting">
                        <span v-if="isSubmitting" class="spinner-border spinner-border-sm me-2" role="status"></span>
                        {{ isSubmitting ? '登入中...' : '登入' }}
                    </button>
                </form>
            </div>
        </div>
    </div>
</template>
