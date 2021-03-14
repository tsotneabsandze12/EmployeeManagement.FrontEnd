using System;
using System.ComponentModel.DataAnnotations;

namespace Client.Helpers
{
    public class ReleaseDateValidator : ValidationAttribute
    {
        public ReleaseDateValidator()
        {
            ErrorMessage = "Release date value is invalid";
        }

        public override bool IsValid(object value)
        {
            var input = value;

            if (value == null)
                return true;

            if (DateTime.TryParse(input.ToString(), out var date))
                return date.CompareTo(DateTime.Today) < 1;

            return false;
        }
    }
}