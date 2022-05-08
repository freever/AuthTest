﻿using System.ComponentModel.DataAnnotations;

namespace AuthServer.ViewModels
{
    public class LoginViewModel
    {
        [Required] public string Username { get; set; } = null!;
        [Required] public string Password { get; set; } = null!;
        public string? ReturnUrl { get; set; } 
    }
}
