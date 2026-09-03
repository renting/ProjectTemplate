<script setup lang="ts">
import { useUserInfoStore } from '@/stores/user-info-store';

const userInfoStore = useUserInfoStore();
</script>

<template>
    <div>
        <div class="mb-4">
            <h2 class="mb-1">Dashboard</h2>
            <p class="text-muted mb-0">歡迎回來，{{ userInfoStore.displayName || '使用者' }}</p>
        </div>

        <div class="row g-3 mb-4">
            <div class="col-sm-6 col-xl-3">
                <div class="card h-100">
                    <div class="card-body d-flex align-items-center gap-3">
                        <div class="bg-primary bg-opacity-10 rounded p-3 flex-shrink-0">
                            <i class="bi bi-people fs-4 text-primary"></i>
                        </div>
                        <div>
                            <div class="text-muted small">使用者</div>
                            <div class="fs-4 fw-bold">—</div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-sm-6 col-xl-3">
                <div class="card h-100">
                    <div class="card-body d-flex align-items-center gap-3">
                        <div class="bg-success bg-opacity-10 rounded p-3 flex-shrink-0">
                            <i class="bi bi-check-circle fs-4 text-success"></i>
                        </div>
                        <div>
                            <div class="text-muted small">已完成</div>
                            <div class="fs-4 fw-bold">—</div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-sm-6 col-xl-3">
                <div class="card h-100">
                    <div class="card-body d-flex align-items-center gap-3">
                        <div class="bg-warning bg-opacity-10 rounded p-3 flex-shrink-0">
                            <i class="bi bi-clock fs-4 text-warning"></i>
                        </div>
                        <div>
                            <div class="text-muted small">進行中</div>
                            <div class="fs-4 fw-bold">—</div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-sm-6 col-xl-3">
                <div class="card h-100">
                    <div class="card-body d-flex align-items-center gap-3">
                        <div class="bg-danger bg-opacity-10 rounded p-3 flex-shrink-0">
                            <i class="bi bi-exclamation-circle fs-4 text-danger"></i>
                        </div>
                        <div>
                            <div class="text-muted small">異常</div>
                            <div class="fs-4 fw-bold">—</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="card">
            <div class="card-header fw-medium">最近活動</div>
            <div class="card-body text-center text-muted py-5">
                <i class="bi bi-clock-history fs-1 d-block mb-2 opacity-25"></i>
                尚無活動記錄
            </div>
        </div>
    </div>
</template>
