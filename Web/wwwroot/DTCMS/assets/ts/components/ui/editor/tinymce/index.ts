import tinymce from "tinymce";
import type { Editor, RawEditorOptions } from "tinymce";
import { uploadFile } from "../../uploaderService";
import fa from "tinymce-i18n/langs7/fa";
import "tinymce/skins/ui/oxide/content";
import "tinymce/skins/ui/oxide/skin";
import "tinymce/themes/silver/index";
import "tinymce/icons/default/index";
import "tinymce/models/dom/index";
import "tinymce/skins/content/default/content";
import "tinymce/plugins/advlist/index";
import "tinymce/plugins/autolink/index";
import "tinymce/plugins/autoresize/index";
import "tinymce/plugins/charmap/index";
import "tinymce/plugins/directionality/index";
import "tinymce/plugins/emoticons/index";
import "tinymce/plugins/emoticons/js/emojis";
import "tinymce/plugins/fullscreen/index";
import "tinymce/plugins/help/index";
import "tinymce/plugins/image/index";
import "tinymce/plugins/importcss/index";
import "tinymce/plugins/insertdatetime/index";
import "tinymce/plugins/link/index";
import "tinymce/plugins/lists/index";
import "tinymce/plugins/media/index";
import "tinymce/plugins/nonbreaking/index";
import "tinymce/plugins/preview/index";
import "tinymce/plugins/quickbars/index";
import "tinymce/plugins/save/index";
import "tinymce/plugins/searchreplace/index";
import "tinymce/plugins/table/index";
import "tinymce/plugins/visualblocks/index";
import "tinymce/plugins/visualchars/index";
import "tinymce/plugins/wordcount/index";
import "tinymce/plugins/help/js/i18n/keynav/fa";
import "tinymce/plugins/help/js/i18n/keynav/en";
import { openBottomSheet } from "../../ai";
import { marked } from "marked";


const uploader = async (blobInfo, progress): Promise<string> => {
  try {
    let res = await uploadFile(
      [blobInfo.blob()],
      "/media",
      () => {},
      () => {}
    );
    console.log(res.data.data[0]);
    return res.data.data[0] as string;
  } catch (ex) {
    return "something went wrong";
  }
};

