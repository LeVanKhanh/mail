using Mail.Models;
using System.Collections.Generic;

namespace Mail.Services
{
    public class ExchangeSendMailError: IExchangeSendMailError
    {
        public List<SendMailError> SendMailErrors { get; set; }
    }
}
