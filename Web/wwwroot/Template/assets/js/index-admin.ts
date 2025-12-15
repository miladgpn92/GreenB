type ApiResponse<T> = {
    isSuccess: boolean;
    message?: string;
    description?: string;
    model?: T;
    code?: number;
};

type SubmissionCategory = {
    id: number;
    title: string;
    createdAt?: string;
    updatedAt?: string;
};

type UserSubmission = {
    id: number;
    phone: string;
    firstName?: string;
    lastName?: string;
    submissionCategoryId: number;
    submissionCategoryTitle?: string;
    createdAt?: string;
};

type BrochureFile = {
    id: number;
    title: string;
    pdfFileUrl: string;
    slug?: string;
    createdAt?: string;
    updatedAt?: string;
};

type PublicSetting = {
    siteTitle?: string;
    logoUrl?: string;
    favIconUrl?: string;
    phonenumber?: string;
    tell?: string;
    address?: string;
    latitude?: string;
    longitude?: string;
    telegramLink?: string;
    whatsappLink?: string;
    instagramLink?: string;
    eaitaLink?: string;
    smsText?: string;
};

const endpoints = {
    categoriesPaged: "/api/v1/SubmissionCategories/PagedList",
    categories: "/api/v1/SubmissionCategories",
    submissionsPaged: "/api/v1/UserSubmissions/PagedList",
    submissions: "/api/v1/UserSubmissions",
    submissionDetails: (id: number) => `/api/v1/UserSubmissions/${id}`,
    upload: "/api/admin/filemanager/Uplaod",
    brochuresPaged: "/api/v1/BrochureFiles/PagedList",
    brochures: "/api/v1/BrochureFiles",
    brochureDetails: (id: number) => `/api/v1/BrochureFiles/${id}`,
    settingGet: "/api/v1/Setting/GetSetting?lang=1",
    settingSave: "/api/v1/Setting/SetPublicSetting?lang=1",
    sendSms: "/api/v1/Setting/SendSMS",
    increaseSms: "/api/v1/Setting/IncreseSMSCharge",
    validateSms: "/api/v1/Setting/ValidateSMSCharge",
};

const unwrapModel = (value: any) => {
    if (value && typeof value === "object") {
        if ("model" in value) return (value as any).model;
        if ("Model" in value) return (value as any).Model;
        if ("data" in value) return (value as any).data;
        if ("Data" in value) return (value as any).Data;
    }
    return value;
};

const normalizeResponse = <T>(payload: any): ApiResponse<T> => {
    const model = unwrapModel(payload?.model ?? payload?.Model ?? payload?.data ?? payload?.Data);
    const statusCode = payload?.statusCode ?? payload?.StatusCode;
    const derivedSuccess = statusCode === 0 || statusCode === "Success";
    return {
        isSuccess: payload?.isSuccess ?? payload?.IsSuccess ?? derivedSuccess ?? false,
        message: payload?.message ?? payload?.Message ?? payload?.description ?? payload?.Description,
        description: payload?.description ?? payload?.Description,
        model,
        code: payload?.code ?? payload?.Code,
    };
};

const apiFetch = async <T>(url: string, options: RequestInit = {}): Promise<ApiResponse<T>> => {
    const response = await fetch(url, {
        credentials: "same-origin",
        ...options,
    });

    const contentType = response.headers.get("content-type") || "";
    const payload = contentType.includes("application/json") ? await response.json() : { message: await response.text() };

    const normalized = normalizeResponse<T>(payload);

    if (!normalized.isSuccess && response.ok && normalized.message === undefined && normalized.description === undefined) {
        normalized.message = "عملیات با خطا مواجه شد.";
    }

    if (!response.ok && !normalized.message) {
        normalized.message = payload?.message ?? payload?.Message ?? response.statusText;
    }

    return normalized;
};

const formatDate = (value?: string) => {
    if (!value) return "—";
    const date = new Date(value);
    return isNaN(date.getTime()) ? "—" : date.toLocaleDateString("fa-IR");
};

const slugify = (input: string) => {
    const map: Record<string, string> = {
        "ي": "ی",
        "ك": "ک",
        "ة": "ه",
        "ۀ": "ه",
        "ؤ": "و",
        "ئ": "ی",
        "أ": "ا",
        "إ": "ا",
        "آ": "ا",
    };
    let text = (input || "").trim();
    Object.entries(map).forEach(([k, v]) => {
        text = text.replace(new RegExp(k, "g"), v);
    });
    text = text.toLowerCase();
    text = text.replace(/[^0-9a-z\u0600-\u06FF\s-]+/g, "");
    text = text.replace(/\s+/g, "-").replace(/-+/g, "-").replace(/^-|-$/g, "");
    if (!text) text = `brochure-${Date.now()}`;
    return text;
};

class ToastManager {
    private container: HTMLElement;

    constructor() {
        const existing = document.querySelector<HTMLElement>("[data-toast-container]");
        if (existing) {
            this.container = existing;
            return;
        }
        this.container = document.createElement("div");
        this.container.dataset.toastContainer = "true";
        this.container.className =
            "fixed left-1/2 top-6 z-[120] flex w-full max-w-xl -translate-x-1/2 flex-col gap-3 px-4";
        document.body.appendChild(this.container);
    }

