using CrypticPay.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay.Data
{
    public class ChatData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        // message content should be encrypted client side
        public string Message { get; set; }
        //message encrypted with recipient's public key
        public string MessageTo { get; set; }
        // message encrypted with sender's public key
        public string MessageFrom { get; set; }
        // ptah for hd key generation
        public string KeyPath { get; set; }
        // has message been read
        public bool IsRead { get; set; }
        // user who sent this message
        public string SenderId { get; set; }
        // group to which this message belongs
        public string GroupId { get; set; }
    
        public DateTime CreationTime { get; set; }
    }
}
