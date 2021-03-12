using System;
using System.ComponentModel.DataAnnotations;

namespace Client.Helpers
{
    public class BirthDateValidator: ValidationAttribute
    {
        private readonly int _minimumAge;
        public BirthDateValidator(int minimumAge)
        {
            _minimumAge = minimumAge;
            ErrorMessage = $"minimum age of {_minimumAge} is required";
        }


        public override bool IsValid(object value)
        {
            var input = value;

            if (value == null)
                return true;

            if (DateTime.TryParse(input.ToString(), out var date))
                return  date.AddYears(_minimumAge) < DateTime.Now && (DateTime.Now.Year - date.Year) < 100;

            return false;
        }
    }
    
}