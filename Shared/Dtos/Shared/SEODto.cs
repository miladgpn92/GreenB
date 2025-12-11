using ResourceLibrary.Resources.ErrorMsg;
using ResourceLibrary.Resources.SEO;
using ResourceLibrary.Resources.Usermanager;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Dtos
{
    public class SEODto
    {

        public int Id { get; set; }

        [Display(Name = "Path", ResourceType = typeof(SEORes))]
        public string Path { get; set; }

        [Display(Name = "SEOTitle", ResourceType = typeof(SEORes))]
        public string SEOTitle { get; set; }

        [Display(Name = "SEODesc", ResourceType = typeof(SEORes))]
        public string SEODesc { get; set; }

        public string SEOPic { get; set; }

        public DateTime? Date { get; set; }
    }
}
