using Common;
using Common.Enums;
using Common.Utilities;
using DariaCMS.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using ResourceLibrary.Resources.Filemanager;
using ResourceLibrary.Resources.Usermanager;
using SharedModels.Dtos;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.CMS
{

    /// This Service must be Optimized
    public class FilemanagerService : IScopedDependency, IFilemanagerService
    {
        private string RootPath = "wwwroot/files";

        private readonly IFileProvider _fileProvider;
        public IStringLocalizer<FilemanagerRes> _Localizer { get; }

        public FilemanagerService(IFileProvider fileProvider, IStringLocalizer<FilemanagerRes> localizer)
        {
            _fileProvider = fileProvider;
            _Localizer = localizer;
        }


        public ResponseModel CreateFolder(string FolderPath)
        {
            if (FolderPath == null)
                FolderPath = RootPath;

            FolderPath = HandleFolderPath(FolderPath);

            if (!FolderPath.StartsWith(RootPath))
            {
                var msg = _Localizer["UserExist"];

                return new ResponseModel(false, msg.Value);
            }
            string uploadpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            foreach (var item in FolderPath.Split("/"))
            {
                if (item != "wwwroot")
                {
                    uploadpath = Path.Combine(uploadpath, item);
                    if (!Directory.Exists(uploadpath))
                    {
                        Directory.CreateDirectory(uploadpath);
                    }
                }
            }

            return new ResponseModel(true, "");
        }


        public ResponseModel<List<FileMangerDto>> GetFiles(GetFileViewModel GetFiles, string BaseUrl)
        {

            if (GetFiles.FilePath == null)
                GetFiles.FilePath = RootPath;

            GetFiles.FilePath = HandleFolderPath(GetFiles.FilePath);


            GetFiles.Pager = NormalizePager(GetFiles.Pager);

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(GetFiles.FilePath);
                FileInfo[] fileInfos = directoryInfo.GetFiles();

                List<FileMangerDto> fileDetailsList = new List<FileMangerDto>();
                if (GetFiles.type != null)
                {
                    fileInfos = fileInfos.Where(fileInfo => FileUtility.IsMatchingFileType(fileInfo.Extension, GetFiles.type.Value)).ToArray();
                }

                if (!string.IsNullOrEmpty(GetFiles.SearchText))
                {
                    fileInfos = fileInfos.Where(fileInfo => fileInfo.Name.Contains(GetFiles.SearchText, StringComparison.OrdinalIgnoreCase)).ToArray();
                }

                foreach (FileInfo fileInfo in fileInfos)
                {

                    FileMangerDto fileDetails = new FileMangerDto
                    {
                        Name = fileInfo.Name,
                        FileUrl = BaseUrl + "/" + fileInfo.Name,
                        Length = fileInfo.Length,
                        LastModified = fileInfo.CreationTime,
                        FileType = FileUtility.GetTypeFromExtention(fileInfo.Extension)
                    };

                    fileDetailsList.Add(fileDetails);

                }

                // Apply pagination

                fileDetailsList = fileDetailsList.Skip((GetFiles.Pager.PageNumber - 1) * GetFiles.Pager.PageSize)
                .Take(GetFiles.Pager.PageSize).ToList();
                return new ResponseModel<List<FileMangerDto>>(true, fileDetailsList);
            }
            catch (DirectoryNotFoundException)
            {
                return new ResponseModel<List<FileMangerDto>>(false, null, _Localizer["PathNotFound"]);
            }
            catch
            {
                return new ResponseModel<List<FileMangerDto>>(false, null, _Localizer["UnexpectedError"]);
            }
        }

        public ResponseModel<List<FolderDto>> GetFolders(GetFolderViewModel GetFolders, string BaseUrl)
        {

            if (GetFolders.FilePath == null)
                GetFolders.FilePath = RootPath;

            GetFolders.FilePath = HandleFolderPath(GetFolders.FilePath);
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(GetFolders.FilePath);
                DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();

                List<FolderDto> folderList = new List<FolderDto>();


                if (!string.IsNullOrEmpty(GetFolders.SearchText))
                {
                    directoryInfos = directoryInfos.Where(fileInfo => fileInfo.Name.Contains(GetFolders.SearchText, StringComparison.OrdinalIgnoreCase)).ToArray();
                }

                foreach (DirectoryInfo fileInfo in directoryInfos)
                {

                    FolderDto folderItem = new FolderDto
                    {
                        Name = fileInfo.Name,
                        LastModified = fileInfo.CreationTime,
                    };

                    folderList.Add(folderItem);

                }

                // Apply pagination

                return new ResponseModel<List<FolderDto>>(true, folderList);
            }
            catch (DirectoryNotFoundException)
            {
                return new ResponseModel<List<FolderDto>>(false, null, _Localizer["PathNotFound"]);
            }
            catch 
            {
                return new ResponseModel<List<FolderDto>>(false, null, _Localizer["UnexpectedError"]);
            }





        }

        public ResponseModel RemoveFiles(string[] files)
        {


            try
            {

                foreach (var filePath in files)
                {

                    string FilePath;
                    FilePath = HandleFolderPath(filePath);

                    if (!FilePath.StartsWith(RootPath))
                    {
                        return new ResponseModel(false, _Localizer["DeletePathForbiden"]);

                    }

                    var FolderServerPath = Directory.GetCurrentDirectory();
                    foreach (var item2 in FilePath.Split("/"))
                    {
                        FolderServerPath += "\\" + item2;
                    }

                    if (System.IO.File.Exists(FilePath))
                    {

                        System.IO.File.Delete(FilePath);

                    }

                }

                return new ResponseModel(true, "");


            }
            catch (Exception ee)
            {

                return new ResponseModel(false, ee.Message);

            }
        }

        public ResponseModel RemoveFolders(string[] folders)
        {

            foreach (var folderPath in folders)
            {
                string FolderPath = folderPath;
                FolderPath = HandleFolderPath(FolderPath);

                if (!FolderPath.StartsWith(RootPath))
                    return new ResponseModel(false, _Localizer["RemoveFolderNotAllowed"]);

                var FolderServerPath = Directory.GetCurrentDirectory();
                foreach (var item2 in FolderPath.Split("/"))
                {
                    FolderServerPath += "\\" + item2;
                }
 
       

                if (Directory.Exists(FolderServerPath))
                {
                    Directory.Delete(FolderServerPath, true);
                }
                else
                {
                    // The directory does not exist.
                }

            

            }
            return new ResponseModel(true, "");
        }


        public ResponseModel<List<string>> UploadFiles(UploadMultiFile files, string BaseUrl)
        {
            // Input validation
            if (string.IsNullOrEmpty(files.Filepath))
                return new ResponseModel<List<string>>(false, null, _Localizer["FolderIsRequired"]);

            files.Filepath = files.Filepath.Trim();
            List<string> FileNames = new List<string>();
            files.Filepath = HandleFolderPath(files.Filepath);

            // Security check for upload path
            if (!files.Filepath.StartsWith(RootPath))
                return new ResponseModel<List<string>>(false, null, _Localizer["UploadPathNotAllowed"]);

            if (files.Files != null)
            {
                // Prepare upload directory
                string uploadpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                foreach (var item in files.Filepath.Split("/"))
                {
                    if (item != "wwwroot")
                    {
                        uploadpath = Path.Combine(uploadpath, item);
                        if (!Directory.Exists(uploadpath))
                        {
                            Directory.CreateDirectory(uploadpath);
                        }
                    }
                }

                // Process each uploaded file
                foreach (var item in files.Files)
                {
                    if (item != null)
                    {
                        // Generate unique filename based on file type
                        string originalExtension = Path.GetExtension(item.FileName).ToLower();
                        string FilePath = GenerateUniqueFilePath(files.Filepath, item.FileName);

                        // Check if the file is an image
                        if (IsImageFile(item.FileName))
                        {
                            // Optimize and convert image to WebP
                            FilePath = Path.ChangeExtension(FilePath, ".webp");
                            FilePath = OptimizeAndConvertToWebP(item, FilePath);
                        }
                        else
                        {
                            // Save non-image files as-is
                            using (var Stream = new FileStream(FilePath, FileMode.Create))
                            {
                                item.CopyTo(Stream);
                            }
                        }

                        // Generate URL for the uploaded file
                        var RealFilePath = BaseUrl + FilePath.Replace(Directory.GetCurrentDirectory(), "").Replace("\\", "/").Replace("wwwroot/", "");
                        FileNames.Add(RealFilePath);
                    }
                }
            }
            return new ResponseModel<List<string>>(true, FileNames);
        }

        // Helper method to generate a unique file path
        private string GenerateUniqueFilePath(string filepath, string originalFileName)
        {
            string FilePath;
            string fileExtension = Path.GetExtension(originalFileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);

            for (int i = 0; ; i++)
            {
                FilePath = Directory.GetCurrentDirectory();
                foreach (var item2 in filepath.Split("/"))
                {
                    FilePath += "\\" + item2;
                }

                // Append counter only if it's not the first attempt
                string counterSuffix = (i == 0 ? "" : "-" + i.ToString());
                FilePath += "\\" + fileNameWithoutExtension + counterSuffix + fileExtension;

                if (!System.IO.File.Exists(FilePath))
                { break; }
            }
            return FilePath;
        }

        // Helper method to check if the file is an image
        private bool IsImageFile(string fileName)
        {
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp" };
            string fileExtension = Path.GetExtension(fileName).ToLower();
            return imageExtensions.Contains(fileExtension);
        }

        // Helper method to optimize and convert image to WebP
        private string OptimizeAndConvertToWebP(IFormFile file, string outputPath)
        {
            using (var stream = file.OpenReadStream())
            using (var image = Image.Load(stream))
            {
                // Resize large images
                int maxWidth = 1920; // Maximum width
                int maxHeight = 1080; // Maximum height

                if (image.Width > maxWidth || image.Height > maxHeight)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(maxWidth, maxHeight),
                        Mode = ResizeMode.Max
                    }));
                }

                // Save as WebP with compression
                using (var outputStream = new FileStream(outputPath, FileMode.Create))
                {
                    image.Save(outputStream, new WebpEncoder
                    {
                        Quality = 85 // Adjust quality (0-100)
                    });
                }
            }

            return outputPath;
        }



        public static Pageres NormalizePager(Pageres pageres)
        {
            if (pageres.PageSize > 100)
                pageres.PageSize = 100;
            if (pageres.PageSize == 0)
                pageres.PageSize = 10;
            if (pageres.PageNumber == 0)
                pageres.PageNumber = 1;

            return pageres;
        }

        /// <summary>
        /// Handles the folder path by adding wwwroot/ to the beginning, replacing any double slashes with single slashes, and removing any trailing slashes.
        /// </summary>
        /// <param name="FolderPath">The folder path to be handled.</param>
        /// <returns>The handled folder path.</returns>

        private string HandleFolderPath(string FolderPath)
        {
            if (!FolderPath.ToLower().StartsWith(RootPath) && !FolderPath.ToLower().StartsWith("/" + RootPath))
                FolderPath = RootPath + "/" + FolderPath;

            FolderPath = FolderPath.Replace("///", "/").Replace("//", "/");

            if (FolderPath.StartsWith("/"))
                FolderPath = FolderPath[1..];

            if (FolderPath.EndsWith("/"))
                FolderPath = FolderPath.Remove(FolderPath.Length - 1);
            return FolderPath.ToLower();
        }

    }
}
