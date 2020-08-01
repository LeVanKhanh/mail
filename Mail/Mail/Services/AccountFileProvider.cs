using Mail.Helper;
using Microsoft.Extensions.FileProviders;

namespace Mail.Services
{
    public class AccountFileProvider : PhysicalFileProvider, IAccountFileProvider
    {
        public AccountFileProvider(IAppSetting appSetting) : base(appSetting.AccountStoredFilesPath)
        {

        }
    }
}
