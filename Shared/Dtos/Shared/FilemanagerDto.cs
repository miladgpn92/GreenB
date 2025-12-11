using Common.Enums;
using DariaCMS.Common;
using Microsoft.AspNetCore.Http;
using ResourceLibrary.Resources.ErrorMsg;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Dtos
{
    public class FileMangerDto
    {
        public long Length { get; set; }
        public string Name { get; set; }
        public DateTimeOffset LastModified { get; set; }
  
        public FileType FileType { get; set; }
        public string FileUrl { get; set; }
 
    }

    public class FolderDto
    {
      
        public string Name { get; set; }
        public DateTimeOffset LastModified { get; set; }

    }

    public class UploadMultiFile
    {

        [Display(Name = "Files", ResourceType = typeof(ResourceLibrary.Resources.Filemanager.FilemanagerRes))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequierdMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        public List<IFormFile> Files { get; set; }

        [Display(Name = "Filepath", ResourceType = typeof(ResourceLibrary.Resources.Filemanager.FilemanagerRes))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequierdMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        public string Filepath { get; set; }
   
    }


    public class RemoveFileModel
    {

        [Display(Name = "Filepath", ResourceType = typeof(ResourceLibrary.Resources.Filemanager.FilemanagerRes))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequierdMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        public string[] Filepath { get; set; }

    }

    public class RemoveFolderModel  
    {

        [Display(Name = "FolderPath", ResourceType = typeof(ResourceLibrary.Resources.Filemanager.FilemanagerRes))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequierdMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        public string[] FolderPath { get; set; }

    }

    public class GetFileViewModel
    {
        public Pageres Pager { get; set; } = new Pageres { PageSize = 10, PageNumber = 1 };
        public string FilePath { get; set; }
        public string SearchText { get; set; } = "";

        public FileType? type { get;set; }

       
    }

    public class GetFolderViewModel
    {
        public string FilePath { get; set; }
        public string SearchText { get; set; } = "";
 
    }


}
