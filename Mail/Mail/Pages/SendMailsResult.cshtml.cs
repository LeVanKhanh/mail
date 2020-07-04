using Mail.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mail
{
    public class SendMailsResult : PageModel
    {
        public IExchangeSendMailError ExchangeSendMailError;
        public SendMailsResult(IExchangeSendMailError exchangeSendMailError)
        {
            ExchangeSendMailError = exchangeSendMailError;
        }
        public IActionResult OnGet()
        {
            return Page();
        }
    }
}