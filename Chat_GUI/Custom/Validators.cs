using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Chat_GUI
{
    public class ValidIPRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string ip = value as string;

            Regex ipRegex = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");

            if (ip == null || ip == "" || !ipRegex.Match(ip).Success)
            {
                return new ValidationResult(false, "Error!");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
    public class FieldNotEmptyRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string name = value as string;

            if (name == null || name == "")
            {
                return new ValidationResult(false, "Can't be left empty!");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
