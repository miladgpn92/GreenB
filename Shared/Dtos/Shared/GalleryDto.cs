using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Dtos.Shared
{
    internal class GalleryDto
    {
    }

    public class GalleryItem
    {
        public string Url { get; set; }

        public MediaFileType? Type { get; set; }

    }
}
