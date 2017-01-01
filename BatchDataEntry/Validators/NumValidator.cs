using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BatchDataEntry.Validators
{
    class NumValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
                return new ValidationResult(false, "Il valore non può essere vuoto. ");
            else
            {
                var regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
                var parsingOk = !regex.IsMatch(value.ToString());
                if (!parsingOk)
                {
                    return new ValidationResult(false, "Il campo deve essere un numero");
                }
            }
            return ValidationResult.ValidResult;
        }
    }
}
