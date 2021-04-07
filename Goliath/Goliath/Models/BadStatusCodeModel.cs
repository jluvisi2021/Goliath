using System.ComponentModel.DataAnnotations;

namespace Goliath.Models
{
    public class BadStatusCodeModel
    {
        [Required]
        [MaxLength(length: 3)]
        [MinLength(length: 3)]
        [DataType(DataType.Text)]
        public string StatusCode { get; set; }

        [Required]
        public string LastPageVisited { get; set; }

        [Required]
        public string UserAgent { get; set; }
    }
}