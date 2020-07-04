namespace Mail.Helper
{
    public interface IAppSetting
    {
        string EmailAddress { get; }
        string EmailPassword { get; }
        string EmailHost { get; }
        int EmailPort { get; }
    }
}
