using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Client.Enums;
using Client.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Client.Models
{
    public class RegisterModel
    {
        [MaxLength(11)]
        public string PersonalId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }


        public GenderEnum? Gender { get; set; }

        [BirthDateValidator(18)]
        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }


        [Required]
        [EmailAddress]
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?")]
        public string Email { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [JsonIgnore]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }
    }
}