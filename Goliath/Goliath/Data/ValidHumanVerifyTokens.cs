﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goliath.Data
{
    /// <summary>
    /// Tokens which have been generated and prove the user is a human.
    /// </summary>
    [Table("ValidCaptchaCookieTokens")]
    public class ValidHumanVerifyTokens
    {
        [Key]
        public int NumericId { get; set; }

        [Required]
        [MinLength(Helper.GoliathHelper.MinimumTokenValueLength)]
        [MaxLength(Helper.GoliathHelper.MaximumTokenValueLength)]
        public string Token { get; set; }

        [Required]
        [MaxLength(22)]
        public string GeneratedDateTime { get; set; }
    }
}