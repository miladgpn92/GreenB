using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class EmailUtility
    {


        /// <summary>
        /// Reads the HTML template from the specified path and returns the content as a string.
        /// </summary>
        /// <param name="TemplateName">The name of the template to be read.</param>
        /// <returns>The content of the template as a string.</returns>
        public static string GetTemplate(string TemplateName)
        {
            string uploadpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DTCMS", "EmailTemplates", TemplateName);
            string body = string.Empty;
            //using streamreader for reading my htmltemplate   
            using (StreamReader reader = new StreamReader(uploadpath))
            {
                body = reader.ReadToEnd();
            }
                
            return body;
        }
    }
}