tinymce
  .init({
    selector: ".tiny__editor",
    language: "fa",
      language_url: fa,
      license_key: 'gpl',
      plugins: "preview importcss searchreplace autolink save directionality  visualblocks visualchars fullscreen image link media  table charmap nonbreaking insertdatetime advlist lists wordcount help charmap quickbars emoticons",
      menubar: "file edit view insert format tools table help",
      toolbar: " ai_generate ai_quickmenu blocks fontsize | bold italic underline strikethrough | align numlist bullist | link image | table media | lineheight outdent indent| forecolor backcolor removeformat | charmap emoticons | fullscreen preview | ltr rtl | undo redo",
      editimage_cors_hosts: ["picsum.photos"],
      setup: (editor) => {
        editor.ui.registry.addIcon("AI",`<img class="!w-6" src="/DTCMS/assets/images/icon/Ai/AI8.svg" />`);
        editor.ui.registry.addIcon("Kholase",`<img class="!w-5" src="/DTCMS/assets/images/icon/Ai/AbstractTo1.svg" />`);
        editor.ui.registry.addIcon("Baznevisi",`<img class="!w-5" src="/DTCMS/assets/images/icon/Ai/edit.svg" />`);
        editor.ui.registry.addIcon("longer",`<img class="!w-5" src="/DTCMS/assets/images/icon/Ai/longer.svg" />`);
        editor.ui.registry.addIcon("longer2",`<img class="!w-5" src="/DTCMS/assets/images/icon/Ai/longer2.svg" />`);
        editor.ui.registry.addIcon("lahn",`<img class="!w-5" src="/DTCMS/assets/images/icon/Ai/lahn.svg" />`);
        editor.ui.registry.addIcon("earth",`<img class="!w-5" src="/DTCMS/assets/images/icon/Ai/Earth.svg" />`);
        editor.ui.registry.addIcon("star",`<img class="!w-3" src="/DTCMS/assets/images/icon/Ai/star.svg" />`);
        editor.ui.registry.addIcon("finder",`<img class="!w-5" src="/DTCMS/assets/images/icon/Ai/finder.svg" />`);
        editor.ui.registry.addIcon("input",`<img class="!w-5" src="/DTCMS/assets/images/icon/Ai/input.svg" />`);

          editor.ui.registry.addButton("ai_generate", {
              text: "دستیار داریا ",
              icon:"AI",
              tooltip:"هوش مصنوعی",
            
              onAction: function () {
                  // پیدا کردن textarea و فوکوس روی آن
                  const aiInput = document.getElementById('ai-input') as HTMLTextAreaElement;
                  if (aiInput) {
                      setTimeout(() => {
                          aiInput.focus();
                      }, 100); // تاخیر کوچک برای اطمینان از اینکه bottom sheet باز شده است
                  }
                  
                  window.activeEditor = editor.id;
                  openBottomSheet();
              }
          });


         


          // اضافه کردن منوی جدید AI برای quickbars
          editor.ui.registry.addMenuButton('ai_quickmenu', {
              icon: 'input',
              tooltip: 'ویراس داری هوش مصنوعی',
              fetch: (callback) => {
                  const items = [
                      {
                          type: 'menuitem' as const,
                          text: 'بررسی گرامر',
                          icon: 'earth',
                          onAction: () => {
                              handleAIAction('grammar', editor.selection.getContent({format: 'text'}), editor);
                          }
                      },
                      {
                          type: 'menuitem' as const,
                          text: 'خلاصه کردن',
                          icon: 'Kholase',
                          onAction: () => {
                              handleAIAction('summarize', editor.selection.getContent({format: 'text'}), editor);
                          }
                      },
                      {
                          type: 'menuitem' as const,
                          text: 'گسترش متن',
                          icon: 'longer2',
                          onAction: () => {
                              handleAIAction('expand', editor.selection.getContent({format: 'text'}), editor);
                          }
                      },
                      {
                          type: 'menuitem' as const,
                          text: 'بازنویسی متن',
                          icon: 'Baznevisi',
                          onAction: () => {
                              handleAIAction('rewrite', editor.selection.getContent({format: 'text'}), editor);
                          }
                      },
                      {
                          type: 'nestedmenuitem' as const,
                          text: 'تغییر لحن',
                          icon: 'finder',
                          getSubmenuItems: () => [
                              {
                                  type: 'menuitem' as const,
                                  text: 'لحن رسمی',
                                  icon: 'star',
                                  onAction: () => {
                                      handleAIAction('tone_formal', editor.selection.getContent({format: 'text'}), editor);
                                  }
                              },
                              {
                                  type: 'menuitem' as const,
                                  text: 'لحن دوستانه',
                                  icon: 'star',
                                  onAction: () => {
                                      handleAIAction('tone_friendly', editor.selection.getContent({format: 'text'}), editor);
                                  }
                              },
                              {
                                  type: 'menuitem' as const,
                                  text: 'لحن تبلیغاتی',
                                  icon: 'star',
                                  onAction: () => {
                                      handleAIAction('tone_promotional', editor.selection.getContent({format: 'text'}), editor);
                                  }
                              },
                              {
                                  type: 'menuitem' as const,
                                  text: 'لحن طنز',
                                  icon: 'star',
                                  onAction: () => {
                                      handleAIAction('tone_humorous', editor.selection.getContent({format: 'text'}), editor);
                                  }
                              },
                              {
                                  type: 'menuitem' as const,
                                  text: 'لحن قانع‌کننده',
                                  icon: 'star',
                                  onAction: () => {
                                      handleAIAction('tone_persuasive', editor.selection.getContent({format: 'text'}), editor);
                                  }
                              },
                              {
                                  type: 'menuitem' as const,
                                  text: 'لحن خبری',
                                  icon: 'star',
                                  onAction: () => {
                                      handleAIAction('tone_news', editor.selection.getContent({format: 'text'}), editor);
                                  }
                              }
                          ]
                      },
                      {
                          type: 'nestedmenuitem' as const,
                          text: 'ترجمه متن',
                          icon: 'translate',
                          getSubmenuItems: () => [
                              {
                                  type: 'menuitem' as const,
                                  text: 'ترجمه به فارسی',
                                  icon: 'star',
                                  onAction: () => {
                                      handleAIAction('translate_fa', editor.selection.getContent({format: 'text'}), editor);
                                  }
                              },
                              {
                                  type: 'menuitem' as const,
                                  text: 'ترجمه به انگلیسی',
                                  icon: 'star',
                                  onAction: () => {
                                      handleAIAction('translate_en', editor.selection.getContent({format: 'text'}), editor);
                                  }
                              },
                              {
                                  type: 'menuitem' as const,
                                  text: 'ترجمه به عربی',
                                  icon: 'star',
                                  onAction: () => {
                                      handleAIAction('translate_ar', editor.selection.getContent({format: 'text'}), editor);
                                  }
                              }
                          ]
                      },
                      {
                          type: 'nestedmenuitem' as const,
                          text: 'تبدیل به لیست',
                          icon: 'unordered-list',
                          getSubmenuItems: () => [
                              {
                                  type: 'menuitem' as const,
                                  text: 'لیست ساده',
                                  icon: 'unordered-list',
                                  onAction: () => {
                                      handleAIAction('convert_bullet_list', editor.selection.getContent({format: 'text'}), editor);
                                  }
                              },
                              {
                                  type: 'menuitem' as const,
                                  text: 'لیست شماره‌دار',
                                  icon: 'ordered-list',
                                  onAction: () => {
                                      handleAIAction('convert_numbered_list', editor.selection.getContent({format: 'text'}), editor);
                                  }
                              },
                              {
                                  type: 'menuitem' as const,
                                  text: 'لیست چک‌باکس',
                                  icon: 'checklist',
                                  onAction: () => {
                                      handleAIAction('convert_checklist', editor.selection.getContent({format: 'text'}), editor);
                                  }
                              },
                              {
                                  type: 'menuitem' as const,
                                  text: 'لیست مرحله‌ای',
                                  icon: 'ordered-list',
                                  onAction: () => {
                                      handleAIAction('convert_steps_list', editor.selection.getContent({format: 'text'}), editor);
                                  }
                              }
                          ]
                      }
                  ];
                  callback(items);
              }
          });
 
      },
    
    image_advtab: true,
    importcss_append: true,
    images_upload_handler: uploader,
    file_picker_callback: (callback, value, meta) => {
      /* Provide file and text for the link dialog */
      if (meta.filetype === "file") {
        callback(value, { text: "My text" });
      }

      /* Provide image and alt text for the image dialog */
      if (meta.filetype === "image") {
        callback(value, {
          alt: "My alt text",
        });
      }

      /* Provide alternative source and posted for the media dialog */
      if (meta.filetype === "media") {
        callback(value, {
          source2: value,
          poster: "",
        });
      }
    },

    height: 600,
    image_caption: true,
    quickbars_selection_toolbar: 'bold italic | quicklink h2 h3 blockquote quickimage quicktable | ai_quickmenu',
    noneditable_class: "mceNonEditable",
    toolbar_mode: "sliding",
    contextmenu: "link image table",
    skin: "oxide",
    content_css: "default",
    content_style: `
        body {
            font-family: 'yekanbakh', 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            font-size: 14px;
            line-height: 1.6;
        }
    `,
  } as RawEditorOptions);

