namespace Goliath.Models
{
    public class ExceptionHandlerModel
    {
        public string StatusCode { get; set; }
        public string OriginalPath { get; set; }
        public string RawExceptionMessage { get; set; }

        public string ExceptionSource { get; set; }

        public string ExceptionTargetSite { get; set; }

        public string ExceptionTargetHelpLink { get; set; }

        public string DateTime { get; set; }

    }
}