    show(message: string, type: "success" | "error" | "info" = "success") {
        const toast = document.createElement("div");
        const palette =
            type === "success"
                ? "bg-emerald-50 text-emerald-800 border-emerald-200"
                : type === "info"
                ? "bg-sky-50 text-sky-800 border-sky-200"
                : "bg-red-50 text-red-800 border-red-200";

        toast.className = `toast-item pointer-events-auto flex w-full items-start gap-3 rounded-2xl border px-4 py-3 shadow-md transition opacity-0 ${palette}`;
        toast.innerHTML = `
            <div class="mt-1 h-2 w-2 rounded-full ${type === "error" ? "bg-red-500" : type === "info" ? "bg-sky-500" : "bg-emerald-500"}"></div>
            <p class="text-sm leading-relaxed">${message}</p>
        `;

        this.container.appendChild(toast);
        requestAnimationFrame(() => toast.classList.remove("opacity-0"));

        setTimeout(() => {
            toast.classList.add("opacity-0", "-translate-y-1");
            toast.addEventListener("transitionend", () => toast.remove());
        }, 4200);
    }
}

class CategoryStore {
    private categories: SubmissionCategory[] = [];
    private loading = false;

    constructor(private toast: ToastManager) {}

    async load(force = false) {
        if (this.categories.length && !force) return this.categories;
        if (this.loading) return this.categories;

        this.loading = true;
        const res = await apiFetch<SubmissionCategory[]>(endpoints.categoriesPaged, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ arg: { pageNumber: 1, pageSize: 100 }, filters: [], sortField: "createdAt", ascending: false }),
        });
        this.loading = false;

        if (!res.isSuccess || !res.model) {
            this.toast.show(res.message || "دریافت دسته‌بندی‌ها انجام نشد.", "error");
            return this.categories;
        }

        this.categories = res.model;
        return this.categories;
    }

    getAll() {
        return this.categories;
    }
}

const setButtonLoading = (button: HTMLButtonElement | null, loading: boolean, loadingText?: string) => {
    if (!button) return;
    if (loading) {
        button.dataset.originalText = button.textContent || "";
        button.textContent = loadingText || "در حال ارسال...";
        button.disabled = true;
        button.classList.add("opacity-70", "cursor-not-allowed");
    } else {
        button.textContent = button.dataset.originalText || button.textContent || "";
        button.disabled = false;
        button.classList.remove("opacity-70", "cursor-not-allowed");
    }
};

const toggleHidden = (element: HTMLElement | null, show: boolean) => {
    if (!element) return;
    if (show) {
        element.classList.remove("hidden");
        document.body.classList.add("overflow-hidden");
    } else {
        element.classList.add("hidden");
        document.body.classList.remove("overflow-hidden");
    }
};

const renderOptions = (select: HTMLSelectElement | null, categories: SubmissionCategory[], placeholder = "انتخاب کنید") => {
    if (!select) return;
    select.innerHTML = "";
    const empty = document.createElement("option");
    empty.value = "";
    empty.textContent = placeholder;
    select.appendChild(empty);
    categories.forEach((cat) => {
        const option = document.createElement("option");
        option.value = cat.id.toString();
        option.textContent = cat.title;
        select.appendChild(option);
    });
};

const quickSubmission = (toast: ToastManager, categoryStore: CategoryStore) => {
    const modal = document.querySelector<HTMLElement>('[data-modal="quick-submission"]');
    const openButton = document.querySelector<HTMLButtonElement>('[data-open-modal="quick-submission"]');
    const closeButtons = modal?.querySelectorAll<HTMLButtonElement>("[data-close-modal]") || [];

    const form = modal?.querySelector<HTMLFormElement>("[data-quick-form]");
    const phoneInput = modal?.querySelector<HTMLInputElement>("[data-quick-phone]");
    const firstNameInput = modal?.querySelector<HTMLInputElement>("[data-quick-firstname]");
    const lastNameInput = modal?.querySelector<HTMLInputElement>("[data-quick-lastname]");
    const categorySelect = modal?.querySelector<HTMLSelectElement>("[data-quick-category]");
    const statusLabel = modal?.querySelector<HTMLElement>("[data-quick-status]");
    const refreshButton = modal?.querySelector<HTMLButtonElement>("[data-refresh-categories]");
    const submitButton = modal?.querySelector<HTMLButtonElement>("[data-submit-quick]");

    const hydrateCategories = async (force = false) => {
        await categoryStore.load(force);
        renderOptions(categorySelect || null, categoryStore.getAll(), "انتخاب دسته‌بندی");
    };

    openButton?.addEventListener("click", async () => {
        await hydrateCategories();
        toggleHidden(modal, true);
        phoneInput?.focus();
    });

    closeButtons.forEach((btn) =>
        btn.addEventListener("click", () => {
            toggleHidden(modal, false);
        }),
    );

    refreshButton?.addEventListener("click", async () => {
        setButtonLoading(refreshButton, true, "در حال بروزرسانی...");
        await hydrateCategories(true);
        setButtonLoading(refreshButton, false);
        toast.show("دسته‌بندی‌ها بروزرسانی شد.", "info");
    });

    form?.addEventListener("submit", async (event) => {
        event.preventDefault();
        const phone = phoneInput?.value.trim();
        const submissionCategoryId = Number(categorySelect?.value || 0);

        if (!phone) {
            toast.show("شماره تماس الزامی است.", "error");
            phoneInput?.focus();
            return;
        }
        if (!submissionCategoryId) {
            toast.show("یک دسته‌بندی انتخاب کنید.", "error");
            categorySelect?.focus();
            return;
        }

        statusLabel && (statusLabel.textContent = "");
        setButtonLoading(submitButton, true, "در حال ثبت...");

        try {
            const res = await apiFetch<number>(endpoints.submissions, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    phone,
                    submissionCategoryId,
                    firstName: firstNameInput?.value || "",
                    lastName: lastNameInput?.value || "",
                }),
            });

            if (!res.isSuccess) {
                const message = res.message || "ثبت اطلاعات انجام نشد.";
                statusLabel && (statusLabel.textContent = message);
                statusLabel && statusLabel.classList.add("text-red-600");
                toast.show(message, "error");
                return;
            }

            toast.show("ثبت شماره با موفقیت انجام شد.", "success");
            statusLabel && (statusLabel.textContent = "ثبت با موفقیت انجام شد. فرم برای ورودی بعدی آماده است.");
            statusLabel && statusLabel.classList.remove("text-red-600");
            if (phoneInput) phoneInput.value = "";
            firstNameInput && (firstNameInput.value = "");
            lastNameInput && (lastNameInput.value = "");
            phoneInput?.focus();
        } catch (error) {
            statusLabel && (statusLabel.textContent = "خطا در ارتباط با سرور.");
            statusLabel && statusLabel.classList.add("text-red-600");
            toast.show("خطا در ارتباط با سرور.", "error");
        } finally {
            setButtonLoading(submitButton, false);
        }
    });

    if (modal && !modal.classList.contains("hidden")) {
        hydrateCategories();
    }
};

