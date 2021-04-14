using System.ComponentModel.DataAnnotations;

namespace Goliath.Models
{
    public class ExceptionHandlerModel
    {
        [Required]
        [StringLength(maximumLength: 7, MinimumLength = 3)]
        public string StatusCode { get; set; }

        [Required]
        public string OriginalPath { get; set; }

        [Required]
        public string RawExceptionMessage { get; set; }

        [Required]
        public string ExceptionSource { get; set; }

        [Required]
        public string ExceptionTargetSite { get; set; }

        [Required]
        public string ExceptionTargetHelpLink { get; set; }

        [Required]
        public string DateTime { get; set; }
    }
}