using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BatchDataEntry.Validators
{
    public class PathValidator: ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
                return new ValidationResult(false, "Il valore non può essere vuoto. ");
            else
            {               
                if (value.ToString().IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0)
                    return new ValidationResult(false, "Il percorso contiene caratteri non validi");
            }
            return ValidationResult.ValidResult;
        }

    }
}
