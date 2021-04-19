using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace GameOfLifeApp {
    public class IntegerValidator : ValidationRule {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            if ((value as string).All(c => char.IsDigit(c))) {
                return ValidationResult.ValidResult;
            } else
                return new ValidationResult(false, "Only digits allowed");
        }
    }
}
