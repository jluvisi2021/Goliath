using System.Collections.Generic;

namespace Goliath.Models.GoliathAes
{
    /// <summary>
    /// A model that represents the result of the user attempting to decrypt their file.
    /// </summary>
    public class DecryptionResultModel
    {
        public bool IsSuccess { get; set; }
        public string FileName { get; set; }
        public string FileBytes { get; set; }
        public List<string> ModelErrors { get; set; }
    }
}