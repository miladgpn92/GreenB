import OpenAI from "openai";
import tinymce from "tinymce";
import { marked } from "marked";
import Typed from 'typed.js';

// متغیرهای عمومی
let conversationHistory: { role: "user" | "assistant" | "system", content: string }[] = [];
let currentPageSource: string = 'unknown'; // متغیر جدید برای نگهداری منبع درخواست
let currentChatGuid: string = '';

// Declare global augmentation for activeEditor
declare global {
    interface Window {
        activeEditor: string;
    }
}

// Close Bottom Sheet
document.getElementById("close-chat")?.addEventListener("click", function () {
    const bottomSheet = document.getElementById("ai-chat-bottom-sheet");
    if (bottomSheet) {
        bottomSheet.classList.add("hidden");
    }
});

function closeBottomSheet() {
    const bottomSheet = document.getElementById("ai-chat-bottom-sheet");
    if (bottomSheet) {
        bottomSheet.classList.add("hidden");
    }
}

// Show Bottom Sheet and set up event listeners - با پارامتر source
export function openBottomSheet(source = 'unknown') {
    // ایجاد GUID جدید برای چت جدید
    currentChatGuid = uuidv4();
    
    const bottomSheet = document.getElementById("ai-chat-bottom-sheet");
    const sendButton = document.getElementById("send-ai") as HTMLButtonElement;
    const inputField = document.getElementById("ai-input") as HTMLInputElement;
    const messagesContainer = document.getElementById("ai-messages") as HTMLDivElement;

    // ذخیره منبع درخواست
    currentPageSource = source;
    
    // ذخیره منبع درخواست به عنوان data-attribute
    if (bottomSheet) {
        bottomSheet.dataset.source = source;
        bottomSheet.classList.remove("hidden");
    }

    // Add event listener to Send button
    if (sendButton) {
        // حذف تمام event listenerهای قبلی
        sendButton.replaceWith(sendButton.cloneNode(true));
        
        // دریافت رفرنس جدید
        const newSendButton = document.getElementById("send-ai");
        
        if (newSendButton) {
            newSendButton.addEventListener("click", () => {
                const currentInput = document.getElementById("ai-input") as HTMLInputElement;
                const currentMessages = document.getElementById("ai-messages") as HTMLDivElement;
                if (currentInput && currentMessages) {
                    sendToAI(currentInput, currentMessages);
                }
            });
        }
    }

    // Allow pressing "Enter" to send the message
    if (inputField) {
        // حذف eventListener قبلی برای جلوگیری از تکرار
        const newInputField = inputField.cloneNode(true) as HTMLInputElement;
        inputField.parentNode?.replaceChild(newInputField, inputField);
        
        newInputField.addEventListener("keypress", function (e) {
            if (e.key === "Enter") {
                sendToAI(newInputField, messagesContainer);
            }
        });
        
        // فوکوس خودکار روی فیلد ورودی
        setTimeout(() => {
            newInputField.focus();
        }, 100);
    }
}

// Generate a unique ID for each message
function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'
        .replace(/[xy]/g, function (c) {
            const r = Math.random() * 16 | 0,
                v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
}

// تشخیص منبع درخواست (صفحه فعلی)
function getPageSource() {
    // ابتدا بررسی کنید که آیا منبع در متغیر عمومی یا در bottom sheet ذخیره شده است
    if (currentPageSource && currentPageSource !== 'unknown') {
        return currentPageSource;
    }
    
    const bottomSheet = document.getElementById("ai-chat-bottom-sheet");
    if (bottomSheet && bottomSheet.dataset.source) {
        return bottomSheet.dataset.source;
    }
    
    // تلاش برای تشخیص نوع صفحه فعلی از URL
    const currentPath = window.location.pathname;
    
    if (currentPath.includes('/articles/') || currentPath.includes('/blog/')) {
        return 'article';
    } else if (currentPath.includes('/products/') || currentPath.includes('/shop/')) {
        return 'product';
    } else if (currentPath.includes('/admin/') || currentPath.includes('/dashboard/')) {
        return 'admin';
    } else if (currentPath.includes('/page/')) {
        return 'page';
    } else if (currentPath.includes('/category/')) {
        return 'category';
    }
    
    // تلاش برای تشخیص از طریق چک کردن المان‌های موجود در صفحه
    if (document.getElementById('article-editor')) {
        return 'article-editor';
    } else if (document.getElementById('product-editor')) {
        return 'product-editor';
    } else if (document.getElementById('page-editor')) {
        return 'page-editor';
    }
    
    // اگر نمی‌توان تشخیص داد، از عنوان صفحه استفاده کنید
    return document.title || 'unknown';
}

// ذخیره گفتگو در سرور
async function saveConversation() {
    try {
        if (conversationHistory.length >= 2) {
            const userMessages = conversationHistory.filter(msg => msg.role === 'user');
            const firstUserMessage = userMessages.length > 0 ? userMessages[0].content : '';
            const title = firstUserMessage.substring(0, 50) + (firstUserMessage.length > 50 ? "..." : "");
            
            const pageSource = getPageSource();
            
            await fetch('/api/v1/AI/SaveConversation', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    title: title,
                    messages: conversationHistory,
                    pageSource: pageSource,
                    chatGuid: currentChatGuid,
                    createdAt: new Date().toISOString()
                })
            });
        }
    } catch (error) {
        console.error("Error saving conversation:", error);
    }
}

