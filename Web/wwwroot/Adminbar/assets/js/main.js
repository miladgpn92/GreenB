import Swal from 'sweetalert2'
import { enableEdit, cancelEdit, saveEdit, uploadImage, toggleEditMode } from './editable-content';

if (document.getElementById('delete-btn')) {
    document.getElementById('delete-btn').addEventListener('click', function () {

        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        const button = this;
        const id = button.getAttribute('data-id');
        const route = button.getAttribute('data-route');

        Swal.fire({
            title: 'آیا از حذف مطمئن هستید',
            text: "این عمل غیر قابل بازگشت است",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'بله حذف شود',
            cancelButtonText: 'لغو'
        }).then((result) => {
            if (result.isConfirmed) {
                // Send delete request
                const data = new FormData();
                data.append("id", id);

                fetch(`/dtcms/${route}?handler=Delete`, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': token // Send Anti-Forgery Token
                    },
                    body: data
                })
                    .then(response => {
                        if (response.ok) {
                            Swal.fire({
                                title: 'حذف شد',
                                text: 'عملیات حذف با موفقیت انجام شد',
                                icon: 'success',
                                showCancelButton: false,
                                confirmButtonText: 'تایید',
                            }




                            ).then((res) => {
                                if (res.isConfirmed) {
                                    window.location.href = '/';
                                }
                            });
                        } else {
                            Swal.fire(
                                'خطا',
                                'متاسفانه خطایی رخ داده مجدد تلاش کنید',
                                'error'
                            );
                        }
                    })
                    .catch(error => {
                        Swal.fire(
                            'خطا',
                            'متاسفانه خطایی رخ داده مجدد تلاش کنید',
                            'error'
                        );
                        console.error("Error:", error);
                    });
            }
        });
    });
}

 

// Expose functions globally
window.enableEdit = enableEdit;
window.cancelEdit = cancelEdit;
window.saveEdit = saveEdit;
window.uploadImage = uploadImage;

window.toggleEditMode = toggleEditMode;

 