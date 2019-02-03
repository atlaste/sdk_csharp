using Bunq.Sdk.Context;
using Bunq.Sdk.Http;
using Bunq.Sdk.Model.Generated.Endpoint;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            // Grab API key from the sandbox:
            //
            // curl https://public-api.sandbox.bunq.com/v1/sandbox-user -X POST --header "Content-Type: application/json" 
            // --header "Cache-Control: none" --header "User-Agent: curl-request" --header "X-Bunq-Client-Request-Id: $(date)randomId" 
            // --header "X-Bunq-Language: nl_NL" --header "X-Bunq-Region: nl_NL" --header "X-Bunq-Geolocation: 0 0 0 0 000"

            var API_KEY = ConfigurationManager.AppSettings["apikey"];
            var DEVICE_DESCRIPTION = ConfigurationManager.AppSettings["device"];

            if (!File.Exists("bunq.conf"))
            {
                var apiContextInit = ApiContext.Create(ApiEnvironmentType.SANDBOX, API_KEY, DEVICE_DESCRIPTION);
                apiContextInit.Save();
            }

            var apiContext = ApiContext.Restore();
            BunqContext.LoadApiContext(apiContext);

            /*
            var ri = RequestInquiry.Create(
                new Bunq.Sdk.Model.Generated.Object.Amount("100", "EUR"),
                new Bunq.Sdk.Model.Generated.Object.Pointer("EMAIL", "sugardaddy@bunq.com"), 
                "Gimme money", true);
            Console.WriteLine(ri.Value);
            */

            var pagination = new Pagination { Count = 10 };
            var accountResults = MonetaryAccount.List();
            foreach (var item in accountResults.Value)
            {
                Console.WriteLine("Balans: {0} {1}", item.MonetaryAccountBank.Balance, item.MonetaryAccountBank.Currency);

                var payments = Payment.List(item.MonetaryAccountBank.Id);
                foreach (var payment in payments.Value)
                {
                    Console.WriteLine("Payment received: {0} {1}, descr: {2}, from {3}",
                        payment.Amount.Value, payment.Amount.Value, payment.Description, payment.CounterpartyAlias.LabelMonetaryAccount.Iban);
                }
            }

            var masterCardTransactions = MasterCardAction.List();
            foreach (var item in masterCardTransactions.Value)
            {
                Console.WriteLine(item);
            }

            
            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
