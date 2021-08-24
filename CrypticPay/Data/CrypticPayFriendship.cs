using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay.Data
{
    public class CrypticPayFriendship
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public bool IsConfirmed { get; set; }
        public string FriendFrom { get; set; }
        public string FriendTo { get; set; }

        [DataType(DataType.Date)]
        public DateTime RequestTime { get; set; }

        [DataType(DataType.Date)]
        public DateTime BecameFriendsTime { get; set; }
    }
}
