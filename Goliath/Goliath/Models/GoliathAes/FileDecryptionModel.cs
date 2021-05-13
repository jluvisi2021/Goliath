using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Goliath.Models.GoliathAes
{
    /// <summary>
    /// The model that is used to decrypt a user data file. This model represents the data the user
    /// inputs including the actual file.
    /// </summary>
    public class FileDecryptionModel
    {
        [Required(ErrorMessage = "Please upload a file.")]
        public IFormFile DataFile { get; set; }

        [Required(ErrorMessage = "Please input the decryption key.")]
        public string SecretKey { get; set; }

        [Required(ErrorMessage = "Please input the Salt value.")]
        public string SaltValue { get; set; }
    }
}