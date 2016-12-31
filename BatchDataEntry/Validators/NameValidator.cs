using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BatchDataEntry.Validators
{
    public class NameValidator: ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
                return new ValidationResult(false, "Il valore non può essere vuoto. ");
            else
            {
                if(value.ToString().Length < 2)
                    return  new ValidationResult(false, "Il Nome è troppo corto. ");
                else if(value.ToString().Length > 255)
                    return new ValidationResult(false, "Il Nome è troppo lungo. ");
            }
            return ValidationResult.ValidResult;
        }
    }
}