// AI interaction logic
async function sendToAI(inputField: HTMLInputElement, messagesContainer: HTMLDivElement) {
    const userMessageText = inputField.value.trim();

    if (!userMessageText) return;

    // حذف کلاس default__chat__empty__box
    const emptyBox = document.querySelector('.defualt__chat__empty__box');
    if (emptyBox) {
        emptyBox.remove();
    }

    // Show user message
    const userMessage = document.createElement("div");
    userMessage.className = "p-[16px] mb-[16px] rounded-lg border border-[#e9e9e9] ai__user__request";
    userMessage.textContent = userMessageText;
    messagesContainer.appendChild(userMessage);

    // Show thinking state
    const thinkingMessage = document.createElement("div");
    thinkingMessage.className = "p-2 text-xs text-[#515151] mb-2 flex items-center thinking-message";
    thinkingMessage.innerHTML = `
        <svg
  width="27"
  height="27"
  viewBox="0 0 27 27"
  fill="none"
  xmlns="http://www.w3.org/2000/svg"
  class="w-[27px] h-[27px]"
  preserveAspectRatio="xMidYMid meet"
>
  <rect width="27" height="27" rx="13.5" fill="url(#paint0_linear_3396_11698)"></rect>
  <path
    fill-rule="evenodd"
    clip-rule="evenodd"
    d="M17.3574 20.2684C17.2948 20.2686 17.2329 20.2825 17.1761 20.309C17.1194 20.3356 17.0691 20.3742 17.0288 20.4222C16.2087 21.3953 14.9661 22 13.5608 22C12.8118 22.0005 12.0738 21.8196 11.41 21.4728C10.7462 21.126 10.1762 20.6236 9.74873 20.0086C9.73707 19.9916 9.72148 19.9778 9.70329 19.9682C9.6851 19.9586 9.66486 19.9536 9.6443 19.9535C8.89416 19.9537 8.15515 19.7721 7.49051 19.4243C6.82587 19.0765 6.25538 18.5728 5.82787 17.9564C5.40036 17.34 5.12856 16.6292 5.03571 15.8848C4.94286 15.1405 5.03173 14.3847 5.29473 13.6821C5.37568 13.4636 5.37568 13.2215 5.29473 13.003C5.0319 12.3005 4.94315 11.5448 5.03608 10.8005C5.12901 10.0562 5.40085 9.34556 5.82834 8.72927C6.25583 8.11297 6.82625 7.60941 7.4908 7.26165C8.15535 6.91389 8.89426 6.73229 9.6443 6.73238C9.85478 6.73238 10.0563 6.6409 10.1964 6.48386C11.0164 5.56505 12.2234 5 13.5826 5C13.8967 5 14.5039 5.04695 15.1507 5.28414C15.6947 5.48329 16.3043 5.83219 16.7665 6.43367C16.8357 6.52515 16.925 6.59956 17.0274 6.6512C17.1298 6.70284 17.2427 6.73033 17.3574 6.73157C18.1193 6.7314 18.8695 6.91868 19.5419 7.2769C20.2142 7.63513 20.7881 8.15329 21.213 8.78569C21.6378 9.4181 21.9005 10.1453 21.9779 10.9032C22.0553 11.6611 21.945 12.4264 21.6568 13.1317C21.5605 13.3678 21.5605 13.6322 21.6568 13.8683C21.945 14.5736 22.0553 15.3389 21.9779 16.0968C21.9005 16.8547 21.6378 17.5819 21.213 18.2143C20.7881 18.8467 20.2142 19.3649 19.5419 19.7231C18.8695 20.0813 18.1193 20.2686 17.3574 20.2684ZM10.67 9.53576C10.6084 7.873 11.8818 6.57371 13.5818 6.57371C13.7664 6.57371 14.1809 6.6061 14.6083 6.76233C14.9936 6.90319 14.9378 7.38971 14.6083 7.63257C14.326 7.83923 14.0683 8.07742 13.8401 8.34252C13.7037 8.50054 13.6357 8.70625 13.6511 8.91439C13.6664 9.12252 13.7638 9.31605 13.9218 9.45238C14.0798 9.58871 14.2855 9.65669 14.4937 9.64136C14.7018 9.62603 14.8953 9.52864 15.0317 9.37062C15.4408 8.89645 15.9852 8.55877 16.5918 8.40289C17.1983 8.247 17.838 8.28037 18.4251 8.49853C19.0122 8.71668 19.5184 9.10917 19.876 9.62333C20.2336 10.1375 20.4253 10.7487 20.4255 11.375C20.4255 11.6964 20.0515 11.8291 19.7771 11.6616C19.0486 11.2156 18.2108 10.9802 17.3566 10.9816C16.9316 10.9816 16.518 11.0382 16.1253 11.1467C16.0257 11.174 15.9324 11.2207 15.8508 11.2841C15.7691 11.3475 15.7008 11.4263 15.6496 11.5161C15.5985 11.6059 15.5655 11.7049 15.5526 11.8074C15.5397 11.91 15.5471 12.014 15.5745 12.1137C15.6018 12.2134 15.6485 12.3067 15.7119 12.3883C15.7752 12.4699 15.8541 12.5382 15.9439 12.5894C16.0337 12.6406 16.1326 12.6735 16.2352 12.6864C16.3377 12.6993 16.4418 12.6919 16.5414 12.6646C17.2466 12.4701 17.998 12.5338 18.6604 12.8442C19.3227 13.1546 19.8524 13.6913 20.1541 14.3577C20.4558 15.0241 20.5097 15.7763 20.3059 16.4788C20.1022 17.1813 19.6543 17.788 19.0429 18.1896C18.5677 18.5037 18.0642 18.0074 18.0472 17.4367C18.0467 17.4265 18.0461 17.4162 18.0455 17.406C17.997 16.0864 17.4206 15.1838 16.6127 14.4876C16.0242 13.98 15.2705 13.5542 14.5476 13.1454C14.3541 13.0361 14.1631 12.9285 13.9777 12.82C13.7421 12.6824 13.5163 12.5553 13.3017 12.4339C12.6266 12.0518 12.0559 11.7296 11.5888 11.3313C11.0294 10.8553 10.6983 10.3299 10.6692 9.53576H10.67ZM12.489 18.9805C12.0397 19.3286 11.9353 20.0223 12.4655 20.2255C12.8055 20.355 13.1746 20.4255 13.56 20.4255C15.26 20.4255 16.5342 19.127 16.4726 17.4642C16.4427 16.6596 16.1245 16.1447 15.5854 15.68C15.1256 15.2834 14.5557 14.9612 13.8611 14.567C13.635 14.4394 13.4097 14.3104 13.1852 14.18C13.003 14.074 12.8128 13.9663 12.6185 13.857C11.9142 13.4603 11.1622 13.0361 10.568 12.5294C9.75359 11.8364 9.14563 10.9257 9.09706 9.59486C9.09554 9.55763 9.09446 9.52039 9.09382 9.48314C9.08573 8.92781 8.6162 8.42995 8.13211 8.70438C7.77631 8.90561 7.46431 9.17591 7.21444 9.4994C6.96456 9.82289 6.78185 10.193 6.67702 10.5881C6.5722 10.9832 6.54738 11.3953 6.60402 11.8001C6.66067 12.2049 6.79763 12.5943 7.00687 12.9455C7.31147 13.458 7.75882 13.8707 8.29421 14.1331C8.8296 14.3955 9.42984 14.4962 10.0215 14.4229C10.1241 14.4104 10.2282 14.4182 10.3277 14.4459C10.4273 14.4736 10.5204 14.5207 10.6018 14.5844C10.6832 14.6481 10.7512 14.7272 10.802 14.8172C10.8528 14.9072 10.8854 15.0063 10.8978 15.1089C10.9103 15.2115 10.9025 15.3156 10.8748 15.4151C10.8471 15.5147 10.8 15.6078 10.7363 15.6892C10.6726 15.7705 10.5935 15.8386 10.5035 15.8894C10.4135 15.9402 10.3144 15.9727 10.2118 15.9852C9.05458 16.1299 7.88549 15.8317 6.93887 15.1506C6.78992 15.043 6.57459 15.1271 6.57459 15.3109C6.57405 16.0259 6.82309 16.7186 7.27874 17.2696C7.73439 17.8205 8.36807 18.1952 9.07043 18.3288C9.77279 18.4625 10.4998 18.3468 11.126 18.0017C11.7521 17.6566 12.2382 17.1038 12.5003 16.4386C12.5383 16.3424 12.5948 16.2547 12.6666 16.1804C12.7385 16.1061 12.8243 16.0466 12.9191 16.0055C13.0139 15.9643 13.1159 15.9422 13.2193 15.9405C13.3227 15.9388 13.4254 15.9574 13.5215 15.9954C13.6177 16.0333 13.7054 16.0898 13.7797 16.1617C13.854 16.2335 13.9135 16.3193 13.9546 16.4142C13.9958 16.509 14.0179 16.611 14.0196 16.7144C14.0213 16.8177 14.0027 16.9204 13.9647 17.0166C13.6578 17.7917 13.1484 18.4702 12.4898 18.9813L12.489 18.9805Z"
    fill="white"
  ></path>
  <defs>
    <linearGradient
      id="paint0_linear_3396_11698"
      x1="-6.5"
      y1="2"
      x2="31.5"
      y2="24"
      gradientUnits="userSpaceOnUse"
    >
      <stop stop-color="#7A57FD"></stop>
      <stop offset="1" stop-color="#FF38B0"></stop>
    </linearGradient>
  </defs>
</svg>
        <span class='mr-2'>در حال فکر کردن...</span>
    `;
    messagesContainer.appendChild(thinkingMessage);

    inputField.value = ""; // Clear the input field

    // Add the user message to conversation history
    conversationHistory.push({ role: "user", content: userMessageText });

    // اسکرول به پایین
    messagesContainer.scrollTop = messagesContainer.scrollHeight;

    try {
        // دریافت منبع درخواست (صفحه فعلی) 
        const pageSource = getPageSource();
        
        // استفاده از API Chat
        const response = await fetch('/api/v1/AI/Chat', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                messages: conversationHistory,
                temperature: 0.7,
                maxTokens: 2000,
                pageSource: pageSource // ارسال منبع درخواست
            })
        });

        if (!response.ok) {
            throw new Error(`خطای سرور: ${response.status}`);
        }

        const chatCompletion = await response.json();
        if (!chatCompletion.data || !chatCompletion.data.choices) {
            throw new Error('پاسخ API در فرمت مورد انتظار نیست');
        }
        let aiText = chatCompletion.data.choices[0]?.message?.content || "No response";

        // Convert Markdown to HTML
        try {
            aiText = await marked(aiText);
        } catch (e) {
            console.error("Markdown conversion error:", e);
        }

        // Remove thinking state
        thinkingMessage.remove();

        // Generate a unique ID for the AI response
        const uuid = uuidv4();

        // ابتدا عنصر HTML را ایجاد کنید
        const aiMessage = document.createElement("div");
        aiMessage.className = "p-2 flex flex-col justify-between";
        aiMessage.innerHTML = `
            <div class='flex items-center text-xs text-[#515151] mb-[12px]'>
                <svg
  width="27"
  height="27"
  viewBox="0 0 27 27"
  fill="none"
  xmlns="http://www.w3.org/2000/svg"
  class="w-[27px] h-[27px]"
  preserveAspectRatio="xMidYMid meet"
>
  <rect width="27" height="27" rx="13.5" fill="url(#paint0_linear_3396_11698)"></rect>
  <path
    fill-rule="evenodd"
    clip-rule="evenodd"
    d="M17.3574 20.2684C17.2948 20.2686 17.2329 20.2825 17.1761 20.309C17.1194 20.3356 17.0691 20.3742 17.0288 20.4222C16.2087 21.3953 14.9661 22 13.5608 22C12.8118 22.0005 12.0738 21.8196 11.41 21.4728C10.7462 21.126 10.1762 20.6236 9.74873 20.0086C9.73707 19.9916 9.72148 19.9778 9.70329 19.9682C9.6851 19.9586 9.66486 19.9536 9.6443 19.9535C8.89416 19.9537 8.15515 19.7721 7.49051 19.4243C6.82587 19.0765 6.25538 18.5728 5.82787 17.9564C5.40036 17.34 5.12856 16.6292 5.03571 15.8848C4.94286 15.1405 5.03173 14.3847 5.29473 13.6821C5.37568 13.4636 5.37568 13.2215 5.29473 13.003C5.0319 12.3005 4.94315 11.5448 5.03608 10.8005C5.12901 10.0562 5.40085 9.34556 5.82834 8.72927C6.25583 8.11297 6.82625 7.60941 7.4908 7.26165C8.15535 6.91389 8.89426 6.73229 9.6443 6.73238C9.85478 6.73238 10.0563 6.6409 10.1964 6.48386C11.0164 5.56505 12.2234 5 13.5826 5C13.8967 5 14.5039 5.04695 15.1507 5.28414C15.6947 5.48329 16.3043 5.83219 16.7665 6.43367C16.8357 6.52515 16.925 6.59956 17.0274 6.6512C17.1298 6.70284 17.2427 6.73033 17.3574 6.73157C18.1193 6.7314 18.8695 6.91868 19.5419 7.2769C20.2142 7.63513 20.7881 8.15329 21.213 8.78569C21.6378 9.4181 21.9005 10.1453 21.9779 10.9032C22.0553 11.6611 21.945 12.4264 21.6568 13.1317C21.5605 13.3678 21.5605 13.6322 21.6568 13.8683C21.945 14.5736 22.0553 15.3389 21.9779 16.0968C21.9005 16.8547 21.6378 17.5819 21.213 18.2143C20.7881 18.8467 20.2142 19.3649 19.5419 19.7231C18.8695 20.0813 18.1193 20.2686 17.3574 20.2684ZM10.67 9.53576C10.6084 7.873 11.8818 6.57371 13.5818 6.57371C13.7664 6.57371 14.1809 6.6061 14.6083 6.76233C14.9936 6.90319 14.9378 7.38971 14.6083 7.63257C14.326 7.83923 14.0683 8.07742 13.8401 8.34252C13.7037 8.50054 13.6357 8.70625 13.6511 8.91439C13.6664 9.12252 13.7638 9.31605 13.9218 9.45238C14.0798 9.58871 14.2855 9.65669 14.4937 9.64136C14.7018 9.62603 14.8953 9.52864 15.0317 9.37062C15.4408 8.89645 15.9852 8.55877 16.5918 8.40289C17.1983 8.247 17.838 8.28037 18.4251 8.49853C19.0122 8.71668 19.5184 9.10917 19.876 9.62333C20.2336 10.1375 20.4253 10.7487 20.4255 11.375C20.4255 11.6964 20.0515 11.8291 19.7771 11.6616C19.0486 11.2156 18.2108 10.9802 17.3566 10.9816C16.9316 10.9816 16.518 11.0382 16.1253 11.1467C16.0257 11.174 15.9324 11.2207 15.8508 11.2841C15.7691 11.3475 15.7008 11.4263 15.6496 11.5161C15.5985 11.6059 15.5655 11.7049 15.5526 11.8074C15.5397 11.91 15.5471 12.014 15.5745 12.1137C15.6018 12.2134 15.6485 12.3067 15.7119 12.3883C15.7752 12.4699 15.8541 12.5382 15.9439 12.5894C16.0337 12.6406 16.1326 12.6735 16.2352 12.6864C16.3377 12.6993 16.4418 12.6919 16.5414 12.6646C17.2466 12.4701 17.998 12.5338 18.6604 12.8442C19.3227 13.1546 19.8524 13.6913 20.1541 14.3577C20.4558 15.0241 20.5097 15.7763 20.3059 16.4788C20.1022 17.1813 19.6543 17.788 19.0429 18.1896C18.5677 18.5037 18.0642 18.0074 18.0472 17.4367C18.0467 17.4265 18.0461 17.4162 18.0455 17.406C17.997 16.0864 17.4206 15.1838 16.6127 14.4876C16.0242 13.98 15.2705 13.5542 14.5476 13.1454C14.3541 13.0361 14.1631 12.9285 13.9777 12.82C13.7421 12.6824 13.5163 12.5553 13.3017 12.4339C12.6266 12.0518 12.0559 11.7296 11.5888 11.3313C11.0294 10.8553 10.6983 10.3299 10.6692 9.53576H10.67ZM12.489 18.9805C12.0397 19.3286 11.9353 20.0223 12.4655 20.2255C12.8055 20.355 13.1746 20.4255 13.56 20.4255C15.26 20.4255 16.5342 19.127 16.4726 17.4642C16.4427 16.6596 16.1245 16.1447 15.5854 15.68C15.1256 15.2834 14.5557 14.9612 13.8611 14.567C13.635 14.4394 13.4097 14.3104 13.1852 14.18C13.003 14.074 12.8128 13.9663 12.6185 13.857C11.9142 13.4603 11.1622 13.0361 10.568 12.5294C9.75359 11.8364 9.14563 10.9257 9.09706 9.59486C9.09554 9.55763 9.09446 9.52039 9.09382 9.48314C9.08573 8.92781 8.6162 8.42995 8.13211 8.70438C7.77631 8.90561 7.46431 9.17591 7.21444 9.4994C6.96456 9.82289 6.78185 10.193 6.67702 10.5881C6.5722 10.9832 6.54738 11.3953 6.60402 11.8001C6.66067 12.2049 6.79763 12.5943 7.00687 12.9455C7.31147 13.458 7.75882 13.8707 8.29421 14.1331C8.8296 14.3955 9.42984 14.4962 10.0215 14.4229C10.1241 14.4104 10.2282 14.4182 10.3277 14.4459C10.4273 14.4736 10.5204 14.5207 10.6018 14.5844C10.6832 14.6481 10.7512 14.7272 10.802 14.8172C10.8528 14.9072 10.8854 15.0063 10.8978 15.1089C10.9103 15.2115 10.9025 15.3156 10.8748 15.4151C10.8471 15.5147 10.8 15.6078 10.7363 15.6892C10.6726 15.7705 10.5935 15.8386 10.5035 15.8894C10.4135 15.9402 10.3144 15.9727 10.2118 15.9852C9.05458 16.1299 7.88549 15.8317 6.93887 15.1506C6.78992 15.043 6.57459 15.1271 6.57459 15.3109C6.57405 16.0259 6.82309 16.7186 7.27874 17.2696C7.73439 17.8205 8.36807 18.1952 9.07043 18.3288C9.77279 18.4625 10.4998 18.3468 11.126 18.0017C11.7521 17.6566 12.2382 17.1038 12.5003 16.4386C12.5383 16.3424 12.5948 16.2547 12.6666 16.1804C12.7385 16.1061 12.8243 16.0466 12.9191 16.0055C13.0139 15.9643 13.1159 15.9422 13.2193 15.9405C13.3227 15.9388 13.4254 15.9574 13.5215 15.9954C13.6177 16.0333 13.7054 16.0898 13.7797 16.1617C13.854 16.2335 13.9135 16.3193 13.9546 16.4142C13.9958 16.509 14.0179 16.611 14.0196 16.7144C14.0213 16.8177 14.0027 16.9204 13.9647 17.0166C13.6578 17.7917 13.1484 18.4702 12.4898 18.9813L12.489 18.9805Z"
    fill="white"
  ></path>
  <defs>
    <linearGradient
      id="paint0_linear_3398_11763"
      x1="-6.5"
      y1="2"
      x2="31.5"
      y2="24"
      gradientUnits="userSpaceOnUse"
    >
      <stop stop-color="#7A57FD"></stop>
      <stop offset="1" stop-color="#FF38B0"></stop>
    </linearGradient>
  </defs>
</svg>
                <span class='mr-2'>پاسخ آماده است ✅</span>
            </div>
            <div class="ai-response-text" id="ai-response-text_${uuid}"></div>
            <div id="insert-ai-response" class="flex justify-center cursor-pointer items-center w-[129px] relative overflow-hidden gap-1 pl-[13px] pr-[11px] py-2 rounded bg-indigo-600 mt-[18px] insert-ai-response__btn" style="box-shadow: 0px 1px 2px 0 rgba(0,0,0,0.05);" data-ai="${aiText}">
                <p class="flex-grow-0 flex-shrink-0 text-sm font-bold text-left text-white">افزودن متن</p>
                <svg width="17" height="16" viewBox="0 0 17 16" fill="none" xmlns="http://www.w3.org/2000/svg" class="flex-grow-0 flex-shrink-0 w-4 h-4 relative" preserveAspectRatio="xMidYMid meet">
                    <path d="M8.5 4V8M8.5 8V12M8.5 8H12.5M8.5 8L4.5 8" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"></path>
                </svg>
            </div>
        `;

        // افزودن المان به DOM
        messagesContainer.appendChild(aiMessage);

        // حالا می‌توانیم به المان دسترسی پیدا کنیم
        const responseContainer = document.getElementById(`ai-response-text_${uuid}`);
        const finalHTML = aiText; // محتوای HTML نهایی

        // نمایش تدریجی محتوا
        let htmlSections = [];
        let currentLength = 0;
        const sectionLength = 50; // تعداد کاراکتر هر بخش

        // تقسیم محتوا به بخش‌های قابل نمایش
        while (currentLength < finalHTML.length) {
            const endPos = Math.min(currentLength + sectionLength, finalHTML.length);
            htmlSections.push(finalHTML.substring(0, endPos));
            currentLength = endPos;
        }

        let sectionIndex = 0;
        function showNextSection() {
            if (sectionIndex < htmlSections.length) {
                // نمایش بخش بعدی با حفظ ساختار HTML
                responseContainer.innerHTML = htmlSections[sectionIndex];
                sectionIndex++;
                
                // اسکرول به پایین
                messagesContainer.scrollTop = messagesContainer.scrollHeight;
                
                // فاصله زمانی بین نمایش بخش‌ها
                setTimeout(showNextSection, 5); // سرعت را افزایش دادم
            }
        }

        // شروع نمایش تدریجی
        showNextSection();

        // Add the AI message to conversation history
        conversationHistory.push({ role: "assistant", content: aiText });

        // ذخیره گفتگو با منبع درخواست
        saveConversation();

        // Add event listener to the Insert button
        const insertButton = document.querySelectorAll(".insert-ai-response__btn");
        insertButton.forEach((button) => {
            button.addEventListener("click", function () {
                const BtnAiText = this.dataset.ai;
                tinymce.get(window.activeEditor).insertContent(BtnAiText);
                closeBottomSheet();
            });
        });

    } catch (error) {
        console.error("AI request error:", error);

        // حذف پیام thinking اگر وجود دارد
        const thinkingMessages = document.querySelectorAll('.thinking-message');
        thinkingMessages.forEach(msg => msg.remove());

        // حذف پیام خطای قبلی اگر وجود دارد
        const existingErrors = document.querySelectorAll('.ai-error-message');
        existingErrors.forEach(err => err.remove());

        // ایجاد پیام خطای جدید
        const errorMessage = document.createElement("div");
        errorMessage.className = "mb-2 ai-error-message";
        errorMessage.innerHTML = `
            <div class='mb-[16px]'>
                <svg width="27" height="27" viewBox="0 0 27 27" fill="none" xmlns="http://www.w3.org/2000/svg" class="w-[27px] h-[27px]" preserveAspectRatio="none">
                    <!-- ... SVG content ... -->
                </svg>
            </div>
            <div class='p-[16px] rounded-lg bg-[#fe1d53]/10 border border-[#e9e9e9]'>
                <p class="text-sm font-medium text-[#ff3131] mb-[8px]">خطایی رخ داده است مجدد تلاش کنید !</p>
                <div id="ai__send__request__agian" class="flex cursor-pointer justify-center items-center w-[129px] relative overflow-hidden gap-1 pl-[13px] pr-[11px] py-2 rounded bg-rose-600" style="box-shadow: 0px 1px 2px 0 rgba(0,0,0,0.05);" data-message="${userMessageText}">
                    <p class="flex-grow-0 flex-shrink-0 text-sm font-bold text-left text-white">تلاش مجدد</p>
                    <svg width="17" height="16" viewBox="0 0 17 16" fill="none" xmlns="http://www.w3.org/2000/svg" class="flex-grow-0 flex-shrink-0 w-4 h-4 relative" preserveAspectRatio="xMidYMid meet">
                        <path d="M3.1665 2.66699V6.00033H3.55418M13.7919 7.33366C13.4638 4.70278 11.2196 2.66699 8.49984 2.66699C6.26159 2.66699 4.34536 4.04577 3.55418 6.00033M3.55418 6.00033H6.49984M13.8332 13.3337V10.0003H13.4455M13.4455 10.0003C12.6543 11.9549 10.7381 13.3337 8.49984 13.3337C5.78009 13.3337 3.53583 11.2979 3.20777 8.66699M13.4455 10.0003H10.4998" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"></path>
                    </svg>
                </div>
            </div>
        `;
        messagesContainer.appendChild(errorMessage);

        // اضافه کردن event listener به دکمه تلاش مجدد
        const tryAgainButton = errorMessage.querySelector("#ai__send__request__agian");
        if (tryAgainButton) {
            console.log('Adding click event to retry button');
            tryAgainButton.addEventListener("click", function() {
                console.log('Retry button clicked');
                // بازیابی پیام اصلی از data-message
                const originalMessage = this.getAttribute('data-message');
                if (originalMessage) {
                    // تنظیم مجدد مقدار در input
                    if (inputField) {
                        inputField.value = originalMessage;
                    }
                    // حذف پیام خطای فعلی
                    errorMessage.remove();
                    // فراخوانی مجدد تابع با همان پارامترها
                    sendToAI(inputField, messagesContainer);
                } else {
                    console.error('Original message not found');
                }
            });
        }
    }
}

