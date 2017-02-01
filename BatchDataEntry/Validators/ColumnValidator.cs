using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BatchDataEntry.Validators
{
    public class ColumnValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if(value == null)
                return new ValidationResult(false, "Il valore non può essere vuoto.");
            else
            {
                if(value.ToString().Length < 2)
                    return new ValidationResult(false, "Il nome della colonna è troppo corto.");
                else if(value.ToString().Length > 255)
                    return new ValidationResult(false, "Il nome è troppo lungo.");
                else if(HasCheckSpace(value.ToString()))
                    return  new ValidationResult(false, "Il nome contiene spazi (usare _ )");
                else if(HasInvalidChar(value.ToString()))
                    return new ValidationResult(false, "Il nome contiene dei caratteri invalidi.");
            }
            return ValidationResult.ValidResult;
        }

        private bool HasCheckSpace(string value)
        {
            return value.Contains(" ");
        }

        public bool HasInvalidChar(string value)
        {
            string pattern = "[^0-9a-zA-Z_]";
            Regex regex = new Regex(pattern);
            return regex.Match(value).Success;
        }
    }
}
