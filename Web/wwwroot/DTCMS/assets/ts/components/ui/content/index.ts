interface ContentItem {
    id: number;
    type: string;
    value: string;
}

interface ContentPage {
    page: string;
    content: ContentItem[];
}

class ContentManager {
    private pages: ContentPage[] = [];
    private currentPage: string | null = null;
    private isEditMode: boolean = false;
    private editingPageIndex: number = -1;
    private editingContentIndex: number = -1;

    constructor() {
        this.pages = []; // اطمینان از اینکه pages همیشه یک آرایه است
        this.initEventListeners();
        this.loadData();
    }

    private initEventListeners(): void {
        // Page related events
        document.getElementById('add-page-btn')?.addEventListener('click', () => this.showPageModal());
        document.getElementById('page-modal-cancel')?.addEventListener('click', () => this.hidePageModal());
        document.getElementById('page-modal-save')?.addEventListener('click', () => this.savePage());

        // Content related events
        document.getElementById('add-content-btn')?.addEventListener('click', () => this.showContentModal());
        document.getElementById('content-modal-cancel')?.addEventListener('click', () => this.hideContentModal());
        document.getElementById('content-modal-save')?.addEventListener('click', () => this.saveContent());

        // Import/Export events
        document.getElementById('import-btn')?.addEventListener('click', () => this.showImportModal());
        document.getElementById('export-btn')?.addEventListener('click', () => this.showExportModal());
        document.getElementById('import-export-modal-cancel')?.addEventListener('click', () => this.hideImportExportModal());
        document.getElementById('import-export-modal-save')?.addEventListener('click', () => this.saveImportExport());
    }

    private async loadData(): Promise<void> {
        if (document.getElementById('content__manager__module')) {
            console.log('ContentManager: Loading data from API...');
            try {
                const response = await fetch('/api/v1/Content/all');
                console.log('API Response:', response);

                if (!response.ok) {
                    throw new Error(`API returned status ${response.status}`);
                }

                const result = await response.json();
                console.log('API Result:', result);

                if (result.isSuccess) {
                    // بررسی ساختار پاسخ API
                    if (result.data && result.data.isSuccess && Array.isArray(result.data.data)) {
                        // ساختار جدید: result.data.data
                        this.pages = result.data.data;
                    } else if (Array.isArray(result.data)) {
                        // ساختار قدیمی: result.data
                        this.pages = result.data;
                    } else {
                        console.error('API returned unexpected data structure:', result);
                        this.pages = [];
                    }

                    console.log('Loaded pages:', this.pages);
                    this.renderPages();
                } else {
                    console.error('API returned error:', result.message);
                    this.showError('خطا در بارگذاری داده‌ها: ' + result.message);
                    this.pages = [];
                    this.renderPages();
                }
            }
            catch (error) {
                console.error('Error loading data:', error);
                this.showError('خطا در بارگذاری داده‌ها: ' + (error as Error).message);
                this.pages = [];
                this.renderPages();
            }
        }
       
    }

