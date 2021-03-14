using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Client.Enums;
using Client.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Client.Models
{
    public class AddEmployeeModel
    {
        [Required]
        [MaxLength(11)]
        [Remote("CheckIfPersonalIdExists", "Home")]
        [RegularExpression(@"^([1-9]\d{10})?$", ErrorMessage = "Invalid format for personal id field")]
        public string PersonalId { get; set; }

        [Required] public string FirstName { get; set; }

        [Required] public string LastName { get; set; }

        [Required] public GenderEnum Gender { get; set; }

        [Required]
        [BirthDateValidator(18)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        [Required] public StatusEnum Status { get; set; }


        [ReleaseDateValidator] public DateTime? DateReleased { get; set; }

        [Phone]
        [Required]
        [MaxLength(9)]
        [Remote("CheckIfPhoneNumberExists", "Home")]
        public string PhoneNumber { get; set; }

        [Required] public int PositionId { get; set; }

        [JsonIgnore] public List<SelectListItem> Positions { get; set; }
    }
}