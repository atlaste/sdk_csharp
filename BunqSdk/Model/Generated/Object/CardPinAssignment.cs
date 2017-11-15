using Bunq.Sdk.Model.Core;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bunq.Sdk.Model.Generated.Object
{
    /// <summary>
    /// </summary>
    public class CardPinAssignment : BunqModel
    {
        /// <summary>
        /// PIN type. Can be PRIMARY, SECONDARY or TERTIARY
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    
        /// <summary>
        /// The 4 digit PIN to be assigned to this account.
        /// </summary>
        [JsonProperty(PropertyName = "pin_code")]
        public string PinCode { get; set; }
    
        /// <summary>
        /// The ID of the monetary account to assign to this pin for the card.
        /// </summary>
        [JsonProperty(PropertyName = "monetary_account_id")]
        public string MonetaryAccountId { get; set; }
    
        public CardPinAssignment(string type)
        {
            Type = type;
        }
    }
}