    private renderPages(): void {
        console.log('Rendering pages:', this.pages);
        
        const container = document.getElementById('pages-container');
        if (!container) {
            console.error('pages-container not found');
            return;
        }

        container.innerHTML = '';

        if (!Array.isArray(this.pages)) {
            console.error('this.pages is not an array:', this.pages);
            this.pages = [];
        }

        if (this.pages.length === 0) {
            console.log('No pages to render');
            container.innerHTML = '<div class="text-gray-500 text-center py-4">هیچ صفحه‌ای یافت نشد. برای شروع یک صفحه جدید اضافه کنید.</div>';
            return;
        }

        this.pages.forEach((page, index) => {
            const pageElement = document.createElement('div');
            pageElement.className = 'bg-gray-50 rounded-md p-4 flex justify-between items-center';
            pageElement.innerHTML = `
                <div class="font-medium text-gray-800">${page.page}</div>
                <div class="flex space-x-2 space-x-reverse">
                    <button class="edit-page-btn text-blue-500 hover:text-blue-700" data-index="${index}">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                        </svg>
                    </button>
                    <button class="delete-page-btn text-red-500 hover:text-red-700" data-index="${index}">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                        </svg>
                    </button>
                    <button class="view-page-btn text-green-500 hover:text-green-700" data-page="${page.page}">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                        </svg>
                    </button>
                </div>
            `;
            container.appendChild(pageElement);
        });

        // Add event listeners to the newly created buttons
        document.querySelectorAll('.edit-page-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const index = parseInt((e.currentTarget as HTMLElement).dataset.index || '-1');
                this.editPage(index);
            });
        });

        document.querySelectorAll('.delete-page-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const index = parseInt((e.currentTarget as HTMLElement).dataset.index || '-1');
                this.deletePage(index);
            });
        });

        document.querySelectorAll('.view-page-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const pageName = (e.currentTarget as HTMLElement).dataset.page || '';
                this.viewPage(pageName);
            });
        });
    }

    private renderContentItems(): void {
        const container = document.getElementById('content-items-table');
        const contentContainer = document.getElementById('content-items-container');
        const currentPageNameElement = document.getElementById('current-page-name');
        
        if (!container || !contentContainer || !currentPageNameElement || !this.currentPage) return;

        contentContainer.classList.remove('hidden');
        currentPageNameElement.textContent = this.currentPage;
        container.innerHTML = '';

        const pageIndex = this.pages.findIndex(p => p.page === this.currentPage);
        if (pageIndex === -1) return;

        const contentItems = this.pages[pageIndex].content;

        if (contentItems.length === 0) {
            container.innerHTML = `
                <tr>
                    <td colspan="5" class="py-4 text-center text-gray-500">هیچ محتوایی یافت نشد. برای شروع یک محتوای جدید اضافه کنید.</td>
                </tr>
            `;
            return;
        }

        contentItems.forEach((item, index) => {
            const row = document.createElement('tr');
            row.className = index % 2 === 0 ? 'bg-white' : 'bg-gray-50';
            
            // Truncate value for display
            let displayValue = item.value;
            if (displayValue.length > 50) {
                displayValue = displayValue.substring(0, 50) + '...';
            }
            
            row.innerHTML = `
                <td class="py-3 px-4 border-b">${item.id}</td>
                <td class="py-3 px-4 border-b">${this.getTypeLabel(item.type)}</td>
                <td class="py-3 px-4 border-b">${displayValue}</td>
                <td class="py-3 px-4 border-b">
                    <div class="flex space-x-2 space-x-reverse">
                        <button class="edit-content-btn text-blue-500 hover:text-blue-700" data-index="${index}">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                            </svg>
                        </button>
                        <button class="delete-content-btn text-red-500 hover:text-red-700" data-index="${index}">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                            </svg>
                        </button>
                    </div>
                </td>
            `;
            container.appendChild(row);
        });

        // Add event listeners to the newly created buttons
        document.querySelectorAll('.edit-content-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const index = parseInt((e.currentTarget as HTMLElement).dataset.index || '-1');
                this.editContent(index);
            });
        });

        document.querySelectorAll('.delete-content-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const index = parseInt((e.currentTarget as HTMLElement).dataset.index || '-1');
                this.deleteContent(index);
            });
        });
    }

    private getTypeLabel(type: string): string {
        switch (type) {
            case 'text': return 'متن';
            case 'textarea': return 'متن چند خطی';
            case 'img': return 'تصویر';
            case 'html': return 'HTML';
            default: return type;
        }
    }

    private showPageModal(): void {
        const modal = document.getElementById('page-modal');
        const titleElement = document.getElementById('page-modal-title');
        const nameInput = document.getElementById('page-name') as HTMLInputElement;
        
        if (!modal || !titleElement || !nameInput) return;
        
        if (this.isEditMode) {
            titleElement.textContent = 'ویرایش صفحه';
            const page = this.pages[this.editingPageIndex];
            nameInput.value = page.page;
        } else {
            titleElement.textContent = 'افزودن صفحه جدید';
            nameInput.value = '';
        }
        
        modal.classList.remove('hidden');
    }

    private hidePageModal(): void {
        const modal = document.getElementById('page-modal');
        if (!modal) return;
        
        modal.classList.add('hidden');
        this.isEditMode = false;
        this.editingPageIndex = -1;
    }

    private savePage(): void {
        const nameInput = document.getElementById('page-name') as HTMLInputElement;
        if (!nameInput) return;
        
        const pageName = nameInput.value.trim();
        if (!pageName) {
            this.showError('نام صفحه نمی‌تواند خالی باشد');
            return;
        }
        
        if (this.isEditMode) {
            // Edit existing page
            if (this.editingPageIndex >= 0 && this.editingPageIndex < this.pages.length) {
                // Check if the new name already exists (except for the current page)
                const nameExists = this.pages.some((p, idx) => idx !== this.editingPageIndex && p.page === pageName);
                if (nameExists) {
                    this.showError('صفحه‌ای با این نام قبلاً وجود دارد');
                    return;
                }
                
                this.pages[this.editingPageIndex].page = pageName;
                
                // If this was the current page, update currentPage
                if (this.currentPage === this.pages[this.editingPageIndex].page) {
                    this.currentPage = pageName;
                }
            }
        } else {
            // Add new page
            // Check if the name already exists
            const nameExists = this.pages.some(p => p.page === pageName);
            if (nameExists) {
                this.showError('صفحه‌ای با این نام قبلاً وجود دارد');
                return;
            }
            
            this.pages.push({
                page: pageName,
                content: []
            });
        }
        
        this.saveData();
        this.renderPages();
        this.hidePageModal();
    }

    private editPage(index: number): void {
        if (index < 0 || index >= this.pages.length) return;
        
        this.isEditMode = true;
        this.editingPageIndex = index;
        this.showPageModal();
    }

    private deletePage(index: number): void {
        if (index < 0 || index >= this.pages.length) return;
        
        if (!confirm(`آیا از حذف صفحه "${this.pages[index].page}" اطمینان دارید؟`)) {
            return;
        }
        
        // If this was the current page, clear currentPage
        if (this.currentPage === this.pages[index].page) {
            this.currentPage = null;
            document.getElementById('content-items-container')?.classList.add('hidden');
        }
        
        this.pages.splice(index, 1);
        this.saveData();
        this.renderPages();
    }

    private viewPage(pageName: string): void {
        this.currentPage = pageName;
        this.renderContentItems();
    }

    private showContentModal(): void {
        const modal = document.getElementById('content-modal');
        const titleElement = document.getElementById('content-modal-title');
        const idInput = document.getElementById('content-id') as HTMLInputElement;
        const typeSelect = document.getElementById('content-type') as HTMLSelectElement;
        const valueTextarea = document.getElementById('content-value') as HTMLTextAreaElement;
        
        if (!modal || !titleElement || !idInput || !typeSelect || !valueTextarea) return;
        
        if (this.isEditMode && this.currentPage) {
            titleElement.textContent = 'ویرایش محتوا';
            
            const pageIndex = this.pages.findIndex(p => p.page === this.currentPage);
            if (pageIndex === -1) return;
            
            const contentItem = this.pages[pageIndex].content[this.editingContentIndex];
            idInput.value = contentItem.id.toString();
            typeSelect.value = contentItem.type;
            valueTextarea.value = contentItem.value;
            
            // Disable ID field in edit mode
            idInput.disabled = true;
        } else {
            titleElement.textContent = 'افزودن محتوای جدید';
            
            // Generate a new unique ID
            const maxId = this.getMaxContentId();
            idInput.value = (maxId + 1).toString();
            typeSelect.value = 'text';
            valueTextarea.value = '';
            
            // Enable ID field in add mode
            idInput.disabled = false;
        }
        
        modal.classList.remove('hidden');
    }

    private hideContentModal(): void {
        const modal = document.getElementById('content-modal');
        if (!modal) return;
        
        modal.classList.add('hidden');
        this.isEditMode = false;
        this.editingContentIndex = -1;
    }

    private getMaxContentId(): number {
        let maxId = 0;
        this.pages.forEach(page => {
            page.content.forEach(item => {
                if (item.id > maxId) {
                    maxId = item.id;
                }
            });
        });
        return maxId;
    }

    private saveContent(): void {
        if (!this.currentPage) return;
        
        const idInput = document.getElementById('content-id') as HTMLInputElement;
        const typeSelect = document.getElementById('content-type') as HTMLSelectElement;
        const valueTextarea = document.getElementById('content-value') as HTMLTextAreaElement;
        
        if (!idInput || !typeSelect || !valueTextarea) return;
        
        const id = parseInt(idInput.value);
        const type = typeSelect.value;
        const value = valueTextarea.value;
        
        if (isNaN(id) || id <= 0) {
            this.showError('شناسه باید یک عدد مثبت باشد');
            return;
        }
        
        const pageIndex = this.pages.findIndex(p => p.page === this.currentPage);
        if (pageIndex === -1) return;
        
        if (this.isEditMode) {
            // Edit existing content
            if (this.editingContentIndex >= 0 && this.editingContentIndex < this.pages[pageIndex].content.length) {
                this.pages[pageIndex].content[this.editingContentIndex] = {
                    id,
                    type,
                    value
                };
            }
        } else {
            // Add new content
            // Check if the ID already exists
            const idExists = this.pages.some(p => 
                p.content.some(c => c.id === id)
            );
            if (idExists) {
                this.showError('محتوایی با این شناسه قبلاً وجود دارد');
                return;
            }
            
            this.pages[pageIndex].content.push({
                id,
                type,
                value
            });
        }
        
        this.saveData();
        this.renderContentItems();
        this.hideContentModal();
    }

    private editContent(index: number): void {
        if (!this.currentPage) return;
        
        const pageIndex = this.pages.findIndex(p => p.page === this.currentPage);
        if (pageIndex === -1) return;
        
        if (index < 0 || index >= this.pages[pageIndex].content.length) return;
        
        this.isEditMode = true;
        this.editingContentIndex = index;
        this.showContentModal();
    }

    private deleteContent(index: number): void {
        if (!this.currentPage) return;
        
        const pageIndex = this.pages.findIndex(p => p.page === this.currentPage);
        if (pageIndex === -1) return;
        
        if (index < 0 || index >= this.pages[pageIndex].content.length) return;
        
        const contentItem = this.pages[pageIndex].content[index];
        if (!confirm(`آیا از حذف محتوای "${contentItem.id}" اطمینان دارید؟`)) {
            return;
        }
        
        this.pages[pageIndex].content.splice(index, 1);
        this.saveData();
        this.renderContentItems();
    }

    private showImportModal(): void {
        const modal = document.getElementById('import-export-modal');
        const titleElement = document.getElementById('import-export-modal-title');
        const contentTextarea = document.getElementById('import-export-content') as HTMLTextAreaElement;
        const saveButton = document.getElementById('import-export-modal-save');
        
        if (!modal || !titleElement || !contentTextarea || !saveButton) return;
        
        titleElement.textContent = 'ورود اطلاعات';
        contentTextarea.value = '';
        saveButton.textContent = 'وارد کردن';
        
        modal.classList.remove('hidden');
    }

    private showExportModal(): void {
        const modal = document.getElementById('import-export-modal');
        const titleElement = document.getElementById('import-export-modal-title');
        const contentTextarea = document.getElementById('import-export-content') as HTMLTextAreaElement;
        const saveButton = document.getElementById('import-export-modal-save');
        
        if (!modal || !titleElement || !contentTextarea || !saveButton) return;
        
        titleElement.textContent = 'خروجی اطلاعات';
        contentTextarea.value = JSON.stringify(this.pages, null, 2);
        saveButton.textContent = 'کپی';
        
        modal.classList.remove('hidden');
    }

    private hideImportExportModal(): void {
        const modal = document.getElementById('import-export-modal');
        if (!modal) return;
        
        modal.classList.add('hidden');
    }

    private saveImportExport(): void {
        const titleElement = document.getElementById('import-export-modal-title');
        const contentTextarea = document.getElementById('import-export-content') as HTMLTextAreaElement;
        
        if (!titleElement || !contentTextarea) return;
        
        if (titleElement.textContent === 'ورود اطلاعات') {
            // Import
            try {
                const importedData = JSON.parse(contentTextarea.value);
                
                if (!Array.isArray(importedData)) {
                    throw new Error('داده‌های وارد شده باید یک آرایه باشند');
                }
                
                // Validate the structure
                for (const page of importedData) {
                    if (!page.page || !Array.isArray(page.content)) {
                        throw new Error('ساختار داده‌ها نامعتبر است');
                    }
                    
                    for (const item of page.content) {
                        if (!item.id || !item.type || item.value === undefined) {
                            throw new Error('ساختار محتوا نامعتبر است');
                        }
                    }
                }
                
                this.pages = importedData;
                this.saveData();
                this.renderPages();
                
                if (this.currentPage) {
                    // Check if the current page still exists
                    const pageExists = this.pages.some(p => p.page === this.currentPage);
                    if (pageExists) {
                        this.renderContentItems();
                    } else {
                        this.currentPage = null;
                        document.getElementById('content-items-container')?.classList.add('hidden');
                    }
                }
                
                this.hideImportExportModal();
                this.showSuccess('داده‌ها با موفقیت وارد شدند');
            } catch (error) {
                this.showError('خطا در وارد کردن داده‌ها: ' + (error as Error).message);
            }
        } else {
            // Export (Copy to clipboard)
            contentTextarea.select();
            document.execCommand('copy');
            this.showSuccess('داده‌ها در کلیپ‌بورد کپی شدند');
        }
    }

    private async saveData(): Promise<void> {
        try {
            const response = await fetch('/api/v1/Content/import', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(this.pages)
            });
            
            const result = await response.json();
            
            if (!result.isSuccess) {
                this.showError('خطا در ذخیره داده‌ها: ' + result.message);
            }
        } catch (error) {
            console.error('Error saving data:', error);
            this.showError('خطا در ذخیره داده‌ها');
        }
    }

    private showError(message: string): void {
        alert(message); // Replace with a better UI notification
    }

    private showSuccess(message: string): void {
        alert(message); // Replace with a better UI notification
    }
}

// تعریف متدهای کلاس به عنوان توابع عمومی در window
(window as any).ContentManager = ContentManager;
(window as any).initContentManager = function() {
    console.log('Initializing ContentManager');
    return new ContentManager();
};

// ایجاد یک نمونه از کلاس هنگام بارگذاری صفحه
document.addEventListener('DOMContentLoaded', () => {
    console.log('DOM loaded, initializing ContentManager');
    (window as any).contentManagerInstance = new ContentManager();
});

// Export کلاس برای استفاده در سایر ماژول‌ها
export default ContentManager;
