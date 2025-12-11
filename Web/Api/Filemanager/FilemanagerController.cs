using Common;
using Common.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Services.Services.CMS;
using Shared.Api;
using SharedModels.Dtos;
using System.Data;

namespace Web.Api.Filemanager
{
    [ApiVersion("1")]
    [Route("api/admin/filemanager")]
    [ApiExplorerSettings(GroupName = RoleConsts.Admin)]
    [Authorize(Roles = RoleConsts.Admin, AuthenticationSchemes = "JwtScheme")]
    public class FilemanagerController : BaseController
    {
        private readonly IFilemanagerService _filemanagerService;
        private readonly ProjectSettings _projectsetting;

        public FilemanagerController(IFilemanagerService filemanagerService, IOptionsSnapshot<ProjectSettings> settings)
        {
            _filemanagerService = filemanagerService;
            _projectsetting = settings.Value;
        }


        /// <summary>
        /// Creates a new folder with the specified name.
        /// </summary>
        /// <param name="FolderName">The name of the folder to be created.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns an Ok result if the folder was created successfully, or a BadRequest result if the folder could not be created.</returns>

        [HttpPost("CreateFolder")]
        public IActionResult CreateFolder(string FolderName, CancellationToken cancellationToken)
        {
            var res = _filemanagerService.CreateFolder(FolderName);

            if (res.IsSuccess)
                return Ok();
            else
                return BadRequest(res.Description);
        }




        /// <summary>
        /// Gets the files from the filemanager service.
        /// </summary>
        /// <param name="model">The GetFileViewModel model.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// Returns an Ok result with the model if successful, otherwise a BadRequest result with the message.
        /// </returns>

        [HttpPost("GetFiles")]
        public IActionResult GetFiles(GetFileViewModel model, CancellationToken cancellationToken)
        {

            var res = _filemanagerService.GetFiles(model, _projectsetting.ProjectSetting.BaseUrl);

            if (res.IsSuccess)
                return Ok(res.Model);
            else
                return BadRequest(res.Message);
        }




        /// <summary>
        /// GetFolders method is used to get the folders from the filemanager service.
        /// </summary>
        /// <param name="model">GetFolderViewModel object</param>
        /// <param name="cancellationToken">CancellationToken object</param>
        /// <returns>
        /// Returns Ok result with model object if success, else returns BadRequest with error message.
        /// </returns>
        [HttpPost("GetFolders")]
        public IActionResult GetFolders(GetFolderViewModel model, CancellationToken cancellationToken)
        {

            var res = _filemanagerService.GetFolders(model, _projectsetting.ProjectSetting.BaseUrl);

            if (res.IsSuccess)
                return Ok(res.Model);
            else
                return BadRequest(res.Message);
        }



        /// <summary>
        /// Uploads multiple files to the server.
        /// </summary>
        /// <param name="model">The model containing the files to be uploaded.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// Returns an OkResult if the upload was successful, or a BadRequestResult if the upload failed.
        /// </returns>
        [HttpPost("Uplaod")]
        [RequestSizeLimit(52428800)]
        public IActionResult Uplaod([FromForm] UploadMultiFile model, CancellationToken cancellationToken)
        {
            var res = _filemanagerService.UploadFiles(model, _projectsetting.ProjectSetting.BaseUrl);
            if (res.IsSuccess)
                return Ok(res.Model);
            else
                return BadRequest(res.Message);
        }




        /// <summary>
        /// Removes files from the specified filepath.
        /// </summary>
        /// <param name="model">The model containing the filepath.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An IActionResult indicating the success or failure of the operation.</returns>
        [HttpDelete("RemoveFiles")]
        public IActionResult RemoveFiles([FromBody] RemoveFileModel model, CancellationToken cancellationToken)
        {

            var res = _filemanagerService.RemoveFiles(model.Filepath);

            if (res.IsSuccess)
                return Ok();
            else
                return BadRequest(res.Description);
        }



        /// <summary>
        /// Removes a folder from the file system.
        /// </summary>
        /// <param name="model">The model containing the folder path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An IActionResult indicating the success or failure of the operation.</returns>
        [HttpDelete("RemoveFolder")]
        public IActionResult RemoveFolder([FromBody] RemoveFolderModel model, CancellationToken cancellationToken)
        {

            var res = _filemanagerService.RemoveFolders(model.FolderPath);

            if (res.IsSuccess)
                return Ok();
            else
                return BadRequest(res.Description);
        }
    }
}
