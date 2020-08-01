using Mail.Helper;
using Microsoft.Extensions.FileProviders;

namespace Mail.Services
{
    public class BalanceFileProvider : PhysicalFileProvider, IBalanceFileProvider
    {
        public BalanceFileProvider(IAppSetting appSetting) : base(appSetting.BalanceStoredFilesPath)
        {

        }
    }
}