const categoriesManager = (toast: ToastManager, categoryStore: CategoryStore) => {
    const list = document.querySelector<HTMLElement>("[data-category-list]");
    const loadButton = document.querySelector<HTMLButtonElement>("[data-category-reload]");
    const form = document.querySelector<HTMLFormElement>("[data-category-form]");
    const titleInput = document.querySelector<HTMLInputElement>("[data-category-title]");
    const hiddenId = document.querySelector<HTMLInputElement>("[data-category-id]");
    const saveButton = document.querySelector<HTMLButtonElement>("[data-category-save]");
    const resetButton = document.querySelector<HTMLButtonElement>("[data-category-reset]");

    const renderList = () => {
        if (!list) return;
        const categories = categoryStore.getAll();
        if (!categories.length) {
            list.innerHTML = `
                <div class="flex flex-col items-center justify-center rounded-2xl border border-dashed border-gray-200 bg-gray-50 px-4 py-6 text-center text-sm text-gray-600">
                    <p class="font-semibold text-gray-800">هنوز دسته‌بندی ثبت نشده است.</p>
                    <p class="mt-1 text-xs text-gray-500">برای شروع روی دکمه «ثبت دسته‌بندی» یا «بارگذاری لیست» بزنید.</p>
                </div>
            `;
            return;
        }

        list.innerHTML = "";
        categories.forEach((cat) => {
            const item = document.createElement("div");
            item.className =
                "flex flex-col gap-3 rounded-2xl border border-gray-200 bg-white p-4 shadow-sm transition hover:-translate-y-0.5 hover:shadow-md";
            item.innerHTML = `
                <div class="flex items-start justify-between gap-3">
                    <div>
                        <p class="text-sm font-semibold text-gray-900">${cat.title}</p>
                        <p class="text-xs text-gray-500 mt-1">ایجاد: ${formatDate(cat.createdAt)}</p>
                    </div>
                    <div class="flex items-center gap-2">
                        <button class="rounded-lg bg-amber-50 px-3 py-1.5 text-xs font-semibold text-amber-700 hover:bg-amber-100" data-action="edit">ویرایش</button>
                        <button class="rounded-lg bg-red-50 px-3 py-1.5 text-xs font-semibold text-red-700 hover:bg-red-100" data-action="delete">حذف</button>
                    </div>
                </div>
            `;
            item.querySelector<HTMLButtonElement>('[data-action="edit"]')?.addEventListener("click", () => {
                if (hiddenId) hiddenId.value = cat.id.toString();
                if (titleInput) titleInput.value = cat.title;
                titleInput?.focus();
            });
            item.querySelector<HTMLButtonElement>('[data-action="delete"]')?.addEventListener("click", async () => {
                const ok = window.confirm(`حذف "${cat.title}"؟`);
                if (!ok) return;
                const res = await apiFetch<boolean>(`${endpoints.categories}/${cat.id}`, { method: "DELETE" });
                if (!res.isSuccess) {
                    toast.show(res.message || "حذف دسته‌بندی انجام نشد.", "error");
                    return;
                }
                toast.show("دسته‌بندی حذف شد.", "success");
                await categoryStore.load(true);
                renderList();
            });
            list.appendChild(item);
        });
    };

    const load = async () => {
        setButtonLoading(loadButton, true, "در حال بارگذاری...");
        await categoryStore.load(true);
        renderList();
        setButtonLoading(loadButton, false);
    };

    loadButton?.addEventListener("click", load);

    resetButton?.addEventListener("click", () => {
        if (hiddenId) hiddenId.value = "";
        if (titleInput) titleInput.value = "";
        titleInput?.focus();
    });

    form?.addEventListener("submit", async (event) => {
        event.preventDefault();
        const title = titleInput?.value.trim();
        if (!title) {
            toast.show("عنوان دسته‌بندی را وارد کنید.", "error");
            titleInput?.focus();
            return;
        }
        const id = Number(hiddenId?.value || 0);
        setButtonLoading(saveButton, true, id ? "در حال بروزرسانی..." : "در حال ثبت...");
        const res = await apiFetch<number>(id ? `${endpoints.categories}/${id}` : endpoints.categories, {
            method: id ? "PUT" : "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(id ? { id, title } : { title }),
        });
        setButtonLoading(saveButton, false);
        if (!res.isSuccess) {
            toast.show(res.message || "ذخیره انجام نشد.", "error");
            return;
        }
        toast.show(id ? "دسته‌بندی بروزرسانی شد." : "دسته‌بندی ثبت شد.", "success");
        hiddenId && (hiddenId.value = "");
        titleInput && (titleInput.value = "");
        await categoryStore.load(true);
        renderList();
    });

    return { renderList, load };
};

