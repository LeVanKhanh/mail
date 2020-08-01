using Mail.Helper;
using Microsoft.Extensions.FileProviders;

namespace Mail.Services
{
    public class MailFileProvider : PhysicalFileProvider, IMailFileProvider
    {
        public MailFileProvider(IAppSetting appSetting) : base(appSetting.MailStoredFilesPath)
        {

        }
    }
}
