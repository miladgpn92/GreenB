using Common;
using Common.Enums;
using SharedModels.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.CMS
{
    public interface IFilemanagerService
    {
        ResponseModel<List<FileMangerDto>> GetFiles(GetFileViewModel GetFiles, string BaseUrl);
        ResponseModel<List<FolderDto>> GetFolders(GetFolderViewModel GetFolders , string BaseUrl);
        ResponseModel RemoveFiles(string[] files);
        ResponseModel RemoveFolders(string[] folders);
        ResponseModel CreateFolder(string FolderPath);
        ResponseModel<List<string>> UploadFiles(UploadMultiFile files ,string BaseUrl);
    }
}
