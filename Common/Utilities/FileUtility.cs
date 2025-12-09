using Common.Enums;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.IO;
using System.Linq;

namespace Common.Utilities
{
    public static class FileUtility
    {



        public static bool IsMatchingFileType(string fileExtension, FileType fileType)
        {
            switch (fileType)
            {
                case FileType.Image:
                    return IsPictureExtension(fileExtension);

                case FileType.Video:
                    return IsVideoExtension(fileExtension);

                case FileType.Audio:
                    return IsAudioExtension(fileExtension);

                case FileType.Text:
                    return IsTextExtension(fileExtension);

                case FileType.PDF:
                    return IsPDFExtension(fileExtension);

                case FileType.Excel:
                    return IsExcelExtension(fileExtension);

                case FileType.Powerpoint:
                    return IsPowerpointExtension(fileExtension);


                case FileType.Zip:
                    return IsZipExtension(fileExtension);


                default:
                    return false;
            }
        }



        public static bool IsPictureExtension(string fileExtension)
        {
            // Add the picture file extensions you want to include
            string[] pictureExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".svg", ".webp", ".tiff" };
            return pictureExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsVideoExtension(string fileExtension)
        {
            // Add the video file extensions you want to include
            string[] videoExtensions = { ".mp4", ".avi", ".mov", ".webm", ".flv", ".mkv", ".wmv", ".mpeg", ".mpg" };
            return videoExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsAudioExtension(string fileExtension)
        {
            // Add the audio file extensions you want to include
            string[] audioExtensions = { ".mp3", ".wav", ".flac", ".ogg", ".aac", ".wma", ".midi", ".mid", ".m4a", ".webm", ".amr" };
            return audioExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }
        public static bool IsTextExtension(string fileExtension)
        {
            // Add the Text file extensions you want to include
            string[] audioExtensions = { ".txt", ".doc", ".docx", ".rtf" };
            return audioExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsPDFExtension(string fileExtension)
        {
            // Add the PDF file extensions you want to include
            string[] audioExtensions = { ".pdf" };
            return audioExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsZipExtension(string fileExtension)
        {
            // Add the Zip file extensions you want to include
            string[] audioExtensions = { ".zip", ".tar", ".gz", ".bz2", ".7z", ".rar" };
            return audioExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsExcelExtension(string fileExtension)
        {
            // Add the Excel file extensions you want to include
            string[] audioExtensions = { ".xls", ".xlsx", ".csv" };
            return audioExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsPowerpointExtension(string fileExtension)
        {
            // Add the Powerpoint file extensions you want to include
            string[] audioExtensions = { ".ppt", ".pptx" };
            return audioExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }
        public static FileType GetTypeFromExtention(string extension)
        {


            if (IsMatchingFileType(extension, FileType.Image))
                return FileType.Image;
            if (IsMatchingFileType(extension, FileType.Video))
                return FileType.Video;

            if (IsMatchingFileType(extension, FileType.Audio))
                return FileType.Audio;

            if (IsMatchingFileType(extension, FileType.Powerpoint))
                return FileType.Powerpoint;

            if (IsMatchingFileType(extension, FileType.PDF))
                return FileType.PDF;


            if (IsMatchingFileType(extension, FileType.Excel))
                return FileType.Excel;

            if (IsMatchingFileType(extension, FileType.Text))
                return FileType.Text;

            if (IsMatchingFileType(extension, FileType.Zip))
                return FileType.Zip;

            else
                return FileType.Unknown;
        }


        public static bool FileTypeIsValid(string file, FileType fileType)
        {
            string currentFileType = '.' + file.Split('.').Last().ToLower();

            var provider = new FileExtensionContentTypeProvider();

            var keys = provider.Mappings.Where(_ => _.Value.Contains(fileType.ToString().ToLower())).Select(_ => _.Key);

            var check = keys.Contains(currentFileType);

            return check;
        }
    }
}