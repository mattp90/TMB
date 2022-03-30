namespace Nop.Core.Configuration
{
    public class FtpConfig : IConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RequestFolder { get; set; }
        public string ResponseFolder { get; set; }
        public string ProcessedFolder { get; set; }
    }
}