using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Verify.V2.Service;


namespace CrypticPay.Services
{
    

    // update, so automatically accepts settings via dependency injection
    public class SmsSender : ISmsSender
    {   
        private string _verificationSID;
        private string _accountSID;
        private string _authToken;
        private string _sendNumber;

        public SmsSender(string verificationSid, string accountSid, string authToken, string sendNumber)
        {
            _verificationSID = verificationSid;
            _accountSID = accountSid;
            _authToken = authToken;
            _sendNumber = sendNumber;
        }


        public async Task<MessageResource> SendSmsAsync(string number, string message)
        {

            TwilioClient.Init(_accountSID, _authToken);
            // send message
            var result = await MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(_sendNumber),
                to: new Twilio.Types.PhoneNumber(number)
            );
            return result;
        }

        // returns twilio verification object which indicates message status
        // used for confirmation of number ownsership
        public async Task<VerificationCheckResource> SendVerificationAsync(string number, string verificationCode)
        {   
            var verification = await VerificationCheckResource.CreateAsync(
                    to: number,
                    code: verificationCode,
                    pathServiceSid: _verificationSID
                );

            return verification;
        }
    }
}
