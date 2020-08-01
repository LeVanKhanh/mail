namespace Mail.Helper
{
    public interface IAppSetting
    {
        string EmailAddress { get; }
        string EmailPassword { get; }
        string EmailHost { get; }
        int EmailPort { get; }
        string AccountStoredFilesPath { get; }
        string BalanceStoredFilesPath { get; }
        string MailStoredFilesPath { get; }
    }
}
