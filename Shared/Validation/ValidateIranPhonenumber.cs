using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharedModels.Validation
{
    public class ValidateIranPhonenumber : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Do nothing and return success
            }


            string errorText = GetErrorMessage();
        

            if (value.ToString().Length > 11 || value.ToString().Length < 11)
            {
           
                    return new ValidationResult(errorText);
            
            
            }



            var regex = @"^[\+]?[(]?[0-9]{4}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$";
            var match = Regex.Match(value.ToString(), regex, RegexOptions.IgnoreCase);




            if (match.Success)
            {
                return ValidationResult.Success;
            }

            

            if (!string.IsNullOrEmpty(errorText))
            {
                return new ValidationResult(errorText);
            }

            // Fallback to default error message
            string defaultErrorMessage = string.Format(ErrorMessage, validationContext.DisplayName);
            return new ValidationResult(defaultErrorMessage);;
        }

        private string GetErrorMessage()
        {
            var errorMessageResourceType = ErrorMessageResourceType;
            if (errorMessageResourceType != null && !string.IsNullOrEmpty(ErrorMessageResourceName))
            {
                var resourceManager = new ResourceManager(errorMessageResourceType);
                return resourceManager.GetString(ErrorMessageResourceName);
            }
            return null;
        }

    }


}
