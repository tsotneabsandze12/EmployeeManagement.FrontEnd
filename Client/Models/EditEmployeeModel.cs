using Client.Enums;
using Client.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Client.Models
{
    public class EditEmployeeModel
    {

        [MaxLength(11)]
        [Remote("CheckIfPersonalIdExists", "Home")]
        [RegularExpression(@"^([1-9]\d{10})?$", ErrorMessage = "Invalid format for personal id field")]
        public string PersonalId { get; set; }

        public string FirstName { get; set; }   

        public string LastName { get; set; }

        public GenderEnum Gender { get; set; }

        [BirthDateValidator(18)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        public StatusEnum Status { get; set; }


        [ReleaseDateValidator]
        public DateTime? DateReleased { get; set; }

        [Phone]
        [MaxLength(9)]
        [Remote("CheckIfPhoneNumberExists", "Home")]
        public string PhoneNumber { get; set; }

        public int PositionId { get; set; }


        [JsonIgnore]
        public List<SelectListItem> Positions { get; set; }
    }
}
