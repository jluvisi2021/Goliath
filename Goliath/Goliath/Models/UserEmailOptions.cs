using System.Collections.Generic;
using System.Collections.Immutable;

namespace Goliath.Models
{
    public class UserEmailOptions
    {
        public List<string> ToEmails { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public ImmutableDictionary<string, string> Placeholders { get; set; }
    }
}