const submissionsManager = (toast: ToastManager, categoryStore: CategoryStore) => {
    const list = document.querySelector<HTMLElement>("[data-submission-list]");
    const detail = document.querySelector<HTMLElement>("[data-submission-detail]");
    const filterCategory = document.querySelector<HTMLSelectElement>("[data-submission-filter-category]");
    const filterPhone = document.querySelector<HTMLInputElement>("[data-submission-search]");
    const refreshButton = document.querySelector<HTMLButtonElement>("[data-submission-refresh]");
    const createForm = document.querySelector<HTMLFormElement>("[data-submission-form]");
    const createButton = document.querySelector<HTMLButtonElement>("[data-submission-save]");
    const uploadInput = document.querySelector<HTMLInputElement>("[data-submission-upload]");
    const uploadStatus = document.querySelector<HTMLElement>("[data-submission-upload-status]");
    const uploadLink = document.querySelector<HTMLElement>("[data-submission-upload-link]");
    const uploadClear = document.querySelector<HTMLButtonElement>("[data-submission-upload-clear]");

    const createFields = {
        phone: createForm?.querySelector<HTMLInputElement>("[data-submission-phone]"),
        category: createForm?.querySelector<HTMLSelectElement>("[data-submission-category]"),
        firstName: createForm?.querySelector<HTMLInputElement>("[data-submission-firstname]"),
        lastName: createForm?.querySelector<HTMLInputElement>("[data-submission-lastname]"),
    };

    let uploadedPdfUrl = "";

    const hydrateCategories = async () => {
        await categoryStore.load();
        const categories = categoryStore.getAll();
        renderOptions(filterCategory || null, categories, "همه دسته‌بندی‌ها");
        renderOptions(createFields.category || null, categories, "انتخاب دسته‌بندی");
    };

    const renderSubmissions = (items: UserSubmission[]) => {
        if (!list) return;
        if (!items.length) {
            list.innerHTML = `
                <div class="rounded-2xl border border-dashed border-gray-200 bg-gray-50 px-4 py-6 text-center text-sm text-gray-600">
                    موردی یافت نشد. فیلترها را تغییر دهید یا یک ثبت جدید اضافه کنید.
                </div>
            `;
            return;
        }

        list.innerHTML = "";
        items.forEach((item) => {
            const card = document.createElement("div");
            card.className =
                "flex flex-col gap-3 rounded-2xl border border-gray-200 bg-white p-4 shadow-sm transition hover:-translate-y-0.5 hover:shadow-md";
            card.innerHTML = `
                <div class="flex items-start justify-between gap-3">
                    <div class="space-y-1">
                        <p class="text-sm font-semibold text-gray-900">${item.phone}</p>
                        <p class="text-xs text-gray-600">دسته‌بندی: ${item.submissionCategoryTitle || "—"}</p>
                        <p class="text-xs text-gray-500">تاریخ ثبت: ${formatDate(item.createdAt)}</p>
                    </div>
                    <div class="flex items-center gap-2">
                        <button class="rounded-lg bg-gray-100 px-3 py-1.5 text-xs font-semibold text-gray-700 hover:bg-gray-200" data-action="details">جزئیات</button>
                        <button class="rounded-lg bg-red-50 px-3 py-1.5 text-xs font-semibold text-red-700 hover:bg-red-100" data-action="delete">حذف</button>
                    </div>
                </div>
                <div class="text-xs text-gray-600">${[item.firstName, item.lastName].filter(Boolean).join(" ") || "نام ثبت نشده"}</div>
            `;

            card.querySelector<HTMLButtonElement>('[data-action="details"]')?.addEventListener("click", async () => {
                const res = await apiFetch<UserSubmission>(endpoints.submissionDetails(item.id));
                if (!res.isSuccess || !res.model) {
                    toast.show(res.message || "دریافت جزئیات انجام نشد.", "error");
                    return;
                }
                if (detail) {
                    detail.innerHTML = `
                        <div class="flex items-start justify-between gap-3">
                            <div>
                                <p class="text-sm font-semibold text-gray-900">شماره: ${res.model.phone}</p>
                                <p class="text-xs text-gray-600 mt-1">${[res.model.firstName, res.model.lastName].filter(Boolean).join(" ") || "بدون نام"}</p>
                                <p class="text-xs text-gray-500 mt-1">دسته‌بندی: ${res.model.submissionCategoryTitle || "—"}</p>
                            </div>
                            <span class="rounded-full bg-emerald-50 px-3 py-1 text-xs font-semibold text-emerald-700">#${res.model.id}</span>
                        </div>
                        <p class="mt-2 text-xs text-gray-500">ایجاد: ${formatDate(res.model.createdAt)}</p>
                    `;
                }
            });

            card.querySelector<HTMLButtonElement>('[data-action="delete"]')?.addEventListener("click", async () => {
                const ok = window.confirm(`ثبت با شماره ${item.phone} حذف شود؟`);
                if (!ok) return;
                const res = await apiFetch<boolean>(`${endpoints.submissions}/${item.id}`, { method: "DELETE" });
                if (!res.isSuccess) {
                    toast.show(res.message || "حذف انجام نشد.", "error");
                    return;
                }
                toast.show("حذف ثبت با موفقیت انجام شد.", "success");
                await loadSubmissions();
            });

            list.appendChild(card);
        });
    };

    const loadSubmissions = async () => {
        setButtonLoading(refreshButton, true, "در حال بارگذاری...");
        const params = new URLSearchParams();
        if (filterCategory?.value) params.set("categoryId", filterCategory.value);
        if (filterPhone?.value) params.set("phone", filterPhone.value.trim());

        const res = await apiFetch<UserSubmission[]>(`${endpoints.submissionsPaged}?${params.toString()}`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ arg: { pageNumber: 1, pageSize: 30 }, filters: [], sortField: "createdAt", ascending: false }),
        });
        setButtonLoading(refreshButton, false);

        if (!res.isSuccess || !res.model) {
            toast.show(res.message || "دریافت لیست انجام نشد.", "error");
            return;
        }

        renderSubmissions(res.model);
    };

    refreshButton?.addEventListener("click", loadSubmissions);
    filterCategory?.addEventListener("change", loadSubmissions);
    filterPhone?.addEventListener("keydown", (e) => {
        if (e.key === "Enter") {
            e.preventDefault();
            loadSubmissions();
        }
    });

    uploadInput?.addEventListener("change", async (event) => {
        const target = event.target as HTMLInputElement;
        const file = target.files?.[0];
        if (!file) return;
        if (file.type !== "application/pdf" && !file.name.toLowerCase().endsWith(".pdf")) {
            toast.show("تنها فایل PDF قابل بارگذاری است.", "error");
            target.value = "";
            return;
        }

        uploadStatus && (uploadStatus.textContent = "در حال بارگذاری فایل...");
        uploadStatus?.classList.remove("text-red-600");
        const formData = new FormData();
        formData.append("Files", file);
        formData.append("Filepath", "files/user-submissions");

        const res = await apiFetch<string[]>(endpoints.upload, {
            method: "POST",
            body: formData,
        });

        if (!res.isSuccess || !res.model?.length) {
            uploadStatus && (uploadStatus.textContent = res.message || "خطا در بارگذاری فایل.");
            uploadStatus && uploadStatus.classList.add("text-red-600");
            toast.show(res.message || "خطا در بارگذاری فایل.", "error");
            return;
        }

        uploadedPdfUrl = res.model[0];
        uploadStatus && (uploadStatus.textContent = "فایل PDF با موفقیت بارگذاری شد.");
        uploadLink &&
            (uploadLink.innerHTML = `<a class="text-emerald-700 underline break-all" href="${uploadedPdfUrl}" target="_blank" rel="noopener">مشاهده فایل</a>`);
        toast.show("فایل PDF بارگذاری شد.", "success");
    });

    uploadClear?.addEventListener("click", () => {
        uploadedPdfUrl = "";
        if (uploadInput) uploadInput.value = "";
        if (uploadStatus) uploadStatus.textContent = "هیچ فایلی انتخاب نشده است.";
        if (uploadLink) uploadLink.textContent = "";
    });

    createForm?.addEventListener("submit", async (event) => {
        event.preventDefault();
        const phone = createFields.phone?.value.trim();
        const categoryId = Number(createFields.category?.value || 0);
        if (!phone) {
            toast.show("شماره تماس الزامی است.", "error");
            createFields.phone?.focus();
            return;
        }
        if (!categoryId) {
            toast.show("دسته‌بندی را انتخاب کنید.", "error");
            createFields.category?.focus();
            return;
        }
        setButtonLoading(createButton, true, "در حال ثبت...");

        const payload: Record<string, unknown> = {
            phone,
            submissionCategoryId: categoryId,
            firstName: createFields.firstName?.value || "",
            lastName: createFields.lastName?.value || "",
        };

        if (uploadedPdfUrl) {
            payload["attachmentUrl"] = uploadedPdfUrl;
        }

        const res = await apiFetch<number>(endpoints.submissions, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload),
        });
        setButtonLoading(createButton, false);

        if (!res.isSuccess) {
            toast.show(res.message || "ثبت انجام نشد.", "error");
            return;
        }

        toast.show("ثبت جدید با موفقیت انجام شد.", "success");
        createFields.phone && (createFields.phone.value = "");
        createFields.firstName && (createFields.firstName.value = "");
        createFields.lastName && (createFields.lastName.value = "");
        createFields.category && (createFields.category.value = "");
        uploadedPdfUrl = "";
        if (uploadInput) uploadInput.value = "";
        if (uploadStatus) uploadStatus.textContent = "هیچ فایلی انتخاب نشده است.";
        if (uploadLink) uploadLink.textContent = "";
        await loadSubmissions();
    });

    return { hydrateCategories, loadSubmissions };
};

