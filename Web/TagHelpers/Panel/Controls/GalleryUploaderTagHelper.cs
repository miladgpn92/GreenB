using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using ResourceLibrary.Panel.Common.Global;

namespace Web.TagHelpers.Panel.Controls
{

    [HtmlTargetElement("dt:gallery-uploader", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class GalleryUploaderTagHelper : TagHelper
    {

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContext { get; set; }
        private readonly IHtmlGenerator _generator;

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

        private readonly IStringLocalizer<Global> GlobalLocalizer;

        public ModelExpression? AspFor { get; set; }

        public string HelperText { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public GalleryUploaderTagHelper(IHtmlGenerator generator, IStringLocalizer<ResourceLibrary.Panel.Common.Global.Global> _GlobalLocalizer)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            GlobalLocalizer = _GlobalLocalizer;
            _generator = generator;

        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var uniqName = AspFor?.Name.Replace('.', '_');
            output.TagName = "div";
            output.Attributes.Add("id", "input__hidden__area__" + uniqName);
            var UploadHidden = _generator.GenerateHidden(ViewContext, AspFor?.ModelExplorer, AspFor?.Name, AspFor?.Model, true, new { });
            UploadHidden.Attributes.Add("data-multi-parentid", "multi__media__preview__container__" + uniqName);
            output.Content.AppendHtml(UploadHidden);


            string? CompressionDataAttribute = null;
            if (DisableCompression)
                CompressionDataAttribute = $@"data-not-compress='false'";

            string? MaxSizeAttribute = null;
            if (MaxSize != null)
            {
                MaxSizeAttribute = $@"data-max-size='{MaxSize}'";
            }

            string? MaxHeightAttribute = null;
            if (MaxHeight != null)
            {
                MaxHeightAttribute = $@"data-max-height='{MaxHeight}'";
            }

            string? CropperBottomSheet = null;
            string? CropperDataAttribute = null;
            if (DisableCropper)
            {
                CropperDataAttribute = $@"data-not-crop='true'";
            }
            else
            {
                CropperBottomSheet = $@"data-sheet-control='multi__media__{uniqName}'";
            }

            string? AspectRatioAttribute = $@"data-aspect-ratio='{AspectRatio}'";


#pragma warning disable CS8604 // Possible null reference argument.
            var GalleryContent = $@"<div
    class=""relative cursor-pointer bg-gray-100 border-[2px] border-gray-200 border-dashed   h-[120px] w-[350px] rounded-md flex justify-center items-center text-center mx-auto flex-col"">
    <input id=""multi__media__input__{uniqName}"" type=""file"" accept=""image/*"" multiple class=""z-[2] opacity-0 w-full h-full absolute""
      oninput=""initMultiCropper('multi__media__cropperjs__container__{uniqName}')"" {CropperBottomSheet} />
    <svg class=""mb-1"" width=""48"" height=""48"" viewBox=""0 0 48 48"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
      <path
        d=""M28 8H12C10.9391 8 9.92172 8.42143 9.17157 9.17157C8.42143 9.92172 8 10.9391 8 12V32M8 32V36C8 37.0609 8.42143 38.0783 9.17157 38.8284C9.92172 39.5786 10.9391 40 12 40H36C37.0609 40 38.0783 39.5786 38.8284 38.8284C39.5786 38.0783 40 37.0609 40 36V28M8 32L17.172 22.828C17.9221 22.0781 18.9393 21.6569 20 21.6569C21.0607 21.6569 22.0779 22.0781 22.828 22.828L28 28M40 20V28M40 28L36.828 24.828C36.0779 24.0781 35.0607 23.6569 34 23.6569C32.9393 23.6569 31.9221 24.0781 31.172 24.828L28 28M28 28L32 32M36 8H44M40 4V12M28 16H28.02""
        stroke=""#9CA3AF"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round"" />
    </svg>
    <div class=""font-normal text-sm text-indigo-600"">  {GlobalLocalizer["SingleUploaderTitle", AspFor?.Metadata.DisplayName]} </div>
  </div>
<div class=""helper__text text-center direction__ltr"">
    <small>
{HelperText}
    </small>
</div>
  <div class=""flex  flex-wrap justify-end w-[350px] mx-auto my-3"" id=""multi__media__preview__container__{uniqName}"">
  </div>

  <div id=""multi__media__{uniqName}"" data-min-height=""100"" data-not-dragable=""true"" class=""sheet z-20"" aria-hidden=""true""
    role=""dialog"">
    <!-- Dark background for the sheet -->
    <div onclick=""cancelAllCrops('multi__media__input__{uniqName}')"" class=""overlay bg-gray-800 opacity-75 w-full""></div>
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
      <div id=""cropper__dismisser"" data-sheet-dismiss class=""w-0 h-0 p-0 m-0""></div>
      <!-- Body of the sheet -->
      <main class=""body bg-gray-800 px-0"">
        <div id=""multi__media__cropperjs__container__{uniqName}"" data-preview-target=""multi__media__cropper__preview__{uniqName}""
          data-retry-target=""multi__media__retry__button__{uniqName}"" data-progress-target=""multi__media__progress__bar__{uniqName}""
          data-remove-target=""multi__media__remove__btn__{uniqName}"" data-percentage-target=""multi__media__percentage__view__{uniqName}""
          data-overlay-target=""multi__media__preview__overlay__{uniqName}"" data-loading-target=""multi__media__preview__loading__{uniqName}""
          data-input-area=""input__hidden__area__{uniqName}"" data-preview-container=""multi__media__preview__container__{uniqName}""
          {MaxSizeAttribute} {MaxHeightAttribute} {AspectRatioAttribute} {CropperDataAttribute}
         {CompressionDataAttribute} class=""hidden relative pt-16  min-w-[360px]  h-[100%] "">
          <div class=""absolute top-4 right-4 text-white""><span class=""file__index"">1</span>/<span
              class=""file__count"">3</span></div>
          <div onclick=""setMultiMediaCompressStatus()""
            class=""multi__media__compressor__status cursor-pointer absolute top-4 left-2 ltr:right-2  bg-green-100 h-5 min-w-[122px] w-fit px-2 py-0.5 flex justify-start rounded-md"">
            <div class="" mt-[1px]"">
              <svg width=""13"" height=""12"" viewBox=""0 0 13 12"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                <path fill-rule=""evenodd"" clip-rule=""evenodd""
                  d=""M10.5243 3.17574C10.7586 3.41005 10.7586 3.78995 10.5243 4.02426L5.72431 8.82426C5.49 9.05858 5.1101 9.05858 4.87578 8.82426L2.47578 6.42426C2.24147 6.18995 2.24147 5.81005 2.47578 5.57574C2.7101 5.34142 3.09 5.34142 3.32431 5.57574L5.30005 7.55147L9.67578 3.17574C9.9101 2.94142 10.29 2.94142 10.5243 3.17574Z""
                  fill=""#34D399"" />
              </svg>
            </div>
            <div class=""text-xs font-normal text-green-800 mx-0.5"">   {GlobalLocalizer["UploaderCompressedTitle"]}  </div>
          </div>

          <div onclick=""setMultiMediaCompressStatus()""
            class=""hidden multi__media__compressor__status cursor-pointer absolute top-4 left-2 ltr:right-2 bg-yellow-100 h-5 min-w-[122px] w-fit px-2 py-0.5 flex justify-start rounded-md"">
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
                max=""1"" step=""0.01"" value=""0"" oninput=""handleMultiMediaZoom()"" />
              <svg width=""16"" height=""16"" viewBox=""0 0 16 16"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                <path
                  d=""M2.66663 10.6665L5.72382 7.60931C6.24452 7.08861 7.08874 7.08861 7.60944 7.60931L10.6666 10.6665M9.33329 9.33317L10.3905 8.27598C10.9112 7.75528 11.7554 7.75528 12.2761 8.27598L13.3333 9.33317M9.33329 5.33317H9.33996M3.99996 13.3332H12C12.7363 13.3332 13.3333 12.7362 13.3333 11.9998V3.99984C13.3333 3.26346 12.7363 2.6665 12 2.6665H3.99996C3.26358 2.6665 2.66663 3.26346 2.66663 3.99984V11.9998C2.66663 12.7362 3.26358 13.3332 3.99996 13.3332Z""
                  stroke=""white"" stroke-width=""1.33333"" stroke-linecap=""round"" stroke-linejoin=""round"" />
              </svg>
            </div>
            <div class=""flex w-full justify-between items-center"">
              <svg onclick=""handleMultiMediaCrop()"" class=""cursor-pointer"" width=""48"" height=""48"" viewBox=""0 0 48 48""
                fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                <path d=""M10 26L18 34L38 14"" stroke=""#10B981"" stroke-width=""4"" stroke-linecap=""round""
                  stroke-linejoin=""round"" />
              </svg>
              <svg onclick=""cancelImage()"" class=""cursor-pointer"" width=""41"" height=""42"" viewBox=""0 0 41 42"" fill=""none""
                xmlns=""http://www.w3.org/2000/svg"">
                <path d=""M10.25 31.25L30.75 10.75M10.25 10.75L30.75 31.25"" stroke=""#DC2626"" stroke-width=""3.41667""
                  stroke-linecap=""round"" stroke-linejoin=""round"" />
              </svg>
            </div>
            <div onclick=""cancelAllCrops('multi__media__input__{uniqName}')"" class=""w-full text-center cursor-pointer mt-6""
              data-sheet-dismiss>
              <div class=""mx-auto text-white text-sm font-normal"">  {GlobalLocalizer["CancelAll"]}</div>
            </div>
          </div>
        </div>

      </main>

    </div>
  </div>";
#pragma warning restore CS8604 // Possible null reference argument.

            output.Content.AppendHtml(GalleryContent);
        }

    }
}
