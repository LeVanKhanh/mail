using Microsoft.Extensions.Configuration;
using System;

namespace Mail.Helper
{
    public class AppSetting : IAppSetting
    {
        private readonly IConfiguration _config;
        public AppSetting(IConfiguration config)
        {
            _config = config;
        }
        public string EmailUserName => _config.GetValue<string>("MailSetting:UserName");

        public string EmailPassword => _config.GetValue<string>("MailSetting:Password");
    }
}