const settingsManager = (toast: ToastManager) => {
    const form = document.querySelector<HTMLFormElement>("[data-settings-form]");
    const saveButton = document.querySelector<HTMLButtonElement>("[data-settings-save]");
    const statusLabel = document.querySelector<HTMLElement>("[data-settings-status]");
    const smsForm = document.querySelector<HTMLFormElement>("[data-sms-form]");
    const smsButton = document.querySelector<HTMLButtonElement>("[data-sms-send]");
    const chargeForm = document.querySelector<HTMLFormElement>("[data-charge-form]");
    const chargeButton = document.querySelector<HTMLButtonElement>("[data-charge-send]");
    const chargeValidateForm = document.querySelector<HTMLFormElement>("[data-charge-validate-form]");
    const chargeValidateButton = document.querySelector<HTMLButtonElement>("[data-charge-validate]");
    const smsTextField = form?.querySelector<HTMLInputElement | HTMLTextAreaElement>('[data-setting-field="smsText"]');

    const fields: Record<string, HTMLInputElement | HTMLTextAreaElement | null> = {
        siteTitle: form?.querySelector('[data-setting-field="siteTitle"]') as HTMLInputElement,
        logoUrl: form?.querySelector('[data-setting-field="logoUrl"]') as HTMLInputElement,
        favIconUrl: form?.querySelector('[data-setting-field="favIconUrl"]') as HTMLInputElement,
        phonenumber: form?.querySelector('[data-setting-field="phonenumber"]') as HTMLInputElement,
        tell: form?.querySelector('[data-setting-field="tell"]') as HTMLInputElement,
        address: form?.querySelector('[data-setting-field="address"]') as HTMLTextAreaElement,
        latitude: form?.querySelector('[data-setting-field="latitude"]') as HTMLInputElement,
        longitude: form?.querySelector('[data-setting-field="longitude"]') as HTMLInputElement,
        telegramLink: form?.querySelector('[data-setting-field="telegramLink"]') as HTMLInputElement,
        whatsappLink: form?.querySelector('[data-setting-field="whatsappLink"]') as HTMLInputElement,
        instagramLink: form?.querySelector('[data-setting-field="instagramLink"]') as HTMLInputElement,
        eaitaLink: form?.querySelector('[data-setting-field="eaitaLink"]') as HTMLInputElement,
        smsText: smsTextField as HTMLTextAreaElement,
    };

    const setStatus = (text: string, isError = false) => {
        if (!statusLabel) return;
        statusLabel.textContent = text;
        statusLabel.classList.toggle("text-red-600", isError);
        statusLabel.classList.toggle("text-emerald-700", !isError);
    };

    const loadSettings = async () => {
        const res = await apiFetch<PublicSetting>(endpoints.settingGet);
        if (!res.isSuccess || !res.model) {
            setStatus(res.message || "دریافت تنظیمات انجام نشد.", true);
            toast.show(res.message || "دریافت تنظیمات انجام نشد.", "error");
            return;
        }

        Object.entries(fields).forEach(([key, input]) => {
            if (!input) return;
            const value = (res.model as any)[key];
            if (value !== undefined && value !== null) {
                input.value = value;
            }
        });
        setStatus("تنظیمات بارگذاری شد.");
    };

    form?.addEventListener("submit", async (event) => {
        event.preventDefault();
        const payload: PublicSetting = {};
        Object.entries(fields).forEach(([key, input]) => {
            if (input) {
                payload[key as keyof PublicSetting] = input.value;
            }
        });

        setButtonLoading(saveButton, true, "در حال ذخیره...");
        const res = await apiFetch(endpoints.settingSave, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload),
        });
        setButtonLoading(saveButton, false);
        if (!res.isSuccess) {
            setStatus(res.message || "ذخیره تنظیمات انجام نشد.", true);
            toast.show(res.message || "ذخیره تنظیمات انجام نشد.", "error");
            return;
        }
        setStatus("تنظیمات با موفقیت ذخیره شد.");
        toast.show("تنظیمات ذخیره شد.", "success");
    });

    smsForm?.addEventListener("submit", async (event) => {
        event.preventDefault();
        const phone = smsForm.querySelector<HTMLInputElement>("[data-sms-phone]")?.value.trim();
        const text =
            smsForm.querySelector<HTMLTextAreaElement>("[data-sms-text]")?.value.trim() ||
            (smsTextField?.value ? smsTextField.value.trim() : "");
        if (!phone || !text) {
            toast.show("شماره و متن پیامک را کامل وارد کنید.", "error");
            return;
        }
        setButtonLoading(smsButton, true, "در حال ارسال...");
        const res = await apiFetch(endpoints.sendSms, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ phonenumber: phone, text }),
        });
        setButtonLoading(smsButton, false);
        if (!res.isSuccess) {
            toast.show(res.message || "ارسال پیامک انجام نشد.", "error");
            return;
        }
        toast.show("پیامک ارسال شد.", "success");
    });

    chargeForm?.addEventListener("submit", async (event) => {
        event.preventDefault();
        const amount = Number(chargeForm.querySelector<HTMLInputElement>("[data-charge-amount]")?.value || 0);
        if (!amount) {
            toast.show("مبلغ شارژ را وارد کنید.", "error");
            return;
        }
        setButtonLoading(chargeButton, true, "در حال ثبت...");
        const res = await apiFetch(endpoints.increaseSms + `?Amount=${amount}`, { method: "GET" });
        setButtonLoading(chargeButton, false);
        if (!res.isSuccess) {
            toast.show(res.message || "افزایش شارژ انجام نشد.", "error");
            return;
        }
        toast.show("درخواست افزایش شارژ ثبت شد.", "success");
    });

    chargeValidateForm?.addEventListener("submit", async (event) => {
        event.preventDefault();
        const amount = Number(chargeValidateForm.querySelector<HTMLInputElement>("[data-charge-validate-amount]")?.value || 0);
        const id = Number(chargeValidateForm.querySelector<HTMLInputElement>("[data-charge-id]")?.value || 0);
        if (!amount || !id) {
            toast.show("مبلغ و شناسه پرداخت را وارد کنید.", "error");
            return;
        }
        setButtonLoading(chargeValidateButton, true, "در حال بررسی...");
        const res = await apiFetch(endpoints.validateSms + `?Amount=${amount}&id=${id}`, { method: "GET" });
        setButtonLoading(chargeValidateButton, false);
        if (!res.isSuccess) {
            toast.show(res.message || "تایید پرداخت انجام نشد.", "error");
            return;
        }
        toast.show("پرداخت تایید شد.", "success");
    });

    return { loadSettings };
};

