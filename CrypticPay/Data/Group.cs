using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay.Data
{
    public class Group
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Public { get; set; }
        // user id of group creator
        public string Creator { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreationTime {get; set;}
    }
}
