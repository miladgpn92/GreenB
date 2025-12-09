using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Utilities
{
    public static class FileUpload
    {
        public static List<string> ImageType()
        {
            return new List<string>
            {
                "jpg",
                "png",
                "jpeg",
                "bmp"
            };
        }
        public static List<string> VideoType()
        {
            return new List<string>
            {
                "mkv",
                "avi",
                "3gp",
                "mp4"
            };
        }
        public static bool IsValidFile(string FileType)
        {
            FileType = FileType.Replace(".", "");
            if (!ImageType().Contains(FileType) && !VideoType().Contains(FileType))
                return false;
            return true;
        }

        public static string Upload(IFormFile file, string FileName, params string[] path)
        {
            string uploadpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload");
            if (!Directory.Exists(uploadpath))
            {
                Directory.CreateDirectory(uploadpath);
            }
            foreach (var item in path)
            {
                uploadpath = Path.Combine(uploadpath, item);
                if (!Directory.Exists(uploadpath))
                {
                    Directory.CreateDirectory(uploadpath);
                }

            }
            if (String.IsNullOrEmpty(FileName))
            {
                FileName = Guid.NewGuid().ToString();
                FileName = FileName + Path.GetExtension(file.FileName);
            }
            //else
            //{
            //    FileName = FileName + Path.GetExtension(file.FileName);
            //}

            var finalpath = Path.Combine(uploadpath, FileName);
            using (var Stream = new FileStream(finalpath, FileMode.Create))
            {
                file.CopyTo(Stream);
            }
            return FileName;
        }

        public static string DeleteFile(string FileName, params string[] path)
        {
            if (FileName.StartsWith("http") || FileName.StartsWith("https"))
                FileName = FileName.Split('/').Last();
            string uploadpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload");
            foreach (var item in path)
            {
                uploadpath = Path.Combine(uploadpath, item);
            }

            var finalpath = Path.Combine(uploadpath, FileName);
            File.Delete(finalpath);
            return FileName;
        }
    }
}