// بارگذاری تاریخچه گفتگوها برای کاربر فعلی
async function loadConversationHistory() {
    if(document.getElementById('chat__list')){
        try {
            const response = await fetch('/api/v1/AI/GetConversations');
            if (response.ok) {
                const conversations = await response.json();
                return conversations;
            }
        } catch (error) {
            console.error("Error loading conversation history:", error);
        }
        return [];
    }

}

// نمایش تاریخچه گفتگوها در رابط کاربری
function displayConversationHistory(conversations) {
    const historyContainer = document.getElementById('ai-history-container');
    if (!historyContainer || !conversations || conversations.length === 0) return;

    // پاک کردن محتویات موجود
    historyContainer.innerHTML = '';
    
    // افزودن عنوان
    const historyTitle = document.createElement('div');
    historyTitle.className = 'text-sm font-medium text-[#303030] mb-3';
    historyTitle.textContent = 'تاریخچه گفتگوها';
    historyContainer.appendChild(historyTitle);
    
    // افزودن هر گفتگو به لیست
    conversations.forEach(conversation => {
        const item = document.createElement('div');
        item.className = 'p-2 border-b border-gray-100 cursor-pointer hover:bg-gray-50 flex items-center justify-between';
        
        // آیکون متناسب با منبع درخواست
        let sourceIcon = '';
        switch (conversation.pageSource) {
            case 'article':
            case 'article-editor':
                sourceIcon = '<svg class="w-4 h-4 text-blue-500" fill="currentColor" viewBox="0 0 20 20"><path d="M4 4a2 2 0 012-2h8a2 2 0 012 2v12a2 2 0 01-2 2H6a2 2 0 01-2-2V4z"></path></svg>';
                break;
            case 'product':
            case 'product-editor':
                sourceIcon = '<svg class="w-4 h-4 text-green-500" fill="currentColor" viewBox="0 0 20 20"><path d="M4 3a2 2 0 100 4h12a2 2 0 100-4H4z"></path><path fill-rule="evenodd" d="M3 8h14v7a2 2 0 01-2 2H5a2 2 0 01-2-2V8zm5 3a1 1 0 011-1h2a1 1 0 110 2H9a1 1 0 01-1-1z" clip-rule="evenodd"></path></svg>';
                break;
            default:
                sourceIcon = '<svg class="w-4 h-4 text-gray-500" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-8-3a1 1 0 00-.867.5 1 1 0 11-1.731-1A3 3 0 0113 8a3.001 3.001 0 01-2 2.83V11a1 1 0 11-2 0v-1a1 1 0 011-1 1 1 0 100-2zm0 8a1 1 0 100-2 1 1 0 000 2z" clip-rule="evenodd"></path></svg>';
        }
        
        // عنوان گفتگو و تاریخ
        const date = new Date(conversation.createdAt);
        const formattedDate = `${date.toLocaleDateString('fa-IR')} - ${date.toLocaleTimeString('fa-IR', {hour: '2-digit', minute:'2-digit'})}`;
        
        item.innerHTML = `
            <div class="flex items-center gap-2">
                ${sourceIcon}
                <span class="text-sm">${conversation.title}</span>
            </div>
            <span class="text-xs text-gray-500">${formattedDate}</span>
        `;
        
        // افزودن رویداد کلیک برای بازگشایی گفتگو
        item.addEventListener('click', function() {
            loadConversation(conversation);
        });
        
        historyContainer.appendChild(item);
    });
}

