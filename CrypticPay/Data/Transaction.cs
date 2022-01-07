using CrypticPay.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay.Data
{   
    public enum Status
    {
        Complete = 0,
        Failure = 1,
        Pending = 2,
        Unassigned = 3
    }
    public enum BroadCast
    {
        Onchain = 0,
        Offchain = 1
    }
    public enum Privacy
    {
        Public = 0,
        FriendsOnly = 1,
        Private = 2
    }
    public class Transaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Message { get; set; }
        public double AmountCrypto { get; set; }
        public double AmountFiat { get; set; }
        public string CoinId { get; set; }
        // user ID for payment sender
        public string SenderId { get; set; }
        // user ID for payment recipient
        public string RecieverId { get; set; }
        public string OutsideAddressTo { get; set; }
        // network status of transaction
        public Status StatusCurrent { get; set; }
        // whether onchain or offchain
        public BroadCast BroadcastType { get; set; }
        // transaction visibility within Kryptik
        public Privacy PrivacyLevel { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreationTime { get; set; }
    }
}
