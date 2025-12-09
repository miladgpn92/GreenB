using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public static class FileTypeUtility
    {
        public static MediaFileType? GetMediaType(string url)
        {
            // Extract the file extension from the URL
            var extension = System.IO.Path.GetExtension(url)?.ToLower();


            // List of image and video extensions
            var imageExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".gif",".webp" };
            var videoExtensions = new HashSet<string> { ".mp4", ".avi", ".mov", ".mkv" };


            if (imageExtensions.Contains(extension))
            {
                return MediaFileType.Image;
            }

            if (videoExtensions.Contains(extension))
            {
                return MediaFileType.Video;
            }

            return null; // Unsupported type
        }
    }
}
