﻿namespace FullStack.Api.Models
{
    public class LoginDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
