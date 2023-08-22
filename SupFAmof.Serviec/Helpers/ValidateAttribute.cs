using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SupFAmof.Service.Helpers
{
    public class ValidateBooleanAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is bool boolValue)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? "Only 'true' or 'false' is allowed.");
        }
    }
}