// بارگذاری یک گفتگو از تاریخچه
function loadConversation(conversation) {
    // ذخیره GUID چت فعلی
    currentChatGuid = conversation.chatGuid;
    
    // پاک کردن تاریخچه فعلی
    conversationHistory = [];
    
    // بارگذاری پیام‌های گفتگوی قبلی
    conversation.messages.forEach(message => {
        conversationHistory.push({
            role: message.role,
            content: message.content
        });
    });
    
    // نمایش پیام‌ها در رابط گرافیکی
    const messagesContainer = document.getElementById('ai-messages');
    if (messagesContainer) {
        messagesContainer.innerHTML = '';
        
        // نمایش هر پیام
        conversation.messages.forEach(message => {
            if (message.role === 'user') {
                // نمایش پیام کاربر
                const userMessage = document.createElement("div");
                userMessage.className = "p-[16px] mb-[16px] rounded-lg border border-[#e9e9e9] ai__user__request";
                userMessage.textContent = message.content;
                messagesContainer.appendChild(userMessage);
            } else if (message.role === 'assistant') {
                // نمایش پاسخ هوش مصنوعی
                const uuid = uuidv4();
                const aiMessage = document.createElement("div");
                aiMessage.className = "p-2 flex flex-col justify-between mb-4";
                aiMessage.innerHTML = `
                    <div class='flex items-center text-xs text-[#515151] mb-[12px]'>
                        <svg width="27" height="27" viewBox="0 0 27 27" fill="none" xmlns="http://www.w3.org/2000/svg" class="w-[27px] h-[27px]">
                            <!-- SVG content -->
                        </svg>
                        <span class='mr-2'>پاسخ هوش مصنوعی</span>
                    </div>
                    <div class="ai-response-text">${message.content}</div>
                    <div class="flex justify-center cursor-pointer items-center w-[129px] relative overflow-hidden gap-1 pl-[13px] pr-[11px] py-2 rounded bg-indigo-600 mt-[18px] insert-ai-response__btn" style="box-shadow: 0px 1px 2px 0 rgba(0,0,0,0.05);" data-ai="${message.content}">
                        <p class="flex-grow-0 flex-shrink-0 text-sm font-bold text-left text-white">افزودن متن</p>
                        <svg width="17" height="16" viewBox="0 0 17 16" fill="none" xmlns="http://www.w3.org/2000/svg" class="flex-grow-0 flex-shrink-0 w-4 h-4 relative">
                            <path d="M8.5 4V8M8.5 8V12M8.5 8H12.5M8.5 8L4.5 8" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"></path>
                        </svg>
                    </div>
                `;
                messagesContainer.appendChild(aiMessage);
            }
        });
        
        // اضافه کردن رویداد کلیک به دکمه‌های افزودن متن
        const insertButtons = document.querySelectorAll(".insert-ai-response__btn");
        insertButtons.forEach((button) => {
            button.addEventListener("click", function () {
                const btnAiText = this.dataset.ai;
                tinymce.get(window.activeEditor).insertContent(btnAiText);
                closeBottomSheet();
            });
        });
        
        // اسکرول به پایین
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
    }
}

