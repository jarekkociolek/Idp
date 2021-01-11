using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Idp.Server.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(200)]
        [Required]
        public string Subject { get; set; }

        [MaxLength(200)]
        public string Username { get; set; }

        [MaxLength(200)]
        public string Password { get; set; }

        [ConcurrencyCheck]
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        [MaxLength(200)]
        public string Email { get; set; }

        public ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();
    }
}
