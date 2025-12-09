using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using ResourceLibrary.Panel.Common.Global;

namespace Web.TagHelpers.Panel.Controls
{

    [HtmlTargetElement("dt:image-uploader", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class ImageUploaderTagHelper : TagHelper
    {

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContext { get; set; }
        private readonly IHtmlGenerator _generator;
        private readonly IHtmlHelper _htmlHelper;

        public ModelExpression? AspFor { get; set; }

        /// <summary>
        /// add custom class to item
        /// </summary>
        [HtmlAttributeName(name: "class")]
        public string? ClassName { get; set; }

        public bool DisableCropper { get; set; } = false;

        public bool DisableCompression { get; set; } = false;


        /// <summary>
        /// byte
        /// </summary>
        public int? MaxSize { get; set; }

        /// <summary>
        /// px
        /// </summary>

        public int? MaxHeight { get; set; }

        public string? AspectRatio { get; set; } = "16/9";

        public bool? Rounded { get; set; }= false;

        public bool? HasDetial { get; set; } = false;


        public string? Format { get; set; }


        public bool? HasTitle { get; set; }=false;

        private readonly IStringLocalizer<Global> GlobalLocalizer;

        public string HelperText { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ImageUploaderTagHelper(IHtmlHelper htmlHelper, IHtmlGenerator generator , IStringLocalizer<ResourceLibrary.Panel.Common.Global.Global> _GlobalLocalizer)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            GlobalLocalizer = _GlobalLocalizer;
            _generator = generator;
            _htmlHelper = htmlHelper;
        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            var uniqName = AspFor?.Name.Replace('.', '_');

            string UploadHiddenClass = "";
            if(Rounded == true)
            {
                UploadHiddenClass = "round__input__value";
            }
            else
            {
                UploadHiddenClass = "nc__input__value__" + uniqName;
            }

          
            var UploadHidden = _generator.GenerateHidden(ViewContext, AspFor?.ModelExplorer, AspFor?.Name, AspFor?.Model, true, new { @class = UploadHiddenClass });
            if(Rounded == false)
                UploadHidden.Attributes.Add("data-parent-id", @$"nc__media__cropperjs__container__{uniqName}");
            output.Content.AppendHtml(UploadHidden);


            if(HasTitle == true || HasDetial == true)
            {
                TagBuilder TitleTag = new TagBuilder("h4");
                TitleTag.AddCssClass("text-gray-700 text-sm font-normal");
#pragma warning disable CS8604 // Possible null reference argument.
                TitleTag.InnerHtml.AppendHtml(AspFor?.Metadata?.DisplayName);
#pragma warning restore CS8604 // Possible null reference argument.
                output.Content.AppendHtml(TitleTag);    
            }

            string? CropperBottomSheet = null;
            string? CropperDataAttribute = null;
            if (DisableCropper)
            {
                CropperDataAttribute = $@"data-not-crop='true'";
            }
            else
            {
                CropperBottomSheet = $@"data-sheet-control='nc__media__{uniqName}'";
            }

            string? CompressionDataAttribute = null;
            if (DisableCompression)
                CompressionDataAttribute = $@"data-not-compress='true'";

            string? MaxSizeAttribute = null;
            if(MaxSize != null)
            {
                MaxSizeAttribute = $@"data-max-size='{MaxSize}'";
            }
         
            string? MaxHeightAttribute = null;

            if (MaxHeight != null)
            {
                MaxHeightAttribute = $@"data-max-height='{MaxHeight}'";
            }
            string? AspectRatioAttribute = $@"data-aspect-ratio='{AspectRatio}'";
           

            if(Rounded == false && HasDetial == false)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                var uploadContent = $@"<div
class=""relative cursor-pointer bg-gray-100 border-[2px] border-gray-200 border-dashed   h-[120px] w-[344px] rounded-md flex justify-center items-center text-center mx-auto flex-col"">
<input type=""file"" accept=""image/*"" class=""z-[4] opacity-0 w-full h-full absolute""
  oninput=""initMediaCropper('nc__media__cropperjs__container__{uniqName}')"" {CropperBottomSheet} />
<svg class=""mb-1"" width=""48"" height=""48"" viewBox=""0 0 48 48"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
  <path
    d=""M28 8H12C10.9391 8 9.92172 8.42143 9.17157 9.17157C8.42143 9.92172 8 10.9391 8 12V32M8 32V36C8 37.0609 8.42143 38.0783 9.17157 38.8284C9.92172 39.5786 10.9391 40 12 40H36C37.0609 40 38.0783 39.5786 38.8284 38.8284C39.5786 38.0783 40 37.0609 40 36V28M8 32L17.172 22.828C17.9221 22.0781 18.9393 21.6569 20 21.6569C21.0607 21.6569 22.0779 22.0781 22.828 22.828L28 28M40 20V28M40 28L36.828 24.828C36.0779 24.0781 35.0607 23.6569 34 23.6569C32.9393 23.6569 31.9221 24.0781 31.172 24.828L28 28M28 28L32 32M36 8H44M40 4V12M28 16H28.02""
    stroke=""#9CA3AF"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round"" />
</svg>
<div class=""font-normal text-sm text-indigo-600"">{GlobalLocalizer["SingleUploaderTitle",AspFor?.Metadata.DisplayName]}</div>
<svg onclick=""handleMediaRemove('nc__media__cropperjs__container__{uniqName}')"" id=""nc__media__remove__btn__{uniqName}""
  class=""hidden cursor-pointer z-10 absolute top-2 right-2"" width=""17"" height=""17"" viewBox=""0 0 17 17"" fill=""none""
  xmlns=""http://www.w3.org/2000/svg"">
  <circle cx=""8.40517"" cy=""8.40517"" r=""8.40517"" fill=""#DC2626"" />
  <path
    d=""M12.2716 6.22037L11.8342 12.344C11.7965 12.8718 11.3573 13.2807 10.8281 13.2807H6.65468C6.12551 13.2807 5.68632 12.8718 5.64862 12.344L5.21122 6.22037M7.73277 8.23761V11.2635M9.75001 8.23761V11.2635M10.2543 6.22037V4.70744C10.2543 4.42891 10.0285 4.20312 9.75001 4.20312H7.73277C7.45425 4.20312 7.22846 4.42891 7.22846 4.70744V6.22037M4.70691 6.22037H12.7759""
    stroke=""white"" stroke-width=""1.34483"" stroke-linecap=""round"" stroke-linejoin=""round"" />
</svg>
<img class=""absolute z-[1] w-full h-full object-cover"" id=""nc__media__cropper__preview__{uniqName}"" />
<div id=""nc__media__preview__overlay__{uniqName}""
  class="" hidden z-[3] bg-gray-700 absolute h-full w-full opacity-50 rounded-md"">

  <svg aria-hidden=""true"" id=""nc__media__preview__loading__{uniqName}""
    class=""hidden absolute bottom-2 right-2    w-8 h-8 mr-2 text-transparent animate-spin dark:text-gray-600 fill-white""
    viewBox=""0 0 100 101"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
    <path
      d=""M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z""
      fill=""currentColor"" />
    <path
      d=""M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z""
      fill=""currentFill"" />
  </svg>
  <div class=""absolute bottom-2 left-2 text-white"" id=""nc__media__percentage__view__{uniqName}"">0%</div>
</div>
<div id=""nc__media__progress__bar__{uniqName}"" dir=""ltr""
  class=""z-[3] hidden absolute bottom-0 w-full bg-gray-200 rounded-full h-1.5 dark:bg-gray-700"">
  <div class=""bg-green-500 h-1.5 rounded-full"" style=""width: 33%"">
  </div>
</div>
<div class=""hidden "" id=""nc__media__retry__button__{uniqName}"">
  <svg onclick=""handleMediaUpload()"" class=""z-[4]  cursor-pointer absolute bottom-2 left-10"" width=""24"" height=""24""
    viewBox=""0 0 24 24"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
    <mask id=""mask0_1250_32739"" style=""mask-type:alpha"" maskUnits=""userSpaceOnUse"" x=""0"" y=""0"" width=""24""
      height=""24"">
      <rect width=""24"" height=""24"" fill=""#D9D9D9"" />
    </mask>
    <g mask=""url(#mask0_1250_32739)"">
      <path
        d=""M12 22C10.75 22 9.57933 21.7627 8.488 21.288C7.396 20.8127 6.446 20.1707 5.638 19.362C4.82933 18.554 4.18733 17.604 3.712 16.512C3.23733 15.4207 3 14.25 3 13H5C5 14.95 5.67933 16.604 7.038 17.962C8.396 19.3207 10.05 20 12 20C13.95 20 15.604 19.3207 16.962 17.962C18.3207 16.604 19 14.95 19 13C19 11.05 18.3207 9.39567 16.962 8.037C15.604 6.679 13.95 6 12 6H11.85L13.4 7.55L12 9L8 5L12 1L13.4 2.45L11.85 4H12C13.25 4 14.421 4.23767 15.513 4.713C16.6043 5.18767 17.5543 5.829 18.363 6.637C19.171 7.44567 19.8127 8.39567 20.288 9.487C20.7627 10.579 21 11.75 21 13C21 14.25 20.7627 15.4207 20.288 16.512C19.8127 17.604 19.171 18.554 18.363 19.362C17.5543 20.1707 16.6043 20.8127 15.513 21.288C14.421 21.7627 13.25 22 12 22Z""
        fill=""white"" />
    </g>
  </svg>
  <svg onclick=""handleMediaRemove('nc__media__cropperjs__container__{uniqName}')""
    class="" z-[4]  cursor-pointer absolute bottom-2 left-2"" width=""25"" height=""25"" viewBox=""0 0 25 25"" fill=""none""
    xmlns=""http://www.w3.org/2000/svg"">
    <circle cx=""12.5"" cy=""12.5"" r=""12.5"" fill=""#DC2626"" />
    <path
      d=""M18.25 9.25L17.5995 18.3569C17.5434 19.1418 16.8903 19.75 16.1033 19.75H9.89668C9.10972 19.75 8.45656 19.1418 8.40049 18.3569L7.75 9.25M11.5 12.25V16.75M14.5 12.25V16.75M15.25 9.25V7C15.25 6.58579 14.9142 6.25 14.5 6.25H11.5C11.0858 6.25 10.75 6.58579 10.75 7V9.25M7 9.25H19""
      stroke=""white"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round"" />
  </svg>
</div>
</div>
<div class=""helper__text text-center direction__ltr"">
    <small>
{HelperText}
    </small>
</div>
<div id=""nc__media__{uniqName}"" data-min-height=""100"" data-not-dragable=""true"" class=""sheet z-50"" aria-hidden=""true""
role=""dialog"">
<div class=""overlay bg-gray-800 opacity-75 w-full""></div>
<div class=""contents"">
  <header class=""controls bg-white !hidden"">
    <button data-sheet-dismiss class=""close-sheet ml-auto w-2"" type=""button""
      title=""Close the sheet"">&times;</button>
 
    <div class=""draggable-area w-full flex justify-center"">
      <div class=""draggable-thumb w-12""></div>
    </div>
  </header>
  <main class=""body bg-gray-800 px-0"">
    <div id=""nc__media__cropperjs__container__{uniqName}"" {CropperDataAttribute} {CompressionDataAttribute}
      data-preview-target=""nc__media__cropper__preview__{uniqName}"" data-retry-target=""nc__media__retry__button__{uniqName}""
      data-progress-target=""nc__media__progress__bar__{uniqName}"" data-remove-target=""nc__media__remove__btn__{uniqName}""
      data-percentage-target=""nc__media__percentage__view__{uniqName}"" data-overlay-target=""nc__media__preview__overlay__{uniqName}""
      data-loading-target=""nc__media__preview__loading__{uniqName}"" data-input-target=""nc__input__value__{uniqName}""
      {AspectRatioAttribute} {MaxHeightAttribute} {MaxSizeAttribute} 
      class=""relative pt-16  min-w-[360px] h-[100vh]  overflow-hidden "">
      <div onclick=""setMediaCompressStatus()""
        class=""media__compressor__status cursor-pointer absolute top-4 left-2 ltr:right-2  bg-green-100 h-5 min-w-[122px] w-fit px-2 py-0.5 flex justify-start rounded-md"">
        <div class="" mt-[1px]"">
          <svg width=""13"" height=""12"" viewBox=""0 0 13 12"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
            <path fill-rule=""evenodd"" clip-rule=""evenodd""
              d=""M10.5243 3.17574C10.7586 3.41005 10.7586 3.78995 10.5243 4.02426L5.72431 8.82426C5.49 9.05858 5.1101 9.05858 4.87578 8.82426L2.47578 6.42426C2.24147 6.18995 2.24147 5.81005 2.47578 5.57574C2.7101 5.34142 3.09 5.34142 3.32431 5.57574L5.30005 7.55147L9.67578 3.17574C9.9101 2.94142 10.29 2.94142 10.5243 3.17574Z""
              fill=""#34D399"" />
          </svg>
        </div>
        <div class=""text-xs font-normal text-green-800 mx-0.5"">{GlobalLocalizer["UploaderCompressedTitle"]}</div>
      </div>
      <div onclick=""setMediaCompressStatus()""
        class=""hidden media__compressor__status cursor-pointer absolute top-4 left-2 ltr:right-2 bg-yellow-100 h-5 min-w-[122px] w-fit px-2 py-0.5 flex justify-start rounded-md"">
        <div class=""mt-[1px]"">
          <svg class=""svg-icon""
            style=""width: 0.8em; height: 0.8em;vertical-align: middle;fill: currentColor;overflow: hidden;""
            viewBox=""0 0 1024 1024"" version=""1.1"" xmlns=""http://www.w3.org/2000/svg"">
            <path
              d=""M1001.661867 796.544c48.896 84.906667 7.68 157.013333-87.552 157.013333H110.781867c-97.834667 0-139.050667-69.504-90.112-157.013333l401.664-666.88c48.896-87.552 128.725333-87.552 177.664 0l401.664 666.88zM479.165867 296.533333v341.333334a32 32 0 1 0 64 0v-341.333334a32 32 0 1 0-64 0z m0 469.333334v42.666666a32 32 0 1 0 64 0v-42.666666a32 32 0 1 0-64 0z""
              fill=""#FAAD14"" />
          </svg>
        </div>
        <div class=""text-xs font-normal text-yellow-800 mx-0.5"">  {GlobalLocalizer["UploaderOriginalSizeTitle"]}</div>
      </div>
      <div class=""bg-gray-800 px-16 pb-12 pt-5 flex flex-col justify-between h-fit absolute bottom-0 right-0 w-full "">
        <div class=""flex w-full items-center my-6"">
          <svg width=""24"" height=""24"" viewBox=""0 0 24 24"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
            <path
              d=""M4 16L8.58579 11.4142C9.36683 10.6332 10.6332 10.6332 11.4142 11.4142L16 16M14 14L15.5858 12.4142C16.3668 11.6332 17.6332 11.6332 18.4142 12.4142L20 14M14 8H14.01M6 20H18C19.1046 20 20 19.1046 20 18V6C20 4.89543 19.1046 4 18 4H6C4.89543 4 4 4.89543 4 6V18C4 19.1046 4.89543 20 6 20Z""
              stroke=""white"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round"" />
          </svg>
          <input style=""direction: ltr"" type=""range"" name="""" id=""rangeInput__{uniqName}"" class=""w-full mx-1 p-0 h-0.5"" min=""0""
            max=""1"" step=""0.01"" value=""0"" oninput=""handleMediaZoom()"" />
          <svg width=""16"" height=""16"" viewBox=""0 0 16 16"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
            <path
              d=""M2.66663 10.6665L5.72382 7.60931C6.24452 7.08861 7.08874 7.08861 7.60944 7.60931L10.6666 10.6665M9.33329 9.33317L10.3905 8.27598C10.9112 7.75528 11.7554 7.75528 12.2761 8.27598L13.3333 9.33317M9.33329 5.33317H9.33996M3.99996 13.3332H12C12.7363 13.3332 13.3333 12.7362 13.3333 11.9998V3.99984C13.3333 3.26346 12.7363 2.6665 12 2.6665H3.99996C3.26358 2.6665 2.66663 3.26346 2.66663 3.99984V11.9998C2.66663 12.7362 3.26358 13.3332 3.99996 13.3332Z""
              stroke=""white"" stroke-width=""1.33333"" stroke-linecap=""round"" stroke-linejoin=""round"" />
          </svg>
        </div>
        <div class=""flex w-full justify-between items-center"">
          <svg data-sheet-dismiss onclick=""handleMediaCrop()"" class=""cursor-pointer"" width=""48"" height=""48""
            viewBox=""0 0 48 48"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
            <path d=""M10 26L18 34L38 14"" stroke=""#10B981"" stroke-width=""4"" stroke-linecap=""round""
              stroke-linejoin=""round"" />
          </svg>
          <svg data-sheet-dismiss class=""cursor-pointer"" width=""41"" height=""42"" viewBox=""0 0 41 42"" fill=""none""
            xmlns=""http://www.w3.org/2000/svg"">
            <path d=""M10.25 31.25L30.75 10.75M10.25 10.75L30.75 31.25"" stroke=""#DC2626"" stroke-width=""3.41667""
              stroke-linecap=""round"" stroke-linejoin=""round"" />
          </svg>
        </div>
      </div>
    </div>
  </main>
</div>
</div>";
#pragma warning restore CS8604 // Possible null reference argument.
                output.Content.AppendHtml(uploadContent);
            }
            else if(Rounded == true && HasDetial == false)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                var RoundUploadreContent = $@"  <div
    class=""overflow-hidden relative cursor-pointer bg-gray-200 h-[140px] w-[140px] rounded-full flex justify-center items-center text-center mx-auto flex-col"">
    <input type=""file"" id=""profile__pic__input"" accept=""image/*"" oninput=""initProfileCropper('cropperjsContainer__{uniqName}')""
      class=""absolute w-full h-full rounded-full opacity-0"" {CropperBottomSheet} />
    <img class=""absolute"" id=""cropper__preview"" />
    <div id=""preview__overlay"" class="" hidden bg-gray-700 absolute h-full w-full opacity-80 rounded-full"">

      <svg aria-hidden=""true"" id=""preview__loading""
        class=""hidden absolute bottom-[41%] right-[40px]    w-8 h-8 mr-2 text-transparent animate-spin dark:text-gray-600 fill-white""
        viewBox=""0 0 100 101"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
        <path
          d=""M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z""
          fill=""currentColor"" />
        <path
          d=""M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z""
          fill=""currentFill"" />
      </svg>

      <div class=""absolute bottom-1 right-[40%] text-white"" id=""percentage__view"">0%</div>
      <div id=""edit__pencil__icon"" class=""hidden absolute bottom-1 right-[42%] cursor-pointer"">
        <input id=""secondary__input__file"" type=""file"" accept=""image/*""
          class=""w-full h-full absolute   right-0 opacity-0"" oninput=""initProfileCropper('cropperjsContainer__{uniqName}')""   {CropperBottomSheet}/>
        <svg width=""22"" height=""22"" viewBox=""0 0 22 22"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
          <path
            d=""M14.9442 3.94417C15.8033 3.08502 17.1963 3.08502 18.0554 3.94417C18.9146 4.80332 18.9146 6.19629 18.0554 7.05544L17.1833 7.92762L14.072 4.81635L14.9442 3.94417Z""
            fill=""white"" />
          <path d=""M12.5164 6.37199L3.2998 15.5885V18.6998H6.41107L15.6276 9.48326L12.5164 6.37199Z"" fill=""white"" />
          <path
            d=""M14.9442 3.94417C15.8033 3.08502 17.1963 3.08502 18.0554 3.94417C18.9146 4.80332 18.9146 6.19629 18.0554 7.05544L17.1833 7.92762L14.072 4.81635L14.9442 3.94417Z""
            stroke=""#FBBF24"" stroke-width=""1.51449"" stroke-linecap=""round"" stroke-linejoin=""round"" />
          <path d=""M12.5164 6.37199L3.2998 15.5885V18.6998H6.41107L15.6276 9.48326L12.5164 6.37199Z"" stroke=""#FBBF24""
            stroke-width=""1.51449"" stroke-linecap=""round"" stroke-linejoin=""round"" />
        </svg>

      </div>
      <div class=""hidden"" id=""retry__button"">
        <svg onclick=""handleUpload()"" class=""cursor-pointer absolute top-[40%] right-[40%]"" width=""24"" height=""24""
          viewBox=""0 0 24 24"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
          <mask id=""mask0_1250_32739"" style=""mask-type:alpha"" maskUnits=""userSpaceOnUse"" x=""0"" y=""0"" width=""24""
            height=""24"">
            <rect width=""24"" height=""24"" fill=""#D9D9D9"" />
          </mask>
          <g mask=""url(#mask0_1250_32739)"">
            <path
              d=""M12 22C10.75 22 9.57933 21.7627 8.488 21.288C7.396 20.8127 6.446 20.1707 5.638 19.362C4.82933 18.554 4.18733 17.604 3.712 16.512C3.23733 15.4207 3 14.25 3 13H5C5 14.95 5.67933 16.604 7.038 17.962C8.396 19.3207 10.05 20 12 20C13.95 20 15.604 19.3207 16.962 17.962C18.3207 16.604 19 14.95 19 13C19 11.05 18.3207 9.39567 16.962 8.037C15.604 6.679 13.95 6 12 6H11.85L13.4 7.55L12 9L8 5L12 1L13.4 2.45L11.85 4H12C13.25 4 14.421 4.23767 15.513 4.713C16.6043 5.18767 17.5543 5.829 18.363 6.637C19.171 7.44567 19.8127 8.39567 20.288 9.487C20.7627 10.579 21 11.75 21 13C21 14.25 20.7627 15.4207 20.288 16.512C19.8127 17.604 19.171 18.554 18.363 19.362C17.5543 20.1707 16.6043 20.8127 15.513 21.288C14.421 21.7627 13.25 22 12 22Z""
              fill=""white"" />
          </g>
        </svg>
        <svg onclick=""handleRemove()"" class=""cursor-pointer absolute bottom-1 right-[40%]"" width=""25"" height=""25""
          viewBox=""0 0 25 25"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
          <circle cx=""12.5"" cy=""12.5"" r=""12.5"" fill=""#DC2626"" />
          <path
            d=""M18.25 9.25L17.5995 18.3569C17.5434 19.1418 16.8903 19.75 16.1033 19.75H9.89668C9.10972 19.75 8.45656 19.1418 8.40049 18.3569L7.75 9.25M11.5 12.25V16.75M14.5 12.25V16.75M15.25 9.25V7C15.25 6.58579 14.9142 6.25 14.5 6.25H11.5C11.0858 6.25 10.75 6.58579 10.75 7V9.25M7 9.25H19""
            stroke=""white"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round"" />
        </svg>

      </div>
    </div>
    <svg width=""46"" height=""46"" viewBox=""0 0 46 46"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
      <path
        d=""M30.6668 13.4167C30.6668 17.6509 27.2343 21.0833 23.0002 21.0833C18.766 21.0833 15.3335 17.6509 15.3335 13.4167C15.3335 9.18248 18.766 5.75 23.0002 5.75C27.2343 5.75 30.6668 9.18248 30.6668 13.4167Z""
        stroke=""#9CA3AF"" stroke-width=""3.16667"" stroke-linecap=""round"" stroke-linejoin=""round"" />
      <path
        d=""M23.0002 26.8333C15.5903 26.8333 9.5835 32.8402 9.5835 40.25H36.4168C36.4168 32.8402 30.41 26.8333 23.0002 26.8333Z""
        stroke=""#9CA3AF"" stroke-width=""3.16667"" stroke-linecap=""round"" stroke-linejoin=""round"" />
    </svg>
    <div class=""text-gray-400 font-normal"">
        {GlobalLocalizer["SingleUploaderTitle", AspFor?.Metadata?.DisplayName]}
    </div>
  </div>
  <div id=""nc__media__{uniqName}"" data-min-height=""100"" data-not-dragable=""true"" class=""sheet z-10"" aria-hidden=""true"" role=""dialog"">
    <div class=""overlay bg-gray-800 opacity-75 w-full""></div>
 
    <div class=""contents"">
      <header class=""controls bg-white !hidden"">
        <button data-sheet-dismiss class=""close-sheet ml-auto w-2"" type=""button""
          title=""Close the sheet"">&times;</button>
        <div class=""draggable-area w-full flex justify-center"">
          <div class=""draggable-thumb w-12""></div>
        </div>
      </header>
      <main class=""body bg-gray-800 px-0 "">
        <div id=""cropperjsContainer__{uniqName}"" {MaxHeightAttribute} {MaxSizeAttribute} data-aspect-ratio=""1""
         {CropperDataAttribute} class=""pt-8 relative min-w-[360px] h-[100vh]  overflow-hidden hidden rounded__cropper   "">
          <div onclick=""setProfileCompressStatus()""
            class=""compressor__status  cursor-pointer absolute top-4 left-2 ltr:right-2  bg-green-100 h-5 min-w-[122px] w-fit px-2 py-0.5 flex justify-start rounded-md"">
            <div class="" mt-[1px]"">
              <svg width=""13"" height=""12"" viewBox=""0 0 13 12"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                <path fill-rule=""evenodd"" clip-rule=""evenodd""
                  d=""M10.5243 3.17574C10.7586 3.41005 10.7586 3.78995 10.5243 4.02426L5.72431 8.82426C5.49 9.05858 5.1101 9.05858 4.87578 8.82426L2.47578 6.42426C2.24147 6.18995 2.24147 5.81005 2.47578 5.57574C2.7101 5.34142 3.09 5.34142 3.32431 5.57574L5.30005 7.55147L9.67578 3.17574C9.9101 2.94142 10.29 2.94142 10.5243 3.17574Z""
                  fill=""#34D399"" />
              </svg>
            </div>
            <div class=""text-xs font-normal text-green-800 mx-0.5"">{GlobalLocalizer["UploaderCompressedTitle"]}    </div>
          </div>
          <div onclick=""setProfileCompressStatus()""
            class="" compressor__status hidden mb-8 cursor-pointer absolute top-4 left-2 ltr:right-2 bg-yellow-100 h-5 min-w-[122px] w-fit px-2 py-0.5 flex justify-start rounded-md"">
            <div class=""mt-[1px]"">
              <svg class=""svg-icon""
                style=""width: 0.8em; height: 0.8em;vertical-align: middle;fill: currentColor;overflow: hidden;""
                viewBox=""0 0 1024 1024"" version=""1.1"" xmlns=""http://www.w3.org/2000/svg"">
                <path
                  d=""M1001.661867 796.544c48.896 84.906667 7.68 157.013333-87.552 157.013333H110.781867c-97.834667 0-139.050667-69.504-90.112-157.013333l401.664-666.88c48.896-87.552 128.725333-87.552 177.664 0l401.664 666.88zM479.165867 296.533333v341.333334a32 32 0 1 0 64 0v-341.333334a32 32 0 1 0-64 0z m0 469.333334v42.666666a32 32 0 1 0 64 0v-42.666666a32 32 0 1 0-64 0z""
                  fill=""#FAAD14"" />
              </svg>
            </div>
            <div class=""text-xs font-normal text-yellow-800 mx-0.5"">{GlobalLocalizer["UploaderOriginalSizeTitle"]}</div>
          </div>

          <div class=""bg-gray-800 px-16 pb-12 pt-5 flex flex-col justify-between h-fit  absolute bottom-0 right-0 w-full "">
            <div class=""flex w-full items-center my-6"">
              <svg width=""24"" height=""24"" viewBox=""0 0 24 24"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                <path
                  d=""M4 16L8.58579 11.4142C9.36683 10.6332 10.6332 10.6332 11.4142 11.4142L16 16M14 14L15.5858 12.4142C16.3668 11.6332 17.6332 11.6332 18.4142 12.4142L20 14M14 8H14.01M6 20H18C19.1046 20 20 19.1046 20 18V6C20 4.89543 19.1046 4 18 4H6C4.89543 4 4 4.89543 4 6V18C4 19.1046 4.89543 20 6 20Z""
                  stroke=""white"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round"" />
              </svg>
              <input style=""direction: ltr"" type=""range"" name="""" id=""rangeInput"" class=""w-full mx-1 p-0 h-0.5 "" min=""0""
                max=""1"" step=""0.01"" value=""0"" oninput=""handleProfileZoom()"" />
              <svg width=""16"" height=""16"" viewBox=""0 0 16 16"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                <path
                  d=""M2.66663 10.6665L5.72382 7.60931C6.24452 7.08861 7.08874 7.08861 7.60944 7.60931L10.6666 10.6665M9.33329 9.33317L10.3905 8.27598C10.9112 7.75528 11.7554 7.75528 12.2761 8.27598L13.3333 9.33317M9.33329 5.33317H9.33996M3.99996 13.3332H12C12.7363 13.3332 13.3333 12.7362 13.3333 11.9998V3.99984C13.3333 3.26346 12.7363 2.6665 12 2.6665H3.99996C3.26358 2.6665 2.66663 3.26346 2.66663 3.99984V11.9998C2.66663 12.7362 3.26358 13.3332 3.99996 13.3332Z""
                  stroke=""white"" stroke-width=""1.33333"" stroke-linecap=""round"" stroke-linejoin=""round"" />
              </svg>
            </div>
            <div class=""flex w-full justify-between items-center"">
              <svg data-sheet-dismiss onclick=""handleProfileCrop()"" class=""cursor-pointer"" width=""48"" height=""48""
                viewBox=""0 0 48 48"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                <path d=""M10 26L18 34L38 14"" stroke=""#10B981"" stroke-width=""4"" stroke-linecap=""round""
                  stroke-linejoin=""round"" />
              </svg>
              <svg data-sheet-dismiss class=""cursor-pointer"" width=""41"" height=""42"" viewBox=""0 0 41 42"" fill=""none""
                xmlns=""http://www.w3.org/2000/svg"">
                <path d=""M10.25 31.25L30.75 10.75M10.25 10.75L30.75 31.25"" stroke=""#DC2626"" stroke-width=""3.41667""
                  stroke-linecap=""round"" stroke-linejoin=""round"" />
              </svg>
            </div>
          </div>
        </div>
        </main>
    </div>
  </div>
";
#pragma warning restore CS8604 // Possible null reference argument.
                output.Content.AppendHtml(RoundUploadreContent);
            }
            else if(Rounded == false && HasDetial == true)
            {
                var DetailedUploader = $@"
                   <div class=""bg-white relative  h-[107px] w-[344px] rounded-md flex justify-right items-center text-center mx-auto"">
    <div
      class=""relative bg-gray-200 h-[91px] w-[91px]  mx-2  first-letter: rounded-md flex items-center justify-center"">
      <input type=""file"" accept=""image/*"" class=""z-[2] opacity-0 w-full h-full absolute top-0 right-0 ""
        oninput=""initMediaCropper('nc__media__cropperjs__container__{uniqName}')"" data-sheet-control=""media__{uniqName}"" />
      <svg width=""49"" height=""49"" viewBox=""0 0 49 49"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
        <mask id=""mask0_2975_6707"" style=""mask-type:alpha"" maskUnits=""userSpaceOnUse"" x=""0"" y=""0"" width=""49""
          height=""49"">
          <rect width=""49"" height=""49"" fill=""#D9D9D9"" />
        </mask>
        <g mask=""url(#mask0_2975_6707)"">
          <path
            d=""M10.2083 42.875C9.08542 42.875 8.12379 42.4755 7.32346 41.6765C6.52449 40.8762 6.125 39.9146 6.125 38.7917V10.2083C6.125 9.08542 6.52449 8.12379 7.32346 7.32346C8.12379 6.52449 9.08542 6.125 10.2083 6.125H28.5833V10.2083H10.2083V38.7917H38.7917V20.4167H42.875V38.7917C42.875 39.9146 42.4755 40.8762 41.6765 41.6765C40.8762 42.4755 39.9146 42.875 38.7917 42.875H10.2083ZM34.7083 18.375V14.2917H30.625V10.2083H34.7083V6.125H38.7917V10.2083H42.875V14.2917H38.7917V18.375H34.7083ZM12.25 34.7083H36.75L29.0938 24.5L22.9688 32.6667L18.375 26.5417L12.25 34.7083Z""
            fill=""#6B7280"" />
        </g>
      </svg>
    </div>
    <div class=""  h-full flex justify-end items-start flex-col"">
      <div class=""font-semibold text-xs mb-4"">{GlobalLocalizer["Format"]}:<span class=""font-light text-xs mx-0.5"">{Format}</span></div>
      <div class=""font-semibold text-xs mb-4"">  {GlobalLocalizer["MaxSize"]}: <span class=""font-light text-xs mx-0.5""> {MaxSize} {GlobalLocalizer["Kilobyte"]}</span>
      </div>
      <div class=""font-semibold text-xs mb-4"">  {GlobalLocalizer["SuggestedHeight"]}: <span class=""font-light text-xs mx-0.5"">{MaxHeight} {GlobalLocalizer["Pixel"]}</span>
      </div>
    </div>


    <svg onclick=""handleMediaRemove('nc__media__cropperjs__container__{uniqName}')"" id=""media__remove__btn__{uniqName}""
      class=""hidden cursor-pointer z-[3] absolute top-4 right-3 ltr:left-3 ltr:right-0"" width=""17"" height=""17"" viewBox=""0 0 17 17"" fill=""none""
      xmlns=""http://www.w3.org/2000/svg"">
      <circle cx=""8.40517"" cy=""8.40517"" r=""8.40517"" fill=""#DC2626"" />
      <path
        d=""M12.2716 6.22037L11.8342 12.344C11.7965 12.8718 11.3573 13.2807 10.8281 13.2807H6.65468C6.12551 13.2807 5.68632 12.8718 5.64862 12.344L5.21122 6.22037M7.73277 8.23761V11.2635M9.75001 8.23761V11.2635M10.2543 6.22037V4.70744C10.2543 4.42891 10.0285 4.20312 9.75001 4.20312H7.73277C7.45425 4.20312 7.22846 4.42891 7.22846 4.70744V6.22037M4.70691 6.22037H12.7759""
        stroke=""white"" stroke-width=""1.34483"" stroke-linecap=""round"" stroke-linejoin=""round"" />
    </svg>
    <img class=""hidden absolute w-[91px] h-[91px] right-2 ltr:left-2 ltr:right-0 h-full object-cover rounded-md""
      id=""media__cropper__preview__{uniqName}"" />
    <div id=""media__preview__overlay__{uniqName}""
      class=""  hidden z-[3] bg-gray-700 absolute h-[91px] w-[91px] top-2 right-2 ltr:left-2 ltr:right-0 opacity-50 rounded-md"">

      <svg aria-hidden=""true"" id=""media__preview__loading__{uniqName}""
        class=""hidden absolute bottom-2 right-2    w-8 h-8 mr-2 text-transparent animate-spin dark:text-gray-600 fill-white""
        viewBox=""0 0 100 101"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
        <path
          d=""M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z""
          fill=""currentColor"" />
        <path
          d=""M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z""
          fill=""currentFill"" />
      </svg>
      <div class=""absolute bottom-2 left-2 text-white"" id=""media__percentage__view__{uniqName}"">0%</div>



    </div>
    <div id=""media__progress__bar__{uniqName}"" dir=""ltr""
      class=""z-[3] hidden absolute bottom-1 w-[97%] right-2 bg-gray-200 rounded-full h-1.5 dark:bg-gray-700"">
      <div class=""bg-green-500 h-1.5 rounded-full"" style=""width: 33%"">
      </div>
    </div>
    <div class=""hidden "" id=""media__retry__button__{uniqName}"">
      <svg onclick=""handleMediaUpload()"" class=""z-[4]  cursor-pointer absolute bottom-4 right-3"" width=""24"" height=""24""
        viewBox=""0 0 24 24"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
        <mask id=""mask0_1250_32739"" style=""mask-type:alpha"" maskUnits=""userSpaceOnUse"" x=""0"" y=""0"" width=""24""
          height=""24"">
          <rect width=""24"" height=""24"" fill=""#D9D9D9"" />
        </mask>
        <g mask=""url(#mask0_1250_32739)"">
          <path
            d=""M12 22C10.75 22 9.57933 21.7627 8.488 21.288C7.396 20.8127 6.446 20.1707 5.638 19.362C4.82933 18.554 4.18733 17.604 3.712 16.512C3.23733 15.4207 3 14.25 3 13H5C5 14.95 5.67933 16.604 7.038 17.962C8.396 19.3207 10.05 20 12 20C13.95 20 15.604 19.3207 16.962 17.962C18.3207 16.604 19 14.95 19 13C19 11.05 18.3207 9.39567 16.962 8.037C15.604 6.679 13.95 6 12 6H11.85L13.4 7.55L12 9L8 5L12 1L13.4 2.45L11.85 4H12C13.25 4 14.421 4.23767 15.513 4.713C16.6043 5.18767 17.5543 5.829 18.363 6.637C19.171 7.44567 19.8127 8.39567 20.288 9.487C20.7627 10.579 21 11.75 21 13C21 14.25 20.7627 15.4207 20.288 16.512C19.8127 17.604 19.171 18.554 18.363 19.362C17.5543 20.1707 16.6043 20.8127 15.513 21.288C14.421 21.7627 13.25 22 12 22Z""
            fill=""white"" />
        </g>
      </svg>
      <svg onclick=""handleMediaRemove('nc__media__cropperjs__container__{uniqName}')""
        class="" z-[4]  cursor-pointer absolute bottom-4 right-10"" width=""25"" height=""25"" viewBox=""0 0 25 25"" fill=""none""
        xmlns=""http://www.w3.org/2000/svg"">
        <circle cx=""12.5"" cy=""12.5"" r=""12.5"" fill=""#DC2626"" />
        <path
          d=""M18.25 9.25L17.5995 18.3569C17.5434 19.1418 16.8903 19.75 16.1033 19.75H9.89668C9.10972 19.75 8.45656 19.1418 8.40049 18.3569L7.75 9.25M11.5 12.25V16.75M14.5 12.25V16.75M15.25 9.25V7C15.25 6.58579 14.9142 6.25 14.5 6.25H11.5C11.0858 6.25 10.75 6.58579 10.75 7V9.25M7 9.25H19""
          stroke=""white"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round"" />
      </svg>

    </div>
  </div>
  <div id=""media__{uniqName}"" data-min-height=""100"" data-not-dragable=""true"" class=""sheet z-10"" aria-hidden=""true"" role=""dialog"">
    <!-- Dark background for the sheet -->
    <div class=""overlay bg-gray-800 opacity-75 w-full""></div>
    <!-- The sheet itself -->
    <div class=""contents"">
      <!-- Sheet controls -->
      <header class=""controls bg-white !hidden"">
        <!-- Button to close the sheet -->
        <button data-sheet-dismiss class=""close-sheet ml-auto w-2"" type=""button""
          title=""Close the sheet"">&times;</button>
        <!-- The thing to drag if you want to resize the sheet -->
        <div class=""draggable-area w-full flex justify-center"">
          <div class=""draggable-thumb w-12""></div>
        </div>
      </header>

      <!-- Body of the sheet -->
      <main class=""body bg-gray-800 px-0"">


        <div id=""nc__media__cropperjs__container__{uniqName}"" data-preview-target=""media__cropper__preview__{uniqName}""
          data-retry-target=""media__retry__button__{uniqName}"" data-progress-target=""media__progress__bar__{uniqName}""
          data-remove-target=""media__remove__btn__{uniqName}"" data-percentage-target=""media__percentage__view__{uniqName}""
          data-overlay-target=""media__preview__overlay__{uniqName}"" data-loading-target=""media__preview__loading__{uniqName}""
           {AspectRatioAttribute} {MaxHeightAttribute} {MaxSizeAttribute}  data-input-target=""{UploadHiddenClass}""
          class=""relative pt-16  min-w-[360px]  h-[100vh]  overflow-hidden"">

          <div onclick=""setMediaCompressStatus()""
            class=""media__compressor__status cursor-pointer absolute top-4 left-2 ltr:right-2  bg-green-100 h-5 min-w-[122px] w-fit px-2 py-0.5 flex justify-start rounded-md"">
            <div class="" mt-[1px]"">
              <svg width=""13"" height=""12"" viewBox=""0 0 13 12"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                <path fill-rule=""evenodd"" clip-rule=""evenodd""
                  d=""M10.5243 3.17574C10.7586 3.41005 10.7586 3.78995 10.5243 4.02426L5.72431 8.82426C5.49 9.05858 5.1101 9.05858 4.87578 8.82426L2.47578 6.42426C2.24147 6.18995 2.24147 5.81005 2.47578 5.57574C2.7101 5.34142 3.09 5.34142 3.32431 5.57574L5.30005 7.55147L9.67578 3.17574C9.9101 2.94142 10.29 2.94142 10.5243 3.17574Z""
                  fill=""#34D399"" />
              </svg>
            </div>
            <div class=""text-xs font-normal text-green-800 mx-0.5"">    {GlobalLocalizer["UploaderCompressedTitle"]}</div>
          </div>

          <div onclick=""setMediaCompressStatus()""
            class=""hidden media__compressor__status cursor-pointer absolute top-4 left-2 ltr:right-2 bg-yellow-100 h-5 min-w-[122px] w-fit px-2 py-0.5 flex justify-start rounded-md"">
            <div class=""mt-[1px]"">
              <svg class=""svg-icon""
                style=""width: 0.8em; height: 0.8em;vertical-align: middle;fill: currentColor;overflow: hidden;""
                viewBox=""0 0 1024 1024"" version=""1.1"" xmlns=""http://www.w3.org/2000/svg"">
                <path
                  d=""M1001.661867 796.544c48.896 84.906667 7.68 157.013333-87.552 157.013333H110.781867c-97.834667 0-139.050667-69.504-90.112-157.013333l401.664-666.88c48.896-87.552 128.725333-87.552 177.664 0l401.664 666.88zM479.165867 296.533333v341.333334a32 32 0 1 0 64 0v-341.333334a32 32 0 1 0-64 0z m0 469.333334v42.666666a32 32 0 1 0 64 0v-42.666666a32 32 0 1 0-64 0z""
                  fill=""#FAAD14"" />
              </svg>
            </div>
            <div class=""text-xs font-normal text-yellow-800 mx-0.5"">  {GlobalLocalizer["UploaderOriginalSizeTitle"]}</div>
          </div>

          <div class=""bg-gray-800 px-16 pb-12 pt-5 flex flex-col justify-between h-fit absolute bottom-0 right-0 w-full"">
            <div class=""flex w-full items-center my-6"">
              <svg width=""24"" height=""24"" viewBox=""0 0 24 24"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                <path
                  d=""M4 16L8.58579 11.4142C9.36683 10.6332 10.6332 10.6332 11.4142 11.4142L16 16M14 14L15.5858 12.4142C16.3668 11.6332 17.6332 11.6332 18.4142 12.4142L20 14M14 8H14.01M6 20H18C19.1046 20 20 19.1046 20 18V6C20 4.89543 19.1046 4 18 4H6C4.89543 4 4 4.89543 4 6V18C4 19.1046 4.89543 20 6 20Z""
                  stroke=""white"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round"" />
              </svg>
              <input style=""direction: ltr"" type=""range"" name="""" id=""rangeInput"" class=""w-full mx-1 p-0 h-0.5"" min=""0""
                max=""1"" step=""0.01"" value=""0"" oninput=""handleMediaZoom()"" />
              <svg width=""16"" height=""16"" viewBox=""0 0 16 16"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                <path
                  d=""M2.66663 10.6665L5.72382 7.60931C6.24452 7.08861 7.08874 7.08861 7.60944 7.60931L10.6666 10.6665M9.33329 9.33317L10.3905 8.27598C10.9112 7.75528 11.7554 7.75528 12.2761 8.27598L13.3333 9.33317M9.33329 5.33317H9.33996M3.99996 13.3332H12C12.7363 13.3332 13.3333 12.7362 13.3333 11.9998V3.99984C13.3333 3.26346 12.7363 2.6665 12 2.6665H3.99996C3.26358 2.6665 2.66663 3.26346 2.66663 3.99984V11.9998C2.66663 12.7362 3.26358 13.3332 3.99996 13.3332Z""
                  stroke=""white"" stroke-width=""1.33333"" stroke-linecap=""round"" stroke-linejoin=""round"" />
              </svg>
            </div>
            <div class=""flex w-full justify-between items-center"">
              <svg data-sheet-dismiss onclick=""handleMediaCrop()"" class=""cursor-pointer"" width=""48"" height=""48""
                viewBox=""0 0 48 48"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                <path d=""M10 26L18 34L38 14"" stroke=""#10B981"" stroke-width=""4"" stroke-linecap=""round""
                  stroke-linejoin=""round"" />
              </svg>
              <svg data-sheet-dismiss class=""cursor-pointer"" width=""41"" height=""42"" viewBox=""0 0 41 42"" fill=""none""
                xmlns=""http://www.w3.org/2000/svg"">
                <path d=""M10.25 31.25L30.75 10.75M10.25 10.75L30.75 31.25"" stroke=""#DC2626"" stroke-width=""3.41667""
                  stroke-linecap=""round"" stroke-linejoin=""round"" />
              </svg>
            </div>

          </div>
        </div>

      </main>

    </div>
  </div>                  
    ";

                output.Content.AppendHtml(DetailedUploader);
            }
          


      

        }
    }
}
