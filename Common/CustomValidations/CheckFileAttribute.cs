using Common.Consts;
using Common.Enums;
using Common.Utilities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Common.CustomValidations
{
    /// <summary>
    /// the attribute for checking files incoming from client.
    /// </summary>
    public class CheckFileAttribute : ValidationAttribute
    {
        private readonly FileType _type;
        private readonly int _maxFileSize;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">file type</param>
        /// <param name="maxFileSize">maximum file size</param>
        public CheckFileAttribute(FileType type, int maxFileSize = 3 * 1024 * 1024)
        {
            _type = type;
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                if (value is IFormFile file)
                {
                    if (!FileUtility.FileTypeIsValid(file.FileName, _type))
                    {
                        return new ValidationResult(ErrMsg.FileTypeIsWrong);
                    }

                    if (file.Length > _maxFileSize)
                    {
                        return new ValidationResult(ErrMsg.FileMaxSize);
                    }
                }
                else if (value is string fileName)
                {
                    if (!FileUtility.FileTypeIsValid(fileName, _type))
                    {
                        return new ValidationResult(ErrMsg.FileTypeIsWrong);
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}