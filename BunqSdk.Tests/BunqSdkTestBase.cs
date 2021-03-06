﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Bunq.Sdk.Context;
using Bunq.Sdk.Exception;
using Bunq.Sdk.Json;
using Bunq.Sdk.Model.Generated.Endpoint;
using Bunq.Sdk.Model.Generated.Object;
using Newtonsoft.Json.Linq;

namespace Bunq.Sdk.Tests
{
    /// <summary>
    /// Base class for all the Bunq SDK tests.
    /// </summary>
    public class BunqSdkTestBase
    {
        /// <summary>
        /// Error constants.
        /// </summary>
        private const string ErrorCouldNotDetermineUserAlias = "Could not determine user alias.";

        /// <summary>
        /// Name of the context configuration file.
        /// </summary>
        protected const string FilenameContextConf = "../../../bunq-test.conf";

        /// <summary>
        /// Constants for payment creation.
        /// </summary>
        protected const string PaymentAmountEur = "0.01";
        protected const string PaymentCurrency = "EUR";
        protected const string PaymentDescription = "C# test Payment";

        /// <summary>
        /// Constants for monetary account.
        /// </summary>
        protected const string MonetaryAccountDescription = "Test C# monetary account";
        
        /// <summary>
        /// Image constants.
        /// </summary>
        protected const string PathAttachment = "../../../Resources";
        protected const string ContentType = "image/png";
        protected const string AttachmentDescription = "C# sdk image test.";
        protected const string AttachmentPathIn = "/bunq_App_Icon_Square@4x.png";
        
        /// <summary>
        /// Device registration constants.
        /// </summary>
        private const string DeviceDescription = "Csharp test device";
        
        /// <summary>
        /// Pointer type constants.
        /// </summary>
        private const string PointerTypeEmail = "EMAIL";
        
        /// <summary>
        /// Email constants.
        /// </summary>
        private const string EmailBravo = "bravo@bunq.com";
        private const string EmailSuggarDaddy = "sugardaddy@bunq.com";
        
        /// <summary>
        /// Spending money constants.
        /// </summary>
        private const string SpendingMoneyAmount = "50.00";
        private const string SpendingMoneyRequestDescription = "sdk c# test, thanks daddy.";

        protected static MonetaryAccountBank SecondMonetaryAccountBank;
        
        /// <summary>
        /// Gets an Api Context, re-creates if needed and returns it.
        /// </summary>
        protected static void SetUpTestCase()
        {
            SetUpApiContext();
            SecondMonetaryAccountBank = SetUpSecondMonetaryAccount();
            RequestSpendingMoney();
            System.Threading.Thread.Sleep(500); // ensure requests are auto accepted.
            BunqContext.UserContext.RefreshUserContext();
        }

        protected static ApiContext SetUpApiContext()
        {
            ApiContext apiContext;

            if (File.Exists(FilenameContextConf))
            {
                apiContext = ApiContext.Restore(FilenameContextConf);
                apiContext.EnsureSessionActive();
            }
            else
            {
                var sandboxUser = GenerateNewSandboxUser();
                apiContext = ApiContext.Create(ApiEnvironmentType.SANDBOX, sandboxUser.ApiKey, DeviceDescription);
            }
            
            BunqContext.LoadApiContext(apiContext);

            return apiContext;
        }
        
        private static SandboxUser GenerateNewSandboxUser()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-Bunq-Client-Request-Id", "unique");
            httpClient.DefaultRequestHeaders.Add("Cache-Control", "no");
            httpClient.DefaultRequestHeaders.Add("X-Bunq-Geolocation", "0 0 0 0 NL");
            httpClient.DefaultRequestHeaders.Add("X-Bunq-Language", "en_US");
            httpClient.DefaultRequestHeaders.Add("X-Bunq-Region", "en_US");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "sdk_csharp_test_case");

            var requestTask = httpClient.PostAsync(ApiEnvironmentType.SANDBOX.BaseUri + "sandbox-user", null);
            requestTask.Wait();

            var responseString = requestTask.Result.Content.ReadAsStringAsync().Result;
            var responseJson = BunqJsonConvert.DeserializeObject<JObject>(responseString);
            
            return BunqJsonConvert.DeserializeObject<SandboxUser>(responseJson.First.First.First.First.First
                .ToString());
        }

        private static MonetaryAccountBank SetUpSecondMonetaryAccount()
        {
            var createdMonetaryAccountId = MonetaryAccountBank.Create(PaymentCurrency, MonetaryAccountDescription);

            return MonetaryAccountBank.Get(createdMonetaryAccountId.Value).Value;
        }

        private static void RequestSpendingMoney()
        {
            RequestInquiry.Create(
                new Amount(SpendingMoneyAmount, PaymentCurrency),
                new Pointer(PointerTypeEmail, EmailSuggarDaddy),
                SpendingMoneyRequestDescription,
                false
                );
            
            RequestInquiry.Create(
                new Amount(SpendingMoneyAmount, PaymentCurrency),
                new Pointer(PointerTypeEmail, EmailSuggarDaddy),
                SpendingMoneyRequestDescription,
                false,
                SecondMonetaryAccountBank.Id
                );
        }

        protected static Pointer GetPointerBravo()
        {
            return new Pointer(PointerTypeEmail, EmailBravo);
        }
        
        protected static Pointer GetAlias()
        {
            var userContex = BunqContext.UserContext;

            if (userContex.IsOnlyUserPersonSet())
            {
                return userContex.UserPerson.Alias.First();
            }
            else if (userContex.IsOnlyUserCompanySet())
            {
                return userContex.UserCompany.Alias.First();
            }
            else
            {
                throw new BunqException(ErrorCouldNotDetermineUserAlias);
            }
        }
    }
}