// تابع مدیریت اکشن‌های AI
async function handleAIAction(action: string, selectedText: string, editor: any) {
    if (!selectedText) {
        editor.notificationManager.open({
            text: 'لطفا ابتدا متنی را انتخاب کنید',
            type: 'warning'
        });
        return;
    }

    const bookmark = editor.selection.getBookmark();
    
    // دریافت متن انتخاب شده با فرمت HTML
    const selectedHtml = editor.selection.getContent();
    // دریافت متن خالص برای ارسال به API
    const plainText = editor.selection.getContent({ format: 'text' });

    let systemMessage = '';
    let shouldFormatAsList = false;
    
    switch(action) {
        case 'grammar':
            systemMessage = 'لطفا این متن فارسی را از نظر گرامری و نگارشی بررسی کن و اصلاحات لازم را با حفظ معنی و مفهوم اصلی متن پیشنهاد بده. فقط متن اصلاح شده را برگردان:';
            break;
        case 'summarize':
            systemMessage = 'لطفا این متن فارسی را به صورت خلاصه و مفید در حداکثر یک سوم متن اصلی بازنویسی کن:';
            break;
        case 'expand':
            systemMessage = 'لطفا این متن فارسی را با جزئیات بیشتر، مثال‌ها و توضیحات تکمیلی گسترش بده و حداقل دو برابر متن اصلی را برگردان:';
            break;
        case 'tone_formal':
            systemMessage = 'لطفا این متن را با لحنی رسمی و حرفه‌ای بازنویسی کن. از واژگان و ساختارهای رسمی استفاده کن و حالت محترمانه و جدی متن را حفظ کن:';
            break;
        case 'tone_friendly':
            systemMessage = 'لطفا این متن را با لحنی دوستانه و صمیمی بازنویسی کن. از واژگان ساده و محاوره‌ای مناسب استفاده کن و حس نزدیکی و گرمی به متن اضافه کن:';
            break;
        case 'tone_promotional':
            systemMessage = 'لطفا این متن را با لحنی تبلیغاتی و ترغیب‌کننده بازنویسی کن. از عبارات جذاب و اقناع‌کننده استفاده کن و مزایا را برجسته کن:';
            break;
        case 'tone_humorous':
            systemMessage = 'لطفا این متن را با لحنی طنزآمیز و سرگرم‌کننده بازنویسی کن. از عناصر طنز مناسب و هوشمندانه استفاده کن و فضای شاد به متن اضافه کن:';
            break;
        case 'tone_persuasive':
            systemMessage = 'لطفا این متن را با لحنی قانع‌کننده و استدلالی بازنویسی کن. از منطق قوی، شواهد و استدلال‌های محکم استفاده کن:';
            break;
        case 'tone_news':
            systemMessage = 'لطفا این متن را با لحنی خبری و بی‌طرفانه بازنویسی کن. از سبک روزنامه‌نگاری استفاده کن و اصول خبرنویسی را رعایت کن:';
            break;
        case 'rewrite':
            systemMessage = 'لطفا این متن را با حفظ معنی و مفهوم اصلی، به شکلی کاملاً متفاوت بازنویسی کن. از کلمات و ساختارهای جمله متفاوت استفاده کن اما مطمئن شو که پیام اصلی حفظ می‌شود. از تکرار کلمات متن اصلی تا حد امکان خودداری کن:';
            break;
        case 'translate_fa':
            systemMessage = 'لطفا این متن را به فارسی ترجمه کن. سعی کن لحن و ساختار اصلی متن را حفظ کنی و ترجمه طبیعی و روان باشد. اگر اصطلاحات تخصصی یا فرهنگی وجود دارد، معادل مناسب فارسی آن را استفاده کن:';
            break;
        case 'translate_en':
            systemMessage = 'لطفا این متن را به انگلیسی ترجمه کن. سعی کن لحن و ساختار اصلی متن را حفظ کنی و ترجمه طبیعی و روان باشد. اگر اصطلاحات تخصصی یا فرهنگی وجود دارد، معادل مناسب انگلیسی آن را استفاده کن:';
            break;
        case 'translate_ar':
            systemMessage = 'لطفا این متن را به عربی ترجمه کن. سعی کن لحن و ساختار اصلی متن را حفظ کنی و ترجمه طبیعی و روان باشد. اگر اصطلاحات تخصصی یا فرهنگی وجود دارد، معادل مناسب عربی آن را استفاده کن:';
            break;
        case 'convert_bullet_list':
            systemMessage = 'لطفا این متن را به یک لیست با بولت‌پوینت تبدیل کن. هر نکته مهم را در یک خط جداگانه قرار بده و با علامت • شروع کن. مطمئن شو که اطلاعات مهم از دست نرود و لیست منطقی و منظم باشد:';
            shouldFormatAsList = true;
            break;
        case 'convert_numbered_list':
            systemMessage = 'لطفا این متن را به یک لیست شماره‌دار تبدیل کن. موارد را به ترتیب منطقی شماره‌گذاری کن و هر مورد را در یک خط جداگانه قرار بده:';
            shouldFormatAsList = true;
            break;
        case 'convert_checklist':
            systemMessage = 'لطفا این متن را به یک لیست چک‌باکس تبدیل کن. هر مورد را با ☐ شروع کن و در یک خط جداگانه قرار بده. موارد را طوری تنظیم کن که به صورت وظایف قابل انجام باشند:';
            shouldFormatAsList = true;
            break;
        case 'convert_steps_list':
            systemMessage = 'لطفا این متن را به یک لیست مرحله‌ای تبدیل کن. هر مرحله را با شماره و عنوان مشخص کن و توضیحات مربوط به آن مرحله را در ادامه بیاور. مراحل را به ترتیب منطقی و با جزئیات کافی تنظیم کن:';
            shouldFormatAsList = true;
            break;
    }

    // ایجاد نوتیفیکیشن با progress bar
    const loadingNotification = editor.notificationManager.open({
        text: 'در حال پردازش...',
        type: 'info',
        progressBar: true
    });

    // تابع انیمیشن progress bar
    let progress = 0;
    let animationFrameId: number;
    
    function animateProgress() {
        progress += 0.5;
        if (progress <= 99) {
            loadingNotification.progressBar.value(progress / 100);
            animationFrameId = requestAnimationFrame(animateProgress);
        }
    }

    animateProgress();

    try {
        const response = await fetch('/api/v1/AI/Chat', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                messages: [
                    { role: 'system', content: systemMessage },
                    { role: 'user', content: plainText }
                ],
                temperature: 0.7,
                maxTokens: 2000
            })
        });

        if (!response.ok) {
            throw new Error('خطا در دریافت پاسخ از سرور');
        }

        const result = await response.json();
        let aiResponse = result.data.choices[0]?.message?.content;

        if (!aiResponse) {
            throw new Error('پاسخی از هوش مصنوعی دریافت نشد');
        }

        // فرمت‌بندی پاسخ برای لیست‌ها
        if (shouldFormatAsList) {
            aiResponse = formatListResponse(aiResponse, action);
        }

        cancelAnimationFrame(animationFrameId);
        loadingNotification.progressBar.value(1);
        
        setTimeout(() => {
            loadingNotification.close();
            editor.notificationManager.open({
                text: 'پردازش با موفقیت انجام شد',
                type: 'success',
                timeout: 3000
            });
        }, 500);

        showAIResultModal(aiResponse, editor, bookmark, selectedHtml);

    } catch (error) {
        console.error('خطا:', error);
        cancelAnimationFrame(animationFrameId);
        loadingNotification.close();
        editor.notificationManager.open({
            text: 'خطا در پردازش درخواست',
            type: 'error',
            timeout: 3000
        });
    }
}

