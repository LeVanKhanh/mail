using Mail.Helper;
using Mail.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Mail
{
    public class DetailsModel : PageModel
    {
        private readonly IFileProvider _fileProvider;
        private readonly IAppSetting _appSetting;
        public DetailsModel(IFileProvider fileProvider, IAppSetting appSetting)
        {
            _fileProvider = fileProvider;
            _appSetting = appSetting;
        }

        public IEnumerable<AccountBalance> AccountBalances { get; set; }
        public string FileName { get; set; }

        public IActionResult OnGet(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return RedirectToPage("/Index");
            }
            FileName = fileName;
            AccountBalances = GetAccountBalances(fileName);
            return Page();
        }

        private IEnumerable<AccountBalance> GetAccountBalances(string fileName)
        {
            var theFile = _fileProvider.GetFileInfo(fileName);
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(theFile.PhysicalPath, XmlReadMode.InferSchema);
            var tblAccountBalance = dataSet.Tables[0];
            foreach (DataRow row in tblAccountBalance.Rows)
            {
                yield return (new AccountBalance
                {
                    AccountId = row[0].ToString(),
                    Balance = row[1].ToString()
                });
            }
        }

        public IActionResult OnPost(string fileName)
        {
            var emailTemplate = GetEmailTemplate();
            var AccountBalances = GetAccountBalances(fileName);
            var accounts = GetAccounts();

            var mailList = AccountBalances.Join(accounts, balance => balance.AccountId, account => account.AccountId,
               (balance, account) => new { balance, account })
                .Select(s => new
                {
                    s.account.Email,
                    s.balance.Balance
                }).ToList();

            foreach (var item in mailList)
            {
                Task.Run(() => SendMail(item.Email, item.Balance, emailTemplate));
            }

            return RedirectToPage("/Index");
        }

        private EmailTemplate GetEmailTemplate()
        {
            var mailTemplateFile = _fileProvider.GetFileInfo("mailTemplate.xml");
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(mailTemplateFile.PhysicalPath, XmlReadMode.InferSchema);
            var emailTemplate = dataSet.Tables[0].Rows[0];
            return new EmailTemplate
            {
                Subject = emailTemplate[0].ToString(),
                Body = emailTemplate[1].ToString()
            };
        }

        private IEnumerable<Account> GetAccounts()
        {
            var accountsFile = _fileProvider.GetFileInfo("accounts.xml");
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(accountsFile.PhysicalPath, XmlReadMode.InferSchema);
            var tblAccount = dataSet.Tables[0];
            foreach (DataRow row in tblAccount.Rows)
            {
                yield return (new Account
                {
                    AccountId = row[0].ToString(),
                    Email = row[1].ToString()
                });
            }
        }

        private void SendMail(string email, string balance, EmailTemplate emailTemplate)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress("levankhanhtpd@gmail.com");
                    mail.To.Add(email);
                    mail.Subject = emailTemplate.Subject;
                    mail.Body = string.Format(emailTemplate.Body, balance);

                    using (var SmtpServer = new SmtpClient("smtp.gmail.com", 587))
                    {
                        SmtpServer.Credentials = new System.Net.NetworkCredential(_appSetting.EmailUserName, _appSetting.EmailPassword);
                        SmtpServer.EnableSsl = true;
                        SmtpServer.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                // Do log exception here
            }
        }
    }
}