using Mail.Helper;
using Mail.Models;
using Mail.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;

namespace Mail
{
    public class DetailsModel : PageModel
    {
        private readonly IFileProvider _fileProvider;
        private readonly IAppSetting _appSetting;
        private IExchangeSendMailError _exchangeSendMailError;
        public DetailsModel(IFileProvider fileProvider, IAppSetting appSetting, IExchangeSendMailError exchangeSendMailError)
        {
            _fileProvider = fileProvider;
            _appSetting = appSetting;
            _exchangeSendMailError = exchangeSendMailError;
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
            var tblAccountBalance = dataSet.Tables["column"];
            var lenght = tblAccountBalance.Rows.Count;
            for (int i = 0; i < lenght; i += 14)
            {
                if (i + 13 >= lenght) break;
                if (string.IsNullOrEmpty(tblAccountBalance.Rows[i][0].ToString())) continue;
                yield return (new AccountBalance
                {
                    Branch = tblAccountBalance.Rows[i][0].ToString(),
                    AccountId = tblAccountBalance.Rows[i + 1][0].ToString(),
                    OldAccountId = tblAccountBalance.Rows[i + 2][0].ToString(),
                    AccountName1 = tblAccountBalance.Rows[i + 3][0].ToString(),
                    CustomerCode = tblAccountBalance.Rows[i + 4][0].ToString(),
                    ShortName = tblAccountBalance.Rows[i + 5][0].ToString(),
                    CustomerName1 = tblAccountBalance.Rows[i + 6][0].ToString(),
                    CustomerName2 = tblAccountBalance.Rows[i + 7][0].ToString(),
                    ProductCode = tblAccountBalance.Rows[i + 8][0].ToString(),
                    ProductName = tblAccountBalance.Rows[i + 9][0].ToString(),
                    Currency = tblAccountBalance.Rows[i + 10][0].ToString(),
                    AvailableBalance = tblAccountBalance.Rows[i + 11][0].ToString(),
                    ActualBalance = tblAccountBalance.Rows[i + 12][0].ToString(),
                    CustomerOfficer = tblAccountBalance.Rows[i + 13][0].ToString()
                });
            }
        }

        public IActionResult OnPost(string fileName, string note)
        {
            var emailTemplate = GetEmailTemplate();
            var AccountBalances = GetAccountBalances(fileName);
            var accounts = GetAccounts();

            var mailList = AccountBalances.Join(accounts, balance => balance.AccountId, account => account.AccountId,
               (balance, account) => new { balance, account })
                .Select(s => new AccountBalance
                {
                    Email = s.account.Email,
                    ActualBalance = s.balance.ActualBalance,
                    AccountName1 = s.balance.AccountName1,
                    AccountId = s.balance.AccountId
                }).ToList();

            _exchangeSendMailError.SendMailErrors = new List<SendMailError>();
            if (!string.IsNullOrEmpty(note))
            {
                note = $"{Environment.NewLine}{note}{Environment.NewLine}";
            }

            foreach (var item in mailList)
            {
                var message = SendMail(item.Email, item, emailTemplate, note);
                if (!string.IsNullOrEmpty(message))
                {
                    _exchangeSendMailError.SendMailErrors.Add(new SendMailError
                    {
                        AccountBalance = item,
                        Message = message
                    });
                }
            }

            return RedirectToPage("/SendMailsResult");
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

        private string SendMail(string email, AccountBalance accountBalance, EmailTemplate emailTemplate, string note)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(_appSetting.EmailAddress);
                    mail.To.Add(email);
                    mail.Subject = emailTemplate.Subject;
                    mail.Body = string.Format(emailTemplate.Body,
                        accountBalance.AccountName1,
                        DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                        accountBalance.AccountId,
                        accountBalance.ActualBalance, note);

                    using (var SmtpServer = new SmtpClient(_appSetting.EmailHost, _appSetting.EmailPort))
                    {
                        SmtpServer.Credentials = new System.Net.NetworkCredential(_appSetting.EmailAddress, _appSetting.EmailPassword);
                        SmtpServer.EnableSsl = true;
                        SmtpServer.Send(mail);
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}