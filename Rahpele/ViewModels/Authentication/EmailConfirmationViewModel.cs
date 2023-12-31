﻿using System.ComponentModel.DataAnnotations;

namespace Rahpele.ViewModels.Authentication
{
    public class EmailConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string EmailConfirmationCode { get; set; }
    }
}