const brochuresManager = (toast: ToastManager) => {
    const list = document.querySelector<HTMLElement>("[data-brochure-list]");
    const refreshButton = document.querySelector<HTMLButtonElement>("[data-brochure-refresh]");
    const form = document.querySelector<HTMLFormElement>("[data-brochure-form]");
    const titleInput = form?.querySelector<HTMLInputElement>("[data-brochure-title]");
    const pdfInput = form?.querySelector<HTMLInputElement>("[data-brochure-pdf]");
    const slugInput = form?.querySelector<HTMLInputElement>("[data-brochure-slug]");
    const saveButton = form?.querySelector<HTMLButtonElement>("[data-brochure-save]");
    const statusLabel = form?.querySelector<HTMLElement>("[data-brochure-status]");

    let uploadUrl = "";
    let editId: number | null = null;
    let slugEdited = false;

    slugInput?.addEventListener("input", () => {
        slugEdited = true;
        if (slugInput) slugInput.value = slugify(slugInput.value);
    });
    titleInput?.addEventListener("input", () => {
        if (!slugEdited && slugInput) {
            slugInput.value = slugify(titleInput.value);
        }
    });

    const renderBrochures = (items: BrochureFile[]) => {
        if (!list) return;
        if (!items.length) {
            list.innerHTML = `
                <div class="rounded-2xl border border-dashed border-gray-200 bg-gray-50 px-4 py-6 text-center text-sm text-gray-600">
                    بروشوری وجود ندارد. بروشور جدید ثبت کنید.
                </div>
            `;
            return;
        }

        list.innerHTML = "";
        items.forEach((item) => {
            const card = document.createElement("div");
            card.className =
                "flex flex-col gap-3 rounded-2xl border border-gray-200 bg-white p-4 shadow-sm transition hover:-translate-y-0.5 hover:shadow-md";
            card.innerHTML = `
                <div class="flex items-start justify-between gap-3">
                    <div class="space-y-1">
                        <p class="text-sm font-semibold text-gray-900">${item.title}</p>
                        <p class="text-xs text-gray-600">اسلاگ: ${item.slug || "—"}</p>
                        <p class="text-xs text-gray-500">ایجاد: ${formatDate(item.createdAt)}</p>
                    </div>
                    <div class="flex items-center gap-2">
                        <a class="rounded-lg bg-gray-100 px-3 py-1.5 text-xs font-semibold text-gray-700 hover:bg-gray-200" href="${item.pdfFileUrl}" target="_blank" rel="noopener">PDF</a>
                        <button class="rounded-lg bg-amber-50 px-3 py-1.5 text-xs font-semibold text-amber-700 hover:bg-amber-100" data-action="edit">ویرایش</button>
                        <button class="rounded-lg bg-red-50 px-3 py-1.5 text-xs font-semibold text-red-700 hover:bg-red-100" data-action="delete">حذف</button>
                    </div>
                </div>
            `;

            card.querySelector<HTMLButtonElement>('[data-action="edit"]')?.addEventListener("click", () => {
                editId = item.id;
                if (titleInput) titleInput.value = item.title || "";
                if (slugInput) slugInput.value = item.slug || "";
                uploadUrl = item.pdfFileUrl || "";
                statusLabel && (statusLabel.textContent = "در حالت ویرایش هستید.");
                slugEdited = true; // جلوگیری از تغییر خودکار هنگام تایپ عنوان در حالت ویرایش
            });

            card.querySelector<HTMLButtonElement>('[data-action="delete"]')?.addEventListener("click", async () => {
                const ok = window.confirm(`حذف بروشور "${item.title}"؟`);
                if (!ok) return;
                const res = await apiFetch<boolean>(`${endpoints.brochures}/${item.id}`, { method: "DELETE" });
                if (!res.isSuccess) {
                    toast.show(res.message || "حذف بروشور انجام نشد.", "error");
                    return;
                }
                toast.show("بروشور حذف شد.", "success");
                await loadBrochures();
            });

            list.appendChild(card);
        });
    };

    const loadBrochures = async () => {
        setButtonLoading(refreshButton, true, "در حال بارگذاری...");
        const res = await apiFetch<BrochureFile[]>(endpoints.brochuresPaged, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ arg: { pageNumber: 1, pageSize: 30 }, filters: [], sortField: "createdAt", ascending: false }),
        });
        setButtonLoading(refreshButton, false);
        if (!res.isSuccess || !Array.isArray(res.model)) {
            toast.show(res.message || "لیست بروشورها دریافت نشد.", "error");
            return;
        }
        renderBrochures(res.model);
    };

    refreshButton?.addEventListener("click", loadBrochures);

    pdfInput?.addEventListener("change", async (event) => {
        const target = event.target as HTMLInputElement;
        const file = target.files?.[0];
        if (!file) return;
        if (file.type !== "application/pdf" && !file.name.toLowerCase().endsWith(".pdf")) {
            toast.show("فقط فایل PDF مجاز است.", "error");
            target.value = "";
            return;
        }
        statusLabel && (statusLabel.textContent = "در حال بارگذاری PDF...");
        const formData = new FormData();
        formData.append("Files", file);
        formData.append("Filepath", "files/brochures");
        const res = await apiFetch<string[]>(endpoints.upload, {
            method: "POST",
            body: formData,
        });
        if (!res.isSuccess || !Array.isArray(res.model) || !res.model.length) {
            statusLabel && (statusLabel.textContent = res.message || "بارگذاری فایل انجام نشد.");
            statusLabel && statusLabel.classList.add("text-red-600");
            toast.show(res.message || "بارگذاری فایل انجام نشد.", "error");
            return;
        }
        uploadUrl = res.model[0];
        statusLabel && statusLabel.classList.remove("text-red-600");
        statusLabel && (statusLabel.textContent = "فایل PDF بارگذاری شد.");
        toast.show("فایل بروشور بارگذاری شد.", "success");
    });

    form?.addEventListener("submit", async (event) => {
        event.preventDefault();
        const title = titleInput?.value.trim();
        if (!title) {
            toast.show("عنوان بروشور را وارد کنید.", "error");
            titleInput?.focus();
            return;
        }
        if (!uploadUrl) {
            toast.show("فایل PDF را بارگذاری کنید.", "error");
            pdfInput?.focus();
            return;
        }
        const finalSlug = slugify((slugInput?.value && slugInput.value.trim()) || title);
        if (slugInput) slugInput.value = finalSlug;
        const payload: any = {
            title,
            pdfFileUrl: uploadUrl,
            slug: finalSlug,
        };
        const isEdit = !!editId;
        setButtonLoading(saveButton, true, isEdit ? "در حال بروزرسانی..." : "در حال ثبت...");
        const res = await apiFetch<number>(isEdit ? `${endpoints.brochures}/${editId}` : endpoints.brochures, {
            method: isEdit ? "PUT" : "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload),
        });
        setButtonLoading(saveButton, false);
        if (!res.isSuccess) {
            toast.show(res.message || "ذخیره بروشور انجام نشد.", "error");
            return;
        }
        toast.show(isEdit ? "بروشور بروزرسانی شد." : "بروشور ثبت شد.", "success");
        editId = null;
        uploadUrl = "";
        if (titleInput) titleInput.value = "";
        if (slugInput) slugInput.value = "";
        if (pdfInput) pdfInput.value = "";
        statusLabel && (statusLabel.textContent = "بروشور جدید اضافه کنید.");
        await loadBrochures();
    });

    return { loadBrochures };
};

