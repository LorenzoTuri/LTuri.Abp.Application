using System.ComponentModel.DataAnnotations;

namespace LTuri.Abp.Application.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IsOneOfValidationAttribute : ValidationAttribute
    {
        protected string ValidValues { get; set; }

        public IsOneOfValidationAttribute(string validValues) : base()
        {
            ValidValues = validValues;
        }

        public override bool IsValid(object? value)
        {
            return ValidValues.Split(",").AsEnumerable().Contains(value);
        }
    }
}