// تابع کمکی برای فرمت‌بندی لیست‌ها
function formatListResponse(text: string, action: string): string {
    const lines = text.split('\n').filter(line => line.trim());
    
    switch(action) {
        case 'convert_bullet_list':
            return lines.map(line => `<li>${line.replace(/^[-•*]\s*/, '')}</li>`).join('\n');
        
        case 'convert_numbered_list':
            return lines.map((line, index) => `<li>${line.replace(/^\d+\.\s*/, '')}</li>`).join('\n');
        
        case 'convert_checklist':
            return lines.map(line => `<li><input type="checkbox"> ${line.replace(/^[☐✓]\s*/, '')}</li>`).join('\n');
        
        case 'convert_steps_list':
            return lines.map((line, index) => `<li><strong>مرحله ${index + 1}:</strong> ${line.replace(/^(مرحله\s*\d+:?\s*)/i, '')}</li>`).join('\n');
        
        default:
            return text;
    }
}

// تابع نمایش نتیجه
async function showAIResultModal(result: string, editor: any, bookmark: any, originalHtml: string) {
    const isListContent = result.includes('<li>');
    const wrappedResult = isListContent ? `<ul class="list-disc list-inside">${result}</ul>` : result;
    
    const modalHtml = `
        <div class="p-4">
            <div class="mb-4 max-h-[400px] overflow-y-auto text-right prose prose-sm" dir="rtl">
                ${wrappedResult}
            </div>
            <div class="flex justify-between gap-2 mt-4">
                <button type="submit" class="apply-ai-result text-center w-ful">
                    اعمال تغییرات
                </button>
                <button class="cancel-ai-result px-4 py-2 bg-gray-200 rounded hover:bg-gray-300">
                    لغو و بازگشت
                </button>
            </div>
        </div>
    `;

    editor.windowManager.open({
        title: 'نتیجه پردازش هوش مصنوعی',
        classes: 'ai__pre__proccess',
        size: 'large',
        body: {
            type: 'panel',
            items: [{
                type: 'htmlpanel',
                html: modalHtml
            }]
        },
        buttons: [],
        onClose: () => {
            editor.selection.moveToBookmark(bookmark);
        }
    });

    setTimeout(() => {
        const applyButton = document.querySelector('.apply-ai-result');
        const cancelButton = document.querySelector('.cancel-ai-result');
        
        if (applyButton) {
            applyButton.addEventListener('click', () => {
                editor.selection.moveToBookmark(bookmark);
                const contentToInsert = isListContent ? wrappedResult : result;
                editor.selection.setContent(contentToInsert);
                editor.windowManager.close();
                editor.notificationManager.open({
                    text: 'تغییرات با موفقیت اعمال شد',
                    type: 'success'
                });
            });
        }

        if (cancelButton) {
            cancelButton.addEventListener('click', () => {
                editor.windowManager.close();
            });
        }
    }, 100);
}
