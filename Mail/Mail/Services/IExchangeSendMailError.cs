using Mail.Models;
using System.Collections.Generic;

namespace Mail.Services
{
    public interface IExchangeSendMailError
    {
        List<SendMailError> SendMailErrors { get; set; }
    }
}
