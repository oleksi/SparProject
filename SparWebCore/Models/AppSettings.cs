namespace SparWebCore.Models
{
    public class AppSettings
    {
        public string ProfilePicsUrl { get; set; }
        public string GymPicsUrl { get; set; }
        public string ElasticApiKey { get; set; }
        public string ElasticEndpoint { get; set; }
        public string ElasticFromEmail { get; set; }
        public string EmailSupport { get; set; }
        public string EmailAdmin { get; set; }
        public string AdminName { get; set; }
        public bool Include3rdPartyScripts { get; set; }
        public bool ElmahMvcDisableHandler { get; set; }
        public bool ElmahMvcDisableHandleErrorFilter { get; set; }
        public bool ElmahMvcRequiresAuthentication { get; set; }
        public bool ElmahMvcIgnoreDefaultRoute { get; set; }
        public string ElmahMvcAllowedRoles { get; set; }
        public string ElmahMvcAllowedUsers { get; set; }
        public string ElmahMvcRoute { get; set; }
        public bool ElmahMvcUserAuthCaseSensitive { get; set; }
        public string ReCaptchaPublicKey { get; set; }
        public string ReCaptchaPrivateKey { get; set; }
    }
}
