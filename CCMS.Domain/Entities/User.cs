// FILE: Entities/User.cs
using System;
using CCMS.Domain.Common;
using CCMS.Domain.Enums;

namespace CCMS.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string Organization { get; set; } = string.Empty;
    }
}
