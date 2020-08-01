using Microsoft.Extensions.Configuration;
using System.IO;

namespace Mail.Helper
{
    public class AppSetting : IAppSetting
    {
        private readonly IConfiguration _config;
        public AppSetting(IConfiguration config)
        {
            _config = config;
        }
        public string EmailAddress => _config.GetValue<string>("MailSetting:EmailAddress");
        public string EmailPassword => _config.GetValue<string>("MailSetting:Password");
        public string EmailHost => _config.GetValue<string>("MailSetting:Host");
        public int EmailPort => _config.GetValue<int>("MailSetting:587");
        public string AccountStoredFilesPath => Path.GetFullPath($"{_config.GetValue<string>("StoredFilesPath")}/Account");
        public string BalanceStoredFilesPath => Path.GetFullPath($"{_config.GetValue<string>("StoredFilesPath")}/Balance");
        public string MailStoredFilesPath => Path.GetFullPath($"{_config.GetValue<string>("StoredFilesPath")}/Mail");
    }
}
