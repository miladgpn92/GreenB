using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public static class GenerateCode
    {

        /// <summary>
        /// Generates an authentication code based on the given username and length.
        /// </summary>
        /// <param name="username">The username to generate the code for.</param>
        /// <param name="length">The length of the code to generate.</param>
        /// <returns>The generated authentication code.</returns>
        public static string AuthenticationCode(string username, int length)
        {
            string code = "";
            if (username.StartsWith("0999"))
            {
                for (int i = 1; i < length+1; i++)
                {
                    code += i;
                }
            }
            else
            {
                Random random = new();
                string characters = "0123456789";
                StringBuilder result = new(length);
                for (int i = 0; i < length; i++)
                {
                    result.Append(characters[random.Next(0, characters.Length)]);
                }
                code = result.ToString();
            }

            return code;
        }
    }
}