// Initialize on DOM load
document.addEventListener("DOMContentLoaded", () => {
    const systemMessages = document.body.dataset.aiDesc;

    // Add system messages from data-ai-desc
    if (systemMessages) {
        systemMessages.split('\n').forEach(line => {
            const trimmed = line.trim();
            if (trimmed) {
                conversationHistory.push({
                    role: "system",
                    content: trimmed
                });
            }
        });
    }

    // بارگذاری تاریخچه گفتگوها 
    loadConversationHistory().then(conversations => {
        displayConversationHistory(conversations);
    });

    // Credit check logic
    const accountElement = document.getElementById("ai__account");
    const creditElement = document.getElementById("rialCeredit");

    if (accountElement && creditElement) {
        console.log('Fetching credit...');
        fetch("/api/v1/AI/GetCredit")
            .then(response => {
                console.log('Response:', response);
                return response.json();
            })
            .then(result => {
                console.log('API Response:', result); // برای دیباگ
                if (result.data && result.data.remaining_irt !== undefined) {
                    const roundedCredit = Math.floor(result.data.remaining_irt);
                    creditElement.textContent = `${roundedCredit.toLocaleString()} تومان `;
                    console.log('Updated Credit:', roundedCredit); // برای دیباگ
                }
            })
            .catch(error => console.error("API Error:", error));
    }
    
    // اضافه کردن دکمه برای نمایش منوی انتخاب منبع
    const aiButton = document.querySelector('.mce-btn[aria-label="AI"]');
    if (aiButton) {
        aiButton.addEventListener('contextmenu', function(event: MouseEvent) {
            event.preventDefault();
            showSourceSelectionMenu(event);
        });
    }

    const settingButton = document.querySelector('.setting__chat');
    const settingModal = document.querySelector('.setting__chat__modal');
    const closeSettingButton = document.querySelector('#close-chat-setting');

    console.log('Setting elements:', { settingButton, settingModal, closeSettingButton }); // برای دیباگ

    if (settingButton && settingModal) {
        // نمایش مودال
        settingButton.addEventListener('click', () => {
            settingModal.classList.add('setting__chat__modal__show');
        });

        // بستن مودال با دکمه close
        if (closeSettingButton) {
            closeSettingButton.addEventListener('click', () => {
                settingModal.classList.remove('setting__chat__modal__show');
            });
        }

        // بستن مودال با کلیک خارج از آن
        document.addEventListener('click', (e: MouseEvent) => {
            if (e.target instanceof Node && 
                !settingModal.contains(e.target) && 
                !settingButton.contains(e.target)) {
                settingModal.classList.remove('setting__chat__modal__show');
            }
        });
    }
});

