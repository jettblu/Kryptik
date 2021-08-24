using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Verify.V2.Service;

namespace CrypticPay
{
    public interface ISmsSender
    {
        public Task<MessageResource> SendSmsAsync(string number, string message);
        public Task<VerificationCheckResource> SendVerificationAsync(string number, string verificationCode);
    }
}