const managementModal = (toast: ToastManager, categoryStore: CategoryStore) => {
    const modal = document.querySelector<HTMLElement>('[data-modal="management"]');
    const openButton = document.querySelector<HTMLButtonElement>('[data-open-modal="management"]');
    const closeButtons = modal?.querySelectorAll<HTMLButtonElement>("[data-close-modal]") || [];
    const tabs = modal?.querySelectorAll<HTMLButtonElement>("[data-admin-tab]") || [];
    const sections = modal?.querySelectorAll<HTMLElement>("[data-admin-section]") || [];

    const categories = categoriesManager(toast, categoryStore);
    const submissions = submissionsManager(toast, categoryStore);
    const brochures = brochuresManager(toast);
    const settings = settingsManager(toast);

    const activateTab = (target: string) => {
        tabs.forEach((tab) => {
            const isActive = tab.dataset.adminTab === target;
            tab.classList.toggle("bg-white", isActive);
            tab.classList.toggle("text-emerald-700", isActive);
            tab.classList.toggle("shadow-sm", isActive);
            tab.classList.toggle("text-gray-500", !isActive);
            tab.classList.toggle("hover:text-gray-800", !isActive);
        });
        sections.forEach((section) => {
            section.classList.toggle("hidden", section.dataset.adminSection !== target);
        });

        if (target === "categories") {
            categories.renderList();
        } else if (target === "submissions") {
            submissions.hydrateCategories().then(() => submissions.loadSubmissions());
        } else if (target === "brochures") {
            brochures.loadBrochures();
        } else if (target === "settings") {
            settings.loadSettings();
        }
    };

    openButton?.addEventListener("click", async () => {
        toggleHidden(modal, true);
        await categoryStore.load();
        categories.renderList();
        submissions.hydrateCategories();
        activateTab("categories");
    });

    closeButtons.forEach((btn) =>
        btn.addEventListener("click", () => {
            toggleHidden(modal, false);
        }),
    );

    tabs.forEach((tab) =>
        tab.addEventListener("click", () => {
            activateTab(tab.dataset.adminTab || "categories");
        }),
    );
};

const initIndexAdmin = () => {
    const root = document.querySelector<HTMLElement>("[data-index-admin]");
    if (!root) return;

    const toast = new ToastManager();
    const categoryStore = new CategoryStore(toast);

    quickSubmission(toast, categoryStore);
    managementModal(toast, categoryStore);
};

document.addEventListener("DOMContentLoaded", initIndexAdmin);