// نمایش منوی انتخاب منبع
function showSourceSelectionMenu(event: MouseEvent) {
    // حذف منوی قبلی اگر وجود دارد
    const existingMenu = document.getElementById('ai-source-menu');
    if (existingMenu) {
        existingMenu.remove();
    }
    
    // ساخت منوی جدید
    const menu = document.createElement('div');
    menu.id = 'ai-source-menu';
    menu.className = 'absolute bg-white border rounded shadow-lg z-50 p-2';
    menu.style.left = `${event.clientX}px`;
    menu.style.top = `${event.clientY}px`;
    
    // اضافه کردن گزینه‌ها
    const sources = [
        { id: 'article-editor', name: 'ویرایشگر مقاله' },
        { id: 'product-editor', name: 'ویرایشگر محصول' },
        { id: 'page-editor', name: 'ویرایشگر صفحه' },
        { id: 'custom', name: 'سفارشی...' }
    ];
    
    sources.forEach(source => {
        const item = document.createElement('div');
        item.className = 'p-2 hover:bg-gray-100 cursor-pointer text-sm';
        item.textContent = source.name;
        
        item.addEventListener('click', function() {
            if (source.id === 'custom') {
                // درخواست ورودی از کاربر برای منبع سفارشی
                const customSource = prompt('لطفاً منبع سفارشی را وارد کنید:');
                if (customSource) {
                    openBottomSheet(customSource);
                }
            } else {
                openBottomSheet(source.id);
            }
            menu.remove();
        });
        
        menu.appendChild(item);
    });
    
    // اضافه کردن منو به صفحه
    document.body.appendChild(menu);
    
    // بستن منو با کلیک خارج از آن
    document.addEventListener('click', function closeMenu(e: MouseEvent) {
        // اصلاح این خط برای رفع خطا:
        if (e.target instanceof Node && !menu.contains(e.target)) {
            menu.remove();
            document.removeEventListener('click', closeMenu);
        }
    });
}