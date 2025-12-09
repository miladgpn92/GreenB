export function enableEdit(element) {
    // Get the parent container that holds both the text and input elements
    let parentContainer = element.closest(".editable-text") || element.closest(".editable-image");

    if (!parentContainer) {
        console.error("Error: Could not find parent container.");
        return;
    }

    let textElement = parentContainer.querySelector(".text-content");
    let inputElement = parentContainer.querySelector(".edit-input");

    if (!textElement || !inputElement) {
        console.error("Error: Could not find textElement or inputElement.");
        return;
    }

    // Copy text into input and toggle visibility
    inputElement.value = textElement.innerText;
    textElement.style.display = "none";
    inputElement.style.display = "flex";

    // Hide edit button, show save & cancel buttons
    let editControls = parentContainer.querySelector(".edit-controls");
    if (editControls) {
        editControls.querySelector(".edit-icon").style.display = "none";
        editControls.querySelector(".save-icon").style.display = "flex";
        editControls.querySelector(".cancel-icon").style.display = "flex";
    }
}

export function cancelEdit(element) {
    let parentContainer = element.closest(".editable-text") || element.closest(".editable-image");

    if (!parentContainer) {
        console.error("Error: Could not find parent container.");
        return;
    }

    let textElement = parentContainer.querySelector(".text-content");
    let inputElement = parentContainer.querySelector(".edit-input");

    if (!textElement || !inputElement) {
        console.error("Error: Could not find textElement or inputElement.");
        return;
    }

    // Restore original text
    inputElement.value = textElement.innerText;

    // Toggle visibility back
    textElement.style.display = "flex";
    inputElement.style.display = "none";

    // Restore button visibility
    let editControls = parentContainer.querySelector(".edit-controls");
    if (editControls) {
        editControls.querySelector(".edit-icon").style.display = "flex";
        editControls.querySelector(".save-icon").style.display = "none";
        editControls.querySelector(".cancel-icon").style.display = "none";
    }
}

export function saveEdit(element) {
    let parentContainer = element.closest(".editable-text") || element.closest(".editable-image");

    if (!parentContainer) {
        console.error("Error: Could not find parent container.");
        return;
    }

    let textElement = parentContainer.querySelector(".text-content");
    let inputElement = parentContainer.querySelector(".edit-input");

    if (!textElement || !inputElement) {
        console.error("Error: Could not find textElement or inputElement.");
        return;
    }

    let contentId = textElement.dataset.id;
    let newValue = inputElement.value;

    fetch("/api/v1/Content/update", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ id: contentId, newValue })
    })
        .then(response => response.json())
        .then(data => {
            if (data.isSuccess) {
                textElement.innerText = newValue;
                cancelEdit(element);
            } else {
                alert("Error: Update failed.");
            }
        })
        .catch(error => {
            console.error("API Error:", error);
            alert("Error: Could not update content.");
        });
}

export function uploadImage(element) {
    let parentContainer = element.closest(".editable-image");
    
    if (!parentContainer) {
        console.error("Error: Could not find parent container.");
        return;
    }
    
    let fileInput = parentContainer.querySelector('input[type="file"]');

    if (!fileInput) {
        console.error("File input not found.");
        return;
    }

    fileInput.click(); // Open file browser

    fileInput.onchange = function () {
        let file = fileInput.files[0];
        if (!file) return;

        let formData = new FormData();
        formData.append("Files", file);
        formData.append("Filepath", "temp"); // Ensure correct API format

        fetch("/api/v1/Filemanager/Uplaod", {
            method: "POST",
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                if (data.isSuccess && data.data.length > 0) {
                    let imageUrl = data.data[0];

                    // Find the associated image and update its source
                    let imgElement = parentContainer.querySelector("img");

                    if (imgElement) {
                        imgElement.src = imageUrl;
                        saveImageEdit(imgElement, imageUrl);
                    } else {
                        console.error("Image element not found.");
                    }
                } else {
                    alert("Upload failed: " + data.message);
                }
            })
            .catch(error => {
                console.error("Upload error:", error);
                alert("Error: Could not upload image.");
            });
    };
}

function saveImageEdit(imgElement, newImageUrl) {
    let contentId = imgElement.dataset.id;

    fetch("/api/v1/Content/update", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ id: contentId, newValue: newImageUrl })
    })
        .then(response => response.json())
        .then(data => {
            if (data.isSuccess) {
                console.log("Image URL updated successfully.");
            } else {
                alert("Error: Failed to update image.");
            }
        })
        .catch(error => {
            console.error("API Error:", error);
            alert("Error: Could not update image URL.");
        });
}


export function toggleEditMode() {
    const isDisabled = document.cookie.includes('content_edit_disabled=true');
    const expiryDate = new Date();
    expiryDate.setFullYear(expiryDate.getFullYear() + 1);

    document.cookie = `content_edit_disabled=${!isDisabled}; expires=${expiryDate.toUTCString()}; path=/`;
    window.location.reload();